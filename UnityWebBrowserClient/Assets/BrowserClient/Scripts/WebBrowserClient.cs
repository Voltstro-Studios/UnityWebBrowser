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

	public Texture2D BrowserTexture { get; private set; }

	private Process serverProcess;
	private ZContext context;
	private ZSocket requester;

	private bool isRunning;

	private EventData eventData;

	public void Init()
	{
		eventData = new EventData();
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
			SendTimeout = new TimeSpan(0, 0, 2),
			ReceiveTimeout = new TimeSpan(0, 0, 2)
		};

		requester.Connect(ipcEndpoint, out ZError error);

		if (!Equals(error, ZError.None))
		{
			Debug.LogError("Server failed to start for some reason!");

			yield break; 
		}

		isRunning = true;

		yield return new WaitForSeconds(0.100f);

		while (isRunning)
		{
			string data = JsonConvert.SerializeObject(eventData);
			requester.Send(new ZFrame(data), out error);

			if (!Equals(error, ZError.None))
			{
				Debug.LogError("Failed to send to server for some reason!");
				continue; 
			}

			using ZFrame reply = requester.ReceiveFrame(out error);

			if (!Equals(error, ZError.None))
			{
				Debug.LogError("Failed to receive from server for some reason!");
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

	public void SetData(string chars, int[] keysDown, int[] keysUp)
	{
		eventData.keysDown = keysDown;
		eventData.keysUp = keysUp;
		eventData.chars = chars;
	}

	public void Shutdown()
    {
	    isRunning = false;
	    eventData.shutdown = true;
	    requester.Send(new ZFrame(JsonConvert.SerializeObject(eventData)));

		requester.Dispose();
		context.Dispose();
    }
}