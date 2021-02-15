using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using ZeroMQ;
using Debug = UnityEngine.Debug;

[Serializable]
public class WebBrowserClient
{
	public string ipcEndpoint = "tcp://127.0.0.1:5555";

	public Texture2D BrowserTexture { get; private set; }

	private Process serverProcess;
	private ZContext context;
	private ZSocket requester;

	public void Init()
	{
		BrowserTexture = new Texture2D(1920, 1080, TextureFormat.BGRA32, false);

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
	}

    public void FixedUpdate()
    {
	    requester.Send(new ZFrame((byte) 0));

	    using ZFrame reply = requester.ReceiveFrame();
	    byte[] bytes = reply.Read();

		if(reply == null || reply.Length == 0)
			return;

		BrowserTexture.LoadRawTextureData(bytes);

		int xN = BrowserTexture.width;
		int yN = BrowserTexture.height;

		for (int i = 0; i < xN; i++)
		{
			for (int j = 0; j < yN; j++)
			{
				BrowserTexture.SetPixel(xN - i - 1, j, BrowserTexture.GetPixel(i, j));
			}
		}

		BrowserTexture.Apply(false);
    }

    public void Shutdown()
    {
	    requester.Send(new ZFrame((byte) 1));

		requester.Dispose();
		context.Dispose();
    }

    private string JoinByteArray(byte[] bytes)
    {
		StringBuilder sBuilder = new StringBuilder();
		foreach (byte b in bytes)
		{
			sBuilder.Append($"{b}, ");
		}

		return sBuilder.ToString();
    }
}