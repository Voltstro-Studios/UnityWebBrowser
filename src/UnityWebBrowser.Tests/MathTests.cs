using NUnit.Framework;
using UnityWebBrowser.Engine.Shared.Core;

namespace UnityWebBrowser.Tests;

public class MathTests
{
    [Test]
    public void HexByteTest()
    {
        byte result = MathHelper.HexToDec("FF");
        Assert.AreEqual(255, result);
    }
    
    [Test]
    public void HexByte15Test()
    {
        byte result = MathHelper.HexToDec("0F");
        Assert.AreEqual(15, result);
    }
}