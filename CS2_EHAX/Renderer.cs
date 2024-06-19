using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace CS2_EHAX
{
    public class Renderer : Overlay
    {   
        // our checkbox values
        public bool aimbot = true;
        public bool aimOnTeam = false;
        public bool aimOnlyOnSpotted= false;

        public float FOV = 50;
        public Vector2 screenSize = new Vector2(2560, 1440);
        public Vector4 circleColor = new Vector4(1, 1, 1, 1);
        protected override void Render()
        {
            ImGui.Begin("E-HAX Menu");
            ImGui.Checkbox("AimBot", ref aimbot);
            ImGui.Checkbox("Aim on teammate", ref aimOnTeam);
            ImGui.Checkbox("Aim on visiable", ref aimOnlyOnSpotted);
            ImGui.SliderFloat("pixel FOV", ref FOV, 10, 300);
            if (ImGui.CollapsingHeader("FOV circle color"))
                ImGui.ColorPicker4("##circlecolor", ref circleColor);

            // draw circle
            DrawOverlay();
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), FOV, ImGui.ColorConvertFloat4ToU32(circleColor));

        }

        void DrawOverlay()
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(new Vector2(0,0));
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
                );

        }

    }
}
