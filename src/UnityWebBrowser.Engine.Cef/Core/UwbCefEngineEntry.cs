using System;
using UnityWebBrowser.Engine.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core
{
	/// <summary>
	///		<see cref="EngineEntryPoint"/> for the Cef engine
	/// </summary>
	public class UwbCefEngineEntry : EngineEntryPoint
	{
		private CefEngineManager cefEngineManager;
		
		protected override void EntryPoint(LaunchArguments launchArguments, string[] args)
		{
			cefEngineManager = new CefEngineManager(launchArguments, args);
			cefEngineManager.Init();
			
			SetupIpc(cefEngineManager, launchArguments);
			Ready(launchArguments);

			//Setup our events
			cefEngineManager.OnUrlChanged += url =>
			{
				if(IsConnected)
					clientEvents?.UrlChange(url);
			};
			cefEngineManager.OnLoadStart += url =>
			{
				if (IsConnected)
					clientEvents?.LoadStart(url);
			};
			cefEngineManager.OnLoadFinish += url =>
			{
				if (IsConnected)
					clientEvents?.LoadFinish(url);
			};
			
			//Calling run message loop will cause the main thread to lock (what we want)
			CefRuntime.RunMessageLoop();
			
			//If the message loop quits
			Logger.Debug("Message loop quit.");
			Dispose();
			Environment.Exit(0);
		}

		#region Destroy

		protected override void ReleaseResources()
		{
			cefEngineManager?.Dispose();
			base.ReleaseResources();
		}

		#endregion
	}
}