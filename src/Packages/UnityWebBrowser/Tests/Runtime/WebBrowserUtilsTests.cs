using NUnit.Framework;
using UnityEngine;
using UnityWebBrowser.Helper;

namespace UnityWebBrowser.Tests
{
    public class WebBrowserUtilsTests
    {
        [Test]
        public void ColorToHexBlackTest()
        {
            Color32 color = Color.black;
            string hex = WebBrowserUtils.ColorToHex(color);
            StringAssert.AreEqualIgnoringCase("000000ff", hex);
        }
        
        [Test]
        public void ColorToHexRedTest()
        {
            Color32 color = Color.red;
            string hex = WebBrowserUtils.ColorToHex(color);
            StringAssert.AreEqualIgnoringCase("ff0000ff", hex);
        }
    }
}