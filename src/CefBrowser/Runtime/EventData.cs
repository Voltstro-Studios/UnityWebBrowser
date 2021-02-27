using System;

namespace UnityWebBrowser
{
	[Serializable]
	public struct EventData
	{
		public bool Shutdown;

		public int[] KeysDown;
		public int[] KeysUp;
		public string Chars;

		public int MouseX;
		public int MouseY;

		public bool LeftDown;
		public bool RightDown;

		public bool LeftUp;
		public bool RightUp;
	}
}