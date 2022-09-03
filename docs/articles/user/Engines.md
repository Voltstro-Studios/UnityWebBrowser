# Engines

An 'Engine' is the outside process that is responsible for running and handling the web engine itself. This external process then communicates back to the client (the Unity side).

UWB can support using many different types of web engines using this approach.

## Changing Engines

To change what engine you are using, goto the Web Browser component, and change the 'Engine' value to the new engine configuration you want to use.

## Cef Engine

The Cef engine uses [CEF](https://bitbucket.org/chromiumembedded/cef/src/master/) as it's web engine. CEF is built using [Chromium](https://www.chromium.org/) (the engine that powers many different browsers including Chrome, Edge, Opera and Brave).

The downsides of using CEF come from the downside of Chromium, it's a memory hog (~400 MB at idle), and has quite large binaries.

### Platform Support

|OS           |Supported                  |Notes                                         |
|-------------|---------------------------|----------------------------------------------|
|Windows      |✔                         |Works natively                                |
|Linux        |✔ (Tested on Ubuntu 22.04)|Works natively                                |
|MacOS (Intel)|✖                         |Planned                                       |
|MacOS (M1)   |✖                         |No physical hardware to test or develop on    |

### Packages

- 'Unity Web Browser CEF Engine' (`unitywebbrowser.engine.cef`)

#### Natives

- 'Unity Web Browser CEF Engine (Windows x64)' (`unitywebbrowser.engine.cef.win.x64`)
- 'Unity Web Browser CEF Engine (Linux x64)' (`unitywebbrowser.engine.cef.linux.x64`)

## WebView2 Engine

The WebView2 Engine is a planned engine. No development has been done on it yet, and no promises have been made on developing it!

[WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) is Microsoft's embedded version of Edge.

### Platform Support

WebView2 will be Windows only.

### Packages

Is only a planned engine, nothing has been done yet.
