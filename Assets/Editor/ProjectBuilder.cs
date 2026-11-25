using System.IO;
using System.Linq;
using SealCatapult.Core;
using SealCatapult.Progress;
using SealCatapult.UI;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;

namespace SealCatapult.EditorTools
{
    /// <summary>
    /// Builds everything that would otherwise be clicked together in the Editor:
    /// sprite import settings, the TMP font, data assets, the four scenes and the Mac build.
    /// Menu: Seal Catapult/… · Batch: -executeMethod SealCatapult.EditorTools.ProjectBuilder.BuildAll
    /// </summary>
    public static class ProjectBuilder
    {
        const string ArtDir = "Assets/Resources/Art";
        const string AudioDir = "Assets/Resources/Audio";
        const string DataDir = "Assets/Data";
        const string FontTtf = "Assets/Fonts/Baloo2.ttf";
        const string FontAsset = "Assets/Resources/Fonts/Baloo2 SDF.asset";
        const string OutlineMat = "Assets/Resources/Fonts/Baloo2 SDF Outline.mat";
        const string TmpEssentials = "Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage";
        const string ApkBuild = "Builds/Android/SealCatapult.apk";
        const string AabBuild = "Builds/Android/SealCatapult.aab";
        const float PixelsPerUnit = 100f;
        const float PaintedBlockPixelsPerUnit = 300f;

        static readonly (string scene, System.Type view)[] Scenes =
        {
            (SceneLoader.Boot, typeof(BootView)),
            (SceneLoader.MainMenu, typeof(MainMenuView)),
            (SceneLoader.LevelMap, typeof(MapView)),
            (SceneLoader.Game, typeof(GameManager)),
        };

        [MenuItem("Seal Catapult/1. Import TMP Essentials")]
        public static void ImportTmpEssentials()
        {
            if (AssetDatabase.IsValidFolder("Assets/TextMesh Pro")) return;
            AssetDatabase.ImportPackage(TmpEssentials, false);
            AssetDatabase.Refresh();
        }

        [MenuItem("Seal Catapult/2. Build Project (keeps edited data)")]
        public static void BuildAll()
        {
            ConfigureSprites();
            ConfigureAudio();
            CreateFont();
            CreateData(false);
            CreateScenes();
            ConfigurePlayer();
            AssetDatabase.SaveAssets();
            Debug.Log("[SealCatapult] Project built.");
        }

        [MenuItem("Seal Catapult/Rebuild Levels (overwrites Level_XX)")]
        public static void RebuildLevels()
        {
            CreateData(true);
            AssetDatabase.SaveAssets();
        }

        /// <summary>Test build: debug-signed APK to sideload on a phone.</summary>
        [MenuItem("Seal Catapult/3. Build Android APK (test)")]
        public static void BuildApk() => BuildAndroid(false);

        /// <summary>QA build: a debuggable development APK (Builds/Android/SealCatapult-qa.apk) so QA can seed
        /// save.json with `adb push` (README.md "Install on a phone": it lives in the external files dir, not under run-as) to reach
        /// later levels. Never upload it.</summary>
        public static void BuildApkQa() => BuildAndroid(false, true);

        /// <summary>Re-applies the sprite import settings to every frame in Resources/Art. Frames the slicer adds later
        /// import as "Multiple" by default and then fail to load as one Sprite (16/11: the new queue fidgets fell back
        /// to old side-view frames and flickered). Run after Tools/slice_sheets.py.</summary>
        [MenuItem("Seal Catapult/Configure Art Imports")]
        public static void ConfigureArt()
        {
            ConfigureSprites();
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Store build: signed AAB for Google Play. Keystore comes from environment variables, never from the repo:
        /// SEAL_KEYSTORE (path), SEAL_KEYSTORE_PASS, SEAL_KEY_ALIAS, SEAL_KEY_PASS.
        /// </summary>
        [MenuItem("Seal Catapult/4. Build Android AAB (Play Store)")]
        public static void BuildAab() => BuildAndroid(true);

        static void BuildAndroid(bool appBundle, bool qa = false)
        {
            ConfigurePlayer();
            ConfigureAudio();
            EditorUserBuildSettings.buildAppBundle = appBundle;
            if (appBundle && !ConfigureSigning()) return;
            if (!appBundle) PlayerSettings.Android.useCustomKeystore = false;

            string path = appBundle ? AabBuild : qa ? ApkBuild.Replace(".apk", "-qa.apk") : ApkBuild;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var options = new BuildPlayerOptions
            {
                scenes = Scenes.Select(s => ScenePath(s.scene)).ToArray(),
                locationPathName = path,
                target = BuildTarget.Android,
                options = qa ? BuildOptions.Development : BuildOptions.None,
            };
            BuildReport report;
            try
            {
                report = BuildPipeline.BuildPlayer(options);
            }
            finally
            {
                // Never leave the keystore path or passwords in the Editor's player settings after a store build.
                if (appBundle)
                {
                    PlayerSettings.Android.keystorePass = "";
                    PlayerSettings.Android.keyaliasPass = "";
                    PlayerSettings.Android.keystoreName = "";
                    PlayerSettings.Android.keyaliasName = "";
                    PlayerSettings.Android.useCustomKeystore = false;
                }
            }
            Debug.Log($"[SealCatapult] Build {report.summary.result}: {report.summary.totalSize / (1024 * 1024)} MB, " +
                      $"{report.summary.totalErrors} errors → {path}");
            if (Application.isBatchMode && report.summary.result != BuildResult.Succeeded) EditorApplication.Exit(1);
        }

        static bool ConfigureSigning()
        {
            string keystore = System.Environment.GetEnvironmentVariable("SEAL_KEYSTORE");
            string storePass = System.Environment.GetEnvironmentVariable("SEAL_KEYSTORE_PASS");
            string alias = System.Environment.GetEnvironmentVariable("SEAL_KEY_ALIAS");
            string keyPass = System.Environment.GetEnvironmentVariable("SEAL_KEY_PASS");
            if (string.IsNullOrEmpty(keystore) || !File.Exists(keystore) || string.IsNullOrEmpty(storePass) ||
                string.IsNullOrEmpty(alias) || string.IsNullOrEmpty(keyPass))
            {
                Debug.LogError("[SealCatapult] AAB needs SEAL_KEYSTORE, SEAL_KEYSTORE_PASS, SEAL_KEY_ALIAS, SEAL_KEY_PASS.");
                if (Application.isBatchMode) EditorApplication.Exit(1);
                return false;
            }
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = keystore;
            PlayerSettings.Android.keystorePass = storePass;
            PlayerSettings.Android.keyaliasName = alias;
            PlayerSettings.Android.keyaliasPass = keyPass;
            return true;
        }

        static string ScenePath(string name) => $"Assets/Scenes/{name}.unity";

        // ---------- audio ----------

        /// <summary>The two music tracks stream from the APK. Decompressed on load (the import default) each one sat in
        /// memory as several MB of PCM; the short effects stay decompressed so they play without a delay.</summary>
        static void ConfigureAudio()
        {
            foreach (var path in Directory.GetFiles(AudioDir, "music_*.wav"))
            {
                var importer = (AudioImporter)AssetImporter.GetAtPath(path);
                if (importer == null) throw new FileNotFoundException($"[SealCatapult] No audio importer for {path}");
                var settings = importer.defaultSampleSettings;
                if (settings.loadType == AudioClipLoadType.Streaming) continue;
                settings.loadType = AudioClipLoadType.Streaming;
                importer.defaultSampleSettings = settings;
                importer.SaveAndReimport();
            }
        }

        // ---------- sprites ----------

        static void ConfigureSprites()
        {
            foreach (var path in Directory.GetFiles(ArtDir, "*.png"))
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                if (importer == null) continue;
                string name = Path.GetFileNameWithoutExtension(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                // Painted blocks (Tools/cut_props.py) are ~250 px a side: denser pixels keep their 9-slice border thin.
                importer.spritePixelsPerUnit = PixelsPerUnitFor(name);
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;   // Editor and other platforms
                importer.wrapMode = TextureWrapMode.Clamp;
                // Android: ASTC. Uncompressed RGBA32 was about 164 MB of texture memory and most of the APK. Full-screen
                // paintings take the smaller 6×6 blocks; sprites and 9-slice UI keep 4×4 so edges and borders stay clean.
                bool painting = name.StartsWith("bg_") || name.StartsWith("story_") || name.StartsWith("map_bg");
                var android = importer.GetPlatformTextureSettings("Android");
                android.overridden = true;
                // A drawn outfit (<outfit>_pup_<anim>_<i>, Tools/slice_sheets.py --outfit) is ~450 more frames: the smaller
                // blocks keep each outfit near 20 MB of texture instead of 45.
                bool outfit = !name.StartsWith("pup_") && name.Contains("_pup_");
                android.format = painting || outfit ? TextureImporterFormat.ASTC_6x6 : TextureImporterFormat.ASTC_4x4;
                android.maxTextureSize = 2048;
                android.compressionQuality = 50;
                importer.SetPlatformTextureSettings(android);

                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                float border = SliceBorder(name);
                bool tiled = name.StartsWith("rock_cobble") || name == "rock_top" || name == "rope" || name.StartsWith("rope_");   // drawn with SpriteDrawMode.Tiled along the rope's curve (rope_<skin>: drawn rope skins)
                settings.spriteMeshType = border > 0 || tiled ? SpriteMeshType.FullRect : SpriteMeshType.Tight;
                importer.SetTextureSettings(settings);
                // beam.png is 320×90: only its ends are caps, so the border is horizontal (a 60 px top/bottom border would squash it).
            // beam and the painted pill buttons only have end caps: horizontal border, the height scales the whole art.
            bool capsOnly = name == "beam" || name.StartsWith("ui_button");
            importer.spriteBorder = capsOnly ? new Vector4(border, 0f, border, 0f) : new Vector4(border, border, border, border);
                importer.SaveAndReimport();
            }
        }

        /// <summary>Painted tiles (Tools/prep_backgrounds.py, cut_props.py) set their world size through pixel density:
        /// a 512 px cobble tile repeats every 3 units, the 96 px snow rim is RimHeight (0.44) tall, blocks keep a thin border.</summary>
        static float PixelsPerUnitFor(string name)
        {
            if (name.StartsWith("block_")) return PaintedBlockPixelsPerUnit;
            if (name.StartsWith("rock_cobble")) return 1024f / 11f;   // the pit painting's own scale: 1024 px = 11 units
            if (name == "rock_top") return 96f / 0.7f;
            return PixelsPerUnit;
        }

        static float SliceBorder(string name)
        {
            if (name.StartsWith("block_")) return 40f;   // the painted outline and rounded corner, at 300 px/unit
            if (name == "cave_wall") return 40f;
            if (name == "beam") return 60f;
            if (name == "ui_panel") return 72f;          // painted panel: snow caps over the corners
            if (name.StartsWith("ui_button")) return 62f; // painted pill: round end caps
            if (name == "ui_bar") return 14f;            // 64×32 px: a 44 px border is larger than the sprite and the sliced bar vanishes (boss life bar, 13/11)
            if (name.StartsWith("ui_")) return 44f;
            return 0f;
        }

        // ---------- font ----------

        static void CreateFont()
        {
            CreateFontAsset();
            CreateOutlineMaterial();
        }

        /// <summary>Outline + drop-shadow preset; saved as an asset so the shader keywords survive build stripping.</summary>
        static void CreateOutlineMaterial()
        {
            var fa = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAsset);
            if (fa == null) return;
            var mat = AssetDatabase.LoadAssetAtPath<Material>(OutlineMat);
            if (mat == null)
            {
                mat = new Material(fa.material) { name = "Baloo2 SDF Outline" };
                AssetDatabase.CreateAsset(mat, OutlineMat);
            }
            mat.EnableKeyword(ShaderUtilities.Keyword_Outline);
            mat.EnableKeyword(ShaderUtilities.Keyword_Underlay);
            mat.SetColor(ShaderUtilities.ID_OutlineColor, new Color32(0x16, 0x30, 0x4f, 0xff));
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.3f);
            mat.SetFloat(ShaderUtilities.ID_FaceDilate, 0.15f);
            mat.SetColor(ShaderUtilities.ID_UnderlayColor, new Color(0.03f, 0.09f, 0.2f, 0.55f));
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.8f);
            mat.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.35f);
            mat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.35f);
            EditorUtility.SetDirty(mat);
        }

        static void CreateFontAsset()
        {
            if (File.Exists(FontAsset)) return;
            var font = AssetDatabase.LoadAssetAtPath<Font>(FontTtf);
            // Thrown, not logged: the build went on and shipped without the font (no Vietnamese glyphs) with exit code 0.
            if (font == null) throw new FileNotFoundException($"[SealCatapult] Missing {FontTtf}");
            Directory.CreateDirectory(Path.GetDirectoryName(FontAsset));
            // Dynamic atlas: glyphs (incl. Vietnamese diacritics) are added on demand from the TTF.
            var fa = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024,
                AtlasPopulationMode.Dynamic, true);
            fa.name = "Baloo2 SDF";
            AssetDatabase.CreateAsset(fa, FontAsset);
            fa.material.name = "Baloo2 SDF Material";
            AssetDatabase.AddObjectToAsset(fa.material, fa);
            fa.atlasTexture.name = "Baloo2 SDF Atlas";
            AssetDatabase.AddObjectToAsset(fa.atlasTexture, fa);
            fa.TryAddCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .,!?+×/:…");
            EditorUtility.SetDirty(fa);
        }

        // ---------- data ----------

        static T LoadOrCreate<T>(string path, out bool created) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            created = asset == null;
            if (created)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }
            return asset;
        }

        static void CreateData(bool overwriteLevels)
        {
            Directory.CreateDirectory(DataDir);
            var tuning = LoadOrCreate<GameTuning>($"{DataDir}/Tuning.asset", out _);

            var upgrades = LoadOrCreate<UpgradeData>($"{DataDir}/Upgrades.asset", out _);
            // The catalogue is code-defined: ids, icons and prices are rewritten every build (levels keep Inspector edits).
            upgrades.upgrades.Clear();
            // 32 stars buy everything; the 18 levels give 54 (an EditMode test keeps the total within reach). The old
            // prices added up to 117, so under a third of the shop could ever be bought.
            upgrades.upgrades.Add(new UpgradeDef { id = UpgradeId.Spiky, icon = "icon_spiky", costs = new[] { 1, 2, 3 } });
            upgrades.upgrades.Add(new UpgradeDef { id = UpgradeId.PuffUp, icon = "icon_ball", costs = new[] { 1, 2, 3 } });
            upgrades.upgrades.Add(new UpgradeDef { id = UpgradeId.School, icon = "icon_pod", costs = new[] { 1, 2, 3 } });
            upgrades.upgrades.Add(new UpgradeDef { id = UpgradeId.Ninja, icon = "icon_ninja", costs = new[] { 2, 2, 3 } });
            upgrades.upgrades.Add(new UpgradeDef { id = UpgradeId.Blaze, icon = "icon_blaze", costs = new[] { 2, 2, 3 } });
            EditorUtility.SetDirty(upgrades);

            var designs = LevelDesigns.All();
            var levels = new LevelData[designs.Length];
            for (int i = 0; i < designs.Length; i++)
            {
                var level = LoadOrCreate<LevelData>($"{DataDir}/Level_{i + 1:00}.asset", out bool created);
                if (created || overwriteLevels) Apply(level, designs[i], tuning);
                levels[i] = level;
            }

            Directory.CreateDirectory("Assets/Resources");
            var db = LoadOrCreate<GameDatabase>("Assets/Resources/GameDatabase.asset", out _);
            db.levels = levels;
            db.upgrades = upgrades;
            db.tuning = tuning;
            EditorUtility.SetDirty(db);
        }

        static void Apply(LevelData level, LevelBlueprint d, GameTuning tuning)
        {
            d.Fill(level, tuning);
            EditorUtility.SetDirty(level);
        }

        // ---------- scenes & player ----------

        static void CreateScenes()
        {
            Directory.CreateDirectory("Assets/Scenes");
            foreach (var (name, view) in Scenes)
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                new GameObject(name, view);
                EditorSceneManager.SaveScene(scene, ScenePath(name));
            }
            EditorBuildSettings.scenes = Scenes.Select(s => new EditorBuildSettingsScene(ScenePath(s.scene), true)).ToArray();
        }

        const string AdMobSettingsPath = "Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset";

        /// <summary>Writes the AdMob app id into the plugin's settings asset (the plugin stops the build when it is empty).
        /// Its settings class is internal, so the field is set through SerializedObject.</summary>
        static void ConfigureAdMob()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AdMobSettingsPath);
            if (settings == null)
            {
                var type = System.Type.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings, GoogleMobileAds.Editor");
                // Thrown, not logged: a batch build must stop here with a non-zero exit code.
                if (type == null)
                    throw new System.InvalidOperationException("[SealCatapult] Google Mobile Ads plugin missing (Packages/manifest.json com.google.ads.mobile)");
                Directory.CreateDirectory(Path.GetDirectoryName(AdMobSettingsPath));
                settings = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(settings, AdMobSettingsPath);
            }
            var so = new SerializedObject(settings);
            so.FindProperty("adMobAndroidAppId").stringValue = SealCatapult.Ads.AdsConfig.AppId;
            so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
        }

        /// <summary>The release version (CHANGELOG.md) and its Play version code; bump both for a release, or pass
        /// SEAL_VERSION_CODE for a rebuild of the same version.</summary>
        const string Version = "0.1.0";
        const int VersionCode = 1;

        static void ConfigurePlayer()
        {
            PlayerSettings.productName = "Seal Catapult";
            PlayerSettings.companyName = "Seal Catapult";
            PlayerSettings.bundleVersion = Version;
            // Play refuses a version code it has already seen: a store build can pass the next one without a code edit.
            string asked = System.Environment.GetEnvironmentVariable("SEAL_VERSION_CODE");
            int code = VersionCode;
            if (!string.IsNullOrEmpty(asked) && (!int.TryParse(asked, out code) || code <= 0))
            {
                // A typo must not quietly build code 1 (Play would refuse the upload after the whole build): the
                // exception stops the build here, in the Editor and in batch mode (which then exits with an error).
                throw new System.ArgumentException($"[SealCatapult] SEAL_VERSION_CODE='{asked}' is not a positive whole number.");
            }
            PlayerSettings.Android.bundleVersionCode = code;
            Debug.Log($"[SealCatapult] version {Version} (code {code})");
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.sealcatapult.game");
            // Android only: landscape (either way up), IL2CPP + ARM64 (required by Google Play).
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            ConfigureAdMob();
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.runInBackground = false;
            // The painted store icon (docs/ASO.md §4); the old pup sprite only if that file is missing.
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Reference/app_icon.png")
                ?? AssetDatabase.LoadAssetAtPath<Texture2D>($"{ArtDir}/pup_ball.png");
            if (icon != null) PlayerSettings.SetIcons(UnityEditor.Build.NamedBuildTarget.Unknown, new[] { icon }, IconKind.Any);
        }
    }
}
