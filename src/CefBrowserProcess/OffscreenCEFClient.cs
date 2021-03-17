using System;
using System.Runtime.InteropServices;
using System.Security;
using UnityWebBrowser;
using Xilium.CefGlue;

namespace CefBrowserProcess
{
	/// <summary>
	///		Offscreen CEF
	/// </summary>
	public class OffscreenCEFClient : CefClient, IDisposable
	{
		private CefSize size;
		private CefBrowserHost host;

		private readonly OffscreenLoadHandler loadHandler;
		private readonly OffscreenRenderHandler renderHandler;
		private readonly OffscreenLifespanHandler lifespanHandler;

		private readonly byte[] pixelBuffer;
		private static readonly object PixelLock = new object();

		/// <summary>
		///		Creates a new <see cref="OffscreenCEFClient"/> instance
		/// </summary>
		/// <param name="size">The size of the window</param>
		public OffscreenCEFClient(CefSize size)
		{
			loadHandler = new OffscreenLoadHandler(this);
			renderHandler = new OffscreenRenderHandler(this);
			lifespanHandler = new OffscreenLifespanHandler();

			pixelBuffer = new byte[size.Width * size.Height * 4];

			this.size = size;
		}

		/// <summary>
		///		Destroys the <see cref="OffscreenCEFClient"/> instance
		/// </summary>
		public void Dispose()
		{
			host?.CloseBrowser(true);
			host?.Dispose();
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///		Gets the pixel data of the CEF window
		/// </summary>
		/// <returns></returns>
		public byte[] GetPixels()
		{
			if(host == null)
				return Array.Empty<byte>();

			//Copy the pixel buffer
			byte[] pixelBytes = new byte[pixelBuffer.Length];
			lock (PixelLock)
			{
				Array.Copy(pixelBuffer, pixelBytes, pixelBuffer.Length);
			}
			return pixelBytes;
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
					WindowsKeyCode = c,
					EventType = CefKeyEventType.Char
				});
			}
		}

		/// <summary>
		///		Process a <see cref="UnityWebBrowser.MouseMoveEvent"/>
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
		///		Process a <see cref="UnityWebBrowser.MouseClickEvent"/>
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
		///		Process a <see cref="UnityWebBrowser.MouseScrollEvent"/>
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
		///		Process a <see cref="UnityWebBrowser.ButtonEvent"/>
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
					NavigateUrl(buttonEvent.UrlToNavigate);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void KeyEvent(CefKeyEvent keyEvent)
		{
			lifespanHandler.Browser.GetHost().SendKeyEvent(keyEvent);
		}

		private void MouseMoveEvent(CefMouseEvent mouseEvent)
		{
			lifespanHandler.Browser.GetHost().SendMouseMoveEvent(mouseEvent, false);
		}

		private void MouseClickEvent(CefMouseEvent mouseEvent, int clickCount, CefMouseButtonType button, bool mouseUp)
		{
			lifespanHandler.Browser.GetHost().SendMouseClickEvent(mouseEvent, button, mouseUp, clickCount);
		}

		private void MouseScrollEvent(CefMouseEvent mouseEvent, int scroll)
		{
			lifespanHandler.Browser.GetHost().SendMouseWheelEvent(mouseEvent, 0, scroll);
		}

		private void NavigateUrl(string url)
		{
			lifespanHandler.Browser.GetMainFrame().LoadUrl(url);
		}

		private void GoBack()
		{
			if(lifespanHandler.Browser.CanGoBack)
				lifespanHandler.Browser.GoBack();
		}

		private void GoForward()
		{
			if(lifespanHandler.Browser.CanGoForward)
				lifespanHandler.Browser.GoForward();
		}

		private void Refresh()
		{
			lifespanHandler.Browser.Reload();
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

		/// <summary>
		///		Offscreen load handler
		/// </summary>
		private class OffscreenLoadHandler : CefLoadHandler
		{
			private readonly OffscreenCEFClient client;

			internal OffscreenLoadHandler(OffscreenCEFClient client)
			{
				this.client = client;
			}

			protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
			{
				if (browser != null)
					client.host = browser.GetHost();

				if(frame.IsMain)
					Console.WriteLine($"START: {browser?.GetMainFrame().Url}");
			}

			protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
			{
				if(frame.IsMain)
					Console.WriteLine($"END: {browser.GetMainFrame().Url}, {httpStatusCode}");
			}
		}

		/// <summary>
		///		Offscreen render handler
		/// </summary>
		private class OffscreenRenderHandler : CefRenderHandler
		{
			private readonly OffscreenCEFClient client;

			internal OffscreenRenderHandler(OffscreenCEFClient client)
			{
				this.client = client;
			}

			protected override CefAccessibilityHandler GetAccessibilityHandler()
			{
				return null;
			}

			protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
			{
				GetViewRect(browser, out CefRectangle newRect);
				rect = newRect;
				return true;
			}

			protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
			{
				screenX = viewX;
				screenY = viewY;
				return true;
			}

			protected override void GetViewRect(CefBrowser browser, out CefRectangle rect)
			{
				rect = new CefRectangle(0, 0, client.size.Width, client.size.Height);
			}

			[SecurityCritical]
			protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width,
				int height)
			{
				if (browser != null)
				{
					lock (PixelLock)
					{
						Marshal.Copy(buffer, client.pixelBuffer, 0, client.pixelBuffer.Length);
					}
				}
			}

			protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
			{
				return false;
			}

			protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
			{
			}

			protected override void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr sharedHandle)
			{
			}

			protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
			{
			}

			protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
			{
			}

			protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
			{
			}
		}

		/// <summary>
		///		Offscreen lifespan handler
		/// </summary>
		private class OffscreenLifespanHandler : CefLifeSpanHandler
		{
			public CefBrowser Browser;

			protected override void OnAfterCreated(CefBrowser browser)
			{
				Browser = browser;
			}

			protected override bool DoClose(CefBrowser browser)
			{
				return false;
			}
		}

		/// <summary>
		///		Offscreen app
		/// </summary>
		public class OffscreenCEFApp : CefApp
		{
		}
	}
}