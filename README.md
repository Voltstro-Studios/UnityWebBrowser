# Unity Web Browser

[![License](https://img.shields.io/github/license/Voltstro-Studios/UnityWebBrowser.svg)](/LICENSE)
[![Build](https://github.com/Voltstro-Studios/UnityWebBrowser/actions/workflows/main.yml/badge.svg)](https://github.com/Voltstro-Studios/UnityWebBrowser/actions/workflows/main.yml)
[![Unity Package](https://img.shields.io/badge/Unity-Package-blue.svg)](https://gitlab.com/Voltstro-Studios/WebBrowser/Package)
[![Discord](https://img.shields.io/badge/Discord-Voltstro-7289da.svg?logo=discord)](https://discord.voltstro.dev) 
[![YouTube](https://img.shields.io/badge/Youtube-Voltstro-red.svg?logo=youtube)](https://www.youtube.com/Voltstro)

Adds a functional web browser to Unity without any issues or hassle. This project is capable of using any desired web engine you want, however for now we only have an engine using [CEF](https://bitbucket.org/chromiumembedded/cef/).

Currently, 2.x is in development, for 1.x document, see the [1.6.3 release](https://github.com/Voltstro-Studios/UnityWebBrowser/tree/1.6.3) (latest).

# Features

- Displays the web via using a web engine.
- Multi-engine capable
- Controls and inputs
- Install as an Unity package
- Doesn't crash the entire editor on reload
- 100% free and open-source

# Getting Started

## How does this work?

Getting an web engine to work in Unity is a pain, it usually ends up just crashing Unity. This project resolves that issue via running the browser engine in separate process and IPCing data between Unity and the engine.

## Package Installation

### Prerequisites

```
Unity 2020.3.x
MessagePack-CSharp
```

**(NOTE)**: Currently the work-in-progress 2.x version of this project doesn't have a ready-to-go UPM package. The [1.x version](https://github.com/Voltstro-Studios/UnityWebBrowser/tree/1.6.3) does however!

1. You will need to install [MessagePack](https://github.com/neuecc/MessagePack-CSharp#installation) first, and its dependencies.

## Git

As always, you will always be able to install via git, we will have this ready once 2.0 is done!

## OpenUPM

We plan on releasing on [OpenUPM](https://openupm.com/) once 2.0 is done. This should make the install process relatively hassle free.

## Contributing

Please READ ALL OF THIS if you want to contribute or work on the project.

### Prerequisites

```
Unity 2020.3.12f1
.NET 5 SDK
Powershell Core 
```

1. Clone the repo with `git clone --recursive https://github.com/Voltstro-Studios/UnityWebBrowser.git` (Clone with sub-modules!)

2. Run `src/setup-all.ps1`. This script might take a little awhile as it will download some rather large binaries and setup everything for you! You will only need to run this once.

3. Open `UnityWebBrowser.UnityProject` with Unity.

## Screenshots

<details>
  <summary>Click to expand!</summary>

![Screenshot 1](media/Screenshot-Editor1.png)
![Screenshot 2](media/Screenshot-Editor2.png)
![Screenshot 3](media/Screenshot-Editor3.png)
![Screenshot 4](media/Screenshot-InPlayer.png)

</details>

# Authors

* **Voltstro** - *Initial work* - [Voltstro](https://github.com/Voltstro)

# License

This project is licensed under the LGPL-3.0 License - see the [LICENSE](https://github.com/Voltstro-Studios/UnityWebBrowser/blob/master/LICENSE) file for details.

# Credits

## Base

- [ZeroMQ](https://zeromq.org/) - Underlying IPC messaging library.
- [MessagePack](https://github.com/neuecc/MessagePack-CSharp) - Fast serialization

## CEF Engine

- [CEF](https://bitbucket.org/chromiumembedded/cef/src/master/) - Underlying web engine.
- [CefGlue](https://gitlab.com/xiliumhq/chromiumembedded/cefglue) - C# wrapper.
- [CefUnitySample](https://github.com/aleab/cef-unity-sample) - CEF directly in Unity. Has crashing problems tho.
- [unity_browser](https://github.com/tunerok/unity_browser) - (Orginally by Vitaly Chashin) CEF working in Unity using IPC, but the project is in a messy state.
- [ChromiumGtk](https://github.com/lunixo/ChromiumGtk) - Linux stuff with CEF