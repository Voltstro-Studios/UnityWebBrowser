using NUnit.Framework;
using UnityWebBrowser.Engine.Shared;

namespace UnityWebBrowser.Tests;

public class ColorTests
{
    [Test]
    public void ColorBlackTest()
    {
        Color color = new("00000000");
        Assert.AreEqual(0, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
        Assert.AreEqual(0, color.A);
    }

    [Test]
    public void ColorBlackNoAlphaTest()
    {
        Color color = new("000000");
        Assert.AreEqual(0, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
        Assert.AreEqual(255, color.A);
    }

    [Test]
    public void ColorBlackSolidTest()
    {
        Color color = new("000000FF");
        Assert.AreEqual(0, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
        Assert.AreEqual(255, color.A);
    }

    [Test]
    public void ColorRedTest()
    {
        Color color = new("ff000000");
        Assert.AreEqual(255, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
        Assert.AreEqual(0, color.A);
    }

    [Test]
    public void ColorRedSolidTest()
    {
        Color color = new("ff0000ff");
        Assert.AreEqual(255, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
        Assert.AreEqual(255, color.A);
    }

    [Test]
    public void ColorYellowTest()
    {
        Color color = new("faff00");
        Assert.AreEqual(250, color.R);
        Assert.AreEqual(255, color.G);
        Assert.AreEqual(0, color.B);
        Assert.AreEqual(255, color.A);
    }
}