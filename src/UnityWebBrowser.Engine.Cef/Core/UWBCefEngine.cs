using System;
using UnityWebBrowser.Engine.Shared;
using UnityWebBrowser.Shared.Events.EngineAction;
using UnityWebBrowser.Shared.Events.EngineActionResponse;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core
{
	/// <summary>
	///		Main class for the CEF Unity Web Browser Engine
	/// </summary>
	public class UWBCefEngine : EngineEntryPoint
	{
		private CefManager cefManager;
		
		protected override void EntryPoint(LaunchArguments launchArguments, string[] args)
		{
			cefManager = new CefManager(launchArguments, args);
			cefManager.Init();
		}

		protected override EngineActionResponse OnEvent(EngineActionEvent actionEvent)
		{
			switch (actionEvent)
			{
				case ShutdownEvent:
					Dispose();
					break;
				case PingEvent:
					return new PixelsResponse
					{
						Pixels = cefManager.GetPixels()
					};
				case GoForwardEvent:
					cefManager.GoForward();
					break;
				case GoBackEvent:
					cefManager.GoBack();
					break;
				case RefreshEvent:
					cefManager.Refresh();
					break;
				case LoadUrlEvent x:
					cefManager.LoadUrl(x.Url);
					break;
				case LoadHtmlEvent x:
					cefManager.LoadHtml(x.Html);
					break;
				case ExecuteJsEvent x:
					cefManager.ExecuteJs(x.Js);
					break;
				case KeyboardEvent x:
					cefManager.HandelKeyboardEvent(x);
					break;
				case MouseMoveEvent x:
					cefManager.HandelMouseMoveEvent(x);
					break;
				case MouseClickEvent x:
					cefManager.HandelMouseClickEvent(x);
					break;
				case MouseScrollEvent x:
					cefManager.HandelMouseScrollEvent(x);
					break;
			}

			return new OkResponse();
		}

		#region Destroy

		public override void Dispose()
		{
			base.Dispose();
			cefManager?.Dispose();
			CefRuntime.Shutdown();
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}