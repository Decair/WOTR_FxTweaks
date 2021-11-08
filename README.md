# FX Tweaks

WOTR Mod which allows users to disable and to override ongoing effects (FX) of buffs, area effects and certain Mythic Classes.

Designed to be minimalistic. Will not impact your save file and can be disabled or removed without side-effects.

By default the mod disables the BlessingOfUnlife and Stoneskin buff FXs. If you don't like this behavior, see How to Use for instructions around how to change this.

# How to Install

1. Download the latest published zip file.
2. Install [UnityModManager](https://www.nexusmods.com/site/mods/21)
3. Install the zip with UnityModManager.

# How to Use

A subset of FX Tweaks configuration is supported in-game within WOTR's Settings-->Graphics menu (scroll to the bottom). You **must restart WOTR** for changes to take effect.

As there are approaching 1200 FX that one can configure, users can fully configure FX Tweaks through direct editing of the Buffs.json file present in the UserSettings folder of the mod.  To configure via Buffs.json: 
-  If this is a new install of FX Tweaks (vs. an upgrade), then you will need to launch WOTR prior to Buffs.json being geneterated and available for configuration.
-  To disable an FX, find the effect whose FX you wish to disable in Buffs.json, and change DisableFx from false to true.
-  To override an FX with a different FX, first determine the effect whose FX you wish to have displayed in game (we'll call that the override buff) and find that in Buffs.json. Copy the id of the override buff. Then find the effect whose FX you wish to replace in Buffs.json. Paste the id of the override buff into the value of OverrideFxId.
    -  Only override the FX with an effect of the same type. FX from a Mythic Class (denoted with "IsMythicClassFx = true", located at the beginning of buffs.json) can only be overriden with the FX of another Mythic Class. FX from an Area Effect (denoted with "IsAreaEffectFx = true") can only be overriden with the FX of another Area Effect. Normal buffs can only be overriden with the FX of another normal buff.
-  To add an unsupported buff FX:  
    -  First you'll need the id for the buff (the Toybox mod is a good source for this)
	-  Then clone an existing buff object in Buffs.json, replacing the Name and Id with the new values
	-  Set IsMythicClassFx to false (as this is a buff)
	-  As this isn't a supported buff, also set ProcessEvenIfUnsupported to true (without doing this FX Tweaks will ignore it as unsupported)
	-  Configure DisableFx or OverrideFxId as desired
	-  Profit.  Although do note that disabling an FX which isn't a typical player initiated buff may result in crashes (especially when entering towns like Drezen). Save early and often.
-  The most challenging part of json configuration is determining which named FX actually corresponds to the FX you're trying to tweak. The naming standard was likely obvious to Owlcat developers, but that doesn't always translate well to us users.

# Known Issues

-  The new in-game configuration experience is currently limited to english.
-  The mechanism used to filter out non-player buffs (which created the Drezen issue in v1.0.0) may have been too restrictive.  If you notice any missing player buffs in Buffs.json, please create an issue in [github](https://github.com/Decair/WOTR_FxTweaks).  Note that it is intended that the in-game configuration UI does not include every tweakable buff and this won't change (not ever going to list 1200 FX in said UI).
-  There are some visual effects (I'm looking at you Enlarge and Polymorph) which are only partially implemented by Owlcat's buff FX, where the remainder of the effect involves things like changing the base character scale and/or model. Currently FX Tweaks cannot override changes to a character's model or scale. I may add this in the future, but note that the Visual Adjustments mod does give you a ton of control over the visual appearance of your characters and can override any model or scale changes made by spells or abilities.

# Q & A
**Q:** If I'm migrating from v1.0.0, can I keep my existing Buffs.json?  
**A:** You can, but its absolutely not recommended. There are a lot of FX described in that settings file which can cause your game to crash if disabled. The recommended upgrade path here is to delete the existing v1.0.0 Buffs.json and to do a clean install of the latest version.

**Q:** What will happen when I migrate from an older version to this new version?  
**A:** FX Tweaks will read any existing Buffs.json found in the UserSettings folder of the mod and import any previous DisableFx and OverrideFxId settings for currently supported FX. Buffs.json will then be overwritten to add any newly supported FX and other settings changes. So it is expected that FX Tweaks will keep what you'd configured previously (so long as the specific FX tweak remains supported). You may want to update your Buffs.json should you wish to tweak any newly supported FX.  See Change List to understand whether new FX have been added between the current version and the version you're upgrading from.

**Q:** What will happen if I have unsupported FX configured in Buffs.json when I migrate from an older version to this new version?  
**A:** FX Tweaks will attempt to protect you from unsupported FX as they can cause the game to crash, and the v1.0.0 version of Buffs.json included definitions for these FX. It will still import these FX, but will flag them as unsupported (you can identify these as they'll have a property of IsSupported = false). By default, FX Tweaks will ignore any unsupported FX. This behavior can be overriden by setting ProcessEvenIfUnsupported to true for any unsupported FX you're sure will not cause a crash (or which you're willing to roll the dice on).
 
# Change List
### v1.3.0
**New Feature:** *You can now disable / override 197 new area effect FXs.* Please consider this experimental and be selective in what you disable. There may be some area effects in Buffs.json which are not typical player abilities or equipment, which can have side-effects within cut-scenes and events if disabled.

Added a dozen additional buffs into the in-game menu as requested by users (these were previously only configurable through Buffs.json).

### v1.2.1
Further hardening against the rare mod-conflict which enables another mod to break FX Tweaks' settings parsing. Die, bug, die.

Additional logging enable better troubleshooting of these types of conflicts.

### v1.2.0
**New Feature:** *With many thanks to Bubbles who added this functionality, you can now configure some of the more common tweaks within the in-game Settings-->Graphics menu (scroll to the bottom).* Configuration through Buffs.json will continue to provide much more functionality and flexibility. Note that you **must restart WOTR** for changes to take effect.

**New Feature:** *Buffs.json setting management has changed to support migration of previous settings.* To support this, the mod no longer ships with a Buffs.json, and the game must be launched at least once to generate Buffs.json.

Fixed a rare mod-conflict which enabled another mod to break FX Tweaks' json parsing due to leveraging shared serialization settings.

Updated the mod name from Buff FX Tweaks to just FX Tweaks, as it's been broadening beyond just the buffs it handed in v1.0.0.

v1.2.0 should be compatible with the latest Beta and Release versions of WOTR (so both v1.0.9 and 1.1.0).  If you notice an issue, please create an issue on [github](https://github.com/Decair/WOTR_FxTweaks).

### v1.1.0
**New Feature:** *You can now disable and override FX for the visual changes stemming from your Mythic Path choices. For example, you can disable or override the visual change from being a Lich.*

The fix for Drezen in 1.0.1 missed including some player buffs. It's unclear which, so please let me know in WOTR Discord should you find something you need missing. Found the following buffs missing and added them back to Buffs.json: Angel Sword Speed of Light, various Wings buffs, Protection from Spells, and certain Stoneskin variations.

### v1.0.1
v1.0.0 supported over 4600 buffs, player and non-player, but disabling certain non-player buffs resulted in a crash entering certain locations like Drezen. Addresses the issue by reducing the list to only player buffs.

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

1. Bubbles, who created the awesome snazzy in-game UI for FX Tweaks!
2. [ThyWoof's mod template](https://github.com/ThyWoof/PathfinderWoTRModTemplate), for an auto-setup template for WOTR mods.
3. [Xenofell's Tweakable Weapon Categories mod](https://github.com/cstamford/WOTR_TweakableWeaponCategories), for getting me going way beyond the base template.
4. [Vek17's TTT mod](https://github.com/Vek17/WrathMods-TabletopTweaks), for many examples of how to make behavioral and blueprint changes.
5. All the folks at Owlcat's Pathfinder WOTR discord #mod-dev-technical channel, for the multiple times they pointed me in the right direction (especially WittleWolfie - thank you!).

