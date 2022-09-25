# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - Unreleased

### Added

- Added UNIX Support package
- Added Popup Handling
- Added disableAllControls, disableMouse and disableKeyboard

### Changed

- Updated VoltRpc
- Use UniTask
- Improved disposing
- Improved performance massively
- Improved copying of files to output
- Improved readying
- Better IsReady & IsConnected checks
- Changed namespace to have `VoltstroStudios.` at the start
- Changed Engine config stuff
- Changed input handling
- Changed events to be structs

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
