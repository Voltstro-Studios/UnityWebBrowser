# UnityWebBrowser Docs

UnityWebBrowser's docs are built from here.

UWB docs are generated using [DocFx](https://dotnet.github.io/docfx/). See the DocFx site for setup of the tool.

## Building Locally

```
docfx metadata
docfx build
```

To preview locally, use:

```
docfx serve _site/
```

## Publishing

The [publicly hosted docs](https://projects.voltstro.dev/UnityWebBrowser/latest/) are automatically published using [VoltProjects](https://github.com/Voltstro/VoltProjects).
