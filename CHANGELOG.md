# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
## Added
- Support for multiple browser engines
- Progress bar when copying browser engine files

## Changed
- Updated CEF to 90.6.7
- CefBrowserProcess was renamed to UnityWebBrowser.Engine.Cef

## Fixed
- Linux version of the CEF browser engine doesn't require `cefsimple` anymore
- Linux version of the CEF browser will now used trimmed binaries (From 1.1GB to 160MB)

## [1.6.3] - 2021-06-08
## Changed
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