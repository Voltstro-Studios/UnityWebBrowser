# Player Build

When creating a player build in Unity, UWB will automatically copy engine files from the native packages to the output folder. This is done using Unity's build pipeline postprocess feature, using the [IPostprocessBuildWithReport](https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Build.IPostprocessBuildWithReport.html) interface.

## Paths

By default, the engines files will be placed in a certain directory in your build folder. The path changes slightly depending what platform you are building for.

|Platform   |Path                             |
|---------- |-------------------------------- |
|Windows x64|`<Game>/<Game>_Data/UWB/`        |
|Linux x64  |`<Game>/<Game>_Data/UWB/`        |
|MacOS x64  |`<Game>.app/Contents/Frameworks/`|
|MacOS arm64|`<Game>.app/Contents/Frameworks/`|

## MacOS

UWB does not provide signed builds of any of its engines. You will need to create the entitlements, sign and notarize the engine app. [Unity has some docs on signing](https://docs.unity3d.com/2021.3/Documentation/Manual/macos-building-notarization.html), a similar process should apply to signing the engine app.

## Disable

To disable UWB's built-in postprocessor, define `UWB_DISABLE_POSTPROCESSOR` in your project's player settings.

![Player Settings](~/assets/images/articles/user/player-build/player-settings.webp)

You may wish to do this if you have customized your engine's pathing, but you will need to copy the engine files from the packages your self
