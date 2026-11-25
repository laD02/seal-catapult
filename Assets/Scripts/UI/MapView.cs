using SealCatapult.Core;
using SealCatapult.Progress;
using TMPro;
using UnityEngine;

namespace SealCatapult.UI
{
    /// <summary>
    /// Level map, one page per castle, laid out like CPC's (report §2, user 16/11: "thiết kế lại trang map, học tập cpc"):
    /// a top-down sea of ice islands with ONE long trail of small "?" stones (LevelCatalog.PerCastle) winding in an S from the igloo
    /// village to the bear fortress. Every tenth stone is a cage on an orange base ("HELP!" by the next one), the last is
    /// the castle's boss; two fish stones are the bonus runs. Cleared stones show their stars; the hero stands on the
    /// next one. Levels open strictly in order, as in CPC.
    /// </summary>
    public class MapView : MonoBehaviour
    {
        const string Painting = "map_bg_world";   // Assets/Art/Reference/map_world.png (ChatGPT, 16/11)
        // The trail through the painting's own pixels (1672×941, y down): the centres of the 51 stones ChatGPT painted on
        // the path (map_world_stones40.png, found by Tools/map_stones.py), so every point lies on the painted path. The
        // level stones are spread evenly along it and drawn on the clean painting (LevelCatalog.PerCastle per castle, 41 since 17/11).
        const float PaintingW = 1672f, PaintingH = 941f;
        static readonly Vector2[] Trail =
        {
            new Vector2(265, 312), new Vector2(326, 296), new Vector2(387, 283), new Vector2(450, 273), new Vector2(514, 265), new Vector2(578, 260),
            new Vector2(643, 260), new Vector2(705, 267), new Vector2(747, 294), new Vector2(760, 334), new Vector2(718, 370), new Vector2(652, 388),
            new Vector2(576, 400), new Vector2(505, 413), new Vector2(431, 426), new Vector2(357, 439), new Vector2(288, 462), new Vector2(229, 496),
            new Vector2(195, 543), new Vector2(193, 595), new Vector2(222, 640), new Vector2(276, 674), new Vector2(348, 700), new Vector2(426, 717),
            new Vector2(505, 724), new Vector2(582, 725), new Vector2(694, 740), new Vector2(814, 763), new Vector2(887, 775), new Vector2(969, 779),
            new Vector2(1051, 773), new Vector2(1132, 754), new Vector2(1180, 714), new Vector2(1160, 671), new Vector2(1108, 641), new Vector2(1057, 608),
            new Vector2(1052, 563), new Vector2(1103, 525), new Vector2(1171, 504), new Vector2(1242, 494), new Vector2(1314, 490), new Vector2(1383, 487),
            new Vector2(1432, 461), new Vector2(1433, 377), new Vector2(1430, 322), new Vector2(1379, 302), new Vector2(1315, 306), new Vector2(1254, 297),
            new Vector2(1235, 263), new Vector2(1273, 235), new Vector2(1360, 238), new Vector2(1440, 250),   // up the ladder, round the loop, to the gate
        };

        /// <summary>The castle's painting and trail (WorldTheme); a world not painted yet falls back to Frost Bay.</summary>
        static (string painting, Vector2[] trail) WorldOf(int castle)
        {
            if (castle == 0) return (Painting, Trail);
            string painting = WorldTheme.Painting(castle);
            var trail = WorldTheme.Trail(castle);
            return Art.Has(painting) && trail != null && trail.Length > 1 ? (painting, trail) : (Painting, Trail);
        }

        static readonly System.Collections.Generic.Dictionary<int, Vector2[]> nodeCache = new System.Collections.Generic.Dictionary<int, Vector2[]>();
        /// <summary>One stone per level of a castle, evenly spaced along its world's trail.</summary>
        static Vector2[] NodesOf(int castle)
        {
            if (!nodeCache.TryGetValue(castle, out var n)) nodeCache[castle] = n = Resample(WorldOf(castle).trail, LevelCatalog.PerCastle);
            return n;
        }

        public static string CastleName(int castle) => WorldTheme.Name(castle);

        static Vector2[] Resample(Vector2[] path, int count)
        {
            float total = 0f;
            for (int i = 1; i < path.Length; i++) total += Vector2.Distance(path[i - 1], path[i]);
            var result = new Vector2[count];
            int seg = 1;
            float walked = 0f;
            for (int n = 0; n < count; n++)
            {
                float target = total * n / (count - 1);
                while (seg < path.Length - 1 && walked + Vector2.Distance(path[seg - 1], path[seg]) < target)
                {
                    walked += Vector2.Distance(path[seg - 1], path[seg]);
                    seg++;
                }
                float len = Vector2.Distance(path[seg - 1], path[seg]);
                result[n] = Vector2.Lerp(path[seg - 1], path[seg], len > 0f ? Mathf.Clamp01((target - walked) / len) : 0f);
            }
            return result;
        }

        const float Stone = 64f;   // 41 roomy stones
        const float PrevArrowUp = 114f;   // the previous-area arrow's bottom edge above the screen's (Tools/map_reach.py)
        const float CaptiveFloor = 0.22f;   // the pup's feet in the cage's base dish (red top at 0.27 of cage.png's height, 19/11)

        /// <summary>Pages that can be opened: every castle reached so far, plus the next one (shown locked).</summary>
        static int PageCount(SaveData data) => Mathf.Min(LevelCatalog.CastleOf(CurrentLevel(data)) + 2, WorldTheme.Count);   // ten castles at most

        int page;
        Canvas canvas;


        TextMeshProUGUI starsLabel, spendLabel;
        Bob spendPulse;
        ShopView shop;

        void Start()
        {
            SaveSystem.EnsureLoaded();
            // map_bg covers the whole screen: no backdrop under it, and a still screen idles at half the frame rate.
            MenuScene.Setup(backdrop: false);
            Application.targetFrameRate = BootView.StillFps;
            page = PageOf(CurrentLevel(SaveSystem.Data));
            hopPending = HopFrom;
            HopFrom = -1;   // one look at the map uses it up, matched or not
            playNext = PlayAfterHop;
            instance = this;
            hopStarted = false;
            PlayAfterHop = -1;
            Build();
            hopPending = -1;   // the star pop and the walk play once, not again when a page is rebuilt
            // The next level starts on a timer from here (not from the walk's end: a page switch destroys the walk).
            if (playNext >= 0) StartCoroutine(PlayAfter(hopStarted ? Hop.Delay + WalkTime + LandPause : NoHopPause));
        }

        /// <summary>The map paintings are a megapixel each and every page visited loaded one: free them with the map.</summary>
        void OnDestroy()
        {
            Art.Release("map_bg");
            Art.Release("bg_menu");
            Art.Release("bg_shop");   // the shop's backdrop, loaded when the shop was opened from the map
        }

        /// <summary>The next level to play: the first one open and not yet won. The castles have no end, so there is
        /// always one.</summary>
        public static int CurrentLevel(SaveData data)
        {
            for (int i = 0; i < LevelCatalog.Total; i++)
                if (Unlocked(i, data) && data.StarsOf(i) == 0) return i;
            return LevelCatalog.Total - 1;   // all ten castles cleared: the last stone stays current
        }

        static int PageOf(int level) => LevelCatalog.CastleOf(level);

        static int FirstLevelOf(int page) => LevelCatalog.FirstOf(page);

        /// <summary>Strictly in order, as in CPC; a level already won stays open (old saves skipped the bonus).</summary>
        static bool Unlocked(int index, SaveData data) =>
            index == 0 || data.StarsOf(index) > 0 || data.StarsOf(index - 1) > 0;

        void ShowPage(int p)
        {
            playNext = -1;   // the player took over: no level starts by itself
            page = p;
            Destroy(canvas.gameObject);   // the shop has its own canvas and is kept
            Build();
        }

        /// <summary>Android Back: the shop first, then the previous map page, then the main menu.</summary>
        void Update()
        {
            if (!GameInput.BackPressed() || ScreenFader.LoadPending) return;
            if (shop.HandleBack()) return;
            if (page > 0) ShowPage(page - 1);   // Back walks the pages back, then leaves for the menu
            else SceneLoader.Load(SceneLoader.MainMenu);
        }

        void Build()
        {
            canvas = UiKit.CreateCanvas("Map");
            // Top-down island fills the screen (cropped to cover any aspect). The part that does not fit is not lost:
            // the painting pans under a finger (audit 20/11: 4:3 cut 12.5 % off each side and 20:9 10 % off the top and
            // bottom, with the stones painted there out of sight and out of reach).
            string painting = WorldOf(page).painting;
            Vector2 view = UiKit.CanvasSize();
            Vector2 paint = new Vector2(1920f, 1080f) * Mathf.Max(view.x / 1920f, view.y / 1080f);
            var pan = UiKit.Stretch(UiKit.Node(canvas.transform, "Pan"));
            var map = UiKit.Image(pan, Art.Has(painting) ? painting : "map_bg", UiKit.Center, Vector2.zero, paint);
            map.preserveAspect = false;
            Vector2 slack = Vector2.Max(Vector2.zero, (paint - view) * 0.5f);   // how far the painting can slide each way
            // Only where there is something to pan (not at 16:9): a ScrollRect takes every drag that starts on a stone,
            // and a finger that slides past the drag threshold while tapping loses its tap (UiKit raises that threshold).
            if (slack.x > 1f || slack.y > 1f)
            {
                map.raycastTarget = true;   // a drag that starts on the painting or on a stone reaches the scroll view
                var scroll = pan.gameObject.AddComponent<UnityEngine.UI.ScrollRect>();
                scroll.content = map.rectTransform;
                scroll.viewport = pan;
                scroll.horizontal = slack.x > 1f;
                scroll.vertical = slack.y > 1f;
                scroll.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;
            }
            var root = UiKit.SafeRoot(canvas);
            // The screen's buttons draw over everything on the painting, the captive's "HELP!" (its own sorted canvas)
            // included: the map pans now, so that word can come to lie under the top bar.
            var controls = root.gameObject.AddComponent<Canvas>();
            controls.overrideSorting = true;
            controls.sortingOrder = 10;
            root.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Layout A top bar (GAME_DESIGN.md §R10): Back, the area's name, stars earned. Sound and music: Settings.
            // The stars are a line under the name, not the pill in the right corner: no place on the map's edge is free
            // of stones for a 260 px pill on every screen (it hid two castle bosses and a stone; Tools/map_reach.py).
            // Name and stars take no taps, so a stone under them can still be played.
            var bar = UiKit.TopBar(root, "", () => SceneLoader.Load(SceneLoader.MainMenu), null, out _);
            UiKit.Text(bar, CastleName(page), 54, UiKit.TopLeft, new Vector2(150, 4), new Vector2(760, 62), Color.white, TextAlignmentOptions.Left, true);
            UiKit.Image(bar, "star_full", UiKit.BottomLeft, new Vector2(150, -2), new Vector2(46, 46));
            starsLabel = UiKit.Text(bar, "", 40, UiKit.BottomLeft, new Vector2(204, -4), new Vector2(300, 48), Color.white, TextAlignmentOptions.Left, true);
            var upgrades = UiKit.Button(root, Loc.T("shop"), ButtonStyle.Warm, UiKit.BottomRight, new Vector2(-24, 24), new Vector2(320, 110),
                () => { playNext = -1; shop.Open(); }, "glyph_shop");
            // Stars left to spend, on the button that spends them (the pill above counts stars earned, a different
            // number); the badge pulses while something is affordable.
            var badge = UiKit.Image(upgrades.transform, "star_full", UiKit.TopRight, new Vector2(4, 50), new Vector2(84, 84));   // on the corner, clear of the label
            spendLabel = UiKit.Text(badge.transform, "", 40, UiKit.Center, new Vector2(0, -4), new Vector2(70, 60));
            spendPulse = badge.gameObject.AddComponent<Bob>();
            spendPulse.pulse = true;
            spendPulse.speed = 5f;
            if (shop == null) shop = ShopView.Create(RefreshStars);
            RefreshStars();
            // Page arrows, one control per corner: next area top-right, previous area bottom-left above the edge
            // (top-left is Back, bottom-right Upgrades). These are the only two places where a 110 px button covers no
            // stone, cage or boss of any of the ten worlds on any screen shape: mid-height on the side edges hid stones
            // of five worlds at 16:9, where nothing can pan. Tools/map_reach.py checks every stone against them.
            if (page + 1 < PageCount(SaveSystem.Data))
                UiKit.Button(root, null, ButtonStyle.Gold, UiKit.TopRight, new Vector2(-24, -24), new Vector2(110, 110),
                    () => ShowPage(page + 1), "glyph_next");
            if (page > 0)
            {
                var prev = UiKit.Button(root, null, ButtonStyle.Gold, UiKit.BottomLeft, new Vector2(24, PrevArrowUp), new Vector2(110, 110),
                    () => ShowPage(page - 1), "glyph_next");
                var glyph = prev.transform.Find("Glyph");   // absent once a painted icon_next exists (UiKit.Button)
                if (glyph != null) glyph.localScale = new Vector3(-1f, 1f, 1f);   // the same arrow, pointing left
            }

            var data = SaveSystem.Data;
            var stones = NodesOf(page);
            int first = FirstLevelOf(page);
            // Nodes sit on the painted trail, so they share the painting's frame (the full canvas), not the safe area.
            // On the painting itself (it envelopes the screen): each stone sits at its painting point, as an anchor.
            var trail = UiKit.Stretch(UiKit.Node(map.transform, "Trail"));
            var spots = new RectTransform[stones.Length];
            for (int i = 0; i < stones.Length; i++)
            {
                var spot = UiKit.Node(trail, "Spot");
                spot.anchorMin = spot.anchorMax = new Vector2(stones[i].x / PaintingW, 1f - stones[i].y / PaintingH);
                spot.sizeDelta = Vector2.zero;
                spot.anchoredPosition = Vector2.zero;
                spots[i] = spot;
            }
            // Open on the stone to play (this page's), so on a screen that crops the painting it is never out of sight.
            int here = CurrentLevel(data) - first;
            if (here >= 0 && here < stones.Length)
            {
                Vector2 at = new Vector2((stones[here].x / PaintingW - 0.5f) * paint.x, (0.5f - stones[here].y / PaintingH) * paint.y);
                map.rectTransform.anchoredPosition = new Vector2(Mathf.Clamp(-at.x, -slack.x, slack.x), Mathf.Clamp(-at.y, -slack.y, slack.y));
            }
            int nextCage = -1;   // the nearest cage not yet opened calls for help
            for (int i = 0; i < stones.Length && nextCage < 0; i++)
                if (LevelCatalog.KindOf(first + i) == LevelKind.Boss && data.StarsOf(first + i) == 0) nextCage = first + i;
            for (int i = 0; i < stones.Length; i++)
                BuildNode(spots[i], first + i, Vector2.zero, data, i > 0 ? spots[i - 1] : null, first + i == nextCage);
            // CPC's map is alive: fish leap out of the open water around the islands.
            if (Art.Has(WorldOf(page).painting)) MapLife.Add(trail, WorldTheme.Water(page), PaintingW, PaintingH);
            // Snow, embers, petals…: each castle's own air, over the stones and under the screen's buttons.
            MapBear.Add(trail, WorldTheme.Bears(page), PaintingW, PaintingH);   // CPC: a bear stands by the trail
            MapWeather.Add(map.transform, WorldTheme.Key(page));
        }

        void RefreshStars()
        {
            var data = SaveSystem.Data;
            // Stars of this castle out of its 180: a total over castles without end would have no ceiling.
            int castleStars = 0;
            for (int i = 0; i < LevelCatalog.PerCastle; i++) castleStars += data.StarsOf(FirstLevelOf(page) + i);
            starsLabel.text = $"{castleStars}/{LevelCatalog.PerCastle * 3}";
            int spendable = Shop.AvailableStars(data);
            spendLabel.text = spendable.ToString();
            spendPulse.gameObject.SetActive(spendable > 0);   // nothing to spend: no badge
            bool affordable = false;
            foreach (var def in GameDatabase.Instance.upgrades.upgrades) affordable |= Shop.CanBuy(data, def);
            spendPulse.enabled = affordable;
            if (!affordable) spendPulse.transform.localScale = Vector3.one;
        }

        /// <summary>CPC: the hero on the green pad waves and raises its helmet while it waits (front-view frames).</summary>
        static void Wait(UnityEngine.UI.Image hero)
        {
            if (hero == null || !Art.Has("pup_wave_7")) return;
            hero.gameObject.AddComponent<FrameLoop>().Set(7f, ("pup_wave_", 8), ("pup_hattip_", 8));
        }

        /// <summary>The level just cleared for the first time; the map hops the hero from it to the next stone once.</summary>
        public static int HopFrom = -1;
        /// <summary>Set by the win card's Continue: the level to start once the hero has hopped to it.</summary>
        public static int PlayAfterHop = -1;
        static int playNext = -1;
        static bool hopStarted;
        static MapView instance;
        const float LandPause = 0.6f, NoHopPause = 1.2f, WalkTime = 1.1f;   // seconds on the map before the next level starts

        System.Collections.IEnumerator PlayAfter(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);   // the walk runs on unscaled time too
            if (playNext < 0 || ScreenFader.LoadPending) yield break;   // the player went elsewhere first
            int level = playNext;
            playNext = -1;
            SceneLoader.PlayLevel(level);
        }
        static int hopPending = -1;
        const float StarPop = 0.1f;   // seconds after the map opens: the stone just won pops into its star

        static System.Collections.IEnumerator SfxAfter(float seconds, string name)
        {
            yield return new WaitForSecondsRealtime(seconds);
            AudioManager.Sfx(name);
        }

        static void BuildNode(Transform root, int index, Vector2 pos, SaveData data, RectTransform previous, bool calls)
        {
            var kind = LevelCatalog.KindOf(index);
            bool final = LevelCatalog.IsFinal(index);
            bool unlocked = Unlocked(index, data);
            int stars = data.StarsOf(index);
            bool current = unlocked && stars == 0;
            float size = final ? Stone * 1.5f : Stone;
            // Whatever moves all the time (the hero and its pad, a captive, the boss) gets its own canvas: animated in the
            // map's one big canvas it re-batched the painting, the 41 stones and the text every frame.
            if (current || (kind == LevelKind.Boss && stars == 0))
            {
                root.gameObject.AddComponent<Canvas>();
                root.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();   // a nested canvas needs its own for taps
            }
            // "You are here": the green pad under the stone, the hero standing on it (CPC: the green disc).
            if (current)
            {
                var pad = UiKit.Image(root, "node_glow", UiKit.Center, pos + new Vector2(0, -6), new Vector2(size * 2f, size * 1.3f));
                pad.gameObject.AddComponent<Bob>().pulse = true;   // the "you are here" pad breathes, so the eye finds it
                if (hopPending == index - 1) pad.gameObject.AddComponent<PopIn>().delay = 0.15f;   // CPC: the next pad lights up, then the hero hops to it
            }
            // Cages and the castle's boss stand on an orange base, as in CPC.
            if (kind == LevelKind.Boss)
            {
                var plinth = UiKit.Image(root, "powder", UiKit.Center, pos + new Vector2(0, -size * 0.18f), new Vector2(size * 1.3f, size * 0.55f));
                plinth.color = new Color(1f, 0.55f, 0.15f);
            }
            // A level not open yet answers the tap (a dull thud and the squash) instead of swallowing it.
            var button = UiKit.ImageButton(root, stars > 0 ? "node_star" : "node_stone", UiKit.Center, pos, new Vector2(size, size),
                unlocked ? () => SceneLoader.PlayLevel(index) : (System.Action)(() => AudioManager.Sfx("thud", 0.06f, 0.5f)), juice: !current);
            // Stones stand 70-120 px apart and each has a 130 px touch area, so the areas overlap: a tap goes to the
            // stone whose centre is nearest (audit 20/11: the next stone's area covered half of this one).
            button.gameObject.AddComponent<NearestStone>();
            // CPC: the stone just won turns into its star disc with a pop as the map opens.
            if (stars > 0 && index == hopPending)
            {
                button.gameObject.AddComponent<PopIn>().delay = StarPop;
                root.gameObject.AddComponent<Sparkle>().delay = StarPop + 0.15f;   // twinkles fly out of the new star
                instance.StartCoroutine(SfxAfter(StarPop + 0.1f, kind == LevelKind.Boss && !final ? "cage" : "star"));
            }
            if (!unlocked) button.GetComponent<UnityEngine.UI.Image>().color = new Color(0.78f, 0.84f, 0.92f, 1f);   // dim, not yet open
            if (kind == LevelKind.Boss && final && stars == 0)
            {
                // The castle's boss waits at the gate, alive (its idle antics).
                // As big as the walking hero, feet on the middle of the stone like the hero's (user 19/11).
                var boss = UiKit.Image(button.transform, Art.Has("bear_idle_0") ? "bear_idle_0" : "bear", UiKit.Center, new Vector2(0, -Stone * 0.05f), new Vector2(Stone * 1.9f, Stone * 1.9f));
                boss.rectTransform.pivot = new Vector2(0.5f, 0f);
                UiKit.StandOut(boss, new Vector2(0, -Stone * 0.05f), Stone * 1.3f);
                if (Art.Has("bear_taunt_3")) boss.gameObject.AddComponent<FrameLoop>().Set(5f, ("bear_punch_", 4), ("bear_punch_", 4), ("bear_idle_", 4), ("bear_taunt_", 4));   // CPC: the boss shadow-boxes
            }
            else if (kind == LevelKind.Boss)
            {
                // CPC's map cage, studied frame by frame (video 18/11; user 19/11: "xem lại hoạt họa trong lồng của cpc và
                // làm lại"): a tall slim bird cage with thin bars on a round red base, the pup INSIDE it, whole and
                // clearly seen through the bars, waving both flippers above its head and shouting for rescue (16 frames,
                // pup_captive). Drawn pup first, cage over it; the cage is as tall as the walking hero, feet on the stone.
                bool freed = stars > 0;
                var cageArt = freed && Art.Has("cage_open") ? "cage_open" : "cage";
                var cageSprite = Art.Get(cageArt);
                float cageH = Stone * 2.3f;   // a little taller than the walking hero: the slim cage reads smaller
                float cageW = cageSprite != null ? cageH * cageSprite.rect.width / cageSprite.rect.height : cageH * 0.72f;
                var holder = UiKit.Place(UiKit.Node(button.transform, "Cage"), UiKit.Center, new Vector2(0, -Stone * 0.05f), new Vector2(cageW, cageH));
                holder.pivot = new Vector2(0.5f, 0f);   // stands on the middle of the stone, like the hero (user 19/11)
                UiKit.Ground(holder, new Vector2(0, -Stone * 0.05f), cageW * 1.2f);
                if (!freed)
                {
                    string loop = Art.Has("pup_captive_15") ? "pup_captive_" : "pup_wave_";
                    // Inside the bars, above the base plate (CaptiveFloor of the cage's height), as tall as fits under the dome.
                    var captive = UiKit.Image(holder, loop + "0", UiKit.Bottom, new Vector2(0, cageH * CaptiveFloor), new Vector2(cageW * 1.0f, cageH * 0.62f));   // fills the bars up to the dome
                    captive.rectTransform.pivot = new Vector2(0.5f, 0f);
                    captive.raycastTarget = false;
                    UiKit.Stroke(captive, 2f);   // the pup reads through the bars (the cage itself is not stroked: it thickened the bars)
                    captive.gameObject.AddComponent<FrameLoop>().Set(loop, Art.Has("pup_captive_15") ? 16 : 8, Art.Has("pup_captive_15") ? 14f : 10f);
                }
                var cage = UiKit.Image(holder, cageArt, UiKit.Center, Vector2.zero, new Vector2(cageW, cageH));
                cage.rectTransform.anchorMin = Vector2.zero;
                cage.rectTransform.anchorMax = Vector2.one;
                cage.rectTransform.offsetMin = cage.rectTransform.offsetMax = Vector2.zero;
                cage.raycastTarget = false;   // taps reach the stone's button under it
                if (calls && !freed)
                {
                    // CPC: a big red "HELP!" by the nearest cage, wobbling, shown as the map opens and again now and then.
                    var cry = UiKit.Text(holder, Loc.T("help_cry"), 58, UiKit.Top, new Vector2(cageW * 0.5f + 90f, -cageH * 0.05f),
                        new Vector2(260, 70), UiKit.Danger, TextAlignmentOptions.Center, true);
                    cry.outlineWidth = 0.22f;
                    cry.outlineColor = new Color32(255, 255, 255, 255);
                    cry.gameObject.AddComponent<Shout>();
                    // Drawn over the stones that come after it in the trail (they hid half the word).
                    var top = cry.gameObject.AddComponent<Canvas>();
                    top.overrideSorting = true;
                    top.sortingOrder = 5;
                    holder.gameObject.AddComponent<Canvas>();   // its own canvas: the shaking does not rebuild the whole map
                    holder.gameObject.AddComponent<Rattle>();   // the nearest captive also shakes the cage
                }
            }
            else if (kind == LevelKind.Bonus) UiKit.Image(button.transform, "fish", UiKit.Center, new Vector2(0, 6), new Vector2(size * 0.6f, size * 0.6f));
            if (data.PerfectOf(index)) UiKit.Image(button.transform, "badge_perfect", UiKit.TopRight, new Vector2(12, 12), new Vector2(34, 34));
            // Cleared: the small star row below the stone (this game's 3-star rating, the shop's currency).
            if (stars > 0)
                for (int s = 0; s < 3; s++)
                    UiKit.Image(button.transform, s < stars ? "star_full" : "star_empty", UiKit.Bottom,
                        new Vector2((s - 1) * 21, -13 + (s == 1 ? -4 : 0)), new Vector2(26, 26));   // 18 px was under 7dp on a phone
            if (current)
            {
                // The hero stands out on the map (user 18/11: "hải cẩu di chuyển trên ô nhìn chưa nổi bật"): bigger than a stone,
                // a soft shadow under it (CPC: the big penguin on its green disc is the eye's anchor; no arrow).
                var hero = UiKit.Image(root, Art.Has("pup_idle_2") ? "pup_idle_2" : "pup", UiKit.Center, pos + new Vector2(0, size * 0.95f), new Vector2(size * 1.9f, size * 1.9f));
                hero.raycastTarget = false;
                UiKit.StandOut(hero, pos + new Vector2(0, size * 0.05f), size * 1.2f);
                // A still frame and the Bob: the drawn breathing frames differ in size and made the hero flicker.
                // CPC (live 16/11): after a first clear the hero hops along the trail from the stone it just won.
                if (hopPending == index - 1 && previous != null)
                {
                    hopPending = -1;
                    var hop = hero.gameObject.AddComponent<Hop>();
                    // Jumping frames in the air, then the waiting loop on the pad.
                    // CPC: the hero waddles along the trail to the next stone (walk frames), with a small step bounce.
                    var jump = Art.Has("pup_walk_7") ? hero.gameObject.AddComponent<FrameLoop>() : null;
                    jump?.Set("pup_walk_", 8, 12f);
                    hopStarted = true;
                    instance.StartCoroutine(SfxAfter(Hop.Delay, "boing"));
                    hop.landed = () =>
                    {
                        AudioManager.Sfx("pop");
                        if (jump != null) Destroy(jump);
                        Wait(hero);
                    };
                    hop.fromSpot = previous;
                    hop.fromOffset = new Vector2(0, size * 0.95f);
                    hop.to = pos + new Vector2(0, size * 0.95f);
                    hop.height = 14f;      // a waddle, not a leap
                    hop.duration = WalkTime;
                }
                else
                {
                    Wait(hero);
                    var bob = hero.gameObject.AddComponent<Bob>();
                    bob.amplitude = 5f;
                    bob.speed = 3f;
                }
                button.gameObject.AddComponent<Bob>().pulse = true;
            }
        }
    }

    /// <summary>Lets a map stone take a tap only when it is the stone nearest to it (their touch areas overlap).</summary>
    public class NearestStone : MonoBehaviour, ICanvasRaycastFilter
    {
        static readonly System.Collections.Generic.List<RectTransform> all = new System.Collections.Generic.List<RectTransform>();
        RectTransform self;

        void Awake()
        {
            self = (RectTransform)transform;
            all.Add(self);
        }

        void OnDestroy() => all.Remove(self);

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            float mine = Distance(self, screenPoint, eventCamera);
            foreach (var other in all)
                if (other != self && other != null && Distance(other, screenPoint, eventCamera) < mine) return false;
            return true;
        }

        static float Distance(RectTransform stone, Vector2 screenPoint, Camera eventCamera) =>
            (RectTransformUtility.WorldToScreenPoint(eventCamera, stone.position) - screenPoint).sqrMagnitude;
    }

    /// <summary>Shared camera + backdrop + music for menu scenes (same landscape framing as the game).</summary>
    public static class MenuScene
    {
        /// <summary>Camera + backdrop for menu scenes; skyOnly when a painting covers the ground (see Backdrop.Create),
        /// no backdrop at all when a full-screen UI picture hides the world (the map).</summary>
        public static Camera Setup(bool skyOnly = false, bool backdrop = true)
        {
            var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color32(0x11, 0x28, 0x5a, 0xff);
            camGo.transform.position = new Vector3(0f, 0.1f, -10f);
            if (backdrop) Backdrop.Create(cam, skyOnly);
            else cam.cullingMask = 0;
            AudioManager.Instance.PlayMusic("music_reef");
            return cam;
        }
    }
}
