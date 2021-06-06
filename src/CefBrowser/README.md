# Unity Web Browser

[![License](https://img.shields.io/github/license/Voltstro-Studios/UnityWebBrowser.svg)](/LICENSE)
[![Unity Package](https://img.shields.io/badge/Unity-Package-blue.svg)](https://gitlab.com/Voltstro-Studios/WebBrowser/Package)
[![Discord](https://img.shields.io/badge/Discord-Voltstro-7289da.svg?logo=discord)](https://discord.voltstro.dev)
[![YouTube](https://img.shields.io/badge/Youtube-Voltstro-red.svg?logo=youtube)](https://www.youtube.com/Voltstro)

Adds a functional web browser to Unity without any issues or hassle.

This web browser uses [CEF](https://bitbucket.org/chromiumembedded/cef/src/master/) and [CEFGlue](https://gitlab.com/xiliumhq/chromiumembedded/cefglue) to render the page to a texture and display it in Unity. This project also doesn't crash when you play in the editor for a second time.

The source code for this project can be found at [Voltstro-Studios/UnityWebBrowser](https://github.com/Voltstro-Studios/UnityWebBrowser).

# Features

- Displays the web using a modern web browser engine (Chromium)
- Load web pages from URL or HTML
- Controls and inputs
- Execute JS code
- Install as an Unity package
- Doesn't crash the entire editor
- Free and open-source
- Windows & Linux

## Package Installation

### Prerequisites

```
Unity 2020.3.x
```

Please read all of this!

To install this project we need to setup some things first.

1. Open up project settings, and set 'Api Compatibility Level' to .NET 4.x under Player.
2. Open up the package manager via Windows -> Package Manager
3. Add `https://github.com/VoltUnityPackages/UnitySystemBuffers.git` as a git package.
4. Add `https://github.com/VoltUnityPackages/UnitySystemThreadingTasksExtensions.git` as a git package.
5. Add `https://github.com/VoltUnityPackages/MessagePack.git` as a git package.

Now we can install the main package. We host a ready to go package on GitLab.

5. Add `https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git` as a git package.