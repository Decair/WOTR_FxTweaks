using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using WOTR_FxTweaks.Config;

namespace WOTR_FxTweaks
{
    static class Spells
    {
		[HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;
                if (ModSettings.BuffsToTweak.Count > 0)
                {
                    if (!Main.Enabled) { return; }

                    foreach (Buff buffToTweak in ModSettings.BuffsToTweak)
                    {
                        Main.Log("Tweaking: " + buffToTweak.Name);
                        var BlueprintBuffToTweak = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(buffToTweak.Id);
                        if (buffToTweak.DisableFx)
                        {
                            BlueprintBuffToTweak.FxOnStart = BlueprintBuffToTweak.FxOnRemove;
                        } else
                        {
                            var OverrideBlueprintBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(buffToTweak.OverrideFxId);
                            BlueprintBuffToTweak.FxOnStart = OverrideBlueprintBuff.FxOnStart;
                        }
                    }
                }
            }
        }
    }
}
