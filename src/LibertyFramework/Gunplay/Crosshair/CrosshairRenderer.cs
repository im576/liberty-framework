using System;
using System.Drawing;
using GTA;
using LibertyFramework.Gunplay.Profiles;

namespace LibertyFramework.Gunplay.Crosshair
{
    // Neutral four-segment crosshair. The gap between segments is the current spread cone
    // projected to screen pixels, so it opens exactly as far as bullets can deviate.
    internal sealed class CrosshairRenderer
    {
        private double displayedGapPixels = -1;

        internal double DisplayedGapPixels { get { return displayedGapPixels; } }

        internal static double ConeToPixels(double coneDegrees, double fovDegrees, bool fovIsVertical, Size resolution)
        {
            double halfFov = Math.Max(1.0, fovDegrees) * 0.5 * Math.PI / 180.0;
            double axisPixels = fovIsVertical ? resolution.Height * 0.5 : resolution.Width * 0.5;
            return Math.Tan(coneDegrees * Math.PI / 180.0) / Math.Tan(halfFov) * axisPixels;
        }

        internal void Reset()
        {
            displayedGapPixels = -1;
        }

        internal void Draw(GTA.Graphics graphics, CrosshairSettings settings, double coneDegrees, double fovDegrees, double pixelsPerTangent, Size resolution, float frameSeconds)
        {
            // Prefer the game's own projection (measured each frame from GET_VIEWPORT_POSITION_OF_COORD); the FOV formula is the fallback.
            double target = pixelsPerTangent > 0 ? Math.Tan(coneDegrees * Math.PI / 180.0) * pixelsPerTangent :
                ConeToPixels(coneDegrees, fovDegrees, settings.FovAxis == "vertical", resolution);
            target = Math.Max(settings.MinimumGapPixels, Math.Min(settings.MaximumGapPixels, target));
            if (displayedGapPixels < 0) { displayedGapPixels = target; }
            else
            {
                double blend = 1.0 - Math.Exp(-settings.GapSmoothingPerSecond * Math.Max(0.0, frameSeconds));
                // Opening follows immediately so the display never under-reports spread after a shot.
                displayedGapPixels = target > displayedGapPixels ? target : displayedGapPixels + (target - displayedGapPixels) * blend;
            }

            float centerX = resolution.Width * 0.5f;
            float centerY = resolution.Height * 0.5f;
            float gap = (float)displayedGapPixels;
            float length = (float)settings.LineLengthPixels;
            float thickness = (float)settings.LineThicknessPixels;
            float outline = (float)settings.OutlinePixels;
            Color color = ToColor(settings.ColorArgb);
            Color outlineColor = ToColor(settings.OutlineArgb);

            graphics.Scaling = FontScaling.Pixel;
            if (outline > 0)
            {
                DrawSegments(graphics, centerX, centerY, gap - outline, length + outline * 2, thickness + outline * 2, outlineColor);
            }
            DrawSegments(graphics, centerX, centerY, gap, length, thickness, color);
            if (settings.ShowCenterDot)
            {
                float dot = (float)settings.CenterDotPixels;
                if (outline > 0) { graphics.DrawRectangle(centerX, centerY, dot + outline * 2, dot + outline * 2, outlineColor); }
                graphics.DrawRectangle(centerX, centerY, dot, dot, color);
            }
        }

        // DrawRectangle(centerX, centerY, width, height) is centre-based in ScriptHookDotNet.
        private static void DrawSegments(GTA.Graphics graphics, float x, float y, float gap, float length, float thickness, Color color)
        {
            float offset = gap + length * 0.5f;
            graphics.DrawRectangle(x, y - offset, thickness, length, color);
            graphics.DrawRectangle(x, y + offset, thickness, length, color);
            graphics.DrawRectangle(x - offset, y, length, thickness, color);
            graphics.DrawRectangle(x + offset, y, length, thickness, color);
        }

        private static Color ToColor(int[] argb)
        {
            return Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);
        }
    }
}
