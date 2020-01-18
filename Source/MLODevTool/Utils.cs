using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static CitizenFX.Core.UI.Screen;
using CitizenFX.Core.UI;

namespace MLODevTool
{
    class Utils
    {
        public static void DrawTextOnScreen(string text, float xPosition, float yPosition, float size, CitizenFX.Core.UI.Alignment justification, int font, bool disableTextOutline)
        {
            if (IsHudPreferenceSwitchedOn() && !IsPlayerSwitchInProgress() && IsScreenFadedIn() && !IsPauseMenuActive() && !IsFrontendFading() && !IsPauseMenuRestarting() && !IsHudHidden())
            {
                SetTextFont(font);
                SetTextScale(1.0f, size);
                if (justification == CitizenFX.Core.UI.Alignment.Right)
                {
                    SetTextWrap(0f, xPosition);
                }
                SetTextJustification((int)justification);
                if (!disableTextOutline) { SetTextOutline(); }
                BeginTextCommandDisplayText("STRING");
                AddTextComponentSubstringPlayerName(text);
                EndTextCommandDisplayText(xPosition, yPosition);
            }
        }
        public static void Draw3DText(float x, float y, float z, string text, int font, float scaleSent, int r, int g, int b)
        {
            var playerCamCoords = GetGameplayCamCoords();
            var dist = GetDistanceBetweenCoords(playerCamCoords[0], playerCamCoords[1], playerCamCoords[2], x, y, z, true);

            var scale = ((1 / dist) * 2) * scaleSent;
            var fov = (1 / GetGameplayCamFov()) * 100;
            scale = scale * fov;

            SetTextScale(0.0f * scale, 1.1f * scale);
            SetTextFont(font);
            SetTextProportional(true);
            SetTextColour(r, g, b, 250);
            SetTextDropshadow(1, 1, 1, 1, 255);
            SetTextEdge(2, 0, 0, 0, 150);
            SetTextDropShadow();
            SetTextOutline();
            SetTextEntry("STRING");
            SetTextCentre(true);
            AddTextComponentString(text);
            SetDrawOrigin(x, y, z + 2, 0);
            DrawText(0.0f, 0.0f);
            ClearDrawOrigin();
        }

        // By Codewalker
        public static Vector3 QMultiply(Quaternion a, Vector3 b)
        {
            float axx = a.X * 2.0f;
            float ayy = a.Y * 2.0f;
            float azz = a.Z * 2.0f;
            float awxx = a.W * axx;
            float awyy = a.W * ayy;
            float awzz = a.W * azz;
            float axxx = a.X * axx;
            float axyy = a.X * ayy;
            float axzz = a.X * azz;
            float ayyy = a.Y * ayy;
            float ayzz = a.Y * azz;
            float azzz = a.Z * azz;
            return new Vector3(((b.X * ((1.0f - ayyy) - azzz)) + (b.Y * (axyy - awzz))) + (b.Z * (axzz + awyy)),
                        ((b.X * (axyy + awzz)) + (b.Y * ((1.0f - axxx) - azzz))) + (b.Z * (ayzz - awxx)),
                        ((b.X * (axzz - awyy)) + (b.Y * (ayzz + awxx))) + (b.Z * ((1.0f - axxx) - ayyy)));
        }
    }
}
