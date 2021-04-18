using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityWebBrowser.EventData;
using ZeroMQ;
using Debug = UnityEngine.Debug;

namespace UnityWebBrowser
{
	/// <summary>
	///		Handles managing the process and worker thread
	/// </summary>
	[Serializable]
	public class WebBrowserClient : IDisposable
	{
		/// <summary>
		///		The initial URl the browser will start at
		/// </summary>
		[Header("Browser Settings")]
		[Tooltip("The initial URl the browser will start at")]
		public string initialUrl = "https://voltstro.dev";

		/// <summary>
		///		The width of the browser
		/// </summary>
		[Tooltip("The width of the browser")]
		public uint width = 1920;

		/// <summary>
		///		The height of the browser
		/// </summary>
		[Tooltip("The height of the browser")]
		public uint height = 1080;
		
		/// <summary>
		///		The background <see cref="Color32"/> of the webpage
		/// </summary>
		[Tooltip("The background color of the webpage")]
		public Color32 backgroundColor = new Color32(255, 255, 255, 255);

		/// <summary>
		///		Enable or disable JavaScript
		/// </summary>
		[Tooltip("Enable or disable JavaScript")]
		public bool javascript = true;

		/// <summary>
		///		Enable or disable the cache
		/// </summary>
		[Tooltip("Enable or disable the cache")]
		public bool cache = true;

		/// <summary>
		///		The port to communicate with the browser process on
		/// </summary>
		[Header("IPC Settings")] 
		[Tooltip("The port to communicate with the browser process on")]
		public int port = 5555;

		/// <summary>
		///		The time between each frame sent the browser process
		/// </summary>
		[Tooltip("The time between each frame sent the browser process")]
		public float eventPollingTime = 0.04f;

		/// <summary>
		///		Enables debug logging for the CEF browser process
		/// </summary>
		[Tooltip("Enables debug logging for the CEF browser process")]
		public bool debugLog;

		/// <summary>
		///		The log severity. Only messages of this severity level or higher will be logged
		/// </summary>
		[Tooltip("The log severity. Only messages of this severity level or higher will be logged")]
		public CefLogSeverity logSeverity;

		/// <summary>
		///		Texture that the browser will paint to
		/// </summary>
		public Texture2D BrowserTexture { get; private set; }

		/// <summary>
		///		<see cref="ILogger"/> that we log to
		/// </summary>
		internal ILogger Logger { get; private set; } = Debug.unityLogger;
		private const string LoggingTag = "[Web Browser]";

		#region Pixels

		private object pixelsLock = new object();
		private byte[] pixels;

		/// <summary>
		///		Raw pixel data of the browser.
		///		<para>Try to use <see cref="BrowserTexture"/> for displaying a texture instead of this!</para>
		/// </summary>
		public byte[] Pixels
		{
			get
			{
				lock (pixelsLock)
				{
					return pixels;
				}
			}
			private set
			{
				lock (pixelsLock)
				{
					pixels = value;
				}
			}
		}

		#endregion

		/// <summary>
		///		Is the web browser client running?
		/// </summary>
		public bool IsRunning => isRunning;
		
		private bool isRunning;

		private FileInfo logPath;

		/// <summary>
		///		The path that CEF will log to
		/// </summary>
		/// <exception cref="WebBrowserRunningException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public FileInfo LogPath
		{
			get => logPath;
			set
			{
				if (IsRunning)
					throw new WebBrowserRunningException();

				logPath = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		private FileInfo cachePath;

		/// <summary>
		///		The path to the cache
		/// </summary>
		/// <exception cref="WebBrowserRunningException"></exception>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public FileInfo CachePath
		{
			get => cachePath;
			set
			{
				if (IsRunning)
					throw new WebBrowserRunningException();

				if (!cache)
					throw new ArgumentException("The cache is disabled!");

				cachePath = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		private Process serverProcess;
		private WebBrowserEventDispatcher eventDispatcher;

		/// <summary>
		///		Inits the browser client
		/// </summary>
		/// <exception cref="FileNotFoundException"></exception>
		internal void Init()
		{
			//Get the path to the CEF browser process and make sure it exists
			string cefProcessPath = WebBrowserUtils.GetCefProcessApplication();
			LogDebug($"Starting CEF browser process from {cefProcessPath}");

			if (!File.Exists(cefProcessPath))
			{
				LogError("The CEF browser process doesn't exist!");
				throw new FileNotFoundException("CEF browser process could not be found!");
			}

			//Setup log path
			LogPath ??= new FileInfo($"{WebBrowserUtils.GetCefMainDirectory()}/cef.log");

			//Setup cache path
			//As funny and stupid you might think this is by just setting the text to be "null", we need to pass it like that if there is no cache
			string cachePathArgument = "";
			cachePath ??= new FileInfo($"{WebBrowserUtils.GetCefMainDirectory()}/CEFCache");
			if (cache)
				cachePathArgument = $"-cache-path \"{cachePath.FullName}\" ";

			//Start the server process
			serverProcess = new Process
			{
				StartInfo = new ProcessStartInfo(cefProcessPath, $" -initial-url \"{initialUrl}\" " +
				                                                 $"-width {width} -height {height} " +
				                                                 $"-javascript {javascript} " +
				                                                 $"-bcr {backgroundColor.r} -bcg {backgroundColor.g} -bcb {backgroundColor.b} -bca {backgroundColor.a} " +
				                                                 $"-log-path \"{logPath.FullName}\" -log-severity {logSeverity} {cachePathArgument}" +
				                                                 $"-port {port} -debug {debugLog}")
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					WorkingDirectory = WebBrowserUtils.GetCefProcessPath()
				},
				EnableRaisingEvents = true,
			};
			serverProcess.OutputDataReceived += HandleCefProcessLog;
			serverProcess.Start();
			serverProcess.BeginOutputReadLine();
			serverProcess.BeginErrorReadLine();

			BrowserTexture = new Texture2D((int)width, (int)height, TextureFormat.BGRA32, false, true);
			eventDispatcher = new WebBrowserEventDispatcher(new TimeSpan(0, 0, 4), port);
			eventDispatcher.StartDispatchingEvents();
		}

		/// <summary>
		///		Starts updating the <see cref="BrowserTexture"/> data
		/// </summary>
		/// <returns></returns>
		internal IEnumerator Start()
		{
			LogDebug("Starting communications between CEF process and Unity...");
			isRunning = true;

			while (isRunning)
			{
				yield return new WaitForSecondsRealtime(eventPollingTime);

				eventDispatcher.QueueEvent(new PingEvent(), LoadPixels);

				byte[] pixelData = Pixels;

				if(pixelData == null || pixelData.Length == 0)
					continue;

				BrowserTexture.LoadRawTextureData(pixelData);
				BrowserTexture.Apply(false);
			}
		}

		private void LoadPixels(ZFrame frame)
		{
			Pixels = frame.Read();
			frame.Dispose();
		}

		#region Logging

		/// <summary>
		///		Logs a debug message
		/// </summary>
		/// <param name="message"></param>
		internal void LogDebug(object message)
		{
			Logger.Log(LogType.Log, LoggingTag, message);
		}

		/// <summary>
		///		Logs a warning
		/// </summary>
		/// <param name="message"></param>
		internal void LogWarning(object message)
		{
			Logger.LogWarning(LoggingTag, message);
		}

		/// <summary>
		///		Logs a error
		/// </summary>
		/// <param name="message"></param>
		internal void LogError(object message)
		{
			Logger.LogError(LoggingTag, message);
		}

		/// <summary>
		///		Replaces the logger the web browser will use
		/// </summary>
		/// <param name="logger"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void ReplaceLogger(ILogger logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		private void HandleCefProcessLog(object sender, DataReceivedEventArgs e)
		{
			if (serverProcess.HasExited) return;

			if(e.Data.StartsWith("DEBUG "))
				LogDebug(e.Data.Replace("DEBUG ", ""));
			else if(e.Data.StartsWith("INFO "))
				LogDebug(e.Data.Replace("INFO ", ""));
			else if(e.Data.StartsWith("WARN "))
				LogWarning(e.Data.Replace("WARN ", ""));
			else if(e.Data.StartsWith("ERROR "))
				LogError(e.Data.Replace("ERROR ", ""));
			else
				LogDebug(e.Data);
		}

		#endregion

		#region CEF Events

		/// <summary>
		///		Sends a keyboard event to the CEF process
		/// </summary>
		/// <param name="keysDown"></param>
		/// <param name="keysUp"></param>
		/// <param name="chars"></param>
		internal void SendKeyboardEvent(int[] keysDown, int[] keysUp, string chars)
		{
			eventDispatcher.QueueEvent(new KeyboardEvent
			{
				KeysDown = keysDown,
				KeysUp = keysUp,
				Chars = chars
			}, HandelEvent);
		}

		///  <summary>
		/// 		Sends a mouse event to the CEF process
		///  </summary>
		///  <param name="mousePos"></param>
		internal void SendMouseMoveEvent(Vector2 mousePos)
		{
			eventDispatcher.QueueEvent(new MouseMoveEvent
			{
				MouseX = (int)mousePos.x,
				MouseY = (int)mousePos.y
			}, HandelEvent);
		}

		///  <summary>
		/// 		Sends a mouse click event to the CEF process
		///  </summary>
		///  <param name="mousePos"></param>
		///  <param name="clickCount"></param>
		///  <param name="clickType"></param>
		///  <param name="eventType"></param>
		internal void SendMouseClickEvent(Vector2 mousePos, int clickCount, MouseClickType clickType, MouseEventType eventType)
		{
			eventDispatcher.QueueEvent(new MouseClickEvent
			{
				MouseX = (int)mousePos.x,
				MouseY = (int)mousePos.y,
				MouseClickCount = clickCount,
				MouseClickType = clickType,
				MouseEventType = eventType
			}, HandelEvent);
		}

		/// <summary>
		///		Sends a mouse scroll event
		/// </summary>
		/// <param name="mouseX"></param>
		/// <param name="mouseY"></param>
		/// <param name="mouseScroll"></param>
		internal void SendMouseScrollEvent(int mouseX, int mouseY, int mouseScroll)
		{
			eventDispatcher.QueueEvent(new MouseScrollEvent
			{
				MouseScroll = mouseScroll,
				MouseX = mouseX,
				MouseY = mouseY
			}, HandelEvent);
		}

		/// <summary>
		///		Sends a button event
		/// </summary>
		/// <param name="buttonType"></param>
		/// <param name="url"></param>
		internal void SendButtonEvent(ButtonType buttonType, string url = null)
		{
			eventDispatcher.QueueEvent(new ButtonEvent
			{
				ButtonType = buttonType,
				UrlToNavigate = url
			}, HandelEvent);
		}

		/// <summary>
		///		Makes the cef client load html
		/// </summary>
		/// <param name="html"></param>
		internal void LoadHtmlEvent(string html)
		{
			eventDispatcher.QueueEvent(new LoadHtmlEvent
			{
				Html = html
			}, HandelEvent);
		}

		/// <summary>
		///		Executes JS in the cef client
		/// </summary>
		/// <param name="js"></param>
		internal void ExecuteJsEvent(string js)
		{
			eventDispatcher.QueueEvent(new ExecuteJsEvent
			{
				Js = js
			}, HandelEvent);
		}

		private void HandelEvent(ZFrame frame)
		{
			frame.Dispose();
		}

		#endregion

		#region Destroying

		~WebBrowserClient()
		{
			ReleaseResources();
		}
		
		/// <summary>
		///		Destroys this <see cref="WebBrowserClient"/> instance
		/// </summary>
		public void Dispose()
		{
			ReleaseResources();
			GC.SuppressFinalize(this);
		}

		private void ReleaseResources()
		{
			if(!isRunning)
				return;

			eventDispatcher.Dispose();

			if(!serverProcess.HasExited)
				serverProcess.KillTree();
			serverProcess.Dispose();

			LogDebug("Web browser shutdown.");
		}

		#endregion
		
		public enum CefLogSeverity
		{
			Default,
			Verbose,
			Debug = Verbose,
			Info,
			Warning,
			Error,
			Fatal,
			Disable
		}
	}
}