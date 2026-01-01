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

## Disable

To disable UWB's built-in postprocessor, define `UWB_DISABLE_POSTPROCESSOR` in your project's player settings.

![Player Settings](~/assets/images/articles/user/player-build/player-settings.webp)

You may wish to do this if you have customized your engine's pathing, but you will need to copy the engine files from the packages your self

## Code Signing

By default, none of the UWB binaries are code signed. It is generally recommended to code sign your entire build (including UWB) for releases of your project that you plan on publicly sharing. On some platforms, it may be required to code sign for CEF to work, [especially if you have sandbox enabled](https://bitbucket.org/chromiumembedded/cef/wiki/SandboxSetup.md#markdown-header-usage).

For MacOS, you will also need to create the entitlements, then sign and notarize the engine app. [Unity has some docs on signing](https://docs.unity3d.com/2021.3/Documentation/Manual/macos-building-notarization.html), a similar process should apply to signing the engine app.

> [!NOTE]
> If anyone has any more info on this topic, and is willing to provide that info, then please open either a discussion with your info, or directly contribute a PR to the docs.

