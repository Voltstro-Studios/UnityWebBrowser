using UnityWebBrowser.Engine.Cef.Core;

namespace UnityWebBrowser.Engine.Cef
{
	/// <summary>
	///		Main class for this program
	/// </summary>
	public static class Program
	{
		/// <summary>
		///		Entry point
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public static int Main(string[] args)
		{
			UwbCefEngineEntry cefEngineEntry = new();
			return cefEngineEntry.Main(args);
		}
	}
}