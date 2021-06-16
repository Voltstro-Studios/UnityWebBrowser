using System;
using UnityWebBrowser.Engine.Shared;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineActions;
using UnityWebBrowser.Shared.Events.EngineEvents;

namespace UnityWebBrowser.Engine.Cef.Core
{
	/// <summary>
	///		Main class for the CEF Unity Web Browser Engine
	/// </summary>
	public class UWBCefEngine : EngineEntryPoint, IDisposable
	{
		private EventReplier<EngineActionEvent, EngineEvent> eventReplier;
		private CefManager cefManager;
		
		protected override void EntryPoint(LaunchArguments launchArguments, string[] args)
		{
			cefManager = new CefManager(launchArguments, args);
			cefManager.Init();
			
			eventReplier = new EventReplier<EngineActionEvent, EngineEvent>(launchArguments.Port, OnEventReceived);
			eventReplier.HandleEventsLoop();
		}

		private EngineEvent OnEventReceived(EngineActionEvent actionEvent)
		{
			switch (actionEvent)
			{
				case ShutdownEvent:
					Dispose();
					break;
				case PingEvent:
					return new PixelsEvent
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

			return new OkEvent();
		}

		#region Destroy

		~UWBCefEngine()
		{
			ReleaseResources();
		}
		
		public void Dispose()
		{
			ReleaseResources();
			GC.SuppressFinalize(this);
		}

		private void ReleaseResources()
		{
			eventReplier?.Dispose();
			cefManager?.Dispose();
		}

		#endregion
	}
}