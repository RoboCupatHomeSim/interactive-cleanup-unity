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

### Import the common Unitypackage

1. Open this project with Unity.
2. Click [Assets]-[Import Package]-[Custom Package...].
3. Select a common unitypackage (e.g. robocup-common.unitypackage) and open the file.
4. Click [Import] button.
5. Please confirm that no error occurred in Console window.

### Import the Oculus Integration for Unity

1. Download Oculus Integration for Unity ver.14.0 from the following link.  
https://developer.oculus.com/downloads/package/unity-integration-archive/14.0/
2. Open this project with Unity.
3. Click [Assets]-[Import Package]-[Custom Package...].
4. Select downloaded OculusIntegration_14.0.unitypackage and open the file.
5. Click [Import] button.
6. Click [Upgrade] when "Update Spatializer Plugins" window displayed.
7. Click [Restart] when "Restart Unity" window displayed.
8. Click [Yes] when "Update Oculus Utilities Plugin" window displayed.
9. Click [Restart] when "Restart Unity" window displayed.
10. Please confirm that no error occurred in Console window.

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
2. Open this project with Unity.
3. Click [File]-[Build Settings].
4. Click [Build]
5. Select the "Build" folder , and type a file name (e.g. InteractiveCleanup) and save the file.


## How to Set Up

### Modify Configuration

1. Open this project with Unity.
2. Click [SIGVerse]-[SIGVerse Settings].  
SIGVerse window will be opened.
3. Type the IP address of ROS to "Rosbridge IP" in SIGVerse window.

## How to Execute Interactive Cleanup Program

Please start the ROS side application beforehand.  

### Execute On Unity Editor
1. Click [SIGVerse]-[Set Default GameView Size].
2. Double click "Assets/Competition/InteractiveCleanup/InteractiveCleanup(.unity)" in Project window.
3. Click the Play button at the top of the Unity editor.

### Execute the Executable file
1. Copy the "SIGVerseConfig" folder and the "TTS" folder into the "Build" folder.
2. Double Click the "InteractiveCleanup.exe" in the "Build" folder.

## License

This project is licensed under the SIGVerse License - see the LICENSE.txt file for details.
