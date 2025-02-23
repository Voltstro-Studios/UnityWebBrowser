# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [2.2.6] - 2025-02-23

### Added

- Added no sandbox flag (#369)
- Added headless mode (#370)

### Changed

- Update CEF to 133.4.2
- Bump copyright year

### Fixed

- Fixed Unity 6 showing engine packages as "pre-release" (#387)
    - Engine packages are now version as `x.x.x`, instead of `x.x.x-<cef-major>.<cef-minor>.<cef-patch>`
- Fixed non-json logs causing errors by adding `ignoreLogProcessJsonErrors` option (#395)

## [2.2.5] - 2024-11-13

### Changed

- Update CEF to 130.1.16
- Bump deps

### Fixed

- Add missing `System.Runtime.InteropServices` for `RuntimeInformation` and `Architecture` in `WebBrowserUtils` (#366)

## [2.2.4] - 2024-10-26

### Fixed

- Fix Windows job handle names being the same (#358)

## [2.2.3] - 2024-10-20

### Added

- Added `AudioMute`

### Changed

- Update CEF to 130.1.2
- Bump deps
- Change ready signal to fire when cef calls OnAfterCreated

## [2.2.2] - 2024-10-16

### Added

- Added ignore SSL errors option (#350)

### Changed

- Move some web browser client options to a new section called "Advanced"

### Fixed

- Fixed `LoadHtml` not working correctly (#351)

### Removed

- Removed CEF engine custom error page. It shows the default chrome one now.

## [2.2.1] - 2024-10-06

### Added

- Added Incognito mode

### Changed

- Updated CEF to 129.0.11
- CachePath can only be set before HasInitialized
- LogPath can only be set before HasInitialized
- Change some path building to use `Path.Combine` instead

### Deprecated

- Cache control is no longer used. A cache path will always be used now. To use a incognito/private mode, where no profile-specific data is persisted to disk, set incognito mode to true.

### Fixed

- Do not pass proxy settings if ProxyServer is false
- Fix missing icudtl.dat on Linux cef (#346)

## [2.2.0] - 2024-09-20

### Added

- Added MacOS support (x64, arm64)
- Added CEF engine sandboxing (Windows, MacOS)
- Added remote debugging allowed origins
- Added CEF Event flags support (allows dragging)
- Added `UWB_DISABLE_POSTPROCESSOR` define to UWB's postprocessor

### Changed

- Bump deps
- Updated CEF to 128.4.9
- CEF engine gets compiled using .NET Native AOT
- Updated logging tags
- Communication layers are loaded by name instead of assemblies
- Update pipes package to use VoltRpc.Communication.Pipes package, instead of embedding
- Mouse click events will always send `clickCount` with at least a value of 1
- Changed control of engine pathing from being fixed (hardcoded) to being controlled by the `Engine` scriptable object
- Improve copying in engine build post-processor
- Change TCP timeouts to max allowed (#300)
- (Dev) Build all projects in CI
- (Dev) Change builds scripts to Python

### Deprecated

- Unix Support Package (`dev.voltstro.unitywebbrowser.unix-support`) - Packages should include the right execute permission from the get-go
- `CommunicationLayer.connectionTimeout` - Timeouts are now set to max
- `Engine.engineFileLocation` - Replaced with `Engine.engineEditorLocation`
- `Engine.EngineFilesNotFoundError` - No longer needed
- `EngineManager.GetEngineDirectory` - Fetching of engine paths is now handled by the engine class
- `EngineManager.GetEngineDirectory` - Fetching of engine paths is now handled by the engine class
- `EngineManager.GetEngineProcessFullPath` - Fetching of engine paths is now handled by the engine class
- `EngineManager.GetEngineProcessFullPath` - Fetching of engine paths is now handled by the engine class
- `WebBrowserUtils.GetBrowserEnginePath` - Fetching of engine paths is now handled by the engine class
- `WebBrowserUtils.GetBrowserEngineProcessPath` - Fetching of engine paths is now handled by the engine class

### Fixed

- Fixed select popups not working (#314)
- Fixed keyboard events not firing (#335)

## [2.1.1] - 2024-03-22

### Added

- Added Dynamic Runtime Sample

### Changed

- Bump deps
- Updated CEF to 122.1.13
- Updated basic sample description

### Fixed

- Implemented a handful of fixes to attempt to resolve issue #166
  - Prevent engine process lingering when main parent process dies
    - Uses Job Objects on Windows
    - Uses prctl on Linux
  - Errors related to binding ports are no longer swallowed

## [2.1.0] - 2024-02-18

### Added

- Added support for allowing Unity to still build your project on unsupported UWB platforms
    - This doesn't mean that UWB will run on them!
- Added set/get zoom level
- Added open dev tools
- Added 'JS Methods'. A way of invoking .NET methods from JS.
- Added `OnClientInitialized` event
- Added `OnClientConnected` event

### Changed

- Bump deps
- Updated CEF to 121.3.13
- Upgrade Engine to .NET 8
- CEF Engine buffer improvements

### Fixed

- Fix Unity 2023 TMP Problems

## [2.0.2] - 2023-05-29

### Added

- Added initial IME support

### Changed

- Updated deps
- Updated CEF to 113.3.1
- Updated copyright year
- Updated embedded pages project

## [2.0.1] - 2022-11-08

### Changed

- Updated `package.json`(s)
- Updated CEF to 106.1.1

### Fixed

- Fixed issue with stripped builds
- Fixed some keyboard stuff (on new input system)
- Fixed null reference error related to when engine fails to launch, but UWB has already been destroyed

## [2.0.0] - 2022-10-18

### Added

- Added UNIX Support package
- Added Popup Handling
- Added disableAllControls, disableMouse and disableKeyboard
- Added project site (https://projects.voltstro.dev/UnityWebBrowser/)

### Changed

- Updated CEF to 106.1.0
- Updated VoltRpc
- Use UniTask
- All base UWB components (other then custom input ones) are now built using `BaseUwbClientManager`
- Updated packages
    - Base packages includes URLs for changelog and docs
- Improved disposing
- Improved performance massively
- Improved copying of files to output
- Improved readying
- Improved engine logging
- Improved copying of engine files at build
- Better IsReady & IsConnected checks
- Changed namespace to have `VoltstroStudios.` at the start
- Changed Engine config stuff
- Changed input handling
    - Inputs are handled by a scriptable object
- Changed events to be structs
- Changed the way the background colour was set

(Not every change in this version is listed here, there are so many)

## [2.0.0-preview.3] - 2021-11-20

### Changed

- Updated CEF to 95.7.18
- Updated VoltRpc to 1.2.0
- CEF engine uses .NET 6
    - Should improve performance
    - App doesn't self-extract on first start anymore
- Updated to use some newer APIs
- Updated shutting down stuff

## [2.0.0-preview.2] - 2021-10-24

### Changed

- Updated CEF to 95.7.10
- Use VoltRpc from [UnityNuGet](https://github.com/xoofx/UnityNuGet)
- Events are executed as a task
- Improved Startup & Shutdown
- Texture is created at start of init with its colours set to background color.
- Engine processes location in build was changed from the plugins folder to a dedicated `UWB/` folder
- Default cache folder was changed to `UWBCache`
- Updated XML docs

### Fixed

- Potential fix for URLs not loading sometimes

## [2.0.0-preview.1] - 2021-09-09

### Added

- Support for multiple browser engines
- Progress bar when copying browser engine files
- Framework for support of multiple browser engines
- Added events for OnUrlChange, OnLoadStart, etc
- Added some Profile Markers
- Added support for IPC to use Pipes instead of TCP.
    - NOTE: `System.IO.Pipes` has a bad implementation in Unity's custom mono. Some platforms and configurations can result in named pipes to not work.

### Changed

- Updated CEF to 93.1.11
- CefBrowserProcess was renamed to UnityWebBrowser.Engine.Cef
- Underlying IPC communication layer was replaced with [VoltRpc](https://github.com/Voltstro-Studios/VoltRpc)
    - This improves the performance of UWB
- Huge massive internal changes
- Most names of 'Cef Browser' were changed to `UWB` or `UnityWebBrowser`
- `Width` and `Height` properties were merged into a custom `UnityWebBrowser.Shared.Resolution` struct
- Settings related to IPC was merged into a `UnityWebBrowser.WebBrowserIpcSettings` class

### Fixed

- Linux version of the CEF browser engine doesn't require `cefsimple` anymore
- Linux version of the CEF browser will now used trimmed binaries (From 1.1GB to 160MB)

## [1.6.3] - 2021-06-08

### Changed

- Texture in WebBrowserClient will no longer be linear.

## [1.6.2] - 2021-06-08

### Changed

- Moved events to new UnityWebBrowser.Shared assembly.

### Added

- Added MessagePack serialization helper functions for IEventData to UnityWebBrowser.Shared
- Use auto-generated, IL2CPP compatible resolvers for IEventData.

## [1.6.1] - 2021-06-06

### Changed

- Updated package dependencies
    - See updated README to see what you also need to install now.

### Fixed

- Fixed compile error when using new input system.

## [1.6.0] - 2021-06-05

### Changed

- Move to [MessagePack](https://github.com/neuecc/MessagePack-CSharp) for faster serialization.
- Renamed browser control methods in `WebBrowserClient.cs` and made them public.

### Removed 

- Newtonsoft.Json serialization.

## [1.5.0] - 2021-06-04

### Added

- Added CHANGELOG.md
