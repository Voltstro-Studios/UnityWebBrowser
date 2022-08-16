// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using NUnit.Framework;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Helper;

namespace VoltstroStudios.UnityWebBrowser.Tests
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