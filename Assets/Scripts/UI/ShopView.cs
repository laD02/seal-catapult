using System;
using SealCatapult.Core;
using SealCatapult.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SealCatapult.UI
{
    /// <summary>The shop page (design 1 of Assets/Art/Reference/shop_mockups.png, user 23/11): a tab list on the left,
    /// a framed 3×3 card grid in the middle, the selected item large on its ice podium on the right, over the bright
    /// bay painting, with its one main button. Tab 0 is the
    /// upgrades, the others are the skin kinds; everything is bought with stars. GAME_DESIGN.md "Shop".</summary>
    public class ShopView : MonoBehaviour
    {
        Canvas canvas;
        Action onChanged;
        Transform content;
        int tab;                                    // 0 = upgrades, 1 + SkinKind otherwise
        readonly string[] picked = new string[1 + Skins.KindCount];   // the card selected on each tab (upgrade or skin id)
        string note;                                // "Stars missing: 2" after a refused purchase, until the next tap

        // Layout in reference pixels, checked for the narrowest (4:3, 1663 wide) and the flattest (20:9, 966 tall) screen:
        // tab list 24..324, grid frame 850 wide centred at x = -40 (18 px clear of the list and 22 of the preview at
        // 4:3), preview 400 wide. The body is 792 tall at 20:9: header 96 + three card rows of 222 + 24 fit the frame; the
        // eight tabs at 130 apart (1040) do not, so the list scrolls.
        const float BodyTop = 150f, BodyBottom = 24f;
        const float RailWidth = 300f, TabPitch = UiKit.MinHit, TabHeight = 116f;
        const float FrameWidth = 850f, FrameCentreX = -40f, HeaderHeight = 96f;   // the heading sits under the frame's snow cap
        const float CardPitchX = 262f, CardPitchY = 222f, CardWidth = 240f, CardHeight = 210f;
        const float PreviewWidth = 400f, PreviewHeight = 760f;
        float railScroll = 1f;                      // the tab list's scroll position (1 = top), kept across rebuilds

        static readonly string[] TabKeys = { "tab_up", "tab_fur", "tab_fit", "tab_ball", "tab_trail", "tab_hit", "tab_cart", "tab_rope" };
        static readonly string[] TabIcons = { "star_full", "hud_pup", "pup_hattip_0", "ball_beach", "fx_heart", "fx_bolt", "catapult_base", "rope" };

        public static ShopView Create(Action onChanged)
        {
            var canvas = UiKit.CreateCanvas("Shop", 20);
            var view = canvas.gameObject.AddComponent<ShopView>();
            view.canvas = canvas;
            view.onChanged = onChanged;
            canvas.enabled = false;
            return view;
        }

        public void Open()
        {
            canvas.enabled = true;
            note = null;
            Rebuild();
        }

        void Close()
        {
            canvas.enabled = false;
            if (content != null) Destroy(content.gameObject);   // a disabled canvas still runs its children's animations
            content = null;
        }

        /// <summary>Android Back: closes the shop when it is open. False when there was nothing to close.</summary>
        public bool HandleBack()
        {
            if (!canvas.enabled) return false;
            Close();
            return true;
        }

        /// <param name="animate">False after a tap inside the page: the cards stay put instead of sliding in again.</param>
        void Rebuild(bool animate = true)
        {
            if (content != null) Destroy(content.gameObject);
            var dim = UiKit.Overlay(canvas.transform, "Overlay");
            dim.GetComponent<Image>().color = new Color(0.03f, 0.08f, 0.18f, 1f); // full screen, hides the map
            content = dim;
            // The bright bay of the approved design; the home painting under a veil until that picture exists.
            if (Art.Has("bg_shop")) UiKit.PaintedBackdrop(dim, 0.12f, "bg_shop");
            else UiKit.PaintedBackdrop(dim, 0.72f);
            // Corner controls stay clear of notches and rounded corners, like every other screen.
            var root = UiKit.SafeRoot(canvas);
            root.SetParent(dim, false);
            var data = SaveSystem.Data;

            // Layout A top bar, the same as the map's: Back, title, the stars to spend.
            // The title rides the ice ribbon in the middle, as in the design.
            var bar = UiKit.TopBar(root, "", Close, Shop.AvailableStars(data).ToString(), out _);
            var banner = UiKit.Image(bar, "ribbon", UiKit.Center, new Vector2(0, -4), new Vector2(440, 120));
            var title = UiKit.Text(banner.transform, Loc.T("shop"), 58, UiKit.Center, new Vector2(0, 8), new Vector2(300, 80), Color.white,
                TextAlignmentOptions.Center, true);
            title.textWrappingMode = TextWrappingModes.NoWrap;
            title.enableAutoSizing = true;
            title.fontSizeMin = 30;
            title.fontSizeMax = 58;

            var body = UiKit.Node(root, "Body");
            body.anchorMin = Vector2.zero;
            body.anchorMax = Vector2.one;
            body.offsetMin = new Vector2(0f, BodyBottom);
            body.offsetMax = new Vector2(0f, -BodyTop);

            float height = UiKit.CanvasSize().y - BodyTop - BodyBottom;
            BuildTabs(body, height);
            // The grid's frame: the snow-capped panel with the open tab's picture and name as its heading.
            float frameHeight = Mathf.Min(height, HeaderHeight + 3 * CardPitchY + 24f);
            var frame = UiKit.Place(UiKit.Node(body, "Frame"), UiKit.Center, new Vector2(FrameCentreX, 0), new Vector2(FrameWidth, frameHeight));
            var frameBg = frame.gameObject.AddComponent<Image>();
            frameBg.sprite = Art.Get("ui_panel");
            frameBg.type = Image.Type.Sliced;
            frameBg.raycastTarget = false;
            UiKit.Image(frame, TabIcons[tab], UiKit.TopLeft, new Vector2(48, -46), new Vector2(56, 48)).material = null;
            var heading = UiKit.Text(frame, Loc.T(TabKeys[tab]), 44, UiKit.TopLeft, new Vector2(116, -40), new Vector2(560, 56), Color.white,
                TextAlignmentOptions.Left, true);
            heading.textWrappingMode = TextWrappingModes.NoWrap;
            if (tab == 0) BuildUpgrades(frame, body, data, animate);
            else BuildSkins(frame, body, data, (SkinKind)(tab - 1), animate);
        }

        // ---------- tabs ----------

        /// <summary>The tab list of the design: one column of ice rows in a frame. Eight rows at ≥130 apart need 1040 px,
        /// more than any screen's body, so the list scrolls inside its frame (it opens where it was left).</summary>
        void BuildTabs(Transform body, float height)
        {
            var rail = UiKit.Place(UiKit.Node(body, "Tabs"), new Vector2(0f, 0.5f), new Vector2(24, 0), new Vector2(RailWidth, height));
            var railBg = rail.gameObject.AddComponent<Image>();
            railBg.sprite = Art.Get("ui_card");
            railBg.type = Image.Type.Sliced;
            railBg.color = new Color(1f, 1f, 1f, 0.82f);
            var view = UiKit.Node(rail, "View");
            view.anchorMin = Vector2.zero;
            view.anchorMax = Vector2.one;
            view.offsetMin = new Vector2(0, 12);
            view.offsetMax = new Vector2(0, -12);
            view.gameObject.AddComponent<RectMask2D>();
            var list = UiKit.Node(view, "List");
            list.anchorMin = new Vector2(0f, 1f);
            list.anchorMax = new Vector2(1f, 1f);
            list.pivot = new Vector2(0.5f, 1f);
            list.sizeDelta = new Vector2(0f, TabKeys.Length * TabPitch);
            var scroll = rail.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = view;
            scroll.content = list;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.verticalNormalizedPosition = railScroll;
            scroll.onValueChanged.AddListener(at => railScroll = at.y);

            for (int i = 0; i < TabKeys.Length; i++)
            {
                int index = i;
                bool on = i == tab;
                // The open tab is the gold row and stands larger: size, not only colour.
                var row = UiKit.Place(UiKit.Node(list, "Tab " + TabKeys[i]), UiKit.Top, new Vector2(0, -(i + 0.5f) * TabPitch + TabHeight * 0.5f),
                    new Vector2(RailWidth - 28, TabHeight));
                var bg = row.gameObject.AddComponent<Image>();
                bg.sprite = Art.Get(on ? "ui_button_warm" : "ui_button");
                bg.type = Image.Type.Sliced;
                if (on) row.localScale = Vector3.one * 1.06f;
                var button = row.gameObject.AddComponent<Button>();
                button.transition = Selectable.Transition.None;
                button.onClick.AddListener(() =>
                {
                    AudioManager.Sfx("click");
                    if (tab == index) return;
                    tab = index;
                    note = null;
                    Rebuild();
                });
                // No skin material on a picture inside the scrolling list: the recolour shader is not clipped by its mask.
                var icon = UiKit.Image(row, TabIcons[i], new Vector2(0f, 0.5f), new Vector2(22, 4), new Vector2(66, 60));
                icon.material = null;
                if (TabIcons[i] == "rope")
                {
                    // A thin upright strip otherwise: laid across, and long enough to read as a rope.
                    icon.preserveAspect = false;
                    icon.rectTransform.sizeDelta = new Vector2(16, 84);
                    icon.rectTransform.pivot = UiKit.Center;
                    icon.rectTransform.anchoredPosition = new Vector2(55, 4);
                    icon.rectTransform.localRotation = Quaternion.Euler(0, 0, 60f);
                }
                var label = UiKit.Text(row, Loc.T(TabKeys[i]), 34, new Vector2(0f, 0.5f), new Vector2(98, 4), new Vector2(RailWidth - 28 - 98 - 20, 60),
                    Color.white, TextAlignmentOptions.Left, true);
                label.textWrappingMode = TextWrappingModes.NoWrap;
                label.enableAutoSizing = true;
                label.fontSizeMin = 24;
                label.fontSizeMax = 34;
            }
        }

        // ---------- shared pieces ----------

        /// <summary>A card's centre from the frame's top centre: three columns, rows under the heading.</summary>
        static Vector2 Cell(int index) => new Vector2((index % 3 - 1) * CardPitchX, -HeaderHeight - (index / 3 + 0.5f) * CardPitchY);

        /// <summary>One grid card: the picture area is left to the caller, the name and the bottom strip are placed here.
        /// The selected card wears a thick gold rim and stands larger (shape and size, not only colour).</summary>
        RectTransform Card(Transform frame, int index, string name, bool selected, bool animate, Action onTap)
        {
            if (selected)
            {
                // The card's own rounded shape, tinted gold, behind it (a flat bar showed square corners).
                var rim = UiKit.Image(frame, "ui_card", UiKit.Top, Cell(index), new Vector2(CardWidth + 30, CardHeight + 30));
                rim.rectTransform.pivot = UiKit.Center;
                rim.type = Image.Type.Sliced;
                rim.preserveAspect = false;
                rim.color = Gold;
            }
            var card = UiKit.Place(UiKit.Node(frame, "Card " + name), UiKit.Top, Cell(index), new Vector2(CardWidth, CardHeight));
            card.pivot = UiKit.Center;
            var bg = card.gameObject.AddComponent<Image>();
            bg.sprite = Art.Get("ui_card");
            bg.type = Image.Type.Sliced;
            if (selected) card.localScale = Vector3.one * 1.06f;
            var button = card.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(() =>
            {
                AudioManager.Sfx("click");
                note = null;
                onTap();
                Rebuild(animate: false);
            });
            if (animate && !selected) card.gameObject.AddComponent<SlideIn>().delay = 0.04f * index;   // the chosen card stays on its rim
            var label = UiKit.Text(card, name, 30, UiKit.Bottom, new Vector2(0, 56), new Vector2(CardWidth - 36, 38), UiKit.Ink);
            label.fontStyle = FontStyles.Bold;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.enableAutoSizing = true;
            label.fontSizeMin = 24;
            label.fontSizeMax = 30;
            return card;
        }

        static readonly Color Gold = new Color32(0xff, 0xb4, 0x1e, 0xff);

        /// <summary>The strip at a card's foot: a small picture and a word or price. Faded with a lock when too dear;
        /// <paramref name="warm"/> is the gold strip of the one in use.</summary>
        static void Strip(Transform card, string icon, string text, bool locked = false, bool warm = false)
        {
            var strip = UiKit.Image(card, warm ? "ui_button_warm" : "ui_pill", UiKit.Bottom, new Vector2(0, 12), new Vector2(CardWidth - 48, 48));
            strip.type = Image.Type.Sliced;
            strip.preserveAspect = false;
            UiKit.Image(strip.transform, locked ? "glyph_lock" : icon, new Vector2(0f, 0.5f), new Vector2(12, 0), new Vector2(44, 38));
            var label = UiKit.Text(strip.transform, text, 28, UiKit.Center, new Vector2(24, 1), new Vector2(CardWidth - 130, 42), Color.white,
                TextAlignmentOptions.Center, true);
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.enableAutoSizing = true;
            label.fontSizeMin = 24;
            label.fontSizeMax = 28;
            if (locked) label.alpha = 0.6f;
        }

        /// <summary>The right-hand stage: no frame, the item stands on the ice podium over the bay. Returns the holder
        /// with the podium, the name and the line of text in place; the caller adds the item and the button.</summary>
        RectTransform Preview(Transform body, string title, string line)
        {
            float height = Mathf.Min(PreviewHeight, UiKit.CanvasSize().y - BodyTop - BodyBottom - 16f);
            var panel = UiKit.Place(UiKit.Node(body, "Preview"), new Vector2(1f, 0.5f), new Vector2(-24, 0), new Vector2(PreviewWidth, height));
            if (Art.Has("shop_podium")) UiKit.Image(panel, "shop_podium", UiKit.Top, new Vector2(0, -300), new Vector2(390, 180));
            else
            {
                // The pale snow oval every modal's cast stands on, until the podium is painted.
                var patch = UiKit.Image(panel, "powder", UiKit.Top, new Vector2(0, -300), new Vector2(320, 80));
                patch.preserveAspect = false;
                patch.color = new Color(0.70f, 0.85f, 0.97f, 0.9f);
            }
            var name = UiKit.Text(panel, title, 46, UiKit.Top, new Vector2(0, -486), new Vector2(PreviewWidth - 20, 60), Color.white,
                TextAlignmentOptions.Center, true);
            name.textWrappingMode = TextWrappingModes.NoWrap;
            name.enableAutoSizing = true;
            name.fontSizeMin = 26;
            name.fontSizeMax = 46;
            var text = UiKit.Text(panel, note ?? line, 30, UiKit.Top, new Vector2(0, -546), new Vector2(PreviewWidth - 20, 84), Color.white,
                TextAlignmentOptions.Center, true);
            text.enableAutoSizing = true;
            text.fontSizeMin = 24;
            text.fontSizeMax = 30;
            return panel;
        }

        static readonly Vector2 MainButton = new Vector2(320, 110);
        static readonly Vector2 MainButtonAt = new Vector2(0, 16);

        /// <summary>A price button: number + currency picture. Too dear: greyed AND a lock; it stays tappable so the
        /// tap can say what is missing.</summary>
        static void PriceButton(Transform panel, int price, string currency, bool affordable, Action onClick)
        {
            // The price alone: with the word Buy the label ran into the icon in German and French (QA 15/11).
            var buy = UiKit.Button(panel, price.ToString(), affordable ? ButtonStyle.Warm : ButtonStyle.Teal, UiKit.Bottom, MainButtonAt, MainButton, onClick);
            UiKit.Image(buy.transform, currency, new Vector2(1f, 0.5f), new Vector2(-34, 4), new Vector2(70, 56));
            var text = buy.GetComponentInChildren<TextMeshProUGUI>();
            var rt = text.rectTransform;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - 36f, rt.anchoredPosition.y);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x - 72f, rt.sizeDelta.y);
            if (affordable) return;
            buy.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.8f);
            text.alpha = 0.6f;
            UiKit.Image(buy.transform, "glyph_lock", UiKit.TopLeft, new Vector2(-10, 12), new Vector2(52, 52));
        }

        /// <summary>Nothing to press: a plain label with a tick, not a button.</summary>
        static void DoneLabel(Transform panel, string text)
        {
            var done = UiKit.Image(panel, "ui_pill", UiKit.Bottom, MainButtonAt + new Vector2(0, 10), new Vector2(320, 90));
            done.type = Image.Type.Sliced;
            done.preserveAspect = false;
            UiKit.Image(done.transform, "mark_tick", new Vector2(0f, 0.5f), new Vector2(18, 0), new Vector2(60, 54));
            var label = UiKit.Text(done.transform, text, 38, UiKit.Center, new Vector2(30, 2), new Vector2(220, 80), Color.white,
                TextAlignmentOptions.Center, true);
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.enableAutoSizing = true;
            label.fontSizeMin = 22;
            label.fontSizeMax = 38;
        }

        void Changed()
        {
            SaveSystem.Save();
            onChanged?.Invoke();
            Rebuild(animate: false);
        }

        // ---------- upgrades ----------

        void BuildUpgrades(Transform frame, Transform body, SaveData data, bool animate)
        {
            var upgrades = GameDatabase.Instance.upgrades.upgrades;
            UpgradeDef chosen = upgrades.Find(u => u.id.ToString() == picked[0]) ?? upgrades[0];
            for (int i = 0; i < upgrades.Count; i++)
            {
                var def = upgrades[i];
                int level = Shop.Level(data, def.id);
                var card = Card(frame, i, Loc.T("up_" + def.id), def == chosen, animate, () => picked[0] = def.id.ToString());
                UiKit.Image(card, def.icon, UiKit.Top, new Vector2(-46, -12), new Vector2(100, 100));
                for (int s = 0; s < UpgradeData.MaxLevel; s++)
                {
                    // A level not bought yet is a small faint gem: size as well as shade (colour is never the only signal).
                    bool got = s < level;
                    var pip = UiKit.Image(card, "gem", UiKit.Top, new Vector2(70, -16 - s * 32), got ? new Vector2(32, 32) : new Vector2(20, 20));
                    pip.color = got ? Color.white : new Color(1f, 1f, 1f, 0.35f);
                }
                int cost = Shop.NextCost(data, def);
                if (cost < 0) Strip(card, "mark_tick", Loc.T("maxed"));
                else Strip(card, "star_full", cost.ToString(), !Shop.CanBuy(data, def));
            }

            var panel = Preview(body, Loc.T("up_" + chosen.id), Describe(chosen, data));
            UiKit.Image(panel, chosen.icon, UiKit.Top, new Vector2(0, -95), new Vector2(250, 250)).gameObject.AddComponent<PopIn>();
            int next = Shop.NextCost(data, chosen);
            if (next < 0) DoneLabel(panel, Loc.T("maxed"));
            else PriceButton(panel, next, "star_full", Shop.CanBuy(data, chosen), () => BuyUpgrade(chosen));
        }

        /// <summary>"Lv 2/3: Smash through 3 ice blocks": the level the next purchase gives (the top level once maxed)
        /// and what it really does.</summary>
        static string Describe(UpgradeDef def, SaveData data)
        {
            int shown = Mathf.Min(Shop.Level(data, def.id) + 1, UpgradeData.MaxLevel);
            int effect = Shop.Effect(def.id, shown, GameDatabase.Instance.tuning.basePierce);
            string key = "upd_" + def.id;
            if (def.id == UpgradeId.Blaze && effect >= Shop.BlazeAll) key += "_all";
            else if (effect == 1 && Loc.Has(key + "_one")) key += "_one";   // "+1 bounce", not "+1 bounces"
            return Loc.T("shop_line", Loc.T("lv", shown, UpgradeData.MaxLevel), Loc.T(key, effect));
        }

        void BuyUpgrade(UpgradeDef def)
        {
            var data = SaveSystem.Data;
            if (!Shop.TryBuy(data, def))
            {
                int missing = Shop.NextCost(data, def) - Shop.AvailableStars(data);
                if (missing > 0) note = Loc.T("need_stars", missing);
                Rebuild(animate: false);
                return;
            }
            AudioManager.Sfx("star", 0f);
            Changed();
        }

        // ---------- skins ----------

        void BuildSkins(Transform frame, Transform body, SaveData data, SkinKind kind, bool animate)
        {
            var worn = Skins.Worn(data, kind);
            var chosen = Skins.Find(picked[tab]);
            if (chosen == null || chosen.kind != kind) chosen = worn;
            int i = 0;
            foreach (var def in Skins.Of(kind))
            {
                var skin = def;
                var card = Card(frame, i++, Loc.T(def.NameKey), def == chosen, animate, () => picked[tab] = skin.id);
                Picture(card, def, data, 92f);
                if (def == worn) Strip(card, "mark_tick", Loc.T("worn"), warm: true);
                else if (Skins.Owns(data, def)) Strip(card, "mark_tick", Loc.T("owned"));
                else Strip(card, "star_full", def.price.ToString(), !Skins.CanBuy(data, def));
            }

            var panel = Preview(body, Loc.T(chosen.NameKey), Loc.T("hint_" + kind));
            Stage(panel, chosen, data);
            if (chosen == worn) DoneLabel(panel, Loc.T("worn"));
            else if (Skins.Owns(data, chosen))
                UiKit.Button(panel, Loc.T("equip"), ButtonStyle.Warm, UiKit.Bottom, MainButtonAt, MainButton, () =>
                {
                    Skins.Wear(SaveSystem.Data, chosen);
                    Changed();
                });
            else PriceButton(panel, chosen.price, "star_full", Skins.CanBuy(data, chosen), () => BuySkin(chosen));
        }

        void BuySkin(SkinDef def)
        {
            var data = SaveSystem.Data;
            if (!Skins.TryBuy(data, def))
            {
                int missing = def.price - Shop.AvailableStars(data);
                if (missing > 0) note = Loc.T("need_stars", missing);
                Rebuild(animate: false);
                return;
            }
            AudioManager.Sfx("star", 0f);
            Changed();
        }

        /// <summary>The pup's material with <paramref name="def"/> tried on over what is worn.</summary>
        static Material PupWith(SkinDef def, SaveData data) => Skins.PupMaterial(
            def.kind == SkinKind.Fur ? def : Skins.Worn(data, SkinKind.Fur),
            def.kind == SkinKind.Outfit ? def : Skins.Worn(data, SkinKind.Outfit));

        /// <summary>A skin's small picture in the top of its card, <paramref name="size"/> tall.</summary>
        static void Picture(Transform card, SkinDef def, SaveData data, float size)
        {
            var at = new Vector2(0, -22);
            switch (def.kind)
            {
                case SkinKind.Fur:
                    UiKit.Image(card, "pup_idle_0", UiKit.Top, at, new Vector2(size, size)).material = PupWith(def, data);
                    break;
                case SkinKind.Outfit:
                {
                    // Each outfit's own standing frame, whatever is worn now.
                    var pup = UiKit.Image(card, "pup_idle_0", UiKit.Top, at, new Vector2(size, size));
                    pup.sprite = Art.Plain(Skins.OutfitPrefix(def) + "pup_idle_0") ?? pup.sprite;
                    pup.material = PupWith(def, data);
                    break;
                }
                case SkinKind.Launcher:
                    UiKit.Image(card, Skins.CartBase(def), UiKit.Top, at, new Vector2(size * 1.6f, size)).material = Skins.Paint(def);
                    break;
                case SkinKind.Rope:
                {
                    // The upright strip laid across the card.
                    var rope = UiKit.Image(card, Skins.RopeSprite(def), UiKit.Top, at, new Vector2(size * 0.2f, size * 1.1f));
                    TileRope(rope);
                    rope.rectTransform.pivot = UiKit.Center;
                    rope.rectTransform.anchoredPosition = at + new Vector2(0, -size * 0.5f);   // centred in the picture area
                    rope.rectTransform.localRotation = Quaternion.Euler(0, 0, 60f);
                    rope.material = Skins.Paint(def);
                    break;
                }
                case SkinKind.Trail:
                case SkinKind.Impact:
                    // Three of the particle, growing, as it shows in flight.
                    for (int n = 0; n < 3; n++)
                    {
                        float s = size * (0.4f + 0.16f * n);
                        UiKit.Image(card, def.sprite, UiKit.Top, at + new Vector2((n - 1) * size * 0.5f, -(size - s) * 0.5f), new Vector2(s, s));
                    }
                    break;
                default:
                    UiKit.Image(card, def.sprite, UiKit.Top, at, new Vector2(size, size));
                    break;
            }
        }

        /// <summary>A rope picture repeats its strip along its length, as in the game (one stretched tile looked stubby).</summary>
        static void TileRope(Image rope)
        {
            rope.preserveAspect = false;
            rope.type = Image.Type.Tiled;
            if (rope.sprite != null) rope.pixelsPerUnitMultiplier = rope.sprite.rect.width / Mathf.Max(1f, rope.rectTransform.sizeDelta.x);
        }

        /// <summary>The selected skin shown large on the preview's snow patch on the podium (its top face is 345 below the holder's top).</summary>
        static void Stage(Transform panel, SkinDef def, SaveData data)
        {
            const float Feet = -345f;
            switch (def.kind)
            {
                case SkinKind.Fur:
                case SkinKind.Outfit:
                {
                    // The front-view wave: the whole pup, its outfit in sight. An outfit tried on is shown in its own
                    // frames: they are loaded while that outfit is switched in, then the worn one comes back.
                    string worn = Art.Outfit;
                    Image pup;
                    try
                    {
                        if (def.kind == SkinKind.Outfit) Art.SetOutfit(Skins.OutfitPrefix(def));
                        pup = UiKit.Cast(panel, "pup_wave", UiKit.Top, new Vector2(0, Feet), 250f, 1.6f);
                    }
                    finally
                    {
                        Art.SetOutfit(worn);   // never leave the menu's pups in the outfit tried on
                    }
                    if (pup != null) pup.material = PupWith(def, data);
                    break;
                }
                case SkinKind.Launcher:
                    UiKit.Image(panel, Skins.CartBase(def), UiKit.Top, new Vector2(0, Feet + 200), new Vector2(330, 200)).material = Skins.Paint(def);
                    break;
                case SkinKind.Rope:
                {
                    var rope = UiKit.Image(panel, Skins.RopeSprite(def), UiKit.Top, new Vector2(0, -30), new Vector2(26, 130));
                    TileRope(rope);
                    rope.material = Skins.Paint(def);
                    UiKit.Image(panel, "pup_hang_0", UiKit.Top, new Vector2(0, -130), new Vector2(190, 190));
                    break;
                }
                case SkinKind.Ball:
                    UiKit.Image(panel, def.sprite, UiKit.Top, new Vector2(0, Feet + 200), new Vector2(200, 200)).gameObject.AddComponent<PopIn>();
                    break;
                default:
                    // Trail and impact: the particles drifting round a pup.
                    UiKit.Image(panel, "pup_idle_0", UiKit.Top, new Vector2(0, Feet + 210), new Vector2(210, 210));
                    foreach (var place in Places)
                    {
                        var bit = UiKit.Image(panel, def.sprite, UiKit.Top, new Vector2(place.x, Feet + place.y), new Vector2(place.z, place.z));
                        var bob = bit.gameObject.AddComponent<Bob>();
                        bob.amplitude = 8f;
                        bob.speed = 2f + place.z * 0.03f;
                    }
                    break;
            }
        }

        // x, height above the feet, size: scattered round the pup, clear of its face
        static readonly Vector3[] Places =
        {
            new Vector3(-150, 250, 62), new Vector3(-128, 120, 46), new Vector3(150, 260, 54),
            new Vector3(136, 130, 66), new Vector3(-20, 290, 40),
        };
    }
}
