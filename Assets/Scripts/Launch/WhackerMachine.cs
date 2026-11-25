using System;
using System.Collections;
using SealCatapult.Core;
using UnityEngine;

namespace SealCatapult.Launch
{
    /// <summary>
    /// Snowball catapult (ChatGPT art: catapult_base, catapult_arm, snowball; cut by Tools/cut_props.py) to the right
    /// of the rope, drawn mirrored (it faces the pit). The arm rests pointing
    /// down-left and sweeps up clockwise, like a forehand: the mallet comes up from under the pup's left side
    /// and moves up-right, so a good hit sends the pup toward the exit at the top right.
    /// Contact is real: every frame of the sweep the mallet circle is tested against the pup, and the hit
    /// direction comes from where they touch plus how the mallet is moving. A sweep that never touches the
    /// pup is a whiff. Arm angles are the drawing's local angles (degrees, counter-clockwise before the
    /// mirror); in the world the mallet sits at Hub + Reach * (-cos, sin).
    /// </summary>
    public class WhackerMachine : MonoBehaviour
    {
        // Measured on the cut art: base hub at (380, 62) of 729x426 px (the 16/11 cart with a standing deck at (140, 220)); arm pivot hole at (52, 100) and cup at
        // (630, 75) of 726x156 px, so the arm is drawn ArmWidth wide to make hole → cup = Reach.
        const float BaseWidth = 2.6f;
        const float ArmWidth = 2.26f;
        public const float HubOffsetY = 0.539f;           // hub above the base picture's centre
        const float HubOffsetX = 0.055f;                   // hub right of the picture's centre (before the mirror): the base shifts back
        static readonly Vector2 ArmPivotOffset = new Vector2(0.969f, 0.068f);  // arm picture centre from its pivot hole
        const float CupLift = 0.08f;                       // the cup bowl sits a little above the beam's axis
        const float BallSize = 0.62f;                      // drawn snowball; the hit test still uses MalletRadius
        public const float Reach = 1.8f;     // hub → mallet centre
        public const float MalletRadius = 0.42f;
        /// <summary>Where the operator pup's feet go, from the machine's centre: on the deck at the back, beside the
        /// lever (user 16/11: it stood on the ground next to the machine, "nó phải ở trên máy").</summary>
        public static readonly Vector2 OperatorSeat = new Vector2(0.86f, -0.03f);   // the painted deck (after the mirror)
        const float RestAngle = -55f;
        const float WindupAngle = -68f;
        const float EndAngle = 75f;
        const float SweepTime = 0.24f;
        const float HitSlack = 0.15f;        // forgiving contact: the snowball only has to brush the pup
        const float BallHitSpeed = 10f;      // snowball speed along its arc at contact (only its direction matters downstream)

        Transform pivot;
        Transform baseTr;
        SpriteRenderer cupBall;   // the snowball fixed on the arm's end: it is the mallet
        Vector3 baseScale;
        float reloadTime;
        float angle;

        public bool Ready { get; private set; } = true;
        public Vector2 Hub => pivot.position;

        public static WhackerMachine Create(Vector2 position, Transform parent, float reloadTime)
        {
            var root = new GameObject("Whacker").transform;
            root.SetParent(parent, false);
            root.position = position;
            root.localScale = new Vector3(-1f, 1f, 1f);   // face the pit on the left
            var m = root.gameObject.AddComponent<WhackerMachine>();
            m.reloadTime = reloadTime;
            var cart = Progress.Skins.Current(Progress.SkinKind.Launcher);
            var b = Art.Spawn(Progress.Skins.CartBase(cart), root, new Vector2(-HubOffsetX, 0f), BaseWidth, 8);
            m.baseTr = b.transform;
            m.baseScale = b.transform.localScale;
            m.pivot = new GameObject("ArmPivot").transform;
            m.pivot.SetParent(root, false);
            m.pivot.localPosition = new Vector2(0f, HubOffsetY);
            var arm = Art.Spawn(Progress.Skins.CartArm(cart), m.pivot, ArmPivotOffset, ArmWidth, 9);
            arm.name = "Arm";
            m.cupBall = Art.Spawn(Progress.Skins.Current(Progress.SkinKind.Ball).sprite, m.pivot, new Vector2(Reach, CupLift + BallSize * 0.35f), BallSize, 10);
            var paint = Progress.Skins.Paint(cart);
            if (paint != null) b.sharedMaterial = arm.sharedMaterial = paint;
            m.SetAngle(RestAngle);
            return m;
        }

        void SetAngle(float deg)
        {
            angle = deg;
            pivot.localRotation = Quaternion.Euler(0, 0, deg);
        }

        Vector2 MalletAt(float deg) => Hub + new Vector2(-Mathf.Cos(deg * Mathf.Deg2Rad), Mathf.Sin(deg * Mathf.Deg2Rad)) * Reach;

        /// <summary>
        /// Swing the arm (report §11, frame by frame from the Flash game: the snowball stays fixed on the arm's end and
        /// the arm sweeps up over the top, so the snowball itself smacks the swinging pup). While sweeping, target()
        /// gives the pup's centre (or null); the snowball's arc is sub-sampled so a fast sweep cannot pass through the
        /// pup, and on the first overlap onHit(contactNormal, snowballVelocity) is called once.
        /// </summary>
        public IEnumerator Swing(Func<Vector2?> target, float targetRadius, Action<Vector2, Vector2> onHit)
        {
            Ready = false;
            AudioManager.Sfx("click", 0.1f, 0.6f);
            yield return Rotate(RestAngle, WindupAngle, 0.08f, false);
            AudioManager.Sfx("launch", 0.05f);
            StartCoroutine(Kick());
            bool hit = false;
            float prev = WindupAngle;
            for (float t = 0f; t <= SweepTime && !hit; t += Time.deltaTime)
            {
                float k = Mathf.Min(1f, t / SweepTime);
                float a = Mathf.Lerp(WindupAngle, EndAngle, k * k);   // accelerating swing
                SetAngle(a);
                hit = Contact(prev, a, target, targetRadius, onHit);
                prev = a;
                if (k >= 1f) break;
                yield return null;
            }
            if (!hit) hit = Contact(prev, EndAngle, target, targetRadius, onHit);   // a slow frame jumped the sweep's end
            SetAngle(EndAngle);
            Fx.Puff(cupBall.transform.position, 3, 0.15f);    // CPC: a snow puff off the arm's tip on every shot
            if (!hit) AudioManager.Sfx("splash", 0.1f, 0.4f);  // whiff
            yield return new WaitForSeconds(0.12f);
            yield return Rotate(EndAngle, RestAngle - 6f, Mathf.Max(0.3f, reloadTime), false);
            yield return Rotate(RestAngle - 6f, RestAngle, 0.08f, false);
            Ready = true;
        }

        /// <summary>Tests the snowball along the arc from angle a0 to a1 (degrees) against the pup.</summary>
        bool Contact(float a0, float a1, Func<Vector2?> target, float targetRadius, Action<Vector2, Vector2> onHit)
        {
            var t = target();
            if (!t.HasValue) return false;
            const int Steps = 6;
            for (int s = 1; s <= Steps; s++)
            {
                float a = Mathf.Lerp(a0, a1, s / (float)Steps);
                Vector2 m = MalletAt(a);
                Vector2 d = t.Value - m;
                if (d.magnitude > MalletRadius + targetRadius + HitSlack) continue;
                float rad = a * Mathf.Deg2Rad;
                Vector2 tangent = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));   // d/dθ of (-cos, sin): up and toward the gap
                Fx.Burst("spark", m + d * 0.5f, 8, 4f, 0.25f, 0.22f, 0f, 0.1f, new Color(1f, 0.95f, 0.5f));
                onHit(d.normalized, tangent * BallHitSpeed);
                return true;
            }
            return false;
        }

        IEnumerator Rotate(float from, float to, float duration, bool easeIn)
        {
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float k = t / duration;
                k = easeIn ? k * k : 1f - (1f - k) * (1f - k);
                SetAngle(Mathf.Lerp(from, to, k));
                yield return null;
            }
            SetAngle(to);
        }

        /// <summary>The sled jolts on impact.</summary>
        IEnumerator Kick()
        {
            for (float t = 0f; t < 0.18f; t += Time.deltaTime)
            {
                float k = Mathf.Sin(t / 0.18f * Mathf.PI) * 0.1f;
                baseTr.localScale = new Vector3(baseScale.x * (1f + k), baseScale.y * (1f - k), 1f);
                yield return null;
            }
            baseTr.localScale = baseScale;
        }
    }
}
