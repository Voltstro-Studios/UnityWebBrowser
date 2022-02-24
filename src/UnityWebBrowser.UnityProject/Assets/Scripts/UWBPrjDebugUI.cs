using System;
using ImGuiNET;
using UImGui;
using UnityEngine;
using UnityWebBrowser;
using Resolution = UnityWebBrowser.Shared.Resolution;

public class UWBPrjDebugUI : MonoBehaviour
{
    public WebBrowserUIBasic webBrowserUIBasic;

    public Resolution[] resolutions;

    private string[] resolutionsText;
    private int selectedIndex;
    private int lastSelectedIndex;
    
    private void Start()
    {
        if (webBrowserUIBasic == null)
            throw new ArgumentNullException(nameof(webBrowserUIBasic), "Web browser UI is unassigned!");
        
        UImGuiUtility.Layout += OnImGuiLayout;

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
    }

    private void OnImGuiLayout(UImGui.UImGui uImGui)
    {
        ImGui.Begin("UWB Debug UI");
        {
            ImGui.Text("UWB Debug UI");
            ImGui.Spacing();
            ImGui.Text("Resolution:");
            
            ImGui.PushItemWidth(100);
            ImGui.ListBox("", ref selectedIndex, resolutionsText, resolutionsText.Length);
            ImGui.PopItemWidth();

        }
        ImGui.End();

        if (selectedIndex != lastSelectedIndex)
        {
            webBrowserUIBasic.browserClient.Resolution = resolutions[selectedIndex];
            lastSelectedIndex = selectedIndex;
        }
    }
}
