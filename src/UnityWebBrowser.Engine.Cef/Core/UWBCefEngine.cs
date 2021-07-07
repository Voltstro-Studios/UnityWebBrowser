using System;
using UnityWebBrowser.Engine.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core
{
	/// <summary>
	///		Main class for the CEF Unity Web Browser Engine
	/// </summary>
	public class UWBCefEngine : EngineEntryPoint
	{
		private CefEngine cefEngine;
		
		protected override void EntryPoint(LaunchArguments launchArguments, string[] args)
		{
			cefEngine = new CefEngine(launchArguments, args);
			cefEngine.Init();
			
			SetupIpc(cefEngine, launchArguments);
			
			//Calling run message loop will cause the main thread to lock (what we want)
			CefRuntime.RunMessageLoop();
			
			//If the message loop quits
			Logger.Debug("Message loop quit.");
			Dispose();
		}

		#region Destroy

		~UWBCefEngine()
		{
			ReleaseResources();
		}

		public override void Dispose()
		{
			base.Dispose();
			ReleaseResources();
			GC.SuppressFinalize(this);
		}

		private void ReleaseResources()
		{
			cefEngine?.Dispose();
		}

		#endregion
	}
}