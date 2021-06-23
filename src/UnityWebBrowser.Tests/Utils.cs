using NUnit.Framework;
using ZeroMQ;

namespace UnityWebBrowser.Tests
{
    public static class Utils
    {
        public static void CreateZmq(ZSocketType socketType, int port, bool bind, out ZContext context, out ZSocket socket)
        {
            context = new ZContext();
            socket = new ZSocket(context, socketType);
            ZError error;
            if (bind)
                socket.Bind($"tcp://127.0.0.1:{port}", out error);
            else
                socket.Connect($"tcp://127.0.0.1:{port}", out error);
            Assert.AreEqual(ZError.None, error);
        }

        public static void Send(this ZSocket socket, byte[] data)
        {
            socket.Send(new ZFrame(data), out ZError error);
            Assert.AreEqual(ZError.None, error);
        }

        public static byte[] Receive(this ZSocket socket)
        {
            ZFrame frame = null;
            try
            {
                frame = socket.ReceiveFrame(out ZError error);
                Assert.AreEqual(ZError.None, error);
                return frame.Read();
            }
            finally
            {
                frame?.Dispose();
            }
        }
    }
}