# Buff FX Tweaks

WOTR Mod which allows users to disable and to override FX effects of buffs.

By default the mod disables the BlessingOfUnlife and Stoneskin buff FXs. If you don't like this behavior, see How to Use for instructions around how to change DisableFx for BlessingOfUnlife and the four Stoneskin buffs from true to false.

# How to Install

1. Download the latest published zip file.
2. Install [UnityModManager](https://www.nexusmods.com/site/mods/21)
3. Install the zip with UnityModManager.

# How to Use

Configure Buff FX Tweaks by editing the Buffs.json file present in the UserSettings folder of the mod.

To disable an FX, find the buff whose FX you wish to disable in Buffs.json, and change DisableFx from false to true.

To override an FX with a different FX, first determine the buff whose FX you wish to have displayed in game (we'll call that the override buff) and find that in Buffs.json. Copy the id of the override buff. Then find the buff whose FX you wish to replace in Buffs.json. Paste the id of the override buff into the value of OverrideFxId.

Notes:
-  Only override the FX of a Mythic Class (denoted with "IsMythicClassFx = true", located at the beginning of buffs.json) with the FX of another Mythic Class. Similarly, only override the FX of a normal buff (won't have an "IsMythicClassFx") with another normal buff.
-  The most challenging part of configuring the mod is determining which named buff actually corresponds to the buff you're trying to tweak. There are a lot of similar buffs in WOTR blueprints, so you may need to experiment a bit.

# Q & A
**Q:** Do I always need to replace Buffs.json or can I continue to use my previous one?  
**A:** I will always try to make non-breaking changes (so things should be additive).  The change from v1.0.0 to v1.0.1 was an exception as it intentionally removes buffs which were creating an issue.  That said, you obviously can't disable / override newly added buffs without them being present in Buffs.json. To keep using your old Buffs.json, prior to an update please copy the old Buffs.json to a new name/location; then after the update you can overwrite the Buffs.json included in the update.

**Q:** On an update, can I just add select json objects / buffs vs. replacing the entire Buffs.json?  
**A:** Yep, this currently works fine. You obviously need an understanding of how to edit json, as the mod won't work if you create malformed json.

**Q:** Having to possibly re-edit my json changes on an update sucks. Is there an easier way?  
**A:** Yeah, I don't like this either. I'm evaluating how to best migrate settings from earlier versions into new ones, and hope to support this in a future release.
 
# Change List
### v1.1.0
**New Feature:** *You can now disable and override FX for the visual changes stemming from your Mythic Path choices. For example, you can disable or override the visual change from being a Lich.*

The fix for Drezen in 1.0.1 missed including some player buffs. It's unclear which, so please let me know in WOTR Discord should you find something you need missing. Found the following buffs missing and added them back to Buffs.json: Angel Sword Speed of Light, various Wings buffs, Protection from Spells, and certain Stoneskin variations.

### v1.0.1
v1.0.0 supported over 4600 buffs, player and non-player, but disabling certain non-player buffs resulted in a crash entering Drezen. Addresses the issue by reducing the list to only player buffs.

Please ensure that you're replacing your Buffs.json file with the one from this release and updating that new Buffs.json file with any tweaks you wish to make. Apologies for the inconvenience.

### v1.0.0
Initial release of Buff FX Tweaks, which enables users to disable and to override FX effects of buffs.

# How to Compile

1. Install all required development pre-requisites:
	- [Visual Studio 2019 Community Edition](https://visualstudio.microsoft.com/downloads/)
	- [.NET "Current" x86 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks)
2. Download and install [Unity Mod Manager (UMM)](https://www.nexusmods.com/site/mods/21)
3. Execute UMM, Select Pathfinder: WoTR, and Install
4. Create the environment variable *WrathInstallDir* and point it to your Pathfinder: WoTR game home folder
	- tip: search for "edit the system environment variables" on windows search bar45. Use "Install Release" or "Install Debug" to have the Mod installed directly to your Game Mods folder

NOTE Unity Mod Manager and this mod template make use of [Harmony](https://go.microsoft.com/fwlink/?linkid=874338)

# Links

Source code: https://github.com/Decair/WOTR_FxTweaks

# Credits

Thanks to:

1. [ThyWoof's mod template](https://github.com/ThyWoof/PathfinderWoTRModTemplate), for an auto-setup template for WOTR mods.
2. [Xenofell's Tweakable Weapon Categories mod](https://github.com/cstamford/WOTR_TweakableWeaponCategories), for getting me going way beyond the base template.
3. [Vek17's TTT mod](https://github.com/Vek17/WrathMods-TabletopTweaks), for many examples of how to make behavioral and blueprint changes.
4. All the folks at Owlcat's Pathfinder WOTR discord #mod-dev-technical channel, for the multiple times they pointed me in the right direction (especially WittleWolfie - thank you!).

