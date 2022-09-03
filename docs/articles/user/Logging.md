# Logging

UWB has two loggers:

|Logger    |Description                                 |
|--------- |------------------------------------------- |
|Web Engine|The native logger by the web engine it-self.|
|Core      |UWB's actual logger.                        |

By default, the web engine's log will be written to a file called `<Engine Name>.log`. This file will be located in your `/Library` folder of your project in the editor, or in your `<Project Name>_Data/` folder in a player build. The location can be changed by setting <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient.Logger*>. Null can be used for no logging by the web engine.

Logging by UWB Core, as well as from the engine's process (not the web engine itself) uses an IWebBrowserLogger. By default, the <xref:VoltstroStudios.UnityWebBrowser.Logging.DefaultUnityWebBrowserLogger> is used, which logs straight to Unity's <xref:UnityEngine.Debug.unityLogger>

To create a custom logger, use <xref:VoltstroStudios.UnityWebBrowser.Logging.IWebBrowserLogger>, and set <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient.Logger*> to your custom instance.

## Logging Settings

You can change the log severity of the web engine's logging in the editor, or by setting <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient.logSeverity>.

![Log Severity](~/assets/images/articles/user/logging/LogSeverity.webp)

> [!NOTE]
> Depending on the web engine, in debug mode there can be quite a lot of log messages!

What level the Core's logger does depends on the <xref:VoltstroStudios.UnityWebBrowser.Logging.IWebBrowserLogger> implementation. As the default <xref:VoltstroStudios.UnityWebBrowser.Logging.DefaultUnityWebBrowserLogger> logs straight to Unity's default logger, it will depend on how you have configured Unity's default logger.
