using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Security;
using Newtonsoft.Json;
using UnityEngine;
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
		public string initialUrl = "https://google.com";

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
		///		The endpoint for the browser process
		/// </summary>
		[Header("IPC Settings")]
		[Tooltip("The endpoint for the browser process")]
		public string ipcEndpoint = "tcp://127.0.0.1:5555";

		/// <summary>
		///		The time between each frame sent the browser process
		/// </summary>
		[Tooltip("The time between each frame sent the browser process")]
		public float eventPollingTime = 0.01f;

		/// <summary>
		///		How many errors until we will just fail
		/// </summary>
		[Tooltip("How many errors until we will just fail")]
		public int errorsTillFail = 4;

		/// <summary>
		///		Show the CEF browser process console?
		/// </summary>
		[Tooltip("Show the CEF browser process console?")]
		public bool showProcessConsole;

		/// <summary>
		///		Texture that the browser will paint to
		/// </summary>
		public Texture2D BrowserTexture { get; private set; }

		private Process serverProcess;
		private ZContext context;
		private ZSocket requester;

		private int errorCount;

		private bool isRunning;

		/// <summary>
		///		Inits the browser client
		/// </summary>
		public void Init()
		{
			BrowserTexture = new Texture2D((int)width, (int)height, TextureFormat.BGRA32, false, true);
		}

		/// <summary>
		///		Starts the process and IPC
		/// </summary>
		/// <returns></returns>
		public IEnumerator Start()
		{
			string cefProcessPath = WebBrowserUtils.GetCefProcessApplication();
			if (!File.Exists(cefProcessPath))
			{
				Debug.LogError("The CEF browser process doesn't exist!");
				yield break;
			}

			//Start the server process
			serverProcess = new Process
			{
				StartInfo = new ProcessStartInfo(cefProcessPath, $"-width {width} -height {height} -url {initialUrl}")
				{
					CreateNoWindow = !showProcessConsole,
					UseShellExecute = showProcessConsole
				},
			};
			serverProcess.Start();

			//Start our client
			context = new ZContext();
			requester = new ZSocket(context, ZSocketType.REQ)
			{
				SendTimeout = new TimeSpan(0, 0, 4),
				ReceiveTimeout = new TimeSpan(0, 0, 4),
				Linger = new TimeSpan(0, 0, 4)
			};

			requester.Connect(ipcEndpoint, out ZError error);

			if (!Equals(error, ZError.None))
			{
				Debug.LogError("Server failed to start for some reason!");

				yield break; 
			}

			isRunning = true;
			errorCount = 0;

			yield return new WaitForSeconds(0.100f);

			while (isRunning)
			{
				if(!SendData(new PingEvent()))
					continue;

				using ZFrame reply = requester.ReceiveFrame(out error);

				if (!Equals(error, ZError.None))
				{
					Debug.LogWarning("Failed to receive from server for some reason!");
					continue; 
				}

				if (!isRunning)
					break;

				byte[] bytes = reply.Read();

				if(reply.Length == 0)
					continue;

				BrowserTexture.LoadRawTextureData(bytes);
				BrowserTexture.Apply(false);

				yield return new WaitForSeconds(eventPollingTime);
			}
		}

		#region CEF process events

		/// <summary>
		///		Sends a keyboard event to the CEF process
		/// </summary>
		/// <param name="keysDown"></param>
		/// <param name="keysUp"></param>
		/// <param name="chars"></param>
		public void SendKeyboardEvent(int[] keysDown, int[] keysUp, string chars)
		{
			//TODO: Maybe don't send if all empty
			if(!SendData(new KeyboardEvent
			{
				Chars = chars,
				KeysDown = keysDown,
				KeysUp = keysUp
			}))
				return;

			using ZFrame frame = requester.ReceiveFrame(out ZError error);
			HandleEventReceiving(frame, error, nameof(KeyboardEvent));
		}

		/// <summary>
		///		Sends a mouse event to the CEF process
		/// </summary>
		/// <param name="mouseX"></param>
		/// <param name="mouseY"></param>
		public void SendMouseMoveEvent(int mouseX, int mouseY)
		{
			if(!SendData(new MouseEvent
			{
				MouseX = mouseX,
				MouseY = mouseY
			}))
				return;

			using ZFrame frame = requester.ReceiveFrame(out ZError error);
			HandleEventReceiving(frame, error, nameof(MouseEvent));
		}

		private void HandleEventReceiving(ZFrame frame, ZError error, string eventName)
		{
			if (!Equals(error, ZError.None))
			{
				Debug.LogError("Failed to receive!");
				return;
			}

			if (frame.ReadInt32() != (int) EventType.Ping)
				Debug.LogError($"Got an incorrect response for a {eventName}!");
		}

		#endregion

		#region Data Methods

		/// <summary>
		///		Send an event to the browser process
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[SecurityCritical]
		protected bool SendData(IEventData data)
		{
			string json = JsonConvert.SerializeObject(data);
			return SendData(new ZFrame(json));
		}

		/// <summary>
		///		Send data to the browser process
		/// </summary>
		/// <param name="frame"></param>
		/// <returns>Returns true if successful</returns>
		[SecurityCritical]
		protected bool SendData(ZFrame frame)
		{
			if (!isRunning)
				return false;

			//Send frame
			requester.SendFrame(frame, out ZError error);
			if (!Equals(error, ZError.None))
			{
				//It didn't send for some reason
				errorCount++;
				Debug.LogWarning($"Failed to send to server for some reason! {errorCount}");

				//We have hit our error count
				if (errorCount >= errorsTillFail)
				{
					Dispose();
					Debug.LogError($"Connection failed {errorCount} times! Quitting!");
					return false;
				}

				//Try to send it again
				SendData(frame);
			}

			return true;
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

			if (errorCount != errorsTillFail)
				SendData(new ShutdownEvent());

			isRunning = false;

			requester.Dispose();
			context.Dispose();

			serverProcess.Kill();
			serverProcess.Dispose();
		}

		#endregion
	}
}