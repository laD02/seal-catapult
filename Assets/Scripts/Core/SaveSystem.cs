using System;
using System.IO;
using UnityEngine;

namespace SealCatapult.Core
{
    [Serializable]
    public class SaveData
    {
        public const int LevelCount = 18;    // the hand-made levels; the arrays grow past it as generated castles are played (no end)
        public const int UpgradeCount = 5;

        public int version = 1;
        public int[] stars = new int[LevelCount];     // best stars per level, 0..3
        public int[] upgrades = new int[UpgradeCount]; // level per UpgradeId, 0..3
        public int[] perfect = new int[LevelCount];   // 1 = every bear knocked out at least once
        public int spentStars;
        public const float DefaultMusic = 0.6f;
        public const float DefaultSfx = 0.9f;
        public float musicVolume = DefaultMusic;
        public float sfxVolume = DefaultSfx;
        public bool muted;
        public string language = "";                   // "" = follow system language
        // Statistics (GDD §R5): every bear ever knocked out, and pups that knocked out 2, 3, or 4+ bears in one dive.
        public int totalBears;
        public int combo2;
        public int combo3;
        public int combo4;
        public bool seenIntro;                         // the opening story played once already
        public bool seenHowTo;                         // unused since 15/11 (the first Play starts level 1); kept so old saves still load
        public bool reviewAsked;                       // Play took the one in-app review request (kept by Erase)
        // Skin shop (Progress/Skins.cs): skins are bought with stars, like the upgrades (spentStars counts both).
        public string[] skins = new string[0];          // ids of the skins bought (the free "classic" ones are never stored)
        public string[] worn = new string[0];           // id worn per SkinKind; "" = that kind's classic

        /// <summary>Repairs data from older or hand-edited files so the game never indexes out of range.</summary>
        public void Normalize()
        {
            // Keep generated levels' results, but never trust a file's own length past the game's last level (a corrupt or
            // hand-edited save with a huge array would allocate it at boot).
            int levels = Mathf.Clamp(Mathf.Max(stars?.Length ?? 0, perfect?.Length ?? 0), LevelCount,
                Mathf.Max(LevelCount, SealCatapult.Progress.LevelCatalog.Total));
            stars = Resize(stars, levels, 0, 3);
            upgrades = Resize(upgrades, UpgradeCount, 0, 3);
            perfect = Resize(perfect, levels, 0, 1);
            musicVolume = float.IsNaN(musicVolume) ? DefaultMusic : Mathf.Clamp01(musicVolume);   // Clamp01 keeps NaN
            sfxVolume = float.IsNaN(sfxVolume) ? DefaultSfx : Mathf.Clamp01(sfxVolume);
            int earned = 0;
            foreach (int s in stars) earned += s;
            spentStars = Mathf.Clamp(spentStars, 0, earned);   // the shop balance never goes negative
            totalBears = Mathf.Max(0, totalBears);
            combo2 = Mathf.Max(0, combo2);
            combo3 = Mathf.Max(0, combo3);
            combo4 = Mathf.Max(0, combo4);
            if (Array.IndexOf(Loc.Codes, language) < 0) language = "";
            SealCatapult.Progress.Skins.Normalize(this);
        }

        /// <summary>Best stars of a level; 0 for one never won (the arrays only reach as far as levels were played).</summary>
        public int StarsOf(int level) => level >= 0 && level < stars.Length ? stars[level] : 0;

        public bool PerfectOf(int level) => level >= 0 && level < perfect.Length && perfect[level] > 0;

        /// <summary>Keeps the best result of a level, growing the arrays for the generated castles.</summary>
        public void Record(int level, int starsWon, bool perfectWon)
        {
            if (level < 0) return;
            if (level >= stars.Length) stars = Resize(stars, level + 1, 0, 3);
            if (level >= perfect.Length) perfect = Resize(perfect, level + 1, 0, 1);
            stars[level] = Mathf.Max(stars[level], Mathf.Clamp(starsWon, 0, 3));
            if (perfectWon) perfect[level] = 1;
        }

        /// <summary>Records one pup's dive: how many bears it knocked out (combos of 2+ are counted by size).</summary>
        public void AddDive(int bears)
        {
            if (bears >= 4) combo4++;
            else if (bears == 3) combo3++;
            else if (bears == 2) combo2++;
        }

        public int PerfectCount()
        {
            int n = 0;
            foreach (int p in perfect) n += p;
            return n;
        }

        static int[] Resize(int[] source, int length, int min, int max)
        {
            var result = new int[length];
            if (source != null)
                for (int i = 0; i < Mathf.Min(length, source.Length); i++)
                    result[i] = Mathf.Clamp(source[i], min, max);
            return result;
        }
    }

    /// <summary>JSON save in Application.persistentDataPath/save.json (readable, hand-editable for debugging).</summary>
    public static class SaveSystem
    {
        public static SaveData Data { get; private set; } = new SaveData();

        public static string DefaultPath => Path.Combine(Application.persistentDataPath, "save.json");

        static bool loaded;

        /// <summary>Editor play mode without a domain reload keeps statics: start each play from the file again.</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            loaded = false;
            SaveFailed = LoadFailed = false;
            Problem = null;
        }

        /// <summary>True while the last write did not reach the disk (storage full): the game retries on the next save
        /// and when it goes to the background, and tells the player once (<see cref="Problem"/>).</summary>
        public static bool SaveFailed { get; private set; }

        /// <summary>True when the save file was there but unreadable, so progress started from zero.</summary>
        public static bool LoadFailed { get; private set; }

        /// <summary>A save problem the player must hear about, as a Loc key ("save_failed", "save_damaged"); UI/SaveNotice
        /// shows it. Raised on the main thread.</summary>
        public static event Action<string> Problem;

        public static void Load()
        {
            Data = LoadFrom(DefaultPath, out bool damaged);
            LoadFailed = damaged;
            SealCatapult.Progress.Skins.NotifyChanged();   // the worn outfit's frames, before any pup is drawn
            if (damaged) Problem?.Invoke("save_damaged");
            loaded = true;
        }

        /// <summary>Lets any scene be started directly in the Editor without passing through Boot.</summary>
        public static void EnsureLoaded()
        {
            if (!loaded) Load();
        }

        public static bool Save()
        {
            bool ok = SaveTo(DefaultPath, Data);
            bool first = !ok && !SaveFailed;
            SaveFailed = !ok;
            if (first) Problem?.Invoke("save_failed");   // told once per failure streak, not on every retry
            return ok;
        }

        /// <summary>Writes again when the last save failed (called as the app goes to the background).</summary>
        public static void RetryIfFailed()
        {
            if (SaveFailed) Save();
        }

        public static void ResetProgress()
        {
            var keep = Data;
            Data = new SaveData
            {
                musicVolume = keep.musicVolume,
                sfxVolume = keep.sfxVolume,
                muted = keep.muted,
                language = keep.language,
                reviewAsked = keep.reviewAsked,
            };
            SealCatapult.Progress.Skins.NotifyChanged();   // skins go with the progress: pups go back to classic
            Save();
        }

        public static SaveData LoadFrom(string path) => LoadFrom(path, out _);

        /// <param name="damaged">True when a save file existed but none could be read: progress starts from zero.</param>
        public static SaveData LoadFrom(string path, out bool damaged)
        {
            // A .tmp only exists when a save was cut off before its swap. Complete (it parses): it is the newest save,
            // newer than save.json. Cut off mid-write: it does not parse and save.json is used.
            string temp = path + ".tmp";
            var data = TryRead(temp, backup: false) ?? TryRead(path, backup: true);
            damaged = data == null && (File.Exists(path) || File.Exists(temp));
            data ??= new SaveData();
            data.Normalize();
            return data;
        }

        /// <summary>Null when the file is missing or unreadable. With <paramref name="backup"/> a broken file is kept as
        /// one .corrupt copy (the newest; a copy per launch filled the folder).</summary>
        static SaveData TryRead(string path, bool backup)
        {
            if (!File.Exists(path)) return null;
            string problem;
            try
            {
                // An empty file (a crash while it was created) parses to null without an exception.
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
                if (data != null) return data;
                problem = "empty";
            }
            catch (Exception e)
            {
                problem = e.Message;
            }
            if (!backup) return null;
            // Keep the broken file for inspection instead of silently overwriting it.
            Debug.LogWarning($"[SealCatapult] Save file unreadable ({path}). {problem}");
            TryCopy(path, path + ".corrupt");
            return null;
        }

        /// <summary>Writes a temp file, then swaps it in atomically, so a kill mid-save never leaves no save at all.</summary>
        public static bool SaveTo(string path, SaveData data)
        {
            try
            {
                string temp = path + ".tmp";
                // Flushed to the disk before the swap: a rename can outlive a power cut that its data blocks did not,
                // which would leave an empty save.
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(data, true));
                using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }
                if (File.Exists(path))
                {
                    try { File.Replace(temp, path, null); }
                    catch (Exception e) when (e is PlatformNotSupportedException || e is IOException)
                    {
                        // Some runtimes lack an atomic replace; a kill between these two lines is recovered from .tmp.
                        File.Delete(path);
                        File.Move(temp, path);
                    }
                }
                else File.Move(temp, path);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SealCatapult] Could not write save file at {path}: {e.Message}");
                return false;
            }
        }

        static void TryCopy(string from, string to)
        {
            try { File.Copy(from, to, true); }
            catch (Exception e) { Debug.LogWarning($"[SealCatapult] Could not back up corrupt save: {e.Message}"); }
        }
    }
}
