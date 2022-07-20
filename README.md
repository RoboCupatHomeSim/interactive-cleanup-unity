# Interactive Cleanup Project

This is a Unity project for the Interactive Cleanup task of the RoboCup@Home Simulation.

Please prepare a common unitypackage in advance, and import the unitypackage to this project.  
For details of using the common unitypackage, please see an instruction in the following repository:  
https://github.com/RoboCupatHomeSim/common-unity.git

See also [wiki page](https://github.com/RoboCupatHomeSim/interactive-cleanup-unity/wiki).

## Prerequisites

Same as below for OS and Unity version.
https://github.com/RoboCupatHomeSim/documents/blob/master/SoftwareManual/Environment.md#windows-pc

## How to Build

### Import unitypackages

1. Download SteamVR Unity Plugin v2.7.3 from the following link.  
https://github.com/ValveSoftware/steamvr_unity_plugin/releases/download/2.7.3/steamvr_2_7_3.unitypackage
1. Open this project with Unity.
1. Click [**Continue**] in the [Unity Package Manager Error] window.
1. Click [**Ignore**] in the [Enter Safe Mode?] window.
1. Click [Assets]-[Import Package]-[Custom Package...].
1. Select a common unitypackage (e.g. robocup-common.unitypackage) and open the file.
1. Click [Import] button.
1. Click [Assets]-[Import Package]-[Custom Package...].
1. Select the steamvr_2_7_3.unitypackage and open the file.
1. Click [Import] button.
1. Click [Assets]-[**Reimport All**].
1. Click [**Reimport**] button.
1. Please confirm that no error occurred in Console window.


### Import Executable file and Dll for TTS

If you want to output speech, please do the following two steps.

#### Windows Settings
Please install the English language if you are using other than English.  
The procedure is like as follows.
1. Open the Windows settings menu
2. Click [Time & Language] - [Region & language]
3. Click [Add a language] in [Languages]-[Preferred languages]
4. Select "English (United States)" and Install

#### Import Files
Please import files by following the steps below.
1. Prepare "ConsoleSimpleTTS.exe" and "Interop.SpeechLib.dll".  
For details on these files, see [here](https://github.com/RoboCupatHomeSim/console-simple-tts).
2. Copy those files to the "TTS" folder in the same directory as SIGVerseConfig folder.


### Build
1. Create a "Build" folder under this project folder.
1. Open this project with Unity.
1. Click [File]-[Build Settings].
1. Click [Build]
1. Select the "Build" folder , and type a file name (e.g. InteractiveCleanup) and save the file.
1. Copy the "TTS" folder under the "Build" folder.


## How to Set Up

### Modify Configuration

1. Open this project with Unity.
2. Click [SIGVerse]-[SIGVerse Settings].  
SIGVerse window will be opened.
3. Type the IP address of ROS to "Rosbridge IP" in SIGVerse window.

## How to Execute Interactive Cleanup

Please start the ROS side application beforehand.  
See [interactive-cleanup-ros](https://github.com/RoboCupatHomeSim/interactive-cleanup-ros) for an example application.

### Execute on Unity Editor
1. Click [SIGVerse]-[Set Default GameView Size].
2. Double click "Assets/Competition/InteractiveCleanup/InteractiveCleanup(.unity)" in Project window.
3. Click the Play button at the top of the Unity editor.

### Execute the Executable file
1. Double Click the "InteractiveCleanup.exe" in the "Build" folder.

## Data generation mode
Please see [wiki page](https://github.com/RoboCupatHomeSim/interactive-cleanup-unity/wiki) for more details.  
If you want to use the data generation mode, you need to prepare your VR device.  
Please install [Steam](https://store.steampowered.com/about/) and [SteamVR](https://store.steampowered.com/app/250820/SteamVR/) on your PC. Download the installer from the official website and install it.  
And please install [Oculus Software](https://www.oculus.com/setup/) to use Oculus Headsets. [Oculus Link](https://support.oculus.com/articles/headsets-and-accessories/oculus-link/index-oculus-link) is also required.

## License

This project is licensed under the SIGVerse License - see the LICENSE.txt file for details.
