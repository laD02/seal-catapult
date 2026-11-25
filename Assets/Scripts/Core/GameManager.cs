using System;
using System.Collections;
using SealCatapult.Ads;
using SealCatapult.Attack;
using SealCatapult.Launch;
using SealCatapult.Progress;
using SealCatapult.Review;
using SealCatapult.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace SealCatapult.Core
{
    /// <summary>
    /// Runs one level (landscape): Launch (fixed pit view — bat pups out through the top-right gap) → Transition →
    /// Attack (fortress view — escaped pups glide in from the left, tap to dive) → Won/Lost. Also camera, pause, saving stars.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public enum State { Launch, Transition, Attack, Won, Lost }

        /// <summary>Half the visible width that must always fit (narrowest landscape screens are 4:3 tablets).</summary>
        const float DesignHalfWidth = 7.1f;
        /// <summary>Half the visible height on 16:9 and wider; wider screens just see more rock and sky.</summary>
        const float MinViewSize = 5f;
        /// <summary>The fortress view is tighter than the pit so blocks and bears read bigger.</summary>
        const float AttackZoom = 1.05f;    // user 20/11: closer, so the pup and its fling styles read (CPC's penguin is ~9 % of the screen height); 1.1 cut the floor and the bears off while the pup glides (emulator); 13/11 was 1.3, before that 0.9
        /// <summary>How far the arena camera follows the pup up and down from the arena's centre.</summary>
        const float FollowUp = 0.3f, FollowDown = 1.0f;   // 1.6 up pushed the floor and the bears under an arch off the screen (emulator 13/11)
        const float CaveFollow = 1.6f;

        public State Current { get; private set; }
        public int LevelIndex { get; private set; }
        public bool HasNextLevel => LevelIndex + 1 < LevelCatalog.Total;   // ten castles (LevelCatalog.Castles)

        LevelData level;
        GameTuning tuning;
        LaunchPhase launch;
        AttackPhase attack;
        HudView hud;
        PauseMenu pause;
        ResultView result;
        Transform focus;
        CinemachineCamera vcam;
        CinemachineImpulseSource impulse;
        bool paused;

        void Start()
        {
            SaveSystem.EnsureLoaded();
            Skins.SetInPlay(true);   // skins are worn in a level only, before any pup or machine is built
            var db = GameDatabase.Instance;
            LevelIndex = Mathf.Max(0, SceneLoader.CurrentLevel);
            level = LevelCatalog.Get(LevelIndex);   // hand-made or generated: the castles have no end
            tuning = db.tuning;
            GameInput.GameplayEnabled = true;
            Time.timeScale = 1f;
            Application.targetFrameRate = BootView.GameFps;

            BuildCamera();
            Backdrop.Create(Camera.main, skyOnly: true);
            var kept = SceneLoader.KeptExits;
            bool quick = kept != null && kept.Length > 0;
            // An attack-only retry starts on the arena's music: reef first restarted the track twice per retry.
            AudioManager.Instance.PlayMusic(quick ? "music_arena" : "music_reef");
            Fx.ShakeRequested += OnShake;

            hud = HudView.Create(this);
            pause = PauseMenu.Create(this);
            result = ResultView.Create(this);

            attack = new GameObject("AttackPhase").AddComponent<AttackPhase>();
            attack.BearsChanged += hud.SetBears;
            attack.ScoreChanged += hud.SetScore;
            hud.SetStarTarget(level.kind == LevelKind.Bonus ? 0 : level.threeStarScore);
            attack.AmmoChanged += hud.SetPuffers;
            attack.Build(level, tuning);
            Bear.Defeated += OnBearDown;   // after AttackPhase subscribed: BearsLeft is already counted down when this runs
            BossBear.Beaten += OnBossDown;

            int bonusPups = AdMob.Policy.TakeBonusPups(LevelIndex);
            Analytics.GameAnalytics.LevelStart(LevelIndex + 1, quick);
            if (quick)
            {
                // Attack-only retry (the lose card): the same pups fly again, straight into the arena. No pit, no card.
                exits = kept;
                Current = State.Transition;
                Vector3 arena = AttackPhase.ViewCenter;
                focus.position = arena;
                Camera.main.transform.position = new Vector3(arena.x, arena.y, -10f);
                var lens = vcam.Lens;
                lens.OrthographicSize = ViewSize(Camera.main.aspect) * AttackZoom;
                vcam.Lens = lens;
                vcam.PreviousStateIsValid = false;   // cut, not a pan from the pit
                hud.SetPuffers(kept.Length + bonusPups);
                StartCoroutine(GoAttack(kept.Length + bonusPups, quick: true));
                return;
            }
            launch = new GameObject("Launcher").AddComponent<LaunchPhase>();
            launch.QueueChanged += hud.SetPuffers;
            int count = level.pufferCount + SaveSystem.Data.upgrades[(int)UpgradeId.School] + bonusPups;
            Current = State.Launch;
            launch.Begin(tuning, count, OnLaunchDone, level.launcher, level.showTutorial);
            if (level.launcher == LaunchStyle.Bungee) StartCoroutine(After(2.4f, () => hud.Banner(Loc.T("bungee_tip"))));
            else if (level.launcher == LaunchStyle.Drop) StartCoroutine(After(2.4f, () => hud.Banner(Loc.T("drop_tip"))));
        }

        /// <summary>Exit velocity of each pup that left the pit (this attempt's, or the kept ones on an attack-only retry).</summary>
        System.Collections.Generic.IReadOnlyList<Vector2> exits;
        bool dipping;
        int dipId;

        /// <summary>A bear going down: a tick in the hand and a beat of hit-stop; the last bear of the level falls in slow motion.</summary>
        void OnBearDown(Bear bear, int score)
        {
            if (Current != State.Attack) return;
            Haptics.Tap();
            // On a boss level the guards are not the last blow: the boss's own fall is (OnBossDown).
            StartCoroutine(TimeDip(attack.BearsLeft == 0 && level.kind != LevelKind.Boss));
        }

        /// <summary>The boss is not a Bear: its fall is the level's last blow.</summary>
        void OnBossDown(BossBear boss)
        {
            if (Current != State.Attack) return;
            Haptics.Tap();
            StartCoroutine(TimeDip(true));
        }

        /// <summary>Slows time for a moment, in real seconds: a beat of hit-stop, or slow motion for the last blow (which
        /// takes over a hit-stop already running: TNT can fell the last two bears in one frame). Pause owns
        /// Time.timeScale: a dip never starts or ends over it.</summary>
        IEnumerator TimeDip(bool last)
        {
            if (paused || (dipping && !last)) yield break;
            int id = ++dipId;
            dipping = true;
            Time.timeScale = last ? 0.3f : 0.05f;
            yield return new WaitForSecondsRealtime(last ? 0.45f : 0.06f);
            if (id != dipId) yield break;   // a later dip took over and ends it
            dipping = false;
            if (!paused) Time.timeScale = 1f;
        }

        void OnDestroy()
        {
            Fx.ShakeRequested -= OnShake;
            Bear.Defeated -= OnBearDown;
            BossBear.Beaten -= OnBossDown;
            Time.timeScale = 1f;
            GameInput.GameplayEnabled = true;
            Skins.LeavePlay();   // last, and silent: the level's pups are being torn down, nothing here needs repainting
        }

        void BuildCamera()
        {
            var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
            var cam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
            cam.orthographic = true;
            cam.orthographicSize = ViewSize(cam.aspect);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color32(0x11, 0x28, 0x5a, 0xff);
            Vector2 c = LaunchPhase.ViewCenter;
            camGo.transform.position = new Vector3(c.x, c.y, -10f);
            camGo.AddComponent<CinemachineBrain>();

            focus = new GameObject("Camera Focus").transform;
            focus.position = c;
            var vgo = new GameObject("CM Camera");
            vcam = vgo.AddComponent<CinemachineCamera>();
            var lens = LensSettings.Default;
            lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            lens.OrthographicSize = cam.orthographicSize;
            lens.NearClipPlane = 0.1f;
            lens.FarClipPlane = 50f;
            vcam.Lens = lens;
            vcam.Follow = focus;
            var composer = vgo.AddComponent<CinemachinePositionComposer>();
            composer.CameraDistance = 10f;
            composer.Damping = new Vector3(0.9f, 0.9f, 0f);
            vgo.AddComponent<CinemachineImpulseListener>();
            impulse = vgo.AddComponent<CinemachineImpulseSource>();
            impulse.ImpulseDefinition.ImpulseDuration = 0.25f;
        }

        /// <summary>Landscape: design height, grown only when the screen is too narrow for the design width.</summary>
        static float ViewSize(float aspect) => Mathf.Max(MinViewSize, DesignHalfWidth / Mathf.Max(0.3f, aspect));

        void OnShake(float strength) => impulse.GenerateImpulseWithForce(strength);

        void Update()
        {
            if (GameInput.BackPressed())
            {
                // Back pauses a running level and leaves a finished one (the result card has no pause).
                // Not while Retry/Continue is already leaving (wipe or ad): a second load would replace that one.
                if (Current != State.Won && Current != State.Lost) TogglePause();
                else if (!ScreenFader.LoadPending) ToMap();
            }
            if (Current == State.Launch && GameInput.ActionPressed()) launch.TryFire();
            else if (Current == State.Transition && Time.time > missionShownAt + MissionTapGrace && GameInput.ActionPressed()) ContinueMission();
            // Tutorial levels always hint; later levels hint only once a pup has lapped the arena untapped.
            var p = attack.Current;
            bool aimHint = level.showTutorial && Current == State.Launch && launch.CanFire;
            bool diveHint = Current == State.Attack && p != null && !p.Dived && !p.Landed && (level.showTutorial || p.NeedsNudge);
            hud.ShowHint(aimHint || diveHint);
            ShowAim(level.showTutorial && diveHint ? p : null);
            hud.SetHintText(aimHint ? Loc.T(level.launcher == LaunchStyle.Bungee ? "hint_aim_bungee" : "hint_aim") : diveHint ? Loc.T("hint_dive") : null);
            var lens = vcam.Lens;
            bool fortressView = focus.position.x > LaunchPhase.ViewCenter.x + 1f;   // zoom starts with the pan, not before it
            float target = ViewSize(Camera.main != null ? Camera.main.aspect : 16f / 9f) * (fortressView ? AttackZoom : 1f);
            if (Mathf.Abs(lens.OrthographicSize - target) > 0.001f)   // settled: skip the lens write (it dirties Cinemachine)
            {
                lens.OrthographicSize = Mathf.Lerp(lens.OrthographicSize, target, 1f - Mathf.Exp(-3f * Time.deltaTime));
                vcam.Lens = lens;
            }
            // The arena camera follows the pup up and down (the composer damps it).
            if (Current == State.Attack)
            {
                var pup = attack.Current;
                Vector2 c = AttackPhase.ViewCenter;
                // A level with a cave lets the camera follow the pup further down, into the chamber.
                float down = FollowDown + (level.HasCave ? CaveFollow : 0f);
                float y = pup != null ? Mathf.Clamp(pup.transform.position.y, c.y - down, c.y + FollowUp) : c.y;
                // Once a pup has dived, the view stays as low as it got while that pup keeps bouncing (reference §16.2);
                // it rises again for the next pup.
                if (pup != heldFor) { heldFor = pup; heldLow = float.MaxValue; }
                if (pup != null && (pup.Dived || pup.Landed)) { heldLow = Mathf.Min(heldLow, y); y = heldLow; }
                // CPC (live 16/11): the arena view drifts a hair sideways, so the scene never sits dead still. Taps are
                // timed, not aimed at screen points, so the drift moves nothing the player has to hit.
                focus.position = new Vector3(c.x + Mathf.Sin(Time.time * CameraDriftSpeed) * CameraDrift, y, 0f);
            }
        }

        const float CameraDrift = 0.15f, CameraDriftSpeed = 0.35f;   // world units, radians per second

        const int AimDots = 12;
        const float AimStep = 0.13f;   // seconds of dive between two dots
        Transform[] aim;

        /// <summary>Tutorial levels: dots along the path a dive would take from where the pup is now, so "tap early"
        /// can be seen (the dive goes forward at a slant, not straight down). Null hides them.</summary>
        void ShowAim(PufferController pup)
        {
            if (aim == null)
            {
                if (pup == null) return;
                aim = new Transform[AimDots];
                for (int i = 0; i < AimDots; i++)
                {
                    var dot = Art.Spawn("powder", null, Vector2.zero, 0.42f - i * 0.015f, 30);   // 0.26 was lost against the bright sky
                    aim[i] = dot.transform;
                }
            }
            for (int i = 0; i < AimDots; i++)
            {
                Vector2 at = pup != null ? pup.DivePoint((i + 1) * AimStep) : default;
                bool show = pup != null && at.y > AttackPhase.Origin.y;   // nothing drawn under the floor
                var go = aim[i].gameObject;
                if (go.activeSelf != show) go.SetActive(show);
                if (show) aim[i].position = at;
            }
        }

        void OnLaunchDone(int passed)
        {
            if (passed == 0)
            {
                hud.Banner(Loc.T("none_through"));
                StartCoroutine(After(1.4f, () => Finish(false, 0, 0)));
                return;
            }
            StartCoroutine(GoAttack(passed));
        }

        IEnumerator GoAttack(int ammo, bool quick = false)
        {
            Current = State.Transition;
            if (!quick)
            {
                exits = launch.Exits;
                // The water curtain drops over the pit, the view moves to the arena behind it, and it lifts on the mission card
                // (reference §16.1).
                yield return hud.Curtain(true);
                focus.position = AttackPhase.ViewCenter;
                yield return new WaitForSeconds(0.7f);
                StartCoroutine(hud.Curtain(false));
                missionGo = false;
                missionShownAt = Time.time;
                hud.MissionCard(LevelIndex + 1, level.kind == LevelKind.Bonus ? 0 : attack.BearsRequired, SaveSystem.Data.totalBears, ammo, ContinueMission,
                    level.kind == LevelKind.Boss);
                while (!missionGo) yield return null;   // the reference waits for Continue (a tap anywhere also counts)
                hud.DismissMission();
            }
            hud.ShowBears(level.kind != LevelKind.Bonus);   // the fish run has no bears to count
            yield return new WaitForSeconds(0.4f);
            Current = State.Attack;
            AudioManager.Instance.PlayMusic("music_arena");
            hud.Banner(Loc.T("attack"));
            attack.Begin(ammo, SaveSystem.Data.upgrades, Finish, exits);   // each pup's exit sets its entry height and speed (user 13/11: keep the link)
        }

        PufferController heldFor;
        float heldLow;
        bool missionGo;
        float missionShownAt;
        const float MissionTapGrace = 0.6f;   // taps still landing from the last launch must not skip the card

        /// <summary>Leaves the mission card for the arena (the card's Continue button, a tap, or a test bot).</summary>
        public void ContinueMission() => missionGo = true;

        void Finish(bool won, int puffersLeft, int score)
        {
            if (ScreenFader.LoadPending) return;   // Retry/Map was tapped: this level is being left, record nothing
            Current = won ? State.Won : State.Lost;
            GameInput.GameplayEnabled = false;
            Application.targetFrameRate = BootView.StillFps;   // the result card is a still screen (heat, battery)
            hud.ShowHint(false);
            if (won)
            {
                AdMob.Policy.RecordWin();
                AdMob.TryConsentAgain();   // a player offline at launch gets the consent check once back online
            }
            // The fish run ends only when the pups run out, so it is rated on the fish caught, not on pups left.
            var rules = level.kind == LevelKind.Bonus
                ? StarRating.BonusRules(won, attack.FishCollected, attack.FishTotal)   // not won: no pup left the pit
                : StarRating.Rules(won, puffersLeft, score, level.threeStarScore);
            int stars = (rules.passed ? 1 : 0) + (rules.second ? 1 : 0) + (rules.third ? 1 : 0);
            bool perfect = won && attack.Perfect;
            var data = SaveSystem.Data;
            int previous = data.StarsOf(LevelIndex);
            data.Record(LevelIndex, stars, perfect);
            if (previous == 0 && stars > 0) MapView.HopFrom = LevelIndex;   // the map shows the hero hopping on
            // Statistics count every attempt, won or lost.
            data.totalBears += attack.BearsDown;
            foreach (int d in attack.Dives) data.AddDive(d);
            SaveSystem.Save();
            Analytics.GameAnalytics.LevelEnd(LevelIndex + 1, won, stars, exits != null ? exits.Count : 0);
            AudioManager.Sfx(won ? "win" : "lose", 0f);
            bool bonus = level.kind == LevelKind.Bonus;
            result.Show(LevelIndex + 1, won, perfect, rules, bonus ? attack.FishCollected : attack.BearsDown,
                bonus ? attack.FishTotal : attack.BearsRequired, bonus, level.kind == LevelKind.Boss);
            if (InAppReview.ShouldAsk(data, LevelIndex, won)) StartCoroutine(InAppReview.AskOnce());
        }

        /// <summary>Going to the background mid-level opens the pause menu, so the player comes back to a still game
        /// instead of a pup already flying (QA 12/11 on the emulator: the game ran on after returning).</summary>
        void OnApplicationPause(bool goingAway)
        {
            if (goingAway && !paused && pause != null && (Current == State.Launch || Current == State.Attack || Current == State.Transition)) TogglePause();
        }

        /// <summary>Losing focus without going to the background (notification shade, a system dialog) pauses too.
        /// Not in the Editor: clicking another window would pause a play session or a test.</summary>
        void OnApplicationFocus(bool focused)
        {
            if (!focused && !Application.isEditor) OnApplicationPause(true);
        }

        public void TogglePause()
        {
            if (ScreenFader.LoadPending) return;   // Back during the wipe must not unpause the level being left
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;
            Application.targetFrameRate = paused ? BootView.StillFps : BootView.GameFps;   // a frozen screen needs no 60 fps
            GameInput.GameplayEnabled = !paused;
            pause.SetVisible(paused);
        }

        /// <summary>The whole level again, from the launch pit (pause menu, win card).</summary>
        public void Retry()
        {
            // leaving: an ad is opening for this card, its callback decides where to go. LoadPending: another button
            // of this card was tapped in the same frame (two fingers), and a second load would spend the ad's bonus pup.
            if (leaving || ScreenFader.LoadPending) return;
            SceneLoader.PlayLevel(LevelIndex);
        }

        /// <summary>True when pups got out this attempt: the lose card can replay just the attack with them.</summary>
        public bool CanRetryAttack => exits != null && exits.Count > 0;

        /// <summary>The lose card's retry: the attack alone with the same pups (about a second, where the full level
        /// meant batting every pup out again). With no pup out there is nothing to keep: the whole level.</summary>
        public void RetryLost()
        {
            if (leaving || ScreenFader.LoadPending) return;
            if (CanRetryAttack) SceneLoader.RetryAttack(LevelIndex, exits);
            else Retry();
        }

        bool leaving;

        /// <summary>Continue on the win card: the only place an interstitial may show (GAME_DESIGN.md §13b).</summary>
        public void NextLevel()
        {
            if (leaving) return;   // a second tap while the ad opens must not load twice
            leaving = true;
            int cleared = 0;
            foreach (int s in SaveSystem.Data.stars) if (s > 0) cleared++;
            bool chapterEnd = IsChapterEnd(level);
            // CPC flow (user 17/11: "khi sang màn mới phải hiện map để hải cẩu di chuyển sang ô mới đã thì mới sang màn
            // tiếp theo"): the map first, the hero hops on to the next stone, then that level starts by itself.
            // HopFrom is set when the level is first cleared (Record above): a replay of a cleared level shows no star pop.
            MapView.PlayAfterHop = LevelIndex + 1;
            AdMob.InterstitialThen(cleared, chapterEnd, () => SceneLoader.Load(SceneLoader.LevelMap));
        }

        /// <summary>The bonus (fish) and boss levels close a chapter: no interstitial when leaving them.</summary>
        public static bool IsChapterEnd(LevelData level)
        {
            foreach (var o in level.objects)
                if (o.kind == ObjectKind.Fish || o.kind == ObjectKind.Boss) return true;
            return false;
        }

        /// <summary>Lose card ▶: a rewarded ad buys one retry of this level with an extra pup.</summary>
        public void RetryWithBonusPup(Action noAd)
        {
            if (leaving) return;
            leaving = true;
            AdMob.ShowRewarded(earned =>
            {
                if (this == null) return;   // the level was left while the ad was up: nothing here to retry or relabel
                leaving = false;
                if (!earned)
                {
                    noAd();
                    return;
                }
                AdMob.Policy.GrantBonusPup(LevelIndex);
                RetryLost();
            });
        }

        public void ToMap()
        {
            if (leaving || ScreenFader.LoadPending) return;
            SceneLoader.Load(SceneLoader.LevelMap);
        }

        static IEnumerator After(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action();
        }
    }
}
