using System;
using System.Diagnostics;
using ZeroMQ;
using Debug = UnityEngine.Debug;

[Serializable]
public class WebBrowserClient
{
	public string ipcEndpoint = "tcp://127.0.0.1:5555";

	private Process serverProcess;
	private ZContext context;
	private ZSocket requester;

	public void Init()
	{
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
	    Debug.Log(reply.ReadString());
    }

    public void Shutdown()
    {
	    requester.Send(new ZFrame((byte) 1));

		requester.Dispose();
		context.Dispose();
    }
}