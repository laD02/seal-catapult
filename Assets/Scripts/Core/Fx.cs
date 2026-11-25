using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SealCatapult.Core
{
    /// <summary>
    /// Lightweight sprite particles (snow puffs, snowfall, ice shards, powder) and floating score text.
    /// Short-lived sprites come from a pool (<see cref="Particle"/>): the pup trail alone spawned 25-40 objects a second.
    /// </summary>
    public static class Fx
    {
        public static event Action<float> ShakeRequested;

        const int PoolCap = 128;   // spares kept for reuse; more live particles than this are created and destroyed as before
        static readonly Stack<Debris> pool = new Stack<Debris>();

        /// <summary>A pooled, fully reset particle sprite: active, white, upright, sized to <paramref name="size"/> world
        /// units, life 1 s, no motion. Callers set the fields they need. It returns to the pool when its life ends.</summary>
        public static Debris Particle(string sprite, Vector2 at, float size, int order)
        {
            Debris d = null;
            while (pool.Count > 0 && d == null) d = pool.Pop();   // entries destroyed by a scene unload read as null
            if (d == null)
            {
                var sr = Art.Spawn(sprite, null, at, size, order);
                d = sr.gameObject.AddComponent<Debris>();
                d.Renderer = sr;
                d.pooled = true;
            }
            else
            {
                var sr = d.Renderer;
                sr.sprite = Art.Get(sprite);
                sr.sortingOrder = order;
                d.transform.localScale = Vector3.one;
                Art.FitWidth(sr, size);
                d.transform.position = at;
                d.gameObject.SetActive(true);
            }
            d.ResetState();
            return d;
        }

        internal static void Release(Debris d)
        {
            if (pool.Count >= PoolCap) { UnityEngine.Object.Destroy(d.gameObject); return; }
            d.gameObject.SetActive(false);
            pool.Push(d);
        }

        public static void Shake(float strength) => ShakeRequested?.Invoke(strength);

        public static void Burst(string sprite, Vector2 at, int count, float speed, float life, float size,
            float gravity = 0f, float spread = 0.3f, Color? tint = null)
        {
            for (int i = 0; i < count; i++)
            {
                var p = Particle(sprite, at + Random.insideUnitCircle * spread, size * Random.Range(0.6f, 1.2f), 40);
                if (tint.HasValue) p.Renderer.color = tint.Value;
                p.velocity = Random.insideUnitCircle.normalized * Random.Range(speed * 0.4f, speed);
                p.life = life * Random.Range(0.7f, 1.2f);
                p.gravity = gravity;
                p.spin = Random.Range(-360f, 360f);
            }
        }

        /// <summary>Soft snow puffs that swell and fade: impacts, launches, landings.</summary>
        public static void Puff(Vector2 at, int count, float spread = 0.4f)
        {
            for (int i = 0; i < count; i++)
            {
                var p = Particle("snow_puff", at + Random.insideUnitCircle * spread, Random.Range(0.35f, 0.7f), 41);
                p.velocity = Random.insideUnitCircle * 1.6f + Vector2.up * 0.4f;
                p.life = Random.Range(0.5f, 0.9f);
                p.grow = 1.2f;
            }
        }

        /// <summary>A ring of snow puffs flung straight out from a hit (CPC, live 16/11: every ice impact).</summary>
        public static void ImpactRing(Vector2 at, int count = 8, float speed = 3.2f)
        {
            for (int i = 0; i < count; i++)
            {
                float a = (i + Random.value * 0.4f) / count * Mathf.PI * 2f;
                var p = Particle("snow_puff", at, Random.Range(0.3f, 0.45f), 41);
                p.velocity = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * speed;
                p.drag = 5f;
                p.life = 0.45f;
                p.grow = 0.9f;
            }
        }

        /// <summary>A quick white round flash where a pup strikes rock (CPC, live 16/11).</summary>
        public static void Flash(Vector2 at, float size = 0.9f)
        {
            var p = Particle("snow_puff", at, size, 42);
            p.life = 0.16f;
            p.grow = 5f;
            p.drag = 0f;
        }

        /// <summary>CPC's ice break (video at 60 fps, 17/11): the block turns into a heap of small chunks that fills its
        /// own outline and hangs there, then twinkles and is gone; a few chunks pop up and drift down slowly.</summary>
        public static void IceBreak(Vector2 at, Vector2 size)
        {
            int cols = Mathf.Clamp(Mathf.RoundToInt(size.x / ChunkCell), 1, 6);
            int rows = Mathf.Clamp(Mathf.RoundToInt(size.y / ChunkCell), 1, 6);
            Vector2 cell = new Vector2(size.x / cols, size.y / rows);
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                {
                    Vector2 p0 = at - size * 0.5f + new Vector2((x + 0.5f) * cell.x, (y + 0.5f) * cell.y);
                    var d = Particle(Chunk(), p0 + Random.insideUnitCircle * cell.x * 0.15f, Mathf.Min(cell.x, cell.y) * Random.Range(0.75f, 1.05f), 11);
                    d.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
                    d.velocity = Random.insideUnitCircle * 0.12f;
                    d.drag = 2f;
                    d.life = Random.Range(1.1f, 1.5f);
                    d.shrinkOut = true;
                    if (Random.value < 0.35f) Twinkle(p0, d.life * 0.8f);
                }
            for (int i = 0; i < FlyingChunks; i++)
            {
                float a = (90f + Random.Range(-60f, 60f)) * Mathf.Deg2Rad;
                var d = Particle(Chunk(), at, Random.Range(0.16f, 0.24f), 41);
                d.velocity = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * Random.Range(3f, 4.6f);
                d.gravity = 2.5f;
                d.drag = 1.2f;
                d.spin = Random.Range(-200f, 200f);
                d.life = Random.Range(1f, 1.4f);
            }
        }

        /// <summary>CPC's bounce mark (video, 17/11): a big white snow cloud at the contact, a gold ring and a few snowballs
        /// scattering out of it.</summary>
        public static void Splat(Vector2 at)
        {
            var skin = SealCatapult.Progress.Skins.Current(SealCatapult.Progress.SkinKind.Impact);
            if (!skin.IsClassic) Burst(skin.sprite, at, 7, 4.5f, 0.6f, 0.32f, 5f, 0.15f);   // the impact skin, on top of the snow
            var cloud = Particle("snow_puff", at, 0.9f, 42);
            cloud.grow = 1.4f;
            cloud.drag = 0f;
            cloud.life = 0.35f;
            var ring = Particle("impact_ring", at, 0.3f, 43);
            ring.grow = 4f;
            ring.life = 0.3f;
            for (int i = 0; i < 5; i++)
            {
                var b = Particle("snowball", at, Random.Range(0.14f, 0.22f), 42);
                b.velocity = Random.insideUnitCircle.normalized * Random.Range(1.5f, 3f) + Vector2.up;
                b.gravity = 4f;
                b.drag = 1f;
                b.life = Random.Range(0.4f, 0.6f);
            }
        }

        /// <summary>Gold rings and grey smoke balls where a pup smashes ice (CPC, 17/11).</summary>
        public static void HitRings(Vector2 at)
        {
            var big = Particle("impact_ring", at, 0.35f, 42);
            big.grow = 4f;
            big.life = 0.3f;
            var small = Particle("impact_ring_small", at + Random.insideUnitCircle * 0.25f, 0.2f, 42);
            small.grow = 4f;
            small.life = 0.35f;
            for (int i = 0; i < 3; i++)
            {
                var p = Particle("smoke_ball", at + Random.insideUnitCircle * 0.25f, Random.Range(0.22f, 0.34f), 41);
                p.velocity = Random.insideUnitCircle * 0.8f + Vector2.up * 0.3f;
                p.drag = 3f;
                p.grow = 0.6f;
                p.life = Random.Range(0.45f, 0.7f);
            }
        }

        static void Twinkle(Vector2 at, float delay)
        {
            var t = Particle("twinkle", at, 0.22f, 43);
            t.delay = delay;
            t.life = delay + 0.35f;
            t.grow = 1.5f;
        }

        static string Chunk() => ChunkNames[Random.Range(0, ChunkNames.Length)];
        static readonly string[] ChunkNames = { "ice_chunk_0", "ice_chunk_1", "ice_chunk_2", "ice_chunk_3", "ice_chunk_4", "ice_chunk_5", "ice_chunk_6", "ice_chunk_7", "ice_chunk_8" };
        const float ChunkCell = 0.3f;   // world units per heap chunk
        const int FlyingChunks = 5;

        /// <summary>Three little snowballs circling a dazed head (the genre's dizzy mark, report §11).</summary>
        public static void DizzyOrbit(Transform head, Vector2 offset, float duration)
        {
            var go = new GameObject("Dizzy");
            var orbit = go.AddComponent<DizzyOrbit>();
            orbit.Init(head, offset, duration);
        }

        /// <summary>A thin white wind line behind a fast diving pup, along its motion.</summary>
        public static void WindStreak(Vector2 at, Vector2 velocity)
        {
            var d = Particle("ui_bar", at + Random.insideUnitCircle * 0.15f, 0.06f, 14);
            var sr = d.Renderer;
            sr.color = new Color(1f, 1f, 1f, 0.75f);
            float len = Mathf.Clamp(velocity.magnitude * 0.07f, 0.3f, 0.9f);
            if (sr.sprite != null) sr.transform.localScale = new Vector3(sr.transform.localScale.x, len / sr.sprite.bounds.size.y, 1f);
            sr.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f);
            d.SetBaseScale();
            d.velocity = -velocity * 0.05f;
            d.life = 0.22f;
            d.drag = 0f;
        }

        /// <summary>One grey smoke dot left behind by a bouncing pup.</summary>
        public static void Smoke(Vector2 at)
        {
            var p = Particle("powder", at, Random.Range(0.2f, 0.32f), 15);
            p.Renderer.color = new Color(0.62f, 0.64f, 0.7f, 0.9f);
            p.velocity = Vector2.up * 0.4f;
            p.life = 0.5f;
            p.grow = 0.8f;
        }

        /// <summary>CPC's flung penguin wears a small cluster of grey puffs ON its body all the way (video tracked at
        /// 60 fps, 20/11), not a dotted line behind it: short-lived blobs drawn over the pup.</summary>
        public static void BodySmoke(Vector2 at, int order)
        {
            var p = Particle("powder", at + Random.insideUnitCircle * 0.14f, Random.Range(0.26f, 0.4f), order);
            p.Renderer.color = new Color(0.62f, 0.64f, 0.7f, 0.95f);
            p.velocity = Random.insideUnitCircle * 0.25f;
            p.life = 0.16f;
            p.grow = 0.5f;
        }

        /// <summary>A soft snow-dust dot left behind a flying pup.</summary>
        public static void Trail(Vector2 at)
        {
            var skin = SealCatapult.Progress.Skins.Current(SealCatapult.Progress.SkinKind.Trail);
            var p = Particle(skin.sprite, at + Random.insideUnitCircle * 0.08f, Random.Range(0.18f, 0.3f), 15);
            p.velocity = Random.insideUnitCircle * 0.3f;
            p.life = Random.Range(0.35f, 0.55f);
            p.grow = 0.6f;
            if (skin.IsClassic) return;
            // A trail skin is a drawn shape, not a dust dot: it tumbles and lingers a little longer so it reads.
            p.spin = Random.Range(-180f, 180f);
            p.life += 0.2f;
            p.grow = 0.2f;
        }

        /// <summary>Celebration burst of stars and snow around a point (results, perfect level).</summary>
        public static void Celebrate(Vector2 at)
        {
            Burst("star_full", at, 14, 6f, 1.4f, 0.35f, 3f, 0.5f);
            Burst("snowflake", at, 24, 7f, 1.6f, 0.2f, 2f, 0.6f);
        }

        static TMP_FontAsset font;
        static Material outline;

        /// <param name="tilt">Degrees, anticlockwise: CPC's call-outs sit tilted.</param>
        /// <param name="rise">Upward drift, world units per second; 0 holds still.</param>
        public static void FloatingText(string text, Vector2 at, Color color, float size = 5f, float life = 1.1f,
            float tilt = 0f, float rise = 1.4f)
        {
            // Pooled like the sprites: every call-out made a new GameObject and TextMeshPro and destroyed it a second later.
            Debris d = null;
            while (textPool.Count > 0 && d == null) d = textPool.Pop();   // entries destroyed by a scene unload read as null
            if (d == null)
            {
                var go = new GameObject("FloatText");
                var made = go.AddComponent<TextMeshPro>();
                if (font == null)
                {
                    font = Resources.Load<TMP_FontAsset>("Fonts/Baloo2 SDF");
                    outline = Resources.Load<Material>("Fonts/Baloo2 SDF Outline");
                    if (font == null) Debug.LogError("[SealCatapult] Missing Resources/Fonts/Baloo2 SDF: floating text falls back to TMP's default font (no Vietnamese glyphs).");
                }
                if (font != null) made.font = font;
                made.alignment = TextAlignmentOptions.Center;
                made.sortingOrder = 60;
                if (outline != null) made.fontSharedMaterial = outline;
                d = go.AddComponent<Debris>();
                d.pooledText = made;
            }
            else d.gameObject.SetActive(true);
            var tmp = d.pooledText;
            d.transform.localScale = Vector3.one;   // the last use left its pop scale
            d.transform.position = at;
            d.ResetState();
            d.transform.rotation = Quaternion.Euler(0, 0, tilt);
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            d.velocity = new Vector2(0, rise);
            d.life = life;
            d.text = tmp;
        }

        const int TextPoolCap = 12;
        static readonly Stack<Debris> textPool = new Stack<Debris>();

        internal static void ReleaseText(Debris d)
        {
            if (textPool.Count >= TextPoolCap) { UnityEngine.Object.Destroy(d.gameObject); return; }
            d.gameObject.SetActive(false);
            textPool.Push(d);
        }
    }

    public class DizzyOrbit : MonoBehaviour
    {
        const int Count = 3;
        readonly SpriteRenderer[] balls = new SpriteRenderer[Count];
        Transform head;
        Vector2 offset;
        float life, age;

        public void Init(Transform target, Vector2 at, float duration)
        {
            head = target;
            offset = at;
            life = duration;
            for (int i = 0; i < Count; i++) balls[i] = Art.Spawn("snowball", transform, Vector2.zero, 0.2f, 45);
        }

        void Update()
        {
            age += Time.deltaTime;
            if (head == null || age > life)
            {
                Destroy(gameObject);
                return;
            }
            Vector2 c = (Vector2)head.position + offset;
            for (int i = 0; i < Count; i++)
            {
                float a = age * 7f + i * Mathf.PI * 2f / Count;
                balls[i].transform.position = c + new Vector2(Mathf.Cos(a) * 0.32f, Mathf.Sin(a) * 0.11f);
                balls[i].sortingOrder = Mathf.Sin(a) > 0f ? 25 : 45;   // passes behind the head at the back of the circle
            }
        }
    }

    public class Debris : MonoBehaviour
    {
        public Vector2 velocity;
        public float life = 1f;
        public float gravity;
        public float spin;
        public bool wobble;
        public float grow;          // extra scale per second (snow puffs swell)
        public float drag = 1.5f;   // air resistance, per second
        public bool shrinkOut;      // shrinks away over the fade instead of only fading (crushed ice)
        public float delay;         // hidden until this age (a twinkle that waits for the heap to go)
        public TMP_Text text;
        public SpriteRenderer Renderer;
        internal bool pooled;
        internal TMP_Text pooledText;   // set on a pooled call-out (Fx.FloatingText): it goes back to the text pool

        const float PopTime = 0.22f;   // text bursts in with an overshoot instead of appearing flat

        /// <summary>Ease-out-back: 0 → about 1.15 → 1, then holds at 1.</summary>
        static float PopIn(float k)
        {
            if (k >= 1f) return 1f;
            const float c = 2.2f;
            float t = k - 1f;
            return 1f + (c + 1f) * t * t * t + c * t * t;
        }

        SpriteRenderer sr;
        float age;
        float phase;
        Vector3 baseScale;
        float startAlpha = -1f;   // alpha at spawn, read on the first Update
        float lastAlpha = -1f;

        void Start()
        {
            if (pooled) return;   // pooled particles are set up by ResetState on every reuse
            sr = GetComponent<SpriteRenderer>();
            phase = Random.value * 10f;
            baseScale = transform.localScale;
        }

        /// <summary>Back to a fresh particle (called by <see cref="Fx.Particle"/> on every reuse).</summary>
        internal void ResetState()
        {
            sr = Renderer;
            velocity = Vector2.zero;
            life = 1f;
            gravity = 0f;
            spin = 0f;
            wobble = false;
            grow = 0f;
            drag = 1.5f;
            shrinkOut = false;
            delay = 0f;
            if (sr != null) sr.enabled = true;
            text = null;
            age = 0f;
            phase = Random.value * 10f;
            transform.rotation = Quaternion.identity;
            if (sr != null) sr.color = Color.white;
            baseScale = transform.localScale;
            startAlpha = -1f;
            lastAlpha = -1f;
        }

        /// <summary>Call after changing the scale of a freshly taken particle so growth starts from the new size.</summary>
        public void SetBaseScale() => baseScale = transform.localScale;

        void Update()
        {
            float dt = Time.deltaTime;
            age += dt;
            if (age >= life)
            {
                if (pooledText != null) Fx.ReleaseText(this);
                else if (pooled) Fx.Release(this);
                else Destroy(gameObject);
                return;
            }
            if (age < delay)
            {
                if (sr != null && sr.enabled) sr.enabled = false;
                return;
            }
            if (sr != null && !sr.enabled) sr.enabled = true;
            velocity += Vector2.down * gravity * dt;
            velocity *= Mathf.Max(0f, 1f - drag * dt);
            var pos = (Vector2)transform.position + velocity * dt;
            if (wobble) pos.x += Mathf.Sin((age + phase) * 2.5f) * 0.006f;
            transform.position = pos;
            transform.Rotate(0, 0, spin * dt);
            if (grow > 0f) transform.localScale = baseScale * (1f + grow * (age - delay));
            if (shrinkOut) transform.localScale = baseScale * Mathf.Clamp01((life - age) / (life * 0.4f));
            if (text != null) transform.localScale = baseScale * PopIn(age / PopTime);
            // The fade scales the alpha the caller spawned with (smoke 0.9, steam 0.95) instead of resetting it to 1.
            if (startAlpha < 0f) startAlpha = text != null ? text.alpha : sr != null ? sr.color.a : 1f;
            float alpha = startAlpha * (1f - Mathf.Clamp01((age - life * 0.6f) / (life * 0.4f)));
            if (alpha == lastAlpha) return;   // no renderer write while the particle is fully opaque
            lastAlpha = alpha;
            if (sr != null) { var c = sr.color; c.a = alpha; sr.color = c; }
            if (text != null) text.alpha = alpha;
        }
    }
}
