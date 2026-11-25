using System;
using SealCatapult.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace SealCatapult.UI
{
    public enum ButtonStyle { Teal, Warm, Gold }

    /// <summary>Small helpers that build the uGUI + TextMeshPro screens in code.</summary>
    public static class UiKit
    {
        public static readonly Color Ink = new Color32(0x1b, 0x2a, 0x49, 0xff);
        public static readonly Color White = Color.white;
        public static readonly Color Dim = new Color(0.03f, 0.08f, 0.18f, 0.86f);
        /// <summary>Numbers and highlights inside panels (the result card's counts, the stats, the mission card).</summary>
        public static readonly Color Accent = new Color32(0x2f, 0x9b, 0xe8, 0xff);
        /// <summary>Secondary text inside panels: credits, the version, small notes.</summary>
        public static readonly Color Muted = new Color32(0x56, 0x63, 0x7b, 0xff);
        /// <summary>Warnings: the "off" slash on a sound button, the captive's cry.</summary>
        public static readonly Color Danger = new Color(0.9f, 0.15f, 0.15f);
        public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 Top = new Vector2(0.5f, 1f);
        public static readonly Vector2 Bottom = new Vector2(0.5f, 0f);
        public static readonly Vector2 TopLeft = new Vector2(0f, 1f);
        public static readonly Vector2 TopRight = new Vector2(1f, 1f);
        public static readonly Vector2 BottomLeft = new Vector2(0f, 0f);
        public static readonly Vector2 BottomRight = new Vector2(1f, 0f);

        static TMP_FontAsset font;

        public static TMP_FontAsset Font
        {
            get
            {
                if (font == null)
                {
                    font = Resources.Load<TMP_FontAsset>("Fonts/Baloo2 SDF");
                    if (font == null) Debug.LogError("[SealCatapult] Missing Resources/Fonts/Baloo2 SDF");
                }
                return font;
            }
        }

        static Material outlineMaterial;

        public static Material OutlineMaterial
        {
            get
            {
                if (outlineMaterial == null)
                {
                    outlineMaterial = Resources.Load<Material>("Fonts/Baloo2 SDF Outline");
                    if (outlineMaterial == null) Debug.LogWarning("[SealCatapult] Missing Resources/Fonts/Baloo2 SDF Outline.mat");
                }
                return outlineMaterial;
            }
        }

        public static Canvas CreateCanvas(string name, int order = 0)
        {
            EnsureEventSystem();
            var go = new GameObject(name, typeof(RectTransform));
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = order;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);  // landscape
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        /// <summary>The size every canvas of <see cref="CreateCanvas"/> has on this screen, in reference pixels (known
        /// before the canvas has been laid out): 1920×1080 at 16:9, 1663×1247 at 4:3, 2147×966 at 20:9.</summary>
        public static Vector2 CanvasSize()
        {
            float w = Mathf.Max(1, Screen.width), h = Mathf.Max(1, Screen.height);
            float scale = Mathf.Sqrt(w / 1920f * (h / 1080f));   // CanvasScaler, match 0.5
            return new Vector2(w, h) / scale;
        }

        static void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            UnityEngine.Object.DontDestroyOnLoad(es);
            // Unity's 10 px is a hair on a 480 dpi phone: a finger that slid that far while tapping lost its tap wherever
            // a scroll view sits behind the button (the map). 0.07 inch instead (11dp): more made the Settings sliders
            // wait too long before they follow a small drag.
            float dpi = Screen.dpi > 0f ? Screen.dpi : 200f;
            es.GetComponent<EventSystem>().pixelDragThreshold = Mathf.Max(10, Mathf.RoundToInt(dpi * 0.07f));
        }

        /// <summary>Child that fits Screen.safeArea, so HUD corners stay clear of phone notches and cutouts.</summary>
        public static RectTransform SafeRoot(Canvas canvas)
        {
            var rt = Stretch(Node(canvas.transform, "SafeArea"));
            rt.gameObject.AddComponent<SafeArea>();
            return rt;
        }

        public static RectTransform Node(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        /// <summary>Space kept free at each end of a button label: 12 px let long words (German) run onto the rounded caps.</summary>
        const float LabelPad = 20f;

        /// <summary>Smallest touch area in reference pixels: about 48dp on a 2400×1080 phone at 480 dpi (scale ~1.1).</summary>
        public const float MinHit = 130f;

        /// <summary>An invisible touch area around a small control, so the tap target is at least <see cref="MinHit"/>
        /// without changing how it looks. Clicks on it reach the control (it is a child).</summary>
        public static void PadHitArea(RectTransform control)
        {
            var size = control.sizeDelta;
            if (size.x >= MinHit && size.y >= MinHit) return;
            var hit = Place(Node(control, "Hit"), Center, Vector2.zero, Vector2.Max(size, new Vector2(MinHit, MinHit)));
            hit.gameObject.AddComponent<Image>().color = Color.clear;
            hit.SetAsFirstSibling();   // behind the control's own pictures
        }

        /// <summary>Anchor + pivot at the same point, then offset and size.</summary>
        public static RectTransform Place(RectTransform rt, Vector2 anchor, Vector2 position, Vector2 size)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            return rt;
        }

        public static RectTransform Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static Image Image(Transform parent, string sprite, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var rt = Place(Node(parent, sprite), anchor, position, size);
            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = Art.Get(sprite);
            img.preserveAspect = true;
            img.raycastTarget = false;
            if (Progress.Skins.IsPup(sprite)) img.material = Progress.Skins.PupMaterial();   // the pup wears its skins on every screen
            return img;
        }

        /// <summary>Full-screen dimmer that also blocks clicks to whatever is behind it.</summary>
        public static RectTransform Overlay(Transform parent, string name)
        {
            var rt = Stretch(Node(parent, name));
            rt.gameObject.AddComponent<Image>().color = Dim;
            return rt;
        }

        /// <summary>A full screen's painted background: the home painting covering any screen shape under a dark veil,
        /// so white outlined text and ice panels read on it (the loading screen, the shop). Nothing when the painting
        /// is missing: the caller's own flat colour shows.</summary>
        public static void PaintedBackdrop(Transform parent, float veil, string painting = "bg_menu")
        {
            if (!Art.Has(painting)) return;
            var bg = Image(parent, painting, Center, Vector2.zero, Vector2.zero);
            bg.preserveAspect = false;
            var fit = bg.gameObject.AddComponent<AspectRatioFitter>();
            fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fit.aspectRatio = bg.sprite.rect.width / bg.sprite.rect.height;
            var shade = Stretch(Node(parent, "Veil")).gameObject.AddComponent<Image>();
            shade.color = new Color(0.03f, 0.08f, 0.2f, veil);
            shade.raycastTarget = false;
        }

        public static Image Panel(Transform parent, Vector2 size, Vector2 position = default)
        {
            var rt = Place(Node(parent, "Panel"), Center, position, size);
            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = Art.Get("ui_panel");
            img.type = UnityEngine.UI.Image.Type.Sliced;
            return img;
        }

        public static TextMeshProUGUI Text(Transform parent, string text, float size, Vector2 anchor, Vector2 position,
            Vector2 box, Color? color = null, TextAlignmentOptions align = TextAlignmentOptions.Center, bool outline = false)
        {
            var rt = Place(Node(parent, "Text"), anchor, position, box);
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.font = Font;
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color ?? Ink;
            tmp.alignment = align;
            tmp.raycastTarget = false;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            // German/French run ~30% longer than English: shrink to fit the box instead of spilling out of it.
            tmp.enableAutoSizing = true;
            tmp.fontSizeMax = size;
            tmp.fontSizeMin = size * 0.65f;
            // Preset material asset (outline + soft drop shadow) so its shader variant ships in the build.
            if (outline && OutlineMaterial != null) tmp.fontSharedMaterial = OutlineMaterial;
            return tmp;
        }

        /// <summary>Big game-logo style text: gold-to-orange gradient, outline and drop shadow.</summary>
        public static TextMeshProUGUI Logo(Transform parent, string text, float size, Vector2 anchor, Vector2 position, Vector2 box)
        {
            var t = Text(parent, text, size, anchor, position, box, Color.white, TextAlignmentOptions.Center, true);
            t.enableVertexGradient = true;
            t.colorGradient = new VertexGradient(new Color32(0xff, 0xf3, 0xa8, 0xff), new Color32(0xff, 0xf3, 0xa8, 0xff),
                new Color32(0xff, 0xa8, 0x1c, 0xff), new Color32(0xff, 0x8a, 0x1c, 0xff));
            t.fontStyle = FontStyles.Bold;
            t.characterSpacing = 2f;
            return t;
        }

        public static Button Button(Transform parent, string label, ButtonStyle style, Vector2 anchor, Vector2 position,
            Vector2 size, Action onClick, string glyph = null)
        {
            var rt = Place(Node(parent, "Button " + (label ?? glyph)), anchor, position, size);
            var img = rt.gameObject.AddComponent<Image>();
            // A glyph-only button with a painted round icon (icon_<glyph name>, Tools/cut_props.py) is just that picture.
            string icon = string.IsNullOrEmpty(label) && glyph != null && glyph.StartsWith("glyph_") ? "icon_" + glyph.Substring(6) : null;
            bool painted = icon != null && Art.Has(icon);
            img.sprite = Art.Get(painted ? icon : style switch
            {
                ButtonStyle.Warm => "ui_button_warm",
                ButtonStyle.Gold => "ui_button_gold",
                _ => "ui_button",
            });
            img.type = painted ? UnityEngine.UI.Image.Type.Simple : UnityEngine.UI.Image.Type.Sliced;
            img.preserveAspect = painted;
            if (painted) glyph = null;
            var button = rt.gameObject.AddComponent<Button>();
            PadHitArea(rt);
            rt.gameObject.AddComponent<PopIn>().delay = 0.05f * (rt.GetSiblingIndex() % 8);
            button.transition = Selectable.Transition.None;
            rt.gameObject.AddComponent<ButtonJuice>();
            button.onClick.AddListener(() =>
            {
                AudioManager.Sfx("click");
                onClick?.Invoke();
            });

            float glyphSize = Mathf.Min(size.y * 0.7f, 90f);
            float textLeft = 0f;
            if (glyph != null)
            {
                bool glyphOnly = string.IsNullOrEmpty(label);
                var g = Image(rt, glyph, glyphOnly ? Center : new Vector2(0f, 0.5f),
                    glyphOnly ? new Vector2(0, 3) : new Vector2(24, 3), new Vector2(glyphSize, glyphSize));
                g.name = "Glyph";
                textLeft = glyphOnly ? 0f : glyphSize + 16f;
            }
            if (!string.IsNullOrEmpty(label))
            {
                var t = Text(rt, label, size.y * 0.42f, Center, new Vector2(textLeft * 0.5f, 4), new Vector2(size.x - textLeft - LabelPad * 2f, size.y),
                    White, TextAlignmentOptions.Center, true);
                t.textWrappingMode = TextWrappingModes.NoWrap;
                t.enableAutoSizing = true;
                t.fontSizeMin = 18;
                t.fontSizeMax = size.y * 0.42f;
            }
            return button;
        }

        /// <summary>A picture that is a button (map nodes): pop-in, squash on press, click sound.</summary>
        public static Button ImageButton(Transform parent, string sprite, Vector2 anchor, Vector2 position, Vector2 size, Action onClick, bool juice = true)
        {
            var img = Image(parent, sprite, anchor, position, size);
            img.raycastTarget = true;
            var button = img.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            PadHitArea(img.rectTransform);
            img.gameObject.AddComponent<PopIn>().delay = 0.04f * (img.transform.GetSiblingIndex() % 10);
            if (juice) img.gameObject.AddComponent<ButtonJuice>();   // not on locked or pulsing buttons (it fights the pulse)
            button.onClick.AddListener(() =>
            {
                AudioManager.Sfx("click");
                onClick?.Invoke();
            });
            return button;
        }

        /// <summary>Layout A's top bar for full screens (map, shop): square Back on the left, the screen's title beside it,
        /// a star pill on the right when <paramref name="starsText"/> is given. GAME_DESIGN.md §R10.</summary>
        public static RectTransform TopBar(Transform root, string title, Action onBack, string starsText, out TextMeshProUGUI starsLabel)
        {
            var bar = Node(root, "Top bar");
            bar.anchorMin = new Vector2(0f, 1f);
            bar.anchorMax = new Vector2(1f, 1f);
            bar.pivot = new Vector2(0.5f, 1f);
            bar.anchoredPosition = new Vector2(0f, -24f);
            bar.sizeDelta = new Vector2(0f, 104f);
            Button(bar, null, ButtonStyle.Teal, new Vector2(0f, 0.5f), new Vector2(24, 0), new Vector2(104, 104), onBack, "glyph_back");
            Text(bar, title, 64, new Vector2(0f, 0.5f), new Vector2(150, 4), new Vector2(760, 100), White, TextAlignmentOptions.Left, true);
            starsLabel = null;
            if (starsText == null) return bar;
            var stars = Place(Node(bar, "Stars"), new Vector2(1f, 0.5f), new Vector2(-24, 0), new Vector2(260, 104));
            var pill = stars.gameObject.AddComponent<Image>();
            pill.sprite = Art.Get("ui_pill");
            pill.type = UnityEngine.UI.Image.Type.Sliced;
            pill.raycastTarget = false;   // a read-out: taps go through to a map stone under it
            Image(stars, "star_full", new Vector2(0f, 0.5f), new Vector2(12, 2), new Vector2(76, 76));
            starsLabel = Text(stars, starsText, 48, new Vector2(1f, 0.5f), new Vector2(-20, 2), new Vector2(160, 80), White, TextAlignmentOptions.Right, true);
            return bar;
        }

        /// <summary>The ribbon title every popup panel wears, overhanging the panel's top by <paramref name="overhang"/>.</summary>
        /// <summary>The game's name as its painted brand logo (Assets/Art/Reference/logo.png, one logo for every language:
        /// user 18/11 "tên game ko theo ngôn ngữ … thiết kế lại tên game nhìn thật đẹp và thương hiệu"), fitted into
        /// <paramref name="box"/>; the gold text only when the picture is missing.</summary>
        public static RectTransform Brand(Transform parent, Vector2 anchor, Vector2 position, Vector2 box)
        {
            if (!Art.Has("logo"))
            {
                var text = Logo(parent, Loc.T("title"), box.y * 0.75f, anchor, position, box);
                text.enableAutoSizing = true;
                text.fontSizeMin = 40;
                text.fontSizeMax = box.y * 0.75f;
                return text.rectTransform;
            }
            return Image(parent, "logo", anchor, position, box).rectTransform;   // preserveAspect: fits inside the box
        }

        /// <summary>The red X on a panel's top-right corner (user 18/11: "các modal ko có dấu x để tắt ak"): closes the
        /// modal exactly like its Back button. Painted icon_close (Assets/Art/Reference/ui_close.png).</summary>
        public static Button CloseButton(Transform panel, Action onClose) =>
            Button(panel, null, ButtonStyle.Warm, TopRight, new Vector2(34, 34), new Vector2(104, 104), onClose, "glyph_close");

        /// <summary>Makes a map character stand out from the painted map (user 19/11: "ở map chưa làm nổi bật hoạt ảnh vì nó là
        /// chính, hiện tại đang bị hòa vào map"): a thick dark navy stroke around the sprite, a soft drop shadow, a warm halo
        /// behind it and a dark contact shadow under its feet. <paramref name="feet"/> is the feet point in the parent of
        /// <paramref name="img"/>, <paramref name="width"/> the character's width.</summary>
        public static void StandOut(Image img, Vector2 feet, float width)
        {
            Stroke(img, 2.5f);
            var drop = img.gameObject.AddComponent<Shadow>();
            drop.effectColor = new Color(0f, 0.04f, 0.14f, 0.45f);
            drop.effectDistance = new Vector2(3f, -6f);
            Ground(img.rectTransform, feet, width);
        }

        /// <summary>A thick dark navy stroke round a sprite (UI Outline).</summary>
        public static void Stroke(Image img, float px)
        {
            var stroke = img.gameObject.AddComponent<Outline>();
            stroke.effectColor = new Color(0.04f, 0.09f, 0.24f, 0.95f);
            stroke.effectDistance = new Vector2(px, -px);
        }

        /// <summary>The warm halo behind and the dark contact shadow under something standing at <paramref name="feet"/>.</summary>
        public static void Ground(RectTransform target, Vector2 feet, float width)
        {
            var parent = target.parent;
            int at = target.GetSiblingIndex();
            var img = target;
            var halo = Image(parent, "powder", Center, Vector2.zero, new Vector2(width * 1.5f, width * 1.5f));   // a soft round light
            halo.rectTransform.anchorMin = halo.rectTransform.anchorMax = img.anchorMin;
            halo.rectTransform.anchoredPosition = feet + new Vector2(0f, width * 0.55f);
            halo.color = new Color(1f, 0.96f, 0.8f, 0.6f);
            halo.transform.SetSiblingIndex(at);
            var contact = Image(parent, "powder", Center, Vector2.zero, new Vector2(width * 0.85f, width * 0.26f));
            contact.preserveAspect = false;
            contact.rectTransform.anchorMin = contact.rectTransform.anchorMax = img.anchorMin;
            contact.rectTransform.anchoredPosition = feet;
            contact.color = new Color(0f, 0.05f, 0.15f, 0.5f);
            contact.transform.SetSiblingIndex(at + 1);
        }

        /// <summary>A modal's title on the ice ribbon overhanging the panel's top (user 18/11: "vẫn thêm ruy băng").</summary>
        public static TextMeshProUGUI Ribbon(Transform panel, string title, float width, float overhang)
        {
            var ribbon = Image(panel, "ribbon", Top, new Vector2(0, overhang), new Vector2(width, width * 0.23f));
            var t = Text(ribbon.transform, title, 64, Center, new Vector2(0, 8), new Vector2(width * 0.74f, 100), White,
                TextAlignmentOptions.Center, true);
            t.fontSizeMin = 30;
            return t;
        }

        /// <summary>A modal's cast member (GAME_DESIGN.md "Bảng"): a looping animation drawn for that modal, standing
        /// against the panel's side; every frame <paramref name="anim"/>_0.._N is played over <paramref name="cycle"/> s.
        /// <paramref name="position"/> is where the feet stand, from <paramref name="anchor"/> (a panel corner puts the character
        /// half outside the panel, against its side). <paramref name="flip"/> mirrors it (the bears are drawn facing right).
        /// Null when the frames are missing.</summary>
        public static Image Cast(Transform parent, string anim, Vector2 anchor, Vector2 position, float height, float cycle, bool flip = false)
        {
            string prefix = anim + "_";
            int count = 0;
            while (count < 32 && Art.Has(prefix + count)) count++;
            if (count == 0) return null;
            var first = Art.Get(prefix + "0");
            float width = height * first.rect.width / first.rect.height;
            var holder = Place(Node(parent, "Cast " + anim), anchor, position, new Vector2(width, height));
            holder.pivot = new Vector2(0.5f, 0f);   // the position is where its feet stand
            if (flip) holder.localScale = new Vector3(-1f, 1f, 1f);   // PopIn owns the image's own scale
            var img = Image(holder, prefix + "0", Center, Vector2.zero, new Vector2(width, height));
            img.preserveAspect = true;
            if (count > 1) img.gameObject.AddComponent<FrameLoop>().Set(prefix, count, count / cycle);
            img.gameObject.AddComponent<PopIn>().delay = 0.2f;
            return img;
        }

        /// <summary>A list row on the frosted strip (stats, star rules): icon, label on the left, value or mark on the right.</summary>
        public static RectTransform ListRow(Transform parent, string icon, string label, float y, float width, float height = 68f, float x = 0f)
        {
            var bar = Image(parent, "ui_row", Top, new Vector2(x, y), new Vector2(width, height));
            bar.type = UnityEngine.UI.Image.Type.Sliced;
            bar.preserveAspect = false;
            var rt = bar.rectTransform;
            if (icon != null) Image(rt, icon, new Vector2(0f, 0.5f), new Vector2(height * 0.3f, 0), new Vector2(height * 0.9f, height * 0.9f)).preserveAspect = true;   // x: left edge
            var t = Text(rt, label, height * 0.5f, new Vector2(0f, 0.5f), new Vector2(height * 1.35f, 2),   // Place pivots on the anchor: x is the left edge
                new Vector2(width - height * 3f, height), Ink, TextAlignmentOptions.Left);
            t.enableAutoSizing = true;
            t.fontSizeMin = 20;
            t.fontSizeMax = height * 0.5f;
            return rt;
        }

        public static Slider Slider(Transform parent, Vector2 anchor, Vector2 position, Vector2 size, float value, Action<float> onChange)
        {
            var root = Place(Node(parent, "Slider"), anchor, position, size);
            var track = Stretch(Node(root, "Track"));
            var trackImg = track.gameObject.AddComponent<Image>();
            // ui_bar (a plain rounded bar) for both: the snow-capped ui_panel and a square fill read as a glitch (QA 15/11).
            trackImg.sprite = Art.Get("ui_bar");
            trackImg.type = UnityEngine.UI.Image.Type.Sliced;
            trackImg.color = new Color32(0x1b, 0x2a, 0x49, 0x66);
            track.offsetMin = new Vector2(0, size.y * 0.25f);
            track.offsetMax = new Vector2(0, -size.y * 0.25f);

            var fillArea = Stretch(Node(root, "Fill Area"));
            fillArea.offsetMin = new Vector2(0, size.y * 0.25f);
            fillArea.offsetMax = new Vector2(0, -size.y * 0.25f);
            var fill = Stretch(Node(fillArea, "Fill"));
            var fillImg = fill.gameObject.AddComponent<Image>();
            fillImg.sprite = Art.Get("ui_bar");
            fillImg.type = UnityEngine.UI.Image.Type.Sliced;
            fillImg.color = new Color32(0x2e, 0xc4, 0xb6, 0xff);

            var handleArea = Stretch(Node(root, "Handle Area"));
            var handle = Node(handleArea, "Handle");
            handle.sizeDelta = new Vector2(size.y * 1.3f, size.y * 1.3f);
            var handleImg = handle.gameObject.AddComponent<Image>();
            handleImg.sprite = Art.Get("gem");

            PadHitArea(root);   // a 60px track is too thin a finger target
            var slider = root.gameObject.AddComponent<Slider>();
            slider.fillRect = fill;
            slider.handleRect = handle;
            slider.targetGraphic = handleImg;
            slider.value = value;
            slider.onValueChanged.AddListener(v => onChange(v));
            return slider;
        }
    }

    /// <summary>Squash on press, pop on release. Uses unscaled time so it works while paused.</summary>
    /// <summary>Press feedback (user 18/11: "khi nhấn các nút chưa có animation j cả"): the button sinks and darkens
    /// while held, then springs back past full size and settles, like the top mobile games. Spring on unscaled time;
    /// idle buttons do no work.</summary>
    public class ButtonJuice : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        const float Pressed = 0.86f, Kick = 6f, Stiffness = 420f, Damping = 16f, Darken = 0.8f;
        float target = 1f, velocity;
        Graphic graphic;
        Color restColor;
        bool held;

        void Awake() => graphic = GetComponent<Graphic>();

        public void OnPointerDown(PointerEventData e)
        {
            target = Pressed;
            velocity = -Kick;   // snaps down at once, not eased: a short tap still shows
            if (graphic == null || held) return;
            held = true;
            restColor = graphic.color;
            graphic.color = new Color(restColor.r * Darken, restColor.g * Darken, restColor.b * Darken, restColor.a);
        }

        public void OnPointerUp(PointerEventData e) => Release(Kick);
        public void OnPointerExit(PointerEventData e) => Release(0f);

        void Release(float kick)
        {
            target = 1f;
            velocity += kick;   // the spring overshoots past full size, then settles
            if (!held) return;
            held = false;
            if (graphic != null) graphic.color = restColor;
        }

        void OnDisable()
        {
            // Hidden while held: no pointer-up follows, and it would come back squashed and dark.
            Release(0f);
            velocity = 0f;
        }

        void Update()
        {
            float s = transform.localScale.x;
            if (Mathf.Abs(s - target) < 0.0005f && Mathf.Abs(velocity) < 0.001f) return;   // at rest: no canvas dirtying
            float dt = Mathf.Min(Time.unscaledDeltaTime, 1f / 30f);
            velocity += ((target - s) * Stiffness - velocity * Damping) * dt;
            s += velocity * dt;
            transform.localScale = new Vector3(s, s, 1f);
        }
    }

    /// <summary>Turns slowly in the screen plane on unscaled time (the light rays behind the victory stars).</summary>
    public class Rotate : MonoBehaviour
    {
        public float speed = 18f;
        void Update() => transform.Rotate(0f, 0f, speed * Time.unscaledDeltaTime);
    }

    /// <summary>Flips a UI image through numbered sprites (prefix0..prefixN-1) at a fixed rate; unscaled time.</summary>
    public class FrameLoop : MonoBehaviour
    {
        Sprite[] frames;
        float[] hold;   // seconds each frame shows
        Image img;
        int index;
        float t;

        public void Set(string prefix, int count, float rate) => Set(rate, (prefix, count));

        /// <summary>A loop stitched from several animations, e.g. a bow then a salute; each part lasts count / rate s.
        /// A part redrawn with more frames than <c>count</c> (the 16-frame redraws, user 18/11: "tăng khung hình cho tất cả
        /// hoạt ảnh") plays all of them in that same time, so callers never need to know how many were drawn. Missing
        /// frames are skipped (the Image keeps its sprite if nothing is found).</summary>
        public void Set(float rate, params (string prefix, int count)[] parts)
        {
            var list = new System.Collections.Generic.List<Sprite>();
            var times = new System.Collections.Generic.List<float>();
            rate = Mathf.Max(rate, 0.01f);
            foreach (var (prefix, count) in parts)
            {
                int drawn = 0;
                while (drawn < 32 && Art.Has(prefix + drawn)) drawn++;
                // Only a full 16-frame redraw of a whole set: a part that asks for one frame (pup_leap_0 as a crouch) or
                // for a slice keeps exactly what it asked for.
                int n = drawn >= 16 && count >= 4 ? drawn : count;
                float each = count / rate / Mathf.Max(1, Mathf.Min(n, Mathf.Max(drawn, 1)));
                for (int i = 0; i < n; i++)
                {
                    var s = Art.Get(prefix + i);
                    if (s == null) continue;
                    list.Add(s);
                    times.Add(each);
                }
            }
            frames = list.Count > 0 ? list.ToArray() : null;
            hold = times.ToArray();
            index = 0;
            t = 0f;
            img = GetComponent<Image>();
        }

        void Update()
        {
            if (frames == null || img == null) return;
            t += Time.unscaledDeltaTime;
            while (t >= hold[index])
            {
                t -= hold[index];
                index = (index + 1) % frames.Length;
            }
            if (img.sprite != frames[index]) img.sprite = frames[index];
        }
    }

    /// <summary>Entrance animation: grows from small with a springy overshoot (unscaled time, works while paused).</summary>
    public class PopIn : MonoBehaviour
    {
        public float delay;
        public float duration = 0.35f;

        System.Collections.IEnumerator Start()
        {
            var t = transform;
            // ButtonJuice also drives the scale: left on, it grew the button during the delay before the pop restarted it.
            var juice = GetComponent<ButtonJuice>();
            if (juice != null) juice.enabled = false;
            t.localScale = Vector3.zero;
            for (float w = 0f; w < delay; w += Time.unscaledDeltaTime) yield return null;
            for (float time = 0f; time < duration; time += Time.unscaledDeltaTime)
            {
                float k = time / duration;
                // easeOutBack
                float c = 1.9f;
                float e = 1f + (c + 1f) * Mathf.Pow(k - 1f, 3) + c * Mathf.Pow(k - 1f, 2);
                t.localScale = Vector3.one * e;
                yield return null;
            }
            t.localScale = Vector3.one;
            if (juice != null) juice.enabled = true;
        }

        /// <summary>Hidden mid-pop (a pause menu closed at once): the coroutine dies and Start never runs again, so
        /// finish the pop here or the button stays tiny with its press animation off.</summary>
        void OnDisable()
        {
            transform.localScale = Vector3.one;
            var juice = GetComponent<ButtonJuice>();
            if (juice != null) juice.enabled = true;
        }
    }

    /// <summary>Slides in from an offset and fades up (banners, cards).</summary>
    public class SlideIn : MonoBehaviour
    {
        public Vector2 from = new Vector2(0, -60);
        public float delay;
        public float duration = 0.4f;

        System.Collections.IEnumerator Start()
        {
            var rt = (RectTransform)transform;
            var target = rt.anchoredPosition;
            var group = gameObject.GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();  // Unity fake-null: no ?? here
            group.alpha = 0f;
            rt.anchoredPosition = target + from;
            for (float w = 0f; w < delay; w += Time.unscaledDeltaTime) yield return null;
            for (float time = 0f; time < duration; time += Time.unscaledDeltaTime)
            {
                float k = 1f - Mathf.Pow(1f - time / duration, 3);
                rt.anchoredPosition = Vector2.LerpUnclamped(target + from, target, k);
                group.alpha = k;
                yield return null;
            }
            rt.anchoredPosition = target;
            group.alpha = 1f;
        }
    }

    /// <summary>Keeps its RectTransform inside Screen.safeArea (re-checked when rotation changes).</summary>
    public class SafeArea : MonoBehaviour
    {
        Rect applied;

        void Update()
        {
            var safe = Screen.safeArea;
            if (safe == applied || Screen.width == 0 || Screen.height == 0) return;
            applied = safe;
            var rt = (RectTransform)transform;
            rt.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            rt.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }
    }

    /// <summary>Turntable spin around the vertical axis (a character showing off on a pedestal).</summary>
    public class Spin : MonoBehaviour
    {
        public float period = 1.6f;
        void Update() => transform.localRotation = Quaternion.Euler(0f, Time.unscaledTime / period * 360f, 0f);
    }

    /// <summary>Gentle bob/pulse for hints and titles.</summary>
    /// <summary>One arcing hop from <see cref="from"/> to <see cref="to"/> (anchored positions), then a gentle bob.</summary>
    public class Hop : MonoBehaviour
    {
        public Vector2 from, to;
        /// <summary>Optional: hop from this element instead of <see cref="from"/> (read every frame, so it holds once
        /// layout has placed it), keeping <see cref="fromOffset"/> above it.</summary>
        public RectTransform fromSpot;
        public Vector2 fromOffset;
        /// <summary>Called once on landing (the map swaps the jump frames back to the waiting loop).</summary>
        public System.Action landed;
        public const float Delay = 0.4f;
        public float duration = 0.7f, height = 120f;   // the map's walk: longer and low (MapView)
        float t = -Delay;

        Vector2 From => fromSpot != null
            ? (Vector2)transform.parent.InverseTransformPoint(fromSpot.position) + fromOffset
            : from;

        void Start() => ((RectTransform)transform).anchoredPosition = From;

        void Update()
        {
            t += UnityEngine.Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            // A low height bounces each step (four steps), a high one is a single leap.
            float lift = height < 40f ? Mathf.Abs(Mathf.Sin(k * Mathf.PI * 4f)) : Mathf.Sin(k * Mathf.PI);
            ((RectTransform)transform).anchoredPosition = Vector2.Lerp(From, to, k) + Vector2.up * height * lift;
            if (k < 1f) return;
            landed?.Invoke();
            var bob = gameObject.AddComponent<Bob>();
            bob.amplitude = 6f;
            bob.speed = 3f;
            Destroy(this);
        }
    }

    public class Bob : MonoBehaviour
    {
        public float amplitude = 10f;
        public float speed = 2f;
        public bool pulse;
        Vector2 start;
        RectTransform rt;

        void Start()
        {
            rt = (RectTransform)transform;
            start = rt.anchoredPosition;
        }

        void Update()
        {
            float w = Mathf.Sin(Time.unscaledTime * speed);
            if (pulse) transform.localScale = Vector3.one * (1f + 0.08f * w);
            else rt.anchoredPosition = start + new Vector2(0, amplitude * w);
        }
    }
}
