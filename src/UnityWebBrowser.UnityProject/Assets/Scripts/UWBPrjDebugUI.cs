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
    public sealed class UWBPrjDebugUI : MonoBehaviour
    {
        /// <summary>
        ///     Reference to <see cref="WebBrowserUIBasic"/>
        /// </summary>
        public WebBrowserUIBasic webBrowserUIBasic;
        
        /// <summary>
        ///     Debug resolution selections
        /// </summary>
        public Resolution[] resolutions;
        
        /// <summary>
        ///     How often to update FPS
        /// </summary>
        public float refreshRate = 1f;
        
        /// <summary>
        ///     Hide the ImGui Window
        /// </summary>
        public bool hide;

        private bool hasInitialized;
        private bool hasConnected;
        
        //Fps/Data tracking
        private ProfilerRecorder getPixelsMarker;
        private ProfilerRecorder applyTextureMarker;
        private int fps;
        private float nextUpdateTime;
        private double applyTextureTime;
        private double getPixelsTime;

        //Popups
        private List<WebBrowserPopupInfo> popups;

        //Resolutions
        private string[] resolutionsText;
        private int selectedResolutionIndex;
        private int lastSelectedResolutionsIndex;
        
        //Console messages
        private bool formatMessages = true;
        private List<string> formattedConsoleItems;
        private List<string> unformattedConsoleItems;

        //URL
        private string inputUrl;
        
        //Zoom
        private double zoomLevel = double.MinValue;

        private void Awake()
        {
            if (webBrowserUIBasic == null)
                throw new ArgumentNullException(nameof(webBrowserUIBasic), "Web browser UI is unassigned!");

            const string startMessage = "Debug UI started.";
            unformattedConsoleItems = new List<string> { startMessage };
            formattedConsoleItems = new List<string> { startMessage };
            popups = new List<WebBrowserPopupInfo>();
            inputUrl = string.Empty;
            
            //Backup, probs shouldn't happen this early
            if(webBrowserUIBasic.browserClient.HasDisposed)
                return;
            
            webBrowserUIBasic.browserClient.OnClientInitialized += OnClientInitialized;
            webBrowserUIBasic.browserClient.OnClientConnected += OnClientConnected;

            UImGuiUtility.Layout += OnImGuiLayout;
        }

        private void OnClientInitialized()
        {
            Debug.Log("BrowserClient initialized...");
            
            webBrowserUIBasic.browserClient.OnUrlChanged += url =>
            {
                inputUrl = url;
            };
            
            getPixelsMarker = ProfilerRecorder.StartNew(WebBrowserClient.markerGetPixels, 15);
            applyTextureMarker = ProfilerRecorder.StartNew(WebBrowserClient.markerLoadTextureApply, 15);

            //Calculate resolution texts
            resolutionsText = new string[resolutions.Length];
            for (int i = 0; i < resolutions.Length; i++)
            {
                Resolution resolution = resolutions[i];
                resolutionsText[i] = resolution.ToString();

                if (!resolution.Equals(webBrowserUIBasic.browserClient.Resolution))
                    continue;

                selectedResolutionIndex = i;
                lastSelectedResolutionsIndex = i;
            }
            
            //Popup Events
            webBrowserUIBasic.browserClient.OnPopup += popup =>
            {
                popups.Add(popup);
                popup.OnDestroyed += () => popups.Remove(popup);
            };

            hasInitialized = true;
        }
        
        private void OnClientConnected()
        {
            Debug.Log("BrowserClient connected!");
            webBrowserUIBasic.browserClient.processLogHandler.OnProcessOutputLog += HandleOutputLogMessage;
            webBrowserUIBasic.browserClient.processLogHandler.OnProcessErrorLog += HandleErrorLogMessage;
            
            hasConnected = true;
        }

        private void Update()
        {
            if(webBrowserUIBasic.browserClient.HasDisposed || !hasConnected)
                return;
            
            fps = (int)(1f / Time.unscaledDeltaTime);

            if (!(Time.unscaledTime > nextUpdateTime)) return;

            getPixelsTime = GetRecorderFrameTimeAverage(getPixelsMarker) * 1e-6f;
            applyTextureTime = GetRecorderFrameTimeAverage(applyTextureMarker) * 1e-6f;

            nextUpdateTime = Time.unscaledTime + refreshRate;
        }

        private void OnDestroy()
        {
            UImGuiUtility.Layout -= OnImGuiLayout;
            
            if(webBrowserUIBasic.browserClient.HasDisposed)
                return;

            getPixelsMarker.Dispose();
            applyTextureMarker.Dispose();
        }

        private void OnImGuiLayout(UImGui.UImGui uImGui)
        {
            if(hide)
                return;
            
            if(webBrowserUIBasic.browserClient.HasDisposed)
                return;
            
            ImGui.Begin("UWB Debug UI");
            {
                if (!hasInitialized)
                {
                    ImGui.Text("UWB is still initializing...");
                }
                
                else if (!hasConnected)
                {
                    ImGui.Text("Waiting for UWB to connect to engine...");
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
                    
                    ImGui.Spacing();
                    ImGui.Separator();
                    
                    //Buttons for getting details
                    {
                        if (ImGui.Button("Get Scroll Pos"))
                            Debug.Log(webBrowserUIBasic.browserClient.GetScrollPosition());
                        
                        ImGui.SameLine();
                        
                        if(ImGui.Button("Open DevTools"))
                            webBrowserUIBasic.browserClient.OpenDevTools();
                        
                        ImGui.SameLine();
                        
                        ImGui.Text("Zoom Percent");
                        ImGui.SameLine();
                        
                        //Get zoom level when ready
                        if (zoomLevel <= 0 && webBrowserUIBasic.browserClient.IsConnected)
                            zoomLevel = webBrowserUIBasic.browserClient.GetZoomLevel();
                    
                        if (ImGui.InputDouble("##zoom", ref zoomLevel))
                        {
                            zoomLevel = Math.Clamp(zoomLevel, 0.1, double.MaxValue);
                            webBrowserUIBasic.browserClient.SetZoomLevelPercent(zoomLevel);
                        }
                    }
                        
                    ImGui.Spacing();
                    ImGui.Separator();
                    
                    //URL
                    ImGui.Text("URL");
                    ImGui.SameLine();
                    ImGui.InputText("##url", ref inputUrl, 1000);
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

                    //Resolution selection
                    ImGui.Text("Resolution:");
                    ImGui.PushItemWidth(100);
                    ImGui.ListBox("", ref selectedResolutionIndex, resolutionsText, resolutionsText.Length);
                    ImGui.PopItemWidth();

                    //Console
                    {
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
            }
            ImGui.End();

            //Update resolution if it changed
            if (selectedResolutionIndex != lastSelectedResolutionsIndex)
            {
                webBrowserUIBasic.browserClient.Resolution = resolutions[selectedResolutionIndex];
                lastSelectedResolutionsIndex = selectedResolutionIndex;
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