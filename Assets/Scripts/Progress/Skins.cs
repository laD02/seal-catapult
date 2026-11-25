using System;
using System.Collections.Generic;
using SealCatapult.Core;
using UnityEngine;

namespace SealCatapult.Progress
{
    /// <summary>Skin categories, in shop tab order. Saved by index (SaveData.worn): append only.</summary>
    public enum SkinKind { Fur, Outfit, Ball, Trail, Impact, Launcher, Rope }

    /// <summary>One skin: looks only, never gameplay. What the fields mean depends on the kind.</summary>
    public sealed class SkinDef
    {
        public readonly string id;        // "<kind prefix>_<name>"; the name part is also its Loc key ("sk_<name>")
        public readonly SkinKind kind;
        public readonly int price;        // stars; 0 = the classic everyone owns
        public readonly string sprite;    // Ball: the ball's picture. Trail/Impact: the particle's picture. Launcher/Outfit: drawn set's prefix. Rope: drawn strip
        public readonly Color color;      // Fur: fur tint. Trail/Impact: particle tint
        public readonly Vector3 hsv;      // hue shift 0..1, saturation ×, value × (no skin uses it now: every launcher and rope is drawn)

        public SkinDef(string id, SkinKind kind, int price, string sprite = null, Color? color = null, Vector3? hsv = null)
        {
            this.id = id;
            this.kind = kind;
            this.price = price;
            this.sprite = sprite;
            this.color = color ?? Color.white;
            this.hsv = hsv ?? new Vector3(0f, 1f, 1f);
        }

        public bool IsClassic => price == 0;

        public string NameKey => "sk_" + id.Substring(id.IndexOf('_') + 1);
    }

    /// <summary>The skin catalogue and what the player wears. Skins cost stars, the same wallet as the upgrades
    /// (user 23/11: "bỏ cá đi, mua bằng sao"). Fur colours are done by Resources/Shaders/Recolor, so a skin needs no new frames of the hand-drawn pup.</summary>
    public static class Skins
    {
        public const int KindCount = 7;

        static Color C(int r, int g, int b) => new Color32((byte)r, (byte)g, (byte)b, 255);
        static Vector3 H(float hue, float sat = 1f, float val = 1f) => new Vector3(hue, sat, val);

        // At most nine per kind: one shop page shows a whole kind without scrolling (ShopView's 3×3 grid).
        public static readonly SkinDef[] All =
        {
            new SkinDef("fur_classic", SkinKind.Fur, 0),
            new SkinDef("fur_silver", SkinKind.Fur, 1, color: C(196, 206, 222)),
            new SkinDef("fur_cream", SkinKind.Fur, 1, color: C(255, 236, 184)),
            new SkinDef("fur_pink", SkinKind.Fur, 2, color: C(255, 198, 220)),
            new SkinDef("fur_sky", SkinKind.Fur, 2, color: C(180, 224, 255)),
            new SkinDef("fur_mint", SkinKind.Fur, 3, color: C(186, 244, 212)),
            new SkinDef("fur_lilac", SkinKind.Fur, 3, color: C(218, 198, 255)),
            new SkinDef("fur_peach", SkinKind.Fur, 4, color: C(255, 204, 158)),
            new SkinDef("fur_coal", SkinKind.Fur, 5, color: C(96, 100, 120)),

            // Whole outfits, drawn frame by frame (user 23/11: skins must look different, not recoloured): `sprite` is the
            // outfit's frame prefix (Tools/slice_sheets.py --outfit <name> -> <name>_pup_<anim>_<i>).
            new SkinDef("fit_classic", SkinKind.Outfit, 0),
            new SkinDef("fit_pirate", SkinKind.Outfit, 5, "pirate"),
            new SkinDef("fit_ninja", SkinKind.Outfit, 5, "ninja"),

            // The ball on the whacker's arm.
            new SkinDef("ball_classic", SkinKind.Ball, 0, "snowball"),
            new SkinDef("ball_stone", SkinKind.Ball, 1, "ball_stone"),
            new SkinDef("ball_melon", SkinKind.Ball, 2, "ball_melon"),
            new SkinDef("ball_coconut", SkinKind.Ball, 2, "ball_coconut"),
            new SkinDef("ball_beach", SkinKind.Ball, 3, "ball_beach"),
            new SkinDef("ball_bowling", SkinKind.Ball, 3, "ball_bowling"),
            new SkinDef("ball_disco", SkinKind.Ball, 4, "ball_disco"),
            new SkinDef("ball_puffer", SkinKind.Ball, 5, "ball_puffer"),
            new SkinDef("ball_golden", SkinKind.Ball, 5, "ball_golden"),

            // What a flying pup leaves behind.
            new SkinDef("trail_classic", SkinKind.Trail, 0, "powder"),
            new SkinDef("trail_flakes", SkinKind.Trail, 1, "snowflake"),
            new SkinDef("trail_stars", SkinKind.Trail, 2, "star_full"),
            new SkinDef("trail_petals", SkinKind.Trail, 2, "petal"),
            new SkinDef("trail_leaves", SkinKind.Trail, 3, "leaf"),
            new SkinDef("trail_bubbles", SkinKind.Trail, 3, "fx_bubble"),
            new SkinDef("trail_hearts", SkinKind.Trail, 4, "fx_heart"),
            new SkinDef("trail_notes", SkinKind.Trail, 4, "fx_note"),
            new SkinDef("trail_flames", SkinKind.Trail, 5, "fx_flame"),

            // What bursts out where a pup crashes.
            new SkinDef("hit_classic", SkinKind.Impact, 0, "snowball"),
            new SkinDef("hit_stars", SkinKind.Impact, 1, "star_full"),
            new SkinDef("hit_flakes", SkinKind.Impact, 2, "snowflake"),
            new SkinDef("hit_blossoms", SkinKind.Impact, 2, "fx_blossom"),
            new SkinDef("hit_bubbles", SkinKind.Impact, 3, "fx_bubble"),
            new SkinDef("hit_hearts", SkinKind.Impact, 3, "fx_heart"),
            new SkinDef("hit_notes", SkinKind.Impact, 4, "fx_note"),
            new SkinDef("hit_bolts", SkinKind.Impact, 4, "fx_bolt"),
            new SkinDef("hit_fish", SkinKind.Impact, 5, "fish"),

            // The whacker cart, every one drawn anew on the classic silhouette (user 23/11: skins must look different, not
            // recoloured). ChatGPT edits of catapult_pair: <sprite>_base and <sprite>_arm, Tools/cut_props.py.
            new SkinDef("cart_classic", SkinKind.Launcher, 0),
            new SkinDef("cart_bamboo", SkinKind.Launcher, 2, "cart_bamboo"),
            new SkinDef("cart_candy", SkinKind.Launcher, 3, "cart_candy"),
            new SkinDef("cart_pirate", SkinKind.Launcher, 3, "cart_pirate"),
            new SkinDef("cart_ice", SkinKind.Launcher, 4, "cart_ice"),
            new SkinDef("cart_rocket", SkinKind.Launcher, 4, "cart_rocket"),
            new SkinDef("cart_lava", SkinKind.Launcher, 4, "cart_lava"),
            new SkinDef("cart_dragon", SkinKind.Launcher, 5, "cart_dragon"),
            new SkinDef("cart_royal", SkinKind.Launcher, 5, "cart_royal"),

            // The swing rope, each drawn as its own seamless strip (Assets/Art/Reference/rope_skins.png, Tools/cut_ropes.py).
            new SkinDef("rope_classic", SkinKind.Rope, 0),
            new SkinDef("rope_vine", SkinKind.Rope, 1, "rope_vine"),
            new SkinDef("rope_candy", SkinKind.Rope, 1, "rope_candy"),
            new SkinDef("rope_ribbon", SkinKind.Rope, 2, "rope_ribbon"),
            new SkinDef("rope_hazard", SkinKind.Rope, 2, "rope_hazard"),
            new SkinDef("rope_ice", SkinKind.Rope, 3, "rope_ice"),
            new SkinDef("rope_rainbow", SkinKind.Rope, 3, "rope_rainbow"),
            new SkinDef("rope_chain", SkinKind.Rope, 4, "rope_chain"),
            new SkinDef("rope_magic", SkinKind.Rope, 4, "rope_magic"),
        };

        static readonly Dictionary<string, SkinDef> byId = Index();
        static readonly SkinDef[] classics = Classics();

        static Dictionary<string, SkinDef> Index()
        {
            var map = new Dictionary<string, SkinDef>(All.Length);
            foreach (var def in All) map.Add(def.id, def);
            return map;
        }

        static SkinDef[] Classics()
        {
            var result = new SkinDef[KindCount];
            foreach (var def in All)
                if (def.IsClassic) result[(int)def.kind] = def;
            return result;
        }

        public static IEnumerable<SkinDef> Of(SkinKind kind)
        {
            foreach (var def in All)
                if (def.kind == kind) yield return def;
        }

        /// <summary>Null for an unknown id.</summary>
        public static SkinDef Find(string id) => id != null && byId.TryGetValue(id, out var def) ? def : null;

        public static SkinDef Classic(SkinKind kind) => classics[(int)kind];

        public static bool Owns(SaveData data, SkinDef def) => def.IsClassic || Array.IndexOf(data.skins, def.id) >= 0;

        /// <summary>The skin worn for a kind; its classic when nothing (or nothing valid) is stored.</summary>
        public static SkinDef Worn(SaveData data, SkinKind kind)
        {
            int k = (int)kind;
            var def = k < data.worn.Length ? Find(data.worn[k]) : null;
            return def != null && def.kind == kind && Owns(data, def) ? def : classics[k];
        }

        public static bool CanBuy(SaveData data, SkinDef def) => !Owns(data, def) && Shop.AvailableStars(data) >= def.price;

        /// <summary>Spends stars, adds the skin and puts it on. Caller persists.</summary>
        public static bool TryBuy(SaveData data, SkinDef def)
        {
            if (!CanBuy(data, def)) return false;
            data.spentStars += def.price;
            var grown = new string[data.skins.Length + 1];
            data.skins.CopyTo(grown, 0);
            grown[grown.Length - 1] = def.id;
            data.skins = grown;
            return Wear(data, def);
        }

        /// <summary>Puts an owned skin on. Caller persists.</summary>
        public static bool Wear(SaveData data, SkinDef def)
        {
            if (!Owns(data, def)) return false;
            if (data.worn.Length < KindCount) Array.Resize(ref data.worn, KindCount);
            data.worn[(int)def.kind] = def.IsClassic ? "" : def.id;
            if (data == SaveSystem.Data) NotifyChanged();
            return true;
        }

        /// <summary>Raised when the worn set changes: pictures of the pup on screen repaint.</summary>
        public static event Action Changed;

        /// <summary>The worn set changed or the save was replaced (load, Erase progress): the outfit's frames are
        /// switched in, then everything on screen repaints.</summary>
        public static void NotifyChanged()
        {
            Art.SetOutfit(OutfitPrefix(Current(SkinKind.Outfit)));
            Changed?.Invoke();
        }

        /// <summary>"pirate_" for a drawn outfit, "" for the classic.</summary>
        public static string OutfitPrefix(SkinDef outfit) => outfit.sprite != null ? outfit.sprite + "_" : "";

        /// <summary>Editor play mode without a domain reload keeps statics: drop last play's listeners and materials.</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            Changed = null;
            InPlay = false;
            Art.SetOutfit("");
            materials.Clear();
            shader = null;
        }

        /// <summary>Saved data is untrusted: unknown, repeated or free ids are dropped, and nothing is worn that is not owned.</summary>
        public static void Normalize(SaveData data)
        {
            var owned = new List<string>();
            if (data.skins != null)
                foreach (string id in data.skins)
                {
                    var def = Find(id);
                    if (def != null && !def.IsClassic && !owned.Contains(id)) owned.Add(id);
                }
            data.skins = owned.ToArray();
            var worn = new string[KindCount];
            for (int k = 0; k < KindCount; k++)
            {
                var def = data.worn != null && k < data.worn.Length ? Find(data.worn[k]) : null;
                worn[k] = def != null && (int)def.kind == k && !def.IsClassic && owned.Contains(def.id) ? def.id : "";
            }
            data.worn = worn;
        }

        // ---- what the game draws ----

        /// <summary>Skins show only while a level is played, phase 1 and phase 2 (user 23/11: "skin chỉ thay ở pha 1 và
        /// pha 2 thôi"): the menu, the map and their pups keep the classic look. GameManager switches this on and off;
        /// the shop shows its previews from the save itself (<see cref="Worn"/>).</summary>
        public static bool InPlay { get; private set; }

        public static void SetInPlay(bool playing)
        {
            if (InPlay == playing) return;
            InPlay = playing;
            NotifyChanged();
        }

        /// <summary>The level is closing: back to the classic look without raising <see cref="Changed"/> (its pups are
        /// being destroyed; repainting them would reload every frame set on the teardown frame, or touch dead renderers).
        /// The next scene builds its pictures fresh.</summary>
        public static void LeavePlay()
        {
            InPlay = false;
            Art.SetOutfit("");
        }

        /// <summary>What the game draws now: the worn skin inside a level, the classic everywhere else.</summary>
        public static SkinDef Current(SkinKind kind) => InPlay ? Worn(SaveSystem.Data, kind) : classics[(int)kind];

        static Shader shader;
        static readonly Dictionary<(Color, Vector3, bool), Material> materials = new Dictionary<(Color, Vector3, bool), Material>();

        /// <summary>A shared recolour material, or null (the renderer's own default) when nothing changes or the shader
        /// is missing.</summary>
        static Material Recolor(Color fur, Vector3 hsv, bool gate)
        {
            if (fur == Color.white && hsv == new Vector3(0f, 1f, 1f)) return null;
            var key = (fur, hsv, gate);
            if (materials.TryGetValue(key, out var material) && material != null) return material;
            if (shader == null) shader = Resources.Load<Shader>("Shaders/Recolor");
            if (shader == null)
            {
                Debug.LogWarning("[SealCatapult] Missing shader Resources/Shaders/Recolor: skins keep their classic colours");
                return null;
            }
            material = new Material(shader) { name = "Recolor" };
            material.SetColor("_Fur", fur);
            material.SetVector("_Hsv", hsv);
            material.SetFloat("_Gate", gate ? 1f : 0f);
            materials[key] = material;
            return material;
        }

        /// <summary>The pup's look: fur tint + outfit colours. Null = as drawn.</summary>
        public static Material PupMaterial(SkinDef fur, SkinDef outfit) => Recolor(fur.color, outfit.hsv, true);

        public static Material PupMaterial() => PupMaterial(Current(SkinKind.Fur), Current(SkinKind.Outfit));

        /// <summary>The cart's two pictures: a drawn launcher skin has its own, a repaint uses the classic ones.</summary>
        public static string CartBase(SkinDef def) => def.sprite != null ? def.sprite + "_base" : "catapult_base";

        public static string CartArm(SkinDef def) => def.sprite != null ? def.sprite + "_arm" : "catapult_arm";

        /// <summary>The rope's tiled picture: a drawn rope skin has its own strip, a repaint uses the classic one.</summary>
        public static string RopeSprite(SkinDef def) => def.sprite ?? "rope";

        /// <summary>Launcher or rope paint. Null = as drawn.</summary>
        public static Material Paint(SkinDef def) => Recolor(Color.white, def.hsv, false);

        /// <summary>True for the sprites that show the pup itself, which wear the fur and outfit skins.</summary>
        public static bool IsPup(string sprite) => Art.IsPup(sprite);
    }
}
