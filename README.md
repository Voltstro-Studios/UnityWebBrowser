<img align="right" width="20%" src="media/UWB-Icon.svg">

# Unity Web Browser

[![License](https://img.shields.io/github/license/Voltstro-Studios/UnityWebBrowser.svg)](/LICENSE.md)
[![Build](https://github.com/Voltstro-Studios/UnityWebBrowser/actions/workflows/main.yml/badge.svg)](https://github.com/Voltstro-Studios/UnityWebBrowser/actions/workflows/main.yml)
[![Discord](https://img.shields.io/badge/Discord-Voltstro-7289da.svg?logo=discord)](https://discord.voltstro.dev) 
[![YouTube](https://img.shields.io/badge/Youtube-Voltstro-red.svg?logo=youtube)](https://www.youtube.com/Voltstro)

Adds a functional web browser to Unity without any issues or hassle. This project is capable of using any desired web engine you want, however for now we only have an engine using [CEF](https://bitbucket.org/chromiumembedded/cef/).

Currently, 2.x is in development, for 1.x document, see the [1.6.3 release](https://github.com/Voltstro-Studios/UnityWebBrowser/tree/1.6.3) (latest).

## Features

- Displays the web in your project via using a web browser engine.
- Full keyboard and mouse input
- API to interface with the web browser engine.
- Install as an Unity package
- Doesn't crash the entire editor on reload
- 100% free and open-source
- Multi-Platform Support

## Getting Started

### How does this work?

Getting an web engine to work in Unity is a pain, it usually ends up just crashing Unity. This project resolves that issue via running the browser engine in separate process and IPCing data between Unity and the engine.

### Package Installation

#### Prerequisites

```
Unity 2021.2.x
```

#### Install

1. Open up the package manager via Windows -> Package Manager
2. Click on the little + sign -> Add package from git URL...
3. Type `https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git#2.x` and add it
4. Type `https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git#engine/cef/base` and add it
5. Type `https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git#engine/cef/win-x64` and add it (If you need Windows support)
6. Type `https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git#engine/cef/linux-x64` and add it (If you need Linux support)
7. Unity will now download and install the package

## Contributing

Please READ ALL OF THIS if you want to contribute or work on the project.

### Prerequisites

```
Unity 2021.3.x
.NET 6 SDK
PowerShell (formally PowerShell Core)
NodeJS & Yarn
Git
```

1. Clone the repo with `git clone --recursive https://github.com/Voltstro-Studios/UnityWebBrowser.git` (Clone with sub-modules!)

2. Run the `src/setup-all.ps1` script with PowerShell. Depending on your system, and your download speeds, this script could take upto a minute or longer. You only need to run this once.

3. You can now open up the `src/UnityWebBrowser.UnityProject` project with Unity.

4. To build the Unity project as a player, open Unity Volt Builder by going to Tools **->** Unity Volt Builder **->** Volt Builder and clicking on 'Build Player'.

## Operating System Support

What OS is supported will depend on the web engine you use.

### CEF Engine

|OS           |Supported                  |Notes                                         |
|-------------|---------------------------|----------------------------------------------|
|Windows      |✔                         |Works natively                                |
|Linux        |✔ (Tested on Ubuntu 21.10)|Works natively                                |
|MacOS (Intel)|✖                         |Planned                                       |
|MacOS (M1)   |✖                         |No physical hardware to test or develop on    |

## Screenshots

<details>
  <summary>Click to expand!</summary>

![Screenshot 1](media/Screenshot-Editor1.png)
![Screenshot 2](media/Screenshot-Editor2.png)
![Screenshot 3](media/Screenshot-Editor3.png)
![Screenshot 4](media/Screenshot-InPlayer.png)

</details>

## Authors

* **Voltstro** - *Initial work* - [Voltstro](https://github.com/Voltstro)

## License

This project is licensed under the MIT License - see the [LICENSE](/LICENSE.md) file for details.

## Credits

### CEF Engine

- [CEF](https://bitbucket.org/chromiumembedded/cef/src/master/) - Underlying web engine.
- [CefGlue](https://gitlab.com/xiliumhq/chromiumembedded/cefglue) - C# wrapper.
- [CefUnitySample](https://github.com/aleab/cef-unity-sample) - CEF directly in Unity. Has crashing problems tho.
- [unity_browser](https://github.com/tunerok/unity_browser) - (Orginally by Vitaly Chashin) CEF working in Unity using IPC, but the project is in a messy state.
- [ChromiumGtk](https://github.com/lunixo/ChromiumGtk) - Linux stuff with CEF