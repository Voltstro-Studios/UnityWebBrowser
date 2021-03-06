using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using ZeroMQ;
using Debug = UnityEngine.Debug;

namespace UnityWebBrowser
{
	/// <summary>
	///		Handles managing the process and worker thread
	/// </summary>
	[Serializable]
	public class WebBrowserClient
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
		public bool showProcessConsole = false;

		/// <summary>
		///		Texture that the browser will paint to
		/// </summary>
		public Texture2D BrowserTexture { get; private set; }

		private Process serverProcess;
		private ZContext context;
		private ZSocket requester;

		private int errorCount;

		private bool isRunning;

		private EventData eventData;

		/// <summary>
		///		Inits the browser client
		/// </summary>
		public void Init()
		{
			eventData = new EventData
			{
				KeysDown = new int[0],
				KeysUp = new int[0],
				Chars = ""
			};
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
				string data = JsonUtility.ToJson(eventData);

				requester.Send(new ZFrame(data), out error);

				eventData.LeftDown = false;
				eventData.LeftUp = false;
				eventData.RightDown = false;
				eventData.RightUp = false;

				if (!Equals(error, ZError.None))
				{
					errorCount++;
					Debug.LogWarning($"Failed to send to server for some reason! {errorCount}");

					if (errorCount >= errorsTillFail)
					{
						Shutdown();

						Debug.LogError($"Connection failed {errorCount} times! Quitting!");

						yield break;
					}

					continue;
				}

				using ZFrame reply = requester.ReceiveFrame(out error);

				if (!Equals(error, ZError.None))
				{
					Debug.LogWarning("Failed to receive from server for some reason!");
					continue; 
				}

				if (!isRunning)
					break;

				byte[] bytes = reply.Read();

				if(reply == null || reply.Length == 0)
					continue;

				BrowserTexture.LoadRawTextureData(bytes);
				BrowserTexture.Apply(false);

				yield return new WaitForSeconds(eventPollingTime);
			}
		}

		public void SetKeyboardData(string chars, int[] keysDown, int[] keysUp)
		{
			eventData.KeysDown = keysDown;
			eventData.KeysUp = keysUp;
			eventData.Chars = chars;
		}

		public void SetMousePosData(int x, int y)
		{
			eventData.MouseX = x;
			eventData.MouseY = y;
		}

		public void Shutdown()
		{
			if(!isRunning)
				return;

			isRunning = false;
			eventData.Shutdown = true;

			if(errorCount != errorsTillFail)
				requester.Send(new ZFrame(JsonUtility.ToJson(eventData)));

			requester.Dispose();
			context.Dispose();
		}
	}
}