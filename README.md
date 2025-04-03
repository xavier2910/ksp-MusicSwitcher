# MusicSwitcher
Plugin to change the music in Kerbal Space Program.
Compatible with version 1.12.5. Presumably compatible with some amount of earlier versions but I don't
have them (or KSP on Steam) so I can't test it out.

## Dependencies
* [ModuleManager](https://github.com/sarbian/ModuleManager)

## Building
These instructions are for building *without* Visual Studio. I don't use VS and so I don't know how to
use it. But never fear; all you need to perform this build process is a terminal, the .NET Framework
\(if you have VS and have worked on a plugin before, you should have this\) OR the latest version of
Mono (I have 6.12), and your favorite text editor (I use VSCode; VS would probably work too).

Ideally you want to have git, too.

To build:

* Make sure you have all [dependencies](#dependencies) installed to the KSP you are going to install
  MusicSwitcher to.
* clone the repo using git (recommended) or download the source zip.
* In the `src/` directory, create the file `MusicSwitcher.csproj.user` and type the following into it:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Project>
    <PropertyGroup>
        <KSPRoot>/path/to/KSP/</KSPRoot>
        <RepoRootPath>/path/to/where/you/cloned/ksp-MusicSwitcher/</RepoRootPath>
    </PropertyGroup>
</Project>
```
* Then open your terminal in the repo root and run `msbuild src/MusicSwitcher.csproj -t:restore` to install
  Nuget packages.
* Now you can finally build the plugin! Run `msbuild src/MusicSwitcher.csproj`.
* Finally, create a symlink to the built plugin in your GameData folder:
  * On Linux, cd into your GameData folder \(the KSP one, *not* the one in the repo\) and
    run `ln -s /path/to/ksp-MusicSwitcher/GameData/MusicSwitcher`
  * On Windows, cd \(or open a new terminal, idc\) into your GameData folder \(the KSP one\) and
    run `mklink /j MusicSwitcher "P:\ath\to\ksp-MusicSwitcher\GameData\MusicSwitcher`
  * On Mac, consult google or your nearest computer dealer.
  * Or, of course, you could just copy the MusicSwitcher folder into your GameData instead of making
    a link.

Congrats; you have now installed MusicSwitcher! Enjoy.
