# Unity Web Browser

[![License](https://img.shields.io/github/license/Voltstro-Studios/UnityWebBrowser.svg)](/LICENSE)
[![Build](https://github.com/Voltstro-Studios/UnityWebBrowser/actions/workflows/main.yml/badge.svg)](https://github.com/Voltstro-Studios/UnityWebBrowser/actions/workflows/main.yml)
[![Discord](https://img.shields.io/badge/Discord-Voltstro-7289da.svg?logo=discord)](https://discord.voltstro.dev) 
[![YouTube](https://img.shields.io/badge/Youtube-Voltstro-red.svg?logo=youtube)](https://www.youtube.com/Voltstro)

Adds a functional web browser to Unity without any issues or hassle. This project is capable of using any desired web engine you want, however for now we only have an engine using [CEF](https://bitbucket.org/chromiumembedded/cef/).

Currently, 2.x is in development, for 1.x document, see the [1.6.3 release](https://github.com/Voltstro-Studios/UnityWebBrowser/tree/1.6.3) (latest).

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
Unity 2021.2.x
```

### Install

1. Open up the package manager via Windows -> Package Manager
2. Click on the little + sign -> Add package from git URL...
3. Type `https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git#2.x` and add it
4. Type `https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git#engine/cef/base` and add it
5. Type `https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git#engine/cef/win-x64` and add it (If you need Windows support)
6. Type `https://gitlab.com/Voltstro-Studios/WebBrowser/Package.git#engine/cef/linux-x64` and add it (If you need Linux support)
7. Unity will now download and install the package