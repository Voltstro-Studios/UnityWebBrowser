// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Collections.Generic;
using NUnit.Framework;
using VoltstroStudios.UnityWebBrowser.Editor;

namespace VoltstroStudios.UnityWebBrowser.Tests.Editor
{
    public class EditorHelperTests
    {
        [Test]
        public void FindAssetsByTypeTest()
        {
            List<EditorHelperFindAssetTestAsset> assets =
                EditorHelper.FindAssetsByType<EditorHelperFindAssetTestAsset>();
            Assert.AreEqual(1, assets.Count);

            Assert.AreEqual(69420, assets[0].value);
        }
    }
}