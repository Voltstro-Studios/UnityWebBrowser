using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZeroMQ;

[Serializable]
public class WebBrowserClient
{
	public string ipcEndpoint = "tcp://127.0.0.1:5555";

	public Texture2D BrowserTexture { get; private set; }

	private Process serverProcess;
	private ZContext context;
	private ZSocket requester;

	private bool isRunning;

	public void Init()
	{
		BrowserTexture = new Texture2D(1920, 1080, TextureFormat.BGRA32, false, true);

		//Start the server process
		serverProcess = new Process
		{
			//TODO: Figure out path, once we have converted the server to C++ for cross-platform
			StartInfo = new ProcessStartInfo(
				$"D:/Projects/2021/UnityWebBrowser/UnityWebBrowserServer/bin/Debug/netcoreapp3.1/UnityWebBrowserServer.exe")
		};
		serverProcess.Start();

		//Start our client
		context = new ZContext();
		requester = new ZSocket(context, ZSocketType.REQ);
		requester.Connect(ipcEndpoint);
		isRunning = true;
	}

    public async UniTaskVoid Update()
    {
	    while (isRunning)
	    {
		    requester.Send(new ZFrame((byte) 0));

		    using ZFrame reply = requester.ReceiveFrame();
		    if (!isRunning)
			    break;

		    byte[] bytes = reply.Read();

		    if(reply == null || reply.Length == 0)
			    continue;

		    BrowserTexture.LoadRawTextureData(bytes);
		    BrowserTexture.Apply(false);

		    await UniTask.Delay(100);
	    }
    }

    public void Shutdown()
    {
	    isRunning = false;
	    requester.Send(new ZFrame((byte) 1));

		requester.Dispose();
		context.Dispose();
    }
}