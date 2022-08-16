// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using NUnit.Framework;
using VoltstroStudios.UnityWebBrowser.Logging;
using VoltstroStudios.UnityWebBrowser.Shared;

namespace VoltstroStudios.UnityWebBrowser.Tests
{
    public class LoggerTests
    {
        [Test]
        public void DebugLogTest()
        {
            const string json =
                "{\"@t\":\"2022-01-02T07:42:28.3014491Z\",\"@m\":\"Some message...\",\"@i\":\"82d21ca4\",\"@l\":\"Debug\"}";
            JsonLogStructure logStructure = ProcessLogHandler.ReadJsonLog(json);
            Assert.AreEqual(LogSeverity.Debug, logStructure.Level);
            Assert.AreEqual(null, logStructure.Exception);
            StringAssert.IsMatch("Some message...", logStructure.Message);
            StringAssert.IsMatch("82d21ca4", logStructure.EventId);
        }

        [Test]
        public void InfoLogTest()
        {
            const string json =
                "{\"@t\":\"2022-01-02T07:42:28.3014491Z\",\"@m\":\"Some message...\",\"@i\":\"82d21ca4\"}";
            JsonLogStructure logStructure = ProcessLogHandler.ReadJsonLog(json);
            Assert.AreEqual(LogSeverity.Info, logStructure.Level);
            Assert.AreEqual(null, logStructure.Exception);
            StringAssert.IsMatch("Some message...", logStructure.Message);
            StringAssert.IsMatch("82d21ca4", logStructure.EventId);
        }

        [Test]
        public void WarnLogTest()
        {
            const string json =
                "{\"@t\":\"2022-01-02T07:42:28.3014491Z\",\"@m\":\"Some message...\",\"@i\":\"82d21ca4\",\"@l\":\"Warning\"}";
            JsonLogStructure logStructure = ProcessLogHandler.ReadJsonLog(json);
            Assert.AreEqual(LogSeverity.Warn, logStructure.Level);
            Assert.AreEqual(null, logStructure.Exception);
            StringAssert.IsMatch("Some message...", logStructure.Message);
            StringAssert.IsMatch("82d21ca4", logStructure.EventId);
        }

        [Test]
        public void ErrorLogTest()
        {
            const string json =
                "{\"@t\":\"2022-01-02T07:42:28.3014491Z\",\"@m\":\"Some message...\",\"@i\":\"82d21ca4\",\"@l\":\"Error\"}";
            JsonLogStructure logStructure = ProcessLogHandler.ReadJsonLog(json);
            Assert.AreEqual(LogSeverity.Error, logStructure.Level);
            Assert.AreEqual(null, logStructure.Exception);
            StringAssert.IsMatch("Some message...", logStructure.Message);
            StringAssert.IsMatch("82d21ca4", logStructure.EventId);
        }

        [Test]
        public void ExceptionLogTest()
        {
            const string json =
                "{\"@t\":\"2022-01-02T08:05:56.7365214Z\",\"@m\":\"Error setting up IPC!\",\"@i\":\"3418fa09\",\"@l\":\"Error\",\"@x\":\"System.TimeoutException: The operation has timed out.\n   at System.IO.Pipes.NamedPipeClientStream.ConnectInternal(Int32 timeout, CancellationToken cancellationToken, Int32 startTime)\n   at System.IO.Pipes.NamedPipeClientStream.Connect(Int32 timeout)\n   at VoltRpc.Communication.Pipes.PipesClient.Connect()\n   at UnityWebBrowser.Engine.Shared.EngineEntryPoint.SetupIpc(IEngine engine, LaunchArguments arguments) in /media/liam/Data/Projects/2021/UnityWebBrowser/src/UnityWebBrowser.Engine.Shared/EngineEntryPoint.cs:line 250\"}";
            JsonLogStructure logStructure = ProcessLogHandler.ReadJsonLog(json);
            Assert.AreEqual(LogSeverity.Error, logStructure.Level);
            StringAssert.IsMatch("Error setting up IPC!", logStructure.Message);
            StringAssert.IsMatch("3418fa09", logStructure.EventId);
        }
    }
}