// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using NUnit.Framework;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

namespace VoltstroStudios.UnityWebBrowser.Tests;

public class MathTests
{
    [Test]
    public void HexByteTest()
    {
        byte result = MathHelper.HexToDec("FF");
        Assert.That(result, Is.EqualTo(255));
    }

    [Test]
    public void HexByte15Test()
    {
        byte result = MathHelper.HexToDec("0F");
        Assert.That(result, Is.EqualTo(15));
    }
}