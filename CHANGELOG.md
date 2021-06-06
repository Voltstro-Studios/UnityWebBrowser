# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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