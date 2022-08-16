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
        Assert.NotNull(stream);
    }
}