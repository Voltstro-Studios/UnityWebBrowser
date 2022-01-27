using System.IO;
using NUnit.Framework;
using UnityWebBrowser.Engine.Shared.Pages;

namespace UnityWebBrowser.Tests;

public class PagesHandlerTests
{
    [Test]
    public void GetAboutPageTest()
    {
        Stream stream = PagesHandler.GetAboutPageCode();
        Assert.NotNull(stream);
    }
}