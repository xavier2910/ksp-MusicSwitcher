# Building
These instructions are for building *without* Visual Studio. I don't use VS and so I don't know how to
use it. But never fear; all you need to perform this build process is a terminal, the .NET SDK \(I have dotnet 8\) --
if you have VS and have worked on a plugin before, you should have this -- and your favorite text editor (I use VSCode; VS would probably work too). If you're not on Windows, you will probably need the latest Mono installed (6.12).

Ideally you want to have git, too.

To build:

* Make sure you have all [dependencies](#dependencies) installed to the KSP you are going to install
  MusicSwitcher to. If you have CKAN, it will automatically install dependencies from CKAN.
* clone the repo using git (recommended) or download the source zip.
* In the `src/` directory, create the file `MusicSwitcher.csproj.user` and type the following into it:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Project>
    <PropertyGroup>
        <KSPRoot>/path/to/KSP/</KSPRoot>
    </PropertyGroup>
</Project>
```
* Then open your terminal in the repo root and run `dotnet restore` to install
  Nuget packages.
* Now you can build the plugin! Run `dotnet build`.
* Finally, create a symlink to the built plugin in your GameData folder:
  * On Linux, cd into your GameData folder \(the KSP one, *not* the one in the repo\) and
    run `ln -s /path/to/ksp-MusicSwitcher/GameData/MusicSwitcher`
  * On Windows, cd \(or open a new terminal, idc\) into your GameData folder \(the KSP one\) and
    run `mklink /j MusicSwitcher "P:\ath\to\ksp-MusicSwitcher\GameData\MusicSwitcher`
  * On Mac, consult google or your nearest computer dealer.
  * Or, of course, you could just copy the MusicSwitcher folder into your GameData instead of making
    a link.

Congrats; you have now installed MusicSwitcher! Enjoy.

>[!IMPORTANT]
>**If you, for whatever reason, cannot install the dotnet SDK on your machine, never fear!** You can simply use msbuild from
>Mono like so: `msbuild src/MusicSwitcher.csproj -t:restore` followed by `msbuild src/MusicSwitcher.csproj` to restore
>Nuget packages and then build the plugin.
