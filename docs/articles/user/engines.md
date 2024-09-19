# Engines

An 'Engine' is the outside process that is responsible for running and handling the web engine itself. This external process then communicates back to the client (the Unity side).

UWB can support using many different types of web engines using this approach.

## Changing Engines

To change what engine you are using, goto the Web Browser component, and change the 'Engine' value to the new engine configuration you want to use.

## Engine List

### CEF Engine

The CEF engine uses [CEF](https://bitbucket.org/chromiumembedded/cef/src/master/) as it's web engine. CEF is built using [Chromium](https://www.chromium.org/) (the engine that powers most web browsers including Chrome, Edge, etc).

#### Platform Support

|Platform     |Supported                  |Notes                                             |
|-------------|---------------------------|--------------------------------------------------|
|Windows      |✔                         |Works natively                                    |
|Linux        |✔ (Tested on Ubuntu 22.04)|Works natively                                    |
|MacOS (Intel)|✔                         |Works natively                                    |
|MacOS (Arm)  |✔                         |While we provide ARM builds, they are not tested  |

#### Packages

- 'Unity Web Browser CEF Engine' (`unitywebbrowser.engine.cef`)

##### Natives

- 'Unity Web Browser CEF Engine (Windows x64)' (`unitywebbrowser.engine.cef.win.x64`)
- 'Unity Web Browser CEF Engine (Linux x64)' (`unitywebbrowser.engine.cef.linux.x64`)
- 'Unity Web Browser CEF Engine (MacOS x64)' (`unitywebbrowser.engine.cef.macos.x64`)
- 'Unity Web Browser CEF Engine (MacOS arm64)' (`unitywebbrowser.engine.cef.macos.arm64`)

#### MacOS Support

Builds of CEF engine are provided for MacOs (both x64 and ARM). However the builds are not code signed! You will need to sign the UnityWebBrowser.Engine.Cef app your self.

## Engine Communication

By default, communication between the engine and core is done using TCP. If the optional pipes communication layer package is installed, pipes may be used instead.

You can change what communication system is used in the client settings.

![Packages](~/assets/images/articles/user/engines/CommunicationLayer.webp)
