using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UImGui;
using Unity.Profiling;
using UnityEngine;
using UnityWebBrowser.Core;
using UnityWebBrowser.Logging;
using Resolution = UnityWebBrowser.Shared.Resolution;

namespace UnityWebBrowser.Prj
{
    public class UWBPrjDebugUI : MonoBehaviour
    {
        public WebBrowserUIBasic webBrowserUIBasic;
        public Resolution[] resolutions;
        public float refreshRate = 1f;

        private string[] resolutionsText;
        private int selectedIndex;
        private int lastSelectedIndex;

        private ProfilerRecorder getPixelsMarker;
        private ProfilerRecorder applyTextureMarker;

        private float timer;
        private int fps;

        private double getPixelsTime;
        private double applyTextureTime;

        private bool formatMessages = true;
        private List<string> unformattedConsoleItems;
        private List<string> formattedConsoleItems;

        private void Start()
        {
            if (webBrowserUIBasic == null)
                throw new ArgumentNullException(nameof(webBrowserUIBasic), "Web browser UI is unassigned!");

            const string startMessage = "Debug UI started.";
            unformattedConsoleItems = new List<string> {startMessage};
            formattedConsoleItems = new List<string> {startMessage};
            webBrowserUIBasic.browserClient.processLogHandler.OnProcessOutputLog += HandleOutputLogMessage;
            webBrowserUIBasic.browserClient.processLogHandler.OnProcessErrorLog += HandleErrorLogMessage;

            UImGuiUtility.Layout += OnImGuiLayout;

            getPixelsMarker = ProfilerRecorder.StartNew(WebBrowserClient.markerGetPixels, 15);
            applyTextureMarker = ProfilerRecorder.StartNew(WebBrowserClient.markerLoadTextureApply, 15);

            resolutionsText = new string[resolutions.Length];
            for (int i = 0; i < resolutions.Length; i++)
            {
                Resolution resolution = resolutions[i];
                resolutionsText[i] = resolution.ToString();

                if (!resolution.Equals(webBrowserUIBasic.browserClient.Resolution)) 
                    continue;
            
                selectedIndex = i;
                lastSelectedIndex = i;
            }
        }

        private void OnDestroy()
        {
            UImGuiUtility.Layout -= OnImGuiLayout;
        
            getPixelsMarker.Dispose();
            applyTextureMarker.Dispose();
        }

        private void Update()
        {
            fps = (int) (1f / Time.unscaledDeltaTime);
        
            if (!(Time.unscaledTime > timer)) return;
        
            getPixelsTime = GetRecorderFrameTimeAverage(getPixelsMarker) * 1e-6f;
            applyTextureTime = GetRecorderFrameTimeAverage(applyTextureMarker) * 1e-6f;
        
            timer = Time.unscaledTime + refreshRate;
        }

        private void OnImGuiLayout(UImGui.UImGui uImGui)
        {
            ImGui.Begin("UWB Debug UI");
            {
                ImGui.Text("UWB Debug UI");
                ImGui.Separator();
                ImGui.Text($"FPS: {fps}");
                ImGui.Text($"Get Texture Pixels: {getPixelsTime:F1}ms");
                ImGui.Text($"Texture Apply Time: {applyTextureTime:F1}ms");
                ImGui.Spacing();
                ImGui.Separator();

                ImGui.Text("Resolution:");
                ImGui.PushItemWidth(100);
                ImGui.ListBox("", ref selectedIndex, resolutionsText, resolutionsText.Length);
                ImGui.PopItemWidth();

                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Text("UWB Console");
                ImGui.Checkbox("Format UWB JSON Messages", ref formatMessages);
                
                float footerHeight = ImGui.GetStyle().ItemSpacing.y + ImGui.GetFrameHeightWithSpacing();
                ImGui.BeginChild("Console", new Vector2(0, -footerHeight), false, ImGuiWindowFlags.HorizontalScrollbar);
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1));
                    {
                        if (formatMessages)
                        {
                            for (int i = 0; i < formattedConsoleItems.Count; i++)
                            {
                                ImGui.TextUnformatted(formattedConsoleItems[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < unformattedConsoleItems.Count; i++)
                            {
                                ImGui.TextUnformatted(unformattedConsoleItems[i]);
                            }
                        }
                    }
                    ImGui.PopStyleVar();
                }
                ImGui.EndChild();

                if (ImGui.Button("Clear"))
                {
                    formattedConsoleItems.Clear();
                    unformattedConsoleItems.Clear();
                }
            }
            ImGui.End();

            if (selectedIndex != lastSelectedIndex)
            {
                webBrowserUIBasic.browserClient.Resolution = resolutions[selectedIndex];
                lastSelectedIndex = selectedIndex;
            }
        }

        private void HandleOutputLogMessage(string message)
        {
            unformattedConsoleItems.Add(message);
            try
            {
                JsonLogStructure json = ProcessLogHandler.ReadJsonLog(message);
                formattedConsoleItems.Add($"[{json.Level}] [{json.Timestamp}] {json.Message}\n{json.Exception}");
            }
            catch (Exception ex)
            {
                formattedConsoleItems.Add($"Error reading JSON!\n{ex}");
            }
        }

        private void HandleErrorLogMessage(string message)
        {
            unformattedConsoleItems.Add(message);
            formattedConsoleItems.Add(message);
        }
    
        private double GetRecorderFrameTimeAverage(ProfilerRecorder recorder)
        {
            int samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            List<ProfilerRecorderSample> samples = new (samplesCount);
            recorder.CopyTo(samples);
            double r = samples.Aggregate<ProfilerRecorderSample, double>(0,
                (current, sample) => current + sample.Value);
            r /= samplesCount;

            return r;
        }
    }
}
