# Packages

UWB is designed to be modular and extended, and as such is split into multiple packages. Each UWB package can be categorized as one of the package types below.

|Package Type  |Required?  |Description                                                                                          |
|------------- |---------- |---------------------------------------------------------------------------------------------------- |
|Core          |✔         |Core UWB package, provides the heart of UWB                                                          |
|Engine        |✔         |Provides an Engine                                                                                   |
|Engine Native |✔ (System)|Provides the native binaries used by an Engine. What ones you install depend on your platform targets|

## Package List

Once you have the registry setup, you should be able to see the packages in the package manager. If not try refreshing, or add the packages you need by their ID.

|Package                                         |Package Type  |Package ID                                            |
|----------------------------------------------- |------------- |----------------------------------------------------- |
|Unity Web Browser                               |Core          |`dev.voltstro.unitywebbrowser`                        |
|Unity Web Browser CEF Engine                    |Engine        |`dev.voltstro.unitywebbrowser.engine.cef`             |
|Unity Web Browser CEF Engine (Linux x64)        |Engine Native |`dev.voltstro.unitywebbrowser.engine.cef.linux.x64`   |
|Unity Web Browser CEF Engine (Windows x64)      |Engine Native |`dev.voltstro.unitywebbrowser.engine.cef.win.x64`     |
|Unity Web Browser CEF Engine (MacOS x64)        |Engine Native |`dev.voltstro.unitywebbrowser.engine.cef.macos.x64`   |
|Unity Web Browser CEF Engine (MacOS arm64)      |Engine Native |`dev.voltstro.unitywebbrowser.engine.cef.macos.arm64` |
|Unity Web Browser Pipes Communication           |Helper        |`dev.voltstro.unitywebbrowser.communication.pipes`    |

## Deprecated Packages

These packages are no longer needed.

|Package                                         |Package Type  |Package ID                                         |
|----------------------------------------------- |------------- |-------------------------------------------------- |
|Unity Web Browser Unix Support                  |Helper        |`dev.voltstro.unitywebbrowser.unix-support`        |
