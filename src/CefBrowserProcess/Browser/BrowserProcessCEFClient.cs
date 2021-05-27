using System;
using CefBrowserProcess.Models;
using UnityWebBrowser.EventData;
using Xilium.CefGlue;

namespace CefBrowserProcess.Browser
{
	/// <summary>
	///		Offscreen CEF
	/// </summary>
	public class BrowserProcessCEFClient : CefClient, IDisposable
	{
		private CefBrowser browser;
		private CefBrowserHost browserHost;
		private CefFrame mainFrame;
		
		private readonly BrowserProcessCEFLoadHandler loadHandler;
		private readonly BrowserProcessCEFRenderHandler renderHandler;
		private readonly BrowserProcessCEFLifespanHandler lifespanHandler;
		private readonly BrowserProcessCEFDisplayHandler displayHandler;
		private readonly BrowserProcessCEFRequestHandler requestHandler;

		///  <summary>
		/// 		Creates a new <see cref="BrowserProcessCEFClient"/> instance
		///  </summary>
		///  <param name="size">The size of the window</param>
		///  <param name="proxySettings"></param>
		public BrowserProcessCEFClient(CefSize size, ProxySettings proxySettings)
		{
			loadHandler = new BrowserProcessCEFLoadHandler(this);
			renderHandler = new BrowserProcessCEFRenderHandler(size);
			lifespanHandler = new BrowserProcessCEFLifespanHandler();
			lifespanHandler.AfterCreated += cefBrowser =>
			{
				browser = cefBrowser;
				browserHost = cefBrowser.GetHost();
				mainFrame = cefBrowser.GetMainFrame();
			};
			displayHandler = new BrowserProcessCEFDisplayHandler();
			requestHandler = new BrowserProcessCEFRequestHandler(proxySettings);
		}

		/// <summary>
		///		Destroys the <see cref="BrowserProcessCEFClient"/> instance
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
		///		Process a <see cref="UnityWebBrowser.EventData.MouseMoveEvent"/>
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
		///		Process a <see cref="UnityWebBrowser.EventData.MouseClickEvent"/>
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
		///		Process a <see cref="UnityWebBrowser.EventData.MouseScrollEvent"/>
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

		/// <summary>
		///		Process a <see cref="ButtonEvent"/>
		/// </summary>
		/// <param name="buttonEvent"></param>
		public void ProcessButtonEvent(ButtonEvent buttonEvent)
		{
			switch (buttonEvent.ButtonType)
			{
				case ButtonType.Back:
					GoBack();
					break;
				case ButtonType.Forward:
					GoForward();
					break;
				case ButtonType.Refresh:
					Refresh();
					break;
				case ButtonType.NavigateUrl:
					LoadUrl(buttonEvent.UrlToNavigate);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
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

		private void LoadUrl(string url)
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

		private void GoBack()
		{
			if(browser.CanGoBack)
				browser.GoBack();
		}

		private void GoForward()
		{
			if(browser.CanGoForward)
				browser.GoForward();
		}

		private void Refresh()
		{
			browser.Reload();
		}

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