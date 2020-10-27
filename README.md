# DAPI

This repository is a collection of some Discord bots written in C#.

## Used packages

* .NET Core 3.1
* [DSharpPlus (nightly)](https://github.com/DSharpPlus/DSharpPlus/blob/master/README.md)

## Bots

* [Luigis Pizza](https://github.com/Grado0699/DAPI/tree/master/Luigis_Pizza)
* [Toast Stalin](https://github.com/Grado0699/DAPI/tree/master/Toast_Stalin)
* [Ram](https://github.com/Grado0699/DAPI/tree/master/Ram)
* [Rem](https://github.com/Grado0699/DAPI/tree/master/Rem)
* Happy (in progress)
* Bunta (in progress)

## Installation

* Clone the repository and build your desired bot in the latest `master` branch. The bots support both Linux (x64, x86, arm64 and arm32) and Windows (x64 and x86). Depending on the OS you are using, proceed with the corresponding instructions:

### Windows

* Requires the .NET Core 3.1 runtime to be installed.
* Just do a release build and your good to go.
* ffmpeg.exe, opus.dll and sodium.dll (which are used for voice) are included in the build.

### Linux

* Requires the .NET Core 3.1 runtime to be installed.
* You need to install the following packages (Debian) for a working voice setup:

        sudo apt-get install libopus0 libsodium18 libopus-dev libsodium-dev

* Depending on your distribution and version, you might need to install different packages. You do not need ffmpeg.exe, opus.dll and sodium.dll. Delete them to save some diskspace.
* Do a release build.
* Execute the `<bot>.dll` in the release folder by running the command:

        dotnet <bot>.dll

* Have fun!

#### Service

With Linux it is also possible to register the Bot as a service. To do that following these steps:

* Create a new `<bot>.serivce` in the directory `/lib/system/systemd/`.
* TODO 