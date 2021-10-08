# Buff Fx Tweaks

WOTR Mod which allows users to disable, and perhaps eventually tweak, FX effects of buffs.

# How to install

0. Download the latest published zip file.
1. Install [UnityModManager](https://www.nexusmods.com/site/mods/21)
2. Install the zip with UnityModManager.

# How to use

Ctrl + F10 to access the mod menu. Then simply navigate to the buff and enable or disable the FX as desired.  Visual effect changes should take place the next time the buff is cast or on reloading a save.

# How to Compile

0. Install all required development pre-requisites:
	- [Visual Studio 2019 Community Edition](https://visualstudio.microsoft.com/downloads/)
	- [.NET "Current" x86 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks)
1. Download and install [Unity Mod Manager (UMM)](https://www.nexusmods.com/site/mods/21)
2. Execute UMM, Select Pathfinder: WoTR, and Install
3. Create the environment variable *WrathInstallDir* and point it to your Pathfinder: WoTR game home folder
	- tip: search for "edit the system environment variables" on windows search bar45. Use "Install Release" or "Install Debug" to have the Mod installed directly to your Game Mods folder

NOTE Unity Mod Manager and this mod template make use of [Harmony](https://go.microsoft.com/fwlink/?linkid=874338)

# Links

Source code: https://github.com/Decair/WOTR_FxTweaks

# Credits

Thanks to:

0. [ThyWoof's mod template](https://github.com/ThyWoof/PathfinderWoTRModTemplate), for an auto-setup template for WOTR mods.
1. [Xenofell's Tweakable Weapon Categories mod](https://github.com/cstamford/WOTR_TweakableWeaponCategories), for getting me going way beyond the base template.
2. [Vek17's TTT mod](https://github.com/Vek17/WrathMods-TabletopTweaks), for many examples of how to make behavioral and blueprint changes.
3. All the folks at Owlcat's Pathfinder WOTR discord #mod-dev-technical channel, for the multiple times they pointed me in the right direction (especially WittleFolfie - thank you!).

