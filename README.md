# DAPI

This repository is a collection of some Discord bots written in C#.

## Used packages

* .NET Core 3.1
* [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus/blob/master/README.md)

## Bots

* [Luigis Pizza](https://github.com/Grado0699/DAPI/tree/master/Luigis_Pizza)
* [Toast Stalin](https://github.com/Grado0699/DAPI/tree/master/Toast_Stalin)
* Bunta (in progress)
* Rem (in progress)
* Ram (in progress)

## Installation

* Clone the repository and build your desired bot in the latest `master` branch. The bots support both Linux (x64, x86, arm64 and arm32) and Windows (x64 and x86). Depending on the OS you are using, proceed with the corresponding instructions:

### Windows

* Just do a release build and your good to go.
* ffmpeg.exe, opus.dll and sodium.dll (which are used for voice) are included in the build.

### Linux

* You need to install the following packages (Debian):

`sudo apt-get install libopus0 libsodium18 libopus-dev libsodium-dev`

Depending on your distro and version, you might need to install different packages.
You do not need ffmpeg.exe, opus.dll and sodium.dll. Delete them to save some diskspace.

* Execute the `<bot>.dll` in the release folder by running the command:

`dotnet <bot>.dll`

* Have fun!
