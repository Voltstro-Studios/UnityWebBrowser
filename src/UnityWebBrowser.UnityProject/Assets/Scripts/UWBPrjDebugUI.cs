using ImGuiNET;
using UImGui;
using UnityEngine;

public class UWBPrjDebugUI : MonoBehaviour
{
    private void Start()
    {
        UImGuiUtility.Layout += OnImGuiLayout;
    }

    private void OnDestroy()
    {
        UImGuiUtility.Layout -= OnImGuiLayout;
    }

    private void OnImGuiLayout(UImGui.UImGui uImGui)
    {
        ImGui.Begin("UWB Debug UI");
        {
            ImGui.Text("Hello World!");
        }
        ImGui.End();
    }
}
