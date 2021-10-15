using Newtonsoft.Json;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using static UnityModManagerNet.UnityModManager;

namespace WOTR_FxTweaks.Config {
    public class Buff {
		public string Name {get;set;}
		public string Id {get;set;}
		public bool DisableFx {get;set;}
		public string OverrideFxId {get;set;}
        public bool IsMythicClassFx { get; set; }
    }

    static class ModSettings {
        public static ModEntry ModEntry;
        public static List<Buff> Buffs;
        public static List<Buff> BuffsToTweak;

        public static void LoadAllSettings() {
			var assembly = Assembly.GetExecutingAssembly();
            string fileName = "Buffs.json";
            string userConfigFolder = ModEntry.Path + "UserSettings";
            var userPath = $"{userConfigFolder}{Path.DirectorySeparatorChar}{fileName}";

            if (File.Exists(userPath)) {
				using (StreamReader reader = new StreamReader(userPath)) {
                    try {
                        string settingsJson = reader.ReadToEnd();
                        Buffs = JsonConvert.DeserializeObject<List<Buff>>(settingsJson);
                    } catch (Exception e)
                    {
                        Main.Error("Failed to load user settings.");
                        Main.Error(e.Message);
                    }
                }
            }
			
			BuffsToTweak = Buffs
				.Where(b => !(string.IsNullOrEmpty(b.OverrideFxId)) || b.DisableFx)
                .OrderBy(b => b.DisableFx)
				.ToList();
            Main.Log($"Found {BuffsToTweak.Count} buffs to tweak.");
        }
    }
}
