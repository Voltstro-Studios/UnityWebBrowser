using System;
using System.Collections;
using System.Diagnostics;
using Newtonsoft.Json;
using UnityEngine;
using ZeroMQ;
using Debug = UnityEngine.Debug;

[Serializable]
public class WebBrowserClient
{
	public string ipcEndpoint = "tcp://127.0.0.1:5555";

	public string initialUrl = "https://google.com";

	public string browserServerPath = @"..\UnityWebBrowserServer\bin\Debug\netcoreapp3.1\UnityWebBrowserServer.exe";

	public int width = 1920;
	public int height = 1080;

	public float eventPollingTime = 0.01f;

	public int errorsTillFail = 4;

	public Texture2D BrowserTexture { get; private set; }

	private Process serverProcess;
	private ZContext context;
	private ZSocket requester;

	private int errorCount;

	private bool isRunning;

	private EventData eventData;

	public void Init()
	{
		eventData = new EventData
		{
			KeysDown = new int[0],
			KeysUp = new int[0],
			Chars = ""
		};
		BrowserTexture = new Texture2D(width, height, TextureFormat.BGRA32, false, true);
	}

	public IEnumerator Start()
	{
		//Start the server process
		serverProcess = new Process
		{
			//TODO: Figure out path, once we have converted the server to C++ for cross-platform
			StartInfo = new ProcessStartInfo(browserServerPath, $"-width {width} -height {height} -url {initialUrl}")
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
			string data = JsonConvert.SerializeObject(eventData);
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

		if(errorCount > errorsTillFail)
			requester.Send(new ZFrame(JsonConvert.SerializeObject(eventData)));

		requester.Dispose();
		context.Dispose();
    }
}