// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.IO;
using NUnit.Framework;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Pages;

namespace VoltstroStudios.UnityWebBrowser.Tests;

public class PagesHandlerTests
{
    [Test]
    public void GetAboutPageTest()
    {
        Stream stream = PagesHandler.GetAboutPageCode();
        Assert.That(stream, Is.Not.Null);
    }
}