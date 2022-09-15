# Unity Web Browser DocsPrj

The `UnityWebBrowser.DocsPrj` project is used for generating API documentation of UWB.

## Getting Started

### Prerequisites

```
xmldocmd
```

You can install [xmldocmd](https://github.com/ejball/XmlDocMarkdown) as a dotnet tool with:

```powershell
dotnet tool install --global xmldocmd
```

### How to Generate

1. Build UnityWebBrowser.UnityProject with Unity and Volt Builder.

2. Build UnityWebBrowser.DocsPrj in Debug configuration

3. Run `./GenDocs.ps1` with PowerShell
