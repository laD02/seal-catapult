using UnityEngine;

namespace SealCatapult.Core
{
    /// <summary>
    /// The hero: a harp seal pup drawn frame by frame (Resources/Art/pup_&lt;anim&gt;_&lt;i&gt;, cut from the ChatGPT
    /// sheets by Tools/slice_sheets.py). One sprite renderer flips frames per pose; nothing is assembled from parts.
    /// Root origin = body centre. Each animation's canvas has an anchor (see ANCHOR in slice_sheets.py):
    /// bottom = feet, rope = top of the drawn rope (the rope end in game), center = body centre.
    /// </summary>
    public class PupRig : MonoBehaviour
    {
        public enum Pose { Idle, Walk, Crouch, Hang, Flail, Launched, Cheer, Ball, Dizzy, Fly, Dive, NinjaHang, Stuck, Spin, Wave, HatTip, Look, Hop, Dance, Yawn, Sneeze, OpWait, OpPull, DropJump, DropFall, Slide, Bungee, Flung, Roll, ThrowBall, HitBall, Exit }

        enum Anchor { Bottom, Rope, Center }

        /// <summary>World units per 100 art pixels (standing pup = 300 px ≈ 1.38 world units).</summary>
        public const float Scale = 0.40f;   // user 12/11: pups a little smaller, bears a little bigger (was 0.46)
        /// <summary>Rope end above the body centre while hanging, world units.</summary>
        public const float HangReach = 1.278f * Scale;
        /// <summary>Body centre above the feet when standing, world units.</summary>
        public const float StandHeight = 1.5f * Scale;
        public const float BallWidth = 2.28f * Scale;

        const float WalkCycle = 0.88f;     // world units walked per full walk cycle, whatever the frame count (8 frames x 0.11)
        static readonly int[] Breath = { 0, 1, 2, 1 };
        const int IdleStill = 2;            // pup_idle_2: the frame used for the still idle
        const float BreathDepth = 0.018f;   // squash and stretch of the code breath
        const float BreathSpeed = 2.4f;     // radians per second
        float breathPhase;

        sealed class Anim
        {
            public Sprite[] Frames;
            public float Fps;
            public int LoopFrom;            // frames before this play once (e.g. the dizzy "oh!" frame)
            public bool PingPong;           // loop 0-1-2-3-2-1: smoother from four drawings (the queue fidgets)
            public Anchor Anchor;
            public float ArtAngle;          // heading the frames are drawn flying at, degrees (flight sets)
        }

        static Anim opWait, opPull, dropJump, dropFall;
        static Anim throwBall, hitBall, exitHop;   // the home screen's snowball fight (sheet pup_sheet_throw.png)
        static Anim idle, walk, crouch, hang, flail, launched, cheer, dizzy, fly, dive, ninjaHang, stuck, spin, wave, hatTip, look, hop, dance, yawn, sneeze;
        static Anim dizzyBare, idleBare, flailBare, stuckBare;
        static Anim slide;   // arena, after the first hit (always bare-headed by then)
        // Several drawn styles per action (user 17/11: "mỗi loại hoạt ảnh sẽ có nhiều kiểu"): each pup picks one of each
        // at random (Variant), so no two pups fly, crash or tumble alike. Sets not drawn yet are left out.
        static Anim[] launchedSets, flungSets;
        // Sheet T2 (user 17/11: "học kiểu của cpc"; CPC live: a penguin under the ceiling lies on its back, face and belly
        // showing, flippers spread; one on a slope lies flat along it on its belly). Ceiling frames hang from their top
        // line (the back), slope frames stand on their bottom line (the belly).
        static Anim stuckCeiling;
        // Sheets F2 + R2 (user 17/11: "phải vẽ lại văng … cpc có kiểu văng lộn lung tung các hướng và kiểu lộn tròn"):
        // hunched flung pose the code tumbles in the air, and a curled-round pup the code rolls along the ground.
        static Anim flung, roll;
        public static bool StuckIsDrawn => stuckCeiling != null;
        // Sheets Y/Z (user 17/11: swing, bungee and no-rope drop each get their own, gently funny animation): the drawn
        // swing and the upside-down bungee bounce. Their frame follows the rope (DrivenPhase), not a clock.
        static Anim swingDrawn, bungee;

        /// <summary>The swing frames lean with the swing themselves, so the rope should not tilt the body further.</summary>
        public static bool SwingIsDrawn => swingDrawn != null;

        /// <summary>0..1 position in a rope-driven loop (swing: left-to-right then back; bungee: sinking then rising), set
        /// each frame by the launcher. Null lets the frames run on their clock.</summary>
        public float? DrivenPhase { get; set; }

        /// <summary>Rope angle over the swing's reach, −1 (far left) .. 1 (far right), set by the launcher while swinging.
        /// The drawn swing is lopsided (QA 17/11, user: "lắc lư trên dây còn chưa đều 2 bên"), so only its left half is
        /// used (frames 0 = far left .. SwingCentre = hanging straight) and mirrored for the right half: both ends match.</summary>
        public float? SwingLean { get; set; }
        const int SwingCentre = 4;

        SpriteRenderer sr, ballSr;
        Anim current;
        float frameTime, blinkTimer, walkDistance;
        int frame;
        Vector3 lastPos;

        public Pose Current { get; private set; } = Pose.Idle;
        /// <summary>Rope angular speed (rad/s) while hanging; a faster swing flips the hang frames faster.</summary>
        public float SwingSpeed { get; set; }

        public static PupRig Create(Transform parent, Vector2 position, int order)
        {
            var root = new GameObject("Pup").transform;
            root.SetParent(parent, false);
            root.position = position;
            var rig = root.gameObject.AddComponent<PupRig>();
            rig.Build(order);
            return rig;
        }

        static string loadedOutfit;

        static void LoadAnims()
        {
            if (idle != null && loadedOutfit == Art.Outfit) return;
            loadedOutfit = Art.Outfit;
            // One still frame (sheet I's cleanest): ChatGPT drew the four breathing frames at slightly different sizes, and
            // cycling them made the queue flicker (user 16/11). The breath is a gentle squash in code instead.
            idle = Load("idle", 1, 1f, 0, Anchor.Bottom, IdleStill) ?? Load("idle", 4, 3f, 0, Anchor.Bottom);
            walk = LoadAll("walk", 1f);   // front-view waddle (sheet O: 8 frames); steps follow the distance walked
            if (walk != null) walk.PingPong = false;
            // 16-frame redraw (20/11): stand -> crouch once in LaunchPhase's 0.25 s edge pause, then hold the crouch.
            crouch = Art.Has("pup_ready_15") ? Load("ready", 16, 64f, 15, Anchor.Bottom) : Load("ready", 1, 0f, 0, Anchor.Bottom);   // sheet K front view
            hang = Swing(Load("hang", 4, 5f, 0, Anchor.Rope));
            swingDrawn = Loop("swing", 1f, Anchor.Rope);
            if (swingDrawn != null) hang = swingDrawn;   // the drawn swing replaces the plain hang
            bungee = Loop("bungee", 1f, Anchor.Rope) ?? hang;
            // Sheets P and Q (16/11, drawn for these jobs, not borrowed): the operator at its own lever on the cart's deck,
            // and the no-rope launch.
            dropJump = Load("drop_jump", 8, 30f, 7, Anchor.Center);   // over in the 0.25 s hop
            dropFall = Swing(LoadAll("drop_fall", 0.7f)) ?? dropJump;
            if (dropFall != null) dropFall.Anchor = Anchor.Center;   // sheet K: legs kicking on a fast swing
            flail = Load("flail", 4, 10f, 0, Anchor.Center);
            launched = Load("launch", 4, 10f, 0, Anchor.Center);
            // Below their fallbacks, which must be loaded first.
            opWait = LoadAll("op_wait", 2.6f, pingPong: false);
            opPull = Load("op_pull", 8, 12f, 7, Anchor.Bottom);   // once, then hold the last frame
            cheer = Load("cheer", 4, 8f, 0, Anchor.Bottom);
            dizzy = Load("dizzy", 4, 6f, 1, Anchor.Bottom);
            // Sheet E (docs/pup-art-brief-2.md): level flight with flapping flippers, the nose-down turn, getting up after
            // a landing, the ninja hang. Until it is drawn, the nearest older set stands in (see Fallback).
            fly = Load("fly", 4, 9f, 0, Anchor.Center);
            // 16-frame redraw (20/11): 0..9 tip nose-down once (0.25 s), 10..15 loop the held torpedo, scarf fluttering
            // (8..9 are drawn a touch steeper than 10..15: looping them would jerk every cycle).
            dive = (Art.Has("pup_dive_15") ? Load("dive", 16, 40f, 10, Anchor.Center) : Load("dive", 2, 10f, 1, Anchor.Center)) ?? launched;
            ninjaHang = Loop("ninja", 0.8f, Anchor.Center) ?? flail;   // one frame, or the 16-frame hover (20/11)
            // Sheet F (v10): stuck on its back kicking, startled by a stray snowball, curled ball spinning.
            stuck = Load("stuck", 4, 8f, 0, Anchor.Center) ?? dizzy;
            spin = Load("spin", 4, 12f, 0, Anchor.Center) ?? flail;
            idleBare = Load("idle_bare", 1, 1f, 0, Anchor.Bottom, IdleStill) ?? Load("idle_bare", 4, 3f, 0, Anchor.Bottom);   // still + code breath, like idle
            flailBare = Load("flail_bare", 4, 10f, 0, Anchor.Center);
            dizzyBare = Load("dizzy_bare", 4, 6f, 1, Anchor.Bottom);
            // Sheet H (CPC live study 16/11): queue fidgets and the stuck pup that lost its helmet (drawn lying beside it).
            wave = LoadAll("wave", 0.9f) ?? cheer;
            // Snowball fight on the home screen (18/11): drawn for it, played once and held on the last frame.
            throwBall = Load("throw", 8, 11f, 7, Anchor.Bottom) ?? wave;
            hitBall = Load("hit", 8, 10f, 7, Anchor.Bottom);
            // CPC's exit (video 19/11): after its turn the pup turns to the viewer scratching its head, hops and drops out.
            exitHop = Loop("exit", 0.8f, Anchor.Center) ?? flail;
            hatTip = LoadAll("hattip", 1.3f) ?? idle;
            look = LoadAll("look", 1.5f) ?? idle;
            stuckBare = Load("stuck_bare", 4, 6f, 0, Anchor.Center);
            // Sheets U-X (17/11, CPC-like variety): flung rag-doll styles in place of the curled ball,
            // two more launch flights, two more ways to be stuck in the rock (head at the frame top = the rock).
            launchedSets = Sets(Angled(launched, 35f), Angled(Loop("launch_panic", 0.57f, Anchor.Center), 25f),
                Angled(Loop("launch_hero", 0.8f, Anchor.Center), 15f));
            // Sheet X2: head in a snow splat at the frame top (anchored there like a rope frame), kicking or hanging limp.
            stuckCeiling = Loop("stuck_splat", 1f, Anchor.Rope) ?? Loop("stuck_ceiling", 0.8f, Anchor.Rope);
            flung = Loop("flung", 0.65f, Anchor.Center);   // one drawn somersault, CPC's 0.65 s a turn
            // CPC's two fling styles (video tracked at 60 fps, 20/11; user: "quan sát thật kĩ các kiểu văng của cpc và làm
            // lại"): curled tight and tumbling through drawn 3D views, or stretched out and spinning like a pinwheel.
            // Each strike switches style (NextStyle). Both are bare-headed and turn in the drawing, never by code.
            // Four more of this game's own (user 20/11: "sáng tạo nhiều kiểu văng thì càng tốt"): a front-view cartwheel, a
            // panicked doggy paddle, a drill roll and an upright spinning top.
            flungSets = Sets(flung, Loop("fling_spin", 0.65f, Anchor.Center), Loop("fling_star", 0.8f, Anchor.Center),
                Loop("fling_paddle", 0.5f, Anchor.Center), Loop("fling_drill", 0.45f, Anchor.Center), Loop("fling_top", 0.55f, Anchor.Center));
            roll = Loop("roll", 0.8f, Anchor.Center) ?? flung;
            // Sheet J (user 16/11: "nhiều hoạt ảnh ngộ nghĩnh"): more queue fidgets, front view (all cut feet-down,
            // so the hop reads as a squash-and-stretch on the spot).
            hop = LoadAll("hop", 0.8f) ?? wave;
            dance = LoadAll("dance", 0.9f) ?? wave;
            yawn = LoadAll("yawn", 1.5f) ?? look;
            sneeze = LoadAll("sneeze", 1.5f, pingPong: false) ?? hatTip;
            // Sheet S (CPC video 17/11): after the bounces the pup skids on its belly, or somersaults like a rag doll.
            slide = Loop("slide", 0.67f, Anchor.Center) ?? flailBare ?? flail;
        }

        static Anim Angled(Anim a, float degrees)
        {
            if (a != null) a.ArtAngle = degrees;
            return a;
        }

        /// <summary>Heading the frames on show are drawn flying at (0 for sets that are not drawn in flight).</summary>
        public float ArtAngle => current != null ? current.ArtAngle : 0f;

        /// <summary>The drawn sets among <paramref name="sets"/> (missing files are null and dropped).</summary>
        static Anim[] Sets(params Anim[] sets) => System.Array.FindAll(sets, a => a != null);

        /// <summary>This pup's pick from a list of styles (null when none is drawn).</summary>
        Anim Pick(Anim[] sets) => sets == null || sets.Length == 0 ? null : sets[variant % sets.Length];

        /// <summary>Style seed of this pup: which of several drawn sets it uses for each action.</summary>
        public int Variant { get => variant; set => variant = Mathf.Abs(value); }
        int variant;   // rolled in Build: Unity forbids Random in a MonoBehaviour field initializer

        /// <summary>Marks a set to loop back and forth (null passes through).</summary>
        static Anim Swing(Anim a)
        {
            if (a == null) return null;
            // A 16-frame redraw is drawn as the whole sway (out, back, other side, back): it loops straight through, at
            // the pace the 4-frame ping-pong had (6 steps a cycle).
            if (a.Frames.Length >= 16) a.Fps *= 4f / 6f;
            else a.PingPong = true;
            return a;
        }

                /// <summary>A queue fidget with as many frames as were drawn (8 from the two-row sheets, else 4), timed so one
        /// back-and-forth cycle (or one pass when <paramref name="pingPong"/> is off) lasts <paramref name="cycle"/> s.</summary>
        static Anim LoadAll(string name, float cycle, bool pingPong = true)
        {
            int count = 0;
            while (count < 32 && Art.Has($"pup_{name}_{count}")) count++;
            if (count == 0) return null;
            if (count >= 16) pingPong = false;   // a 16-frame redraw is drawn as a seamless loop (18/11), not a there-and-back
            var a = Load(name, count, (pingPong ? count * 2 - 2 : count) / cycle, 0, Anchor.Bottom);
            a.PingPong = pingPong;
            return a;
        }

        /// <summary>A looping set with as many frames as were drawn (16 for the smoothed sets, user 17/11: "tăng khung
        /// hình lên để hoạt ảnh mượt mà"), timed so one loop lasts <paramref name="cycle"/> s whatever the count.</summary>
        static Anim Loop(string name, float cycle, Anchor anchor)
        {
            int count = 0;
            while (count < 32 && Art.Has($"pup_{name}_{count}")) count++;
            return count == 0 ? null : Load(name, count, count / cycle, 0, anchor);
        }

        /// <summary>Loads a frame set; null when its first frame is missing (optional sets fall back to an older set).</summary>
        static Anim Load(string name, int count, float fps, int loopFrom, Anchor anchor, int first = 0)
        {
            if (!Art.Has($"pup_{name}_{first}")) return null;
            if (first == 0 && count > 1)
            {
                // A set redrawn with more frames (16-frame redraws, user 18/11: "tăng khung hình cho tất cả hoạt ảnh") keeps
                // its timing: all its frames, played faster; the held tail starts at the same share of the set.
                int drawn = count;
                while (drawn < 32 && Art.Has($"pup_{name}_{drawn}")) drawn++;
                // Only a full 16-frame redraw: sets that share one frame run (lever_0..3 ready, lever_4..7 pull; reach,
                // grab) must keep their own slice.
                if (drawn > count && drawn >= 16)
                {
                    float k = (float)drawn / count;
                    fps *= k;
                    loopFrom = Mathf.Min(drawn - 1, Mathf.RoundToInt(loopFrom * k + (loopFrom == count - 1 ? k - 1f : 0f)));
                    count = drawn;
                }
            }
            var frames = new Sprite[count];
            for (int i = 0; i < count; i++) frames[i] = Art.Get($"pup_{name}_{first + i}");
            return new Anim { Frames = frames, Fps = fps, LoopFrom = loopFrom, Anchor = anchor };
        }

        Material defaultMaterial;

        /// <summary>Fur and outfit skins: one shared material for every pup (the renderer's own when nothing is worn).</summary>
        string myOutfit;   // the outfit this rig's frames come from (each rig replays once when it changes)

        void WearSkin()
        {
            if (sr == null || ballSr == null) return;
            sr.sharedMaterial = ballSr.sharedMaterial = SealCatapult.Progress.Skins.PupMaterial() ?? defaultMaterial;
            if (myOutfit == Art.Outfit || current == null) return;
            // A drawn outfit is another set of frames: load it (once, shared) and carry on in the same pose.
            LoadAnims();
            myOutfit = Art.Outfit;
            if (Current != Pose.Ball) Play(For(Current));
            Place();
        }

        void OnDestroy() => SealCatapult.Progress.Skins.Changed -= WearSkin;

        void Build(int order)
        {
            LoadAnims();
            sr = new GameObject("Sprite").AddComponent<SpriteRenderer>();
            sr.transform.SetParent(transform, false);
            sr.transform.localScale = Vector3.one * Scale;
            sr.sortingOrder = order;

            ballSr = new GameObject("Ball").AddComponent<SpriteRenderer>();
            ballSr.transform.SetParent(transform, false);
            ballSr.sortingOrder = order;
            SetBallSprite("pup_ball");
            ballSr.enabled = false;
            defaultMaterial = sr.sharedMaterial;
            myOutfit = Art.Outfit;   // LoadAnims above loaded this outfit's frames
            WearSkin();
            SealCatapult.Progress.Skins.Changed += WearSkin;

            variant = Random.Range(0, 1 << 16);
            blinkTimer = Random.Range(1.5f, 4f);
            frameTime = Random.value;
            breathPhase = Random.value * Mathf.PI * 2f;   // pups in the queue breathe out of step
            lastPos = transform.position;
            Play(idle);
            Place();
        }

        public void SetPose(Pose pose)
        {
            if (Current == pose) return;
            Current = pose;
            bool ball = pose == Pose.Ball;
            ballSr.enabled = ball;
            sr.enabled = !ball;
            if (ball) return;
            Play(For(pose));
            Place();
        }

        /// <summary>Switches to the next drawn style of the pose on show (a flung pup changes its tumble at every strike,
        /// as CPC's penguin does), from that style's first frame.</summary>
        public void NextStyle()
        {
            int styles = Current == Pose.Flung && flungSets != null ? flungSets.Length : 0;
            variant += styles > 1 ? Random.Range(1, styles) : 1;   // 1..n-1 steps on: always a different style, at random
            if (Current == Pose.Ball) return;
            Play(For(Current));
            Place();
        }

        bool bare;

        /// <summary>The hat is gone (knocked off in the arena): from now on the bare-headed frame sets are used where
        /// they exist (get up, walk, dizzy, cheer).</summary>
        public void GoBare() => bare = true;

        /// <summary>World height of the frame on show (rotation ignored).</summary>
        public float FrameHeight => sr != null && sr.sprite != null ? sr.sprite.bounds.size.y * sr.transform.lossyScale.y : StandHeight;

        Anim For(Pose pose)
        {
            if (bare)
                switch (pose)
                {
                    case Pose.Idle: return idleBare ?? idle;
                    case Pose.Flail: return flailBare ?? flail;
                    case Pose.Dizzy: return dizzyBare ?? dizzy;
                    case Pose.Stuck: return stuckCeiling ?? stuckBare ?? stuck;
                    case Pose.Flung: return Pick(flungSets) ?? flailBare ?? flail;
                    case Pose.Roll: return roll ?? flailBare ?? flail;
                }
            switch (pose)
            {
                case Pose.Walk: return walk;
                case Pose.Crouch: return crouch;
                case Pose.Hang: return hang;
                case Pose.Bungee: return bungee;
                case Pose.Flail: return flail;
                case Pose.Launched: return Pick(launchedSets) ?? launched;
                case Pose.OpWait: return opWait;
                case Pose.OpPull: return opPull;
                case Pose.DropJump: return dropJump;
                case Pose.DropFall: return dropFall;
                case Pose.Cheer: return cheer;
                case Pose.Dizzy: return dizzy;
                case Pose.Fly: return fly;
                case Pose.Dive: return dive;
                case Pose.NinjaHang: return ninjaHang;
                case Pose.Stuck: return stuck;
                case Pose.Spin: return spin;
                case Pose.Wave: return wave;
                case Pose.ThrowBall: return throwBall;
                case Pose.HitBall: return hitBall;
                case Pose.Exit: return exitHop;
                case Pose.HatTip: return hatTip;
                case Pose.Look: return look;
                case Pose.Hop: return hop;
                case Pose.Dance: return dance;
                case Pose.Yawn: return yawn;
                case Pose.Sneeze: return sneeze;
                case Pose.Slide: return slide;
                case Pose.Flung: return Pick(flungSets) ?? flail;
                default: return idle;
            }
        }

        void Play(Anim anim)
        {
            var sc = sr.transform.localScale;
            sr.transform.localScale = new Vector3(Mathf.Sign(sc.x) * Scale, Scale, 1f);   // undo the idle breath
            current = anim;
            frame = 0;
            frameTime = 0f;
            Show(0);
        }

        void Show(int i)
        {
            var sprite = current.Frames[i];
            if (sr.sprite != sprite) sr.sprite = sprite;
        }

        /// <summary>Puts the canvas anchor on the right point of the root (feet, rope end or body centre).</summary>
        void Place()
        {
            if (sr.sprite == null) return;
            float half = sr.sprite.bounds.extents.y * sr.transform.localScale.y;
            float y;
            switch (current.Anchor)
            {
                case Anchor.Bottom: y = -StandHeight + half; break;
                case Anchor.Rope: y = HangReach - half; break;
                default: y = 0f; break;
            }
            sr.transform.localPosition = new Vector3(0f, y, 0f);
        }

        /// <summary>Curled-up look: plain ball or the Ice Spikes ball.</summary>
        public void SetBallSprite(string sprite)
        {
            ballSr.sprite = Art.Get(sprite);
            Art.FitWidth(ballSr, BallWidth);
        }

        /// <summary>True when the frames are shown as drawn (facing right).</summary>
        public bool FacingRight => sr.transform.localScale.x >= 0f;

        public void Face(bool right)
        {
            var s = sr.transform.localScale;
            s.x = Mathf.Abs(s.x) * (right ? 1f : -1f);
            sr.transform.localScale = s;
        }

        /// <summary>Draw order of the pup's sprites (the exit drops it in front of the rock).</summary>
        public void SetOrder(int order)
        {
            sr.sortingOrder = order;
            ballSr.sortingOrder = order;
        }

        public void SetAlpha(float a)
        {
            var c = sr.color; c.a = a; sr.color = c;
            var b = ballSr.color; b.a = a; ballSr.color = b;
        }

        void Update()
        {
            float dt = Time.deltaTime;
            Vector3 moved = transform.position - lastPos;
            lastPos = transform.position;
            if (Current == Pose.Ball)
            {
                ballSr.transform.Rotate(0, 0, -moved.x / (BallWidth * 0.5f) * Mathf.Rad2Deg);
                return;
            }

            int count = current.Frames.Length;
            if (SwingLean.HasValue && current == swingDrawn && current != null && count > SwingCentre)
            {
                float lean = Mathf.Clamp(SwingLean.Value, -1f, 1f);
                frame = Mathf.Clamp(Mathf.RoundToInt((1f - Mathf.Abs(lean)) * SwingCentre), 0, SwingCentre);
                Face(lean <= 0f);   // the left-half frames, mirrored when the pup is on the right
            }
            else if (DrivenPhase.HasValue && (current == swingDrawn || current == bungee) && current != null)
                frame = Mathf.Clamp((int)(Mathf.Repeat(DrivenPhase.Value, 1f) * count), 0, count - 1);
            else if (current == walk)
            {
                // Steps follow the ground covered, so a pup that stops also stops stepping.
                walkDistance += Mathf.Abs(moved.x) + Mathf.Abs(moved.y);
                frame = (int)(walkDistance / WalkCycle * count) % count;
            }
            else if ((current == idle || current == idleBare) && count == 1)
            {
                frameTime += dt;
                float k = BreathDepth * Mathf.Sin(frameTime * BreathSpeed + breathPhase);
                var sc = sr.transform.localScale;
                sr.transform.localScale = new Vector3(Mathf.Sign(sc.x) * Scale * (1f - k), Scale * (1f + k), 1f);
                Place();   // the feet stay on the ground while the body breathes
                frame = 0;
            }
            else if (current == idle)
            {
                // Breathe 0-1-2-1; frame 3 is the blink.
                frameTime += dt * current.Fps;
                frame = Breath[(int)frameTime % Breath.Length];
                blinkTimer -= dt;
                if (blinkTimer < 0f) frame = 3;
                if (blinkTimer < -0.14f) blinkTimer = Random.Range(1.8f, 4.2f);
            }
            else
            {
                float fps = current == hang ? current.Fps * (1f + Mathf.Abs(SwingSpeed) * 0.5f) : current.Fps;
                frameTime += dt * fps;
                int i = (int)frameTime;
                if (current.PingPong && count > 2)
                {
                    int cycle = count * 2 - 2;
                    int k = i % cycle;
                    frame = k < count ? k : cycle - k;
                }
                else
                {
                    int loop = count - current.LoopFrom;
                    frame = i < count ? i : current.LoopFrom + (i - current.LoopFrom) % loop;
                }
            }
            Show(frame);
        }
    }
}
