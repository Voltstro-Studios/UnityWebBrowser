// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UImGui;
using Unity.Profiling;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Core;
using VoltstroStudios.UnityWebBrowser.Core.Popups;
using VoltstroStudios.UnityWebBrowser.Logging;
using Resolution = VoltstroStudios.UnityWebBrowser.Shared.Resolution;

namespace VoltstroStudios.UnityWebBrowser.Prj
{
    public class UWBPrjDebugUI : MonoBehaviour
    {
        public WebBrowserUIBasic webBrowserUIBasic;
        public Resolution[] resolutions;
        public float refreshRate = 1f;
        public bool hide;
        private ProfilerRecorder applyTextureMarker;
        private double applyTextureTime;

        private bool formatMessages = true;
        private List<string> formattedConsoleItems;
        private int fps;

        private ProfilerRecorder getPixelsMarker;

        private double getPixelsTime;
        private int lastSelectedIndex;

        private List<WebBrowserPopupInfo> popups;

        private string[] resolutionsText;
        private int selectedIndex;

        private float timer;
        private List<string> unformattedConsoleItems;

        private string inputUrl;

        private void Start()
        {
            if (webBrowserUIBasic == null)
                throw new ArgumentNullException(nameof(webBrowserUIBasic), "Web browser UI is unassigned!");

            const string startMessage = "Debug UI started.";
            unformattedConsoleItems = new List<string> { startMessage };
            formattedConsoleItems = new List<string> { startMessage };
            popups = new List<WebBrowserPopupInfo>();
            webBrowserUIBasic.browserClient.processLogHandler.OnProcessOutputLog += HandleOutputLogMessage;
            webBrowserUIBasic.browserClient.processLogHandler.OnProcessErrorLog += HandleErrorLogMessage;

            webBrowserUIBasic.browserClient.OnPopup += popup =>
            {
                popups.Add(popup);
                popup.OnDestroyed += () => popups.Remove(popup);
            };

            webBrowserUIBasic.browserClient.OnUrlChanged += url =>
            {
                inputUrl = url;
            };

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

        private void Update()
        {
            fps = (int)(1f / Time.unscaledDeltaTime);

            if (!(Time.unscaledTime > timer)) return;

            getPixelsTime = GetRecorderFrameTimeAverage(getPixelsMarker) * 1e-6f;
            applyTextureTime = GetRecorderFrameTimeAverage(applyTextureMarker) * 1e-6f;

            timer = Time.unscaledTime + refreshRate;
        }

        private void OnDestroy()
        {
            UImGuiUtility.Layout -= OnImGuiLayout;

            getPixelsMarker.Dispose();
            applyTextureMarker.Dispose();
        }

        private void OnImGuiLayout(UImGui.UImGui uImGui)
        {
            if(hide)
                return;
            
            ImGui.Begin("UWB Debug UI");
            {
                if (!webBrowserUIBasic.browserClient.ReadySignalReceived || webBrowserUIBasic.browserClient.HasDisposed)
                {
                    ImGui.Text("UWB is not ready...");
                }
                else
                {
                    //Basic details
                    ImGui.Text("UWB Debug UI");
                    ImGui.Separator();
                    ImGui.Text($"Player FPS: {fps}");
                    ImGui.Text($"UWB FPS: {webBrowserUIBasic.browserClient.FPS}");
                    ImGui.Text($"Get Texture Pixels: {getPixelsTime:F1}ms");
                    ImGui.Text($"Texture Apply Time: {applyTextureTime:F1}ms");
                    
                    //Mouse position
                    webBrowserUIBasic.GetMousePosition(out Vector2 mousePos);
                    ImGui.Text($"Mouse Position: {mousePos}");
                    if (ImGui.Button("Get Scroll Position"))
                        webBrowserUIBasic.browserClient.logger?.Debug(webBrowserUIBasic.browserClient.GetScrollPosition());
                    ImGui.Spacing();
                    ImGui.Separator();
                    
                    //URL
                    if (ImGui.InputText("URL", ref inputUrl, 1000))
                        inputUrl = inputUrl;
                    ImGui.SameLine();
                    if(ImGui.Button("Go"))
                        webBrowserUIBasic.NavigateUrl(inputUrl);
                    
                    ImGui.Spacing();
                    ImGui.Separator();

                    //Popups
                    {
                        ImGui.Text("Popups");
                        if (ImGui.Button("Show a Popup"))
                            webBrowserUIBasic.browserClient.ExecuteJs(
                                "open('https://voltstro.dev', 'Voltstro', 'width=600,height=400')");

                        //Display all of our popups
                        foreach (WebBrowserPopupInfo popupInfo in popups)
                        {
                            ImGui.Text(popupInfo.PopupGuid.ToString());

                            if (ImGui.Button("Execute JS"))
                                popupInfo.ExecuteJs("console.log('Hello world from popup!')");

                            if (ImGui.Button("Destroy"))
                                popupInfo.Dispose();
                        }
                    }
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
                                for (int i = 0; i < formattedConsoleItems.Count; i++)
                                    ImGui.TextUnformatted(formattedConsoleItems[i]);
                            else
                                for (int i = 0; i < unformattedConsoleItems.Count; i++)
                                    ImGui.TextUnformatted(unformattedConsoleItems[i]);
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

            List<ProfilerRecorderSample> samples = new(samplesCount);
            recorder.CopyTo(samples);
            double r = samples.Aggregate<ProfilerRecorderSample, double>(0,
                (current, sample) => current + sample.Value);
            r /= samplesCount;

            return r;
        }
    }
}