using Newtonsoft.Json;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
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
                var blueprint = ResourcesLibrary.TryGetBlueprint(BlueprintGuid.Parse(id));
                bool isClassFX = false;
                if (blueprint is BlueprintBuff)
                {
                    name = (blueprint as BlueprintBuff).Name;
                }
                else if (blueprint is BlueprintCharacterClass)
                {
                    name = (blueprint as BlueprintCharacterClass).Name;
                    isClassFX = true;
                }

                buff = new Buff
                {
                    DisableFx = false,
                    IsMythicClassFx = isClassFX,
                    Id = id,
                    OverrideFxId = null,
                    Name = name
                };

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

            Main.Log("Injecting settings");

            try
            {

                var ui = ModSettings.NativeSettingsToDisable.Select(id =>
                {
                    var (setting, buff) = AddNativeUI(id);
                    var control = ScriptableObject.CreateInstance<UISettingsEntityBool>();

                    control.m_Description = ModSettings.CreateString($"{setting.Key}.description", $"Disable FX for {buff.Name}");
                    control.m_TooltipDescription = ModSettings.CreateString($"{setting.Key}.tooltip-description", $"Disable FX for {buff.Name}");
                    control.LinkSetting(setting);
                    return (control, buff);
                });

                var classDisables = ui.Where(tuple => tuple.buff.IsMythicClassFx).Select(tuple => tuple.control).ToArray();
                if (classDisables.Length > 0)
                    Game.Instance.UISettingsManager.m_GraphicsSettingsList.Add(MakeSettingsGroup("decair.wotr.fx-tweaks.classes", "Disable Class FX", classDisables));

                var buffDisables = ui.Where(tuple => !tuple.buff.IsMythicClassFx).Select(tuple => tuple.control).ToArray();
                if (buffDisables.Length > 0)
                    Game.Instance.UISettingsManager.m_GraphicsSettingsList.Add(MakeSettingsGroup("decair.wotr.fx-tweaks.buffs", "Disable Buff FX", buffDisables));
            }
            catch (Exception ex)
            {
                Main.Error(ex);
            }
        }
    }

    static class ModSettings
    {
        public static ModEntry ModEntry;
        public static List<Buff> Buffs;
        public static List<Buff> BuffsToTweak;
        public static Dictionary<string, Buff> BuffsByID = new Dictionary<string, Buff>();

        public static string SettingsPath(string file)
        {
            return Path.Combine(ModEntry.Path, "UserSettings", file);
        }

        private static bool TryReadSettings(string file, out string contents)
        {
            var path = SettingsPath(file);
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

        private static JsonSerializerSettings writeSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public static void WriteBuffList(List<Buff> buffList)
        {
            var serializer = JsonSerializer.Create(writeSettings);
            var path = SettingsPath("Buffs.json");

            using var fileStream = File.CreateText(path);
            using var jsonWriter = new JsonTextWriter(fileStream);

            serializer.Serialize(jsonWriter, buffList);
        }

        public static void LoadAllSettings()
        {
            if (TryReadSettings("Buffs.json", out var settingsAsJson))
            {
                Buffs = JsonConvert.DeserializeObject<List<Buff>>(settingsAsJson);
            }

            foreach (var buff in Buffs)
                BuffsByID.Add(buff.Id, buff);

            Buffs.Sort(Buff.CompareByNameThenGuid);
            WriteBuffList(Buffs);

            BuffsToTweak = Buffs
                .Where(b => !(string.IsNullOrEmpty(b.OverrideFxId)) || b.DisableFx)
                .OrderBy(b => b.DisableFx)
                .ToList();

            Main.Log($"Found {BuffsToTweak.Count} buffs to tweak.");
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
            "7aeaf147211349b40bb55c57fec8e28d",
            "6215b25fbc1a36748b5606ebc0092074",
            "9a3b2c63afa79744cbca46bea0da9a16",
            "0da7299aac601d445a355152084c251a",
        };
    }
}
