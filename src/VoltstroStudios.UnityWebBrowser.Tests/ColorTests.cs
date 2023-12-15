// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using NUnit.Framework;
using VoltstroStudios.UnityWebBrowser.Engine.Shared;

namespace VoltstroStudios.UnityWebBrowser.Tests;

public class ColorTests
{
    [Test]
    public void ColorBlackTest()
    {
        Color color = new("00000000");
        Assert.That(color.R, Is.EqualTo(0));
        Assert.That(color.G, Is.EqualTo(0));
        Assert.That(color.B, Is.EqualTo(0));
        Assert.That(color.A, Is.EqualTo(0));
    }

    [Test]
    public void ColorBlackNoAlphaTest()
    {
        Color color = new("000000");
        Assert.That(color.R, Is.EqualTo(0));
        Assert.That(color.G, Is.EqualTo(0));
        Assert.That(color.B, Is.EqualTo(0));
        Assert.That(color.A, Is.EqualTo(255));
    }

    [Test]
    public void ColorBlackSolidTest()
    {
        Color color = new("000000FF");
        Assert.That(color.R, Is.EqualTo(0));
        Assert.That(color.G, Is.EqualTo(0));
        Assert.That(color.B, Is.EqualTo(0));
        Assert.That(color.A, Is.EqualTo(255));
    }

    [Test]
    public void ColorRedTest()
    {
        Color color = new("ff000000");
        Assert.That(color.R, Is.EqualTo(255));
        Assert.That(color.G, Is.EqualTo(0));
        Assert.That(color.B, Is.EqualTo(0));
        Assert.That(color.A, Is.EqualTo(0));
    }

    [Test]
    public void ColorRedSolidTest()
    {
        Color color = new("ff0000ff");
        Assert.That(color.R, Is.EqualTo(255));
        Assert.That(color.G, Is.EqualTo(0));
        Assert.That(color.B, Is.EqualTo(0));
        Assert.That(color.A, Is.EqualTo(255));
    }

    [Test]
    public void ColorYellowTest()
    {
        Color color = new("faff00");
        Assert.That(color.R, Is.EqualTo(250));
        Assert.That(color.G, Is.EqualTo(255));
        Assert.That(color.B, Is.EqualTo(0));
        Assert.That(color.A, Is.EqualTo(255));
    }
}