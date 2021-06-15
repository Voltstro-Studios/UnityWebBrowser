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
	public class CefUWBEngine : IDisposable
	{
		private readonly EventReplier<EngineActionEvent, EngineEvent> eventReplier;
		private readonly CefManager cefManager;

		///  <summary>
		/// 		Creates a new <see cref="CefUWBEngine"/> instance
		///  </summary>
		///  <param name="launchArguments"></param>
		///  <param name="cefArgs"></param>
		///  <exception cref="Exception"></exception>
		public CefUWBEngine(LaunchArguments launchArguments, string[] cefArgs)
		{
			cefManager = new CefManager(launchArguments, cefArgs);
			cefManager.Init();
			eventReplier = new EventReplier<EngineActionEvent, EngineEvent>(launchArguments.Port, OnEventReceived);
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
				case NavigateUrlEvent x:
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

		/// <summary>
		///		Starts a loop that deals with the incoming events
		/// </summary>
		public void HandelEventsLoop()
		{
			eventReplier.HandleEventsLoop().RunSynchronously();
		}

		#region Destroy

		~CefUWBEngine()
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