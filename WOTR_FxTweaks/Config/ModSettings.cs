using Newtonsoft.Json;
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static UnityModManagerNet.UnityModManager;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Settings;
using Kingmaker.UI.SettingsUI;
using UnityEngine;
using Kingmaker.Localization;
using HarmonyLib;
using Kingmaker;

namespace WOTR_FxTweaks.Config {
    public class Buff {
		public string Name {get;set;}
		public string Id {get;set;}
		public bool DisableFx {get;set;}
		public string OverrideFxId {get;set;}
        public bool IsMythicClassFx { get; set; }
        public bool IsAreaEffectFx { get; set; }
        public bool IsSupported { get; set; }
        public bool ProcessEvenIfUnsupported { get; set; }
        public static int CompareByNameThenGuid(Buff a, Buff b)
        {
            int nameComparison = a.Name.CompareTo(b.Name);
            if (nameComparison == 0)
                nameComparison = a.Id.CompareTo(b.Id);

            return nameComparison;
        }
    }


    [HarmonyPatch(typeof(UISettingsManager), "Initialize")]
    static class SettingsInjector
    {
        static bool Initialized = false;

        private static UISettingsGroup MakeSettingsGroup(string key, string name, params UISettingsEntityBase[] settings)
        {
            UISettingsGroup group = ScriptableObject.CreateInstance<UISettingsGroup>();
            group.Title = ModSettings.CreateString(key, name);

            group.SettingsList = settings;

            return group;
        }
        private static (SettingsEntityBool, Buff) AddNativeUI(string id)
        {
            string name = "";

            if (!ModSettings.BuffsByID.TryGetValue(id, out var buff))
            {
                Main.Error($"Could not find Buff {id} when creating in-game UI. Attempting to recreate buff.");
                var blueprint = ResourcesLibrary.TryGetBlueprint(BlueprintGuid.Parse(id));
                bool isClassFX = false;
                if (blueprint is BlueprintBuff)
                {
                    name = (blueprint as BlueprintBuff).Name;
                }
                else if (blueprint is BlueprintCharacterClass)
                {
                    name = "_" + (blueprint as BlueprintCharacterClass).Name;
                    isClassFX = true;
                }

                buff = new Buff
                {
                    Name = name,
                    Id = id,
                    DisableFx = false,
                    OverrideFxId = null,
                    IsMythicClassFx = isClassFX,
                    IsAreaEffectFx = false,
                    IsSupported = true,
                    ProcessEvenIfUnsupported = false
                }; // technically should parse to check IsAreaEffectFx, but really this is never being called so will deal with it if needed

                ModSettings.BuffsByID.Add(id, buff);
            }

            var listToWrite = ModSettings.BuffsByID.Values.ToList();
            listToWrite.Sort(Buff.CompareByNameThenGuid);

            var setting = new SettingsEntityBool($"decair.wotr.fx-tweaks.{id}", false, false, true);
            setting.SetValueAndConfirm(buff.DisableFx);
            (setting as IReadOnlySettingEntity<bool>).OnValueChanged += (newValue) =>
            {
                buff.DisableFx = newValue;

                ModSettings.WriteBuffList(listToWrite);
            };
            return (setting, buff);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "harmony patch")]
        static void Postfix()
        {
            if (Initialized) return;
            Initialized = true;
            if (!Main.Enabled) { return; }

            Main.Log("Injecting settings into Graphics Options...");

            try
            {

                var ui = ModSettings.NativeSettingsToDisable.Select(id =>
                {
                    var (setting, buff) = AddNativeUI(id);
                    var control = ScriptableObject.CreateInstance<UISettingsEntityBool>();

                    if (buff.IsMythicClassFx)
                    {
                        // Prettify the class name by removing underscore, adding spacing and getting rid of trailing Mythic Class
                        string displayName = Regex.Replace(Regex.Replace(buff.Name.TrimStart('_'), "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1"), @" Mythic Class$", String.Empty);
                        control.m_Description = ModSettings.CreateString($"{setting.Key}.description", $"Disable FX for {displayName}");
                        control.m_TooltipDescription = ModSettings.CreateString($"{displayName}.tooltip-description", $"Disables ongoing visual and audio effects for the {displayName} mythic class.\n\nRequires a game restart to take effect.\n\nFor additional configuration options, see FX Tweaks Readme.md.");
                    }
                    else if (buff.IsAreaEffectFx)
                    {
                        // Prettify the buff name by adding spacing and getting rid of trailing Area
                        string displayName = Regex.Replace(Regex.Replace(buff.Name, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1"), @" Area$", String.Empty);
                        control.m_Description = ModSettings.CreateString($"{setting.Key}.description", $"Disable FX for {displayName}");
                        control.m_TooltipDescription = ModSettings.CreateString($"{setting.Key}.tooltip-description", $"Disables ongoing visual and audio effects for the {displayName} area effect.\n\nRequires a game restart to take effect.\n\nFor additional configuration options, see FX Tweaks Readme.md.");
                    }
                    else
                    {
                        // Prettify the buff name by adding spacing and getting rid of trailing Buff
                        string displayName = Regex.Replace(Regex.Replace(buff.Name, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1"), @" Buff$", String.Empty);
                        control.m_Description = ModSettings.CreateString($"{setting.Key}.description", $"Disable FX for {displayName}");
                        control.m_TooltipDescription = ModSettings.CreateString($"{setting.Key}.tooltip-description", $"Disables ongoing visual and audio effects for the {displayName} buff.\n\nRequires a game restart to take effect.\n\nFor additional configuration options, see FX Tweaks Readme.md.");
                    }
                    control.LinkSetting(setting);
                    return (control, buff);
                });

                var classDisables = ui.Where(tuple => tuple.buff.IsMythicClassFx).Select(tuple => tuple.control).ToArray();
                if (classDisables.Length > 0)
                    Game.Instance.UISettingsManager.m_GraphicsSettingsList.Add(MakeSettingsGroup("decair.wotr.fx-tweaks.classes", "FX Tweaks - Mythic Classes", classDisables));

                var aoeDisables = ui.Where(tuple => tuple.buff.IsAreaEffectFx).Select(tuple => tuple.control).ToArray();
                if (aoeDisables.Length > 0)
                    Game.Instance.UISettingsManager.m_GraphicsSettingsList.Add(MakeSettingsGroup("decair.wotr.fx-tweaks.aoes", "FX Tweaks - Common Area Effects", aoeDisables));

                var buffDisables = ui.Where(tuple => !tuple.buff.IsMythicClassFx && !tuple.buff.IsAreaEffectFx).Select(tuple => tuple.control).ToArray();
                if (buffDisables.Length > 0)
                    Game.Instance.UISettingsManager.m_GraphicsSettingsList.Add(MakeSettingsGroup("decair.wotr.fx-tweaks.buffs", "FX Tweaks - Common Buffs", buffDisables));
            }
            catch (Exception ex)
            {
                Main.Error(ex);
            }
            Main.Log("Injection process complete.");
        }
    }

    static class ModSettings
    {
        public static ModEntry ModEntry;
        public static List<Buff> Buffs;
        public static List<Buff> BuffsToTweak;
        public static Dictionary<string, Buff> BuffsByID = new Dictionary<string, Buff>();
        private static string UserConfigFolder => ModEntry.Path + "UserSettings";

        public static string SettingsPath(string file)
        {
            return Path.Combine(UserConfigFolder, file);
        }

        public static string ResourcePath(string file)
        {
            return $"WOTR_FxTweaks.UserSettings.{file}";
        }

        private static bool TryReadSettingsResource(string file, out string contents)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resource = ResourcePath(file);

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            using (StreamReader streamReader = new StreamReader(stream))
            {
                try
                {
                    contents = streamReader.ReadToEnd();
                    return true;
                }
                catch (Exception e)
                {
                    Main.Error(e.Message);
                    contents = null;
                    return false;
                }
            }
        }

        private static bool TryReadSettingsFile(string file, out string contents)
        {
            string path = SettingsPath(file);

            if (File.Exists(path))
            {
                contents = File.ReadAllText(path);
                return true;
            }
            else
            {
                contents = null;
                return false;
            }
        }

        private static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            CheckAdditionalContent = false,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            DefaultValueHandling = DefaultValueHandling.Include,
            FloatParseHandling = FloatParseHandling.Double,
            Formatting = Formatting.Indented,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            StringEscapeHandling = StringEscapeHandling.Default,
        };

        public static void WriteBuffList(List<Buff> buffList)
        {
            var serializer = JsonSerializer.Create(SerializerSettings);
            var path = SettingsPath("Buffs.json");

            Main.Log("Persisting updated settings.");
            Directory.CreateDirectory(UserConfigFolder);

            using var fileStream = File.CreateText(path);
            using var jsonWriter = new JsonTextWriter(fileStream);

            serializer.Serialize(jsonWriter, buffList);
        }

        public static void LoadAllSettings()
        {
            Main.Log("Processing user settings.");
            var serializer = JsonSerializer.Create(SerializerSettings);
            JsonReader reader = null;

            if (TryReadSettingsFile("Debug.json", out var fileDebugAsJson))
            {
                Main.EnableDebugLogging = true;
                Main.Log("Debug logging enabled.");
            }

            if (TryReadSettingsResource("Buffs.json", out var resourceSettingsAsJson))
            {
                // Load supported FX from resource and add to dictionary
                reader = new JsonTextReader(new StringReader(resourceSettingsAsJson));
                Buffs = serializer.Deserialize<List<Buff>>(reader);
                Main.DebugLog("Loaded Buffs from resource.");
                foreach (var buff in Buffs)
                    BuffsByID.Add(buff.Id, buff);
                Main.DebugLog("Loaded dictionary from Buffs.");

                // Read from settings file, override defaults, add unsupported (ie. user added via file)
                if (TryReadSettingsFile("Buffs.json", out var fileSettingsAsJson))
                {
                    Main.DebugLog("Found file Buffs.json. Updating Buffs with user settings.");
                    try
                    {
                        reader = new JsonTextReader(new StringReader(fileSettingsAsJson));
                        var buffsFromFile = serializer.Deserialize<List<Buff>>(reader);
                        foreach (var fbuff in buffsFromFile)
                        {
                            if (BuffsByID.TryGetValue(fbuff.Id, out var mbuff))
                            {
                                mbuff.DisableFx = fbuff.DisableFx;
                                mbuff.OverrideFxId = fbuff.OverrideFxId;
                                mbuff.IsSupported = true;
                            } else
                            {
                                Main.DebugLog($"Found new unsupported buff. Adding to Buffs and dictionary: {fbuff.Name + ':' + fbuff.Id}");
                                var nbuff = new Buff
                                {
                                    Name = fbuff.Name,
                                    Id = fbuff.Id,
                                    DisableFx = fbuff.DisableFx,
                                    OverrideFxId = fbuff.OverrideFxId,
                                    IsMythicClassFx = fbuff.IsMythicClassFx,
                                    IsAreaEffectFx = fbuff.IsAreaEffectFx,
                                    IsSupported = false,
                                    ProcessEvenIfUnsupported = fbuff.ProcessEvenIfUnsupported
                                };

                                Buffs.Add(nbuff);
                                BuffsByID.Add(nbuff.Id, nbuff);
                            }
                        }
                        Main.DebugLog("Completed Buffs.json processing.");
                    }
                    catch (Exception e)
                    {
                        Main.Error(e.Message);
                        Main.Error("Unable to parse user settings (Buffs.json). Malformed json? Archiving settings contents to BROKEN_Buffs.json. Settings will be rebuilt using defaults.");
                        try
                        {
                            File.Copy(SettingsPath("Buffs.json"), SettingsPath("BROKEN_Buffs.json"), true);
                        }
                        catch
                        {
                            Main.Error("Failed to archive broken settings.");
                        }
                    }
                } else
                {
                    Main.Log("No previous user settings (Buffs.json) found. Using default settings.");
                }

                // Load list we use for processing buffs, sorting so that we handling overrides prior to FX removal
                BuffsToTweak = Buffs
                    .Where(b => !(string.IsNullOrEmpty(b.OverrideFxId)) || b.DisableFx)
                    .Where(b => b.IsSupported || b.ProcessEvenIfUnsupported)
                    .OrderBy(b => b.DisableFx)
                    .ToList();

                Main.Log($"Found {BuffsToTweak.Count} buffs to tweak.");

                // Write updated Buffs.json
                Buffs.Sort(Buff.CompareByNameThenGuid);
                WriteBuffList(Buffs);
            }
            else
            {
                Main.Error("Failed to load default settings from internal resource.  Mod disabled.");
                Main.Enabled = false;
            }
        }

        // All localized strings created in this mod, mapped to their localized key. Populated by CreateString.
        static readonly Dictionary<String, LocalizedString> textToLocalizedString = new Dictionary<string, LocalizedString>();
        public static LocalizedString CreateString(string key, string value)
        {
            // See if we used the text previously.
            // (It's common for many features to use the same localized text.
            // In that case, we reuse the old entry instead of making a new one.)
            LocalizedString localized;
            if (textToLocalizedString.TryGetValue(value, out localized))
            {
                return localized;
            }
            var strings = LocalizationManager.CurrentPack.Strings;
            String oldValue;
            if (strings.TryGetValue(key, out oldValue) && value != oldValue)
            {
#if DEBUG
                Main.Log($"Info: duplicate localized string `{key}`, different text.");
#endif
            }
            strings[key] = value;
            localized = new LocalizedString
            {
                m_Key = key
            };
            textToLocalizedString[value] = localized;
            return localized;
        }

        public static List<string> NativeSettingsToDisable = new List<string>()
        {
            "15a85e67b7d69554cab9ed5830d0268e", // Aeon
            "a5a9fe8f663d701488bd1db8ea40484e", // Angel
            "9a3b2c63afa79744cbca46bea0da9a16", // Azata
            "8e19495ea576a8641964102d177e34b7", // Demon
            "211f49705f478b3468db6daa802452a2", // Devil
            "daf1235b6217787499c14e4e32142523", // GoldenDragon
            "5d501618a28bdc24c80007a5c937dcb7", // Lich
            "5295b8e13c2303f4c88bdb3d7760a757", // Swarm
            "8df873a8c6e48294abdb78c45834aa0a", // Trickster
            "533592a86adecda4e9fd5ed37a028432", // Barkskin
            "e4e9f9169c9b28e40aa2c9d10c369254", // BlessingOfUnlife
            "dd3ad347240624d46a11a092b4dd4674", // Blur
            "632cad6b7c66b8449a300c6476fd5e1b", // DenyDeath
            "00402bae4442a854081264e498e7a833", // Displacement
            "b574e1583768798468335d8cdb77e94c", // FieryBody
            "082caf8c1005f114ba6375a867f638cf", // GeniekindDjinni
            "d47f45f29c4cfc0469f3734d02545e0b", // GeniekindEfreeti
            "4f37fc07fe2cf7f4f8076e79a0a3bfe9", // GeniekindMarid
            "1d498104f8e35e246b5d8180b0faed43", // GeniekindShaitan
            "8d20b0a6129bd814eb0146041879f38a", // Haste
            "a6da7d6a5c9377047a7bd2680912860f", // IceBody
            "98dc7e7cc6ef59f4abe20c65708ac623", // MirrorImage
            "e244d6faef3daf747baa5592482a8b79", // NegativeEruption
            "bb08ad05d0b4505488775090954c2317", // ProfaneNimbus
            "57b1c6a69c53f4d4ea9baec7d0a3a93a", // SacredNimbus
            "dea0dba1f7bff064987e03f1307bfa84", // SenseVitals
            "7aeaf147211349b40bb55c57fec8e28d", // Stoneskin
            "6215b25fbc1a36748b5606ebc0092074", // StoneskinBuffCL11
            "e089f04285f995443a2e295d9b7a40e0", // StoneskinBuffLichImproved
            "714244637d461354b85b1808e7c6c814", // StoneskinCommunal
            "b2ad92b1b417b4441a457bda918a303f", // ClemencyOfShadowsArea
            "b21bc337e2beaa74b8823570cd45d6dd", // SiroccoArea
            "26a69e73f7d7188439e30aff30e76134"  // StarlightArea
        };
    }
}
