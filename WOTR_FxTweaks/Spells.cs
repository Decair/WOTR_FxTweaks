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

namespace WOTR_FxTweaks
{
    static class Spells
    {
        // Leveraged concepts for the below from https://github.com/Vek17/WrathMods-TabletopTweaks and https://github.com/cstamford/WOTR_TweakableWeaponCategories, see Readme.
		[HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;
                PatchBlessingOfUnlife();

                static void PatchBlessingOfUnlife()
                {
                    if (!Main.Enabled) { return; }

                    // 
					var BlessingOfUnlifeBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("e4e9f9169c9b28e40aa2c9d10c369254");
                    BlessingOfUnlifeBuff.FxOnStart = BlessingOfUnlifeBuff.FxOnRemove;
                }
            }
        }
    }
}
