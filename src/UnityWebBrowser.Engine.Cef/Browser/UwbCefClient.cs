using System;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser
{
	/// <summary>
	///		Offscreen CEF
	/// </summary>
	public class UwbCefClient : CefClient, IDisposable
	{
		private CefBrowser browser;
		private CefBrowserHost browserHost;
		private CefFrame mainFrame;
		
		private readonly UwbCefLoadHandler loadHandler;
		private readonly UwbCefRenderHandler renderHandler;
		private readonly UwbCefLifespanHandler lifespanHandler;
		private readonly UwbCefDisplayHandler displayHandler;
		private readonly UwbCefRequestHandler requestHandler;

		public event Action<string> OnUrlChange;
		public event Action<string> OnLoadStart;
		public event Action<string> OnLoadFinish; 

		///  <summary>
		/// 		Creates a new <see cref="UwbCefClient"/> instance
		///  </summary>
		///  <param name="size">The size of the window</param>
		///  <param name="proxySettings"></param>
		public UwbCefClient(CefSize size, ProxySettings proxySettings)
		{
			//Setup our handlers
			loadHandler = new UwbCefLoadHandler(this);
			renderHandler = new UwbCefRenderHandler(size);
			lifespanHandler = new UwbCefLifespanHandler();
			lifespanHandler.AfterCreated += cefBrowser =>
			{
				browser = cefBrowser;
				browserHost = cefBrowser.GetHost();
				mainFrame = cefBrowser.GetMainFrame();
			};
			displayHandler = new UwbCefDisplayHandler(this);
			requestHandler = new UwbCefRequestHandler(proxySettings);
		}

		/// <summary>
		///		Destroys the <see cref="UwbCefClient"/> instance
		/// </summary>
		public void Dispose()
		{
			browserHost?.CloseBrowser(true);
			browserHost?.Dispose();
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///		Gets the pixel data of the CEF window
		/// </summary>
		/// <returns></returns>
		public byte[] GetPixels()
		{
			if(browserHost == null)
				return Array.Empty<byte>();

			return renderHandler.Pixels;
		}

		#region Engine Events

		/// <summary>
		///		Process a <see cref="KeyboardEvent"/>
		/// </summary>
		/// <param name="keyboardEvent"></param>
		public void ProcessKeyboardEvent(KeyboardEvent keyboardEvent)
		{
			//Keys down
			foreach (int i in keyboardEvent.KeysDown)
			{
				KeyEvent(new CefKeyEvent
				{
					WindowsKeyCode = i,
					EventType = CefKeyEventType.KeyDown
				});
			}

			//Keys up
			foreach (int i in keyboardEvent.KeysUp)
			{
				KeyEvent(new CefKeyEvent
				{
					WindowsKeyCode = i,
					EventType = CefKeyEventType.KeyUp
				});
			}

			//Chars
			foreach (char c in keyboardEvent.Chars)
			{
				KeyEvent(new CefKeyEvent
				{
#if WINDOWS
					WindowsKeyCode = c,
#else
					Character = c,
#endif
					EventType = CefKeyEventType.Char
				});
			}
		}

		/// <summary>
		///		Process a <see cref="Shared.Events.EngineActions.MouseMoveEvent"/>
		/// </summary>
		/// <param name="mouseEvent"></param>
		public void ProcessMouseMoveEvent(MouseMoveEvent mouseEvent)
		{
			MouseMoveEvent(new CefMouseEvent
			{
				X = mouseEvent.MouseX,
				Y = mouseEvent.MouseY
			});
		}

		/// <summary>
		///		Process a <see cref="Shared.Events.EngineActions.MouseClickEvent"/>
		/// </summary>
		/// <param name="mouseClickEvent"></param>
		public void ProcessMouseClickEvent(MouseClickEvent mouseClickEvent)
		{
			MouseClickEvent(new CefMouseEvent
			{
				X = mouseClickEvent.MouseX,
				Y = mouseClickEvent.MouseY
			}, mouseClickEvent.MouseClickCount, 
				(CefMouseButtonType)mouseClickEvent.MouseClickType, 
				mouseClickEvent.MouseEventType == MouseEventType.Up);
		}

		/// <summary>
		///		Process a <see cref="Shared.Events.EngineActions.MouseScrollEvent"/>
		/// </summary>
		/// <param name="mouseScrollEvent"></param>
		public void ProcessMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
		{
			MouseScrollEvent(new CefMouseEvent
			{
				X = mouseScrollEvent.MouseX,
				Y = mouseScrollEvent.MouseY
			}, mouseScrollEvent.MouseScroll);
		}

		private void KeyEvent(CefKeyEvent keyEvent)
		{
			browserHost.SendKeyEvent(keyEvent);
		}

		private void MouseMoveEvent(CefMouseEvent mouseEvent)
		{
			browserHost.SendMouseMoveEvent(mouseEvent, false);
		}

		private void MouseClickEvent(CefMouseEvent mouseEvent, int clickCount, CefMouseButtonType button, bool mouseUp)
		{
			browserHost.SendMouseClickEvent(mouseEvent, button, mouseUp, clickCount);
		}

		private void MouseScrollEvent(CefMouseEvent mouseEvent, int scroll)
		{
			browserHost.SendMouseWheelEvent(mouseEvent, 0, scroll);
		}

		public void LoadUrl(string url)
		{
			mainFrame.LoadUrl(url);
		}

		public void LoadHtml(string html)
		{
			mainFrame.LoadUrl($"data:text/html,{html}");
		}

		public void ExecuteJs(string js)
		{
			mainFrame.ExecuteJavaScript(js, "", 0);
		}

		public void GoBack()
		{
			if(browser.CanGoBack)
				browser.GoBack();
		}

		public void GoForward()
		{
			if(browser.CanGoForward)
				browser.GoForward();
		}

		public void Refresh()
		{
			browser.Reload();
		}
		
		#endregion

		#region CEF Events

		public void UrlChange(string url)
		{
			OnUrlChange?.Invoke(url);
		}

		public void LoadStart(string url)
		{
			OnLoadStart?.Invoke(url);
		}

		public void LoadFinish(string url)
		{
			OnLoadFinish?.Invoke(url);
		}

		#endregion
		
		protected override CefLoadHandler GetLoadHandler()
		{
			return loadHandler;
		}

		protected override CefRenderHandler GetRenderHandler()
		{
			return renderHandler;
		}

		protected override CefLifeSpanHandler GetLifeSpanHandler()
		{
			return lifespanHandler;
		}

		protected override CefDisplayHandler GetDisplayHandler()
		{
			return displayHandler;
		}

		protected override CefRequestHandler GetRequestHandler()
		{
			return requestHandler;
		}
	}
}