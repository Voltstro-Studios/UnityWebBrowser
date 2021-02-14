using System;
using ZeroMQ;

namespace UnityWebBrowserServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using ZContext context = new ZContext();
			using ZSocket responder = new ZSocket(context, ZSocketType.REP);
			responder.Bind("tcp://*:5555");

			while (true)
			{
				using ZFrame request = responder.ReceiveFrame();
				byte message = request.ReadAsByte();
				if(message == 1)
					break;

				responder.Send(new ZFrame("Hello World!"));
			}
		}
	}
}