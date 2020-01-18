using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace MLODevTool
{
    public class MLODevTool : BaseScript
    {
        private static bool ShowDebugBox { get; set; } = true;
        private static bool ShowCorners { get; set; } = true;

        public MLODevTool()
        {
            Tick += OnTick;
        }

        [Command("intpoly")]
        public void ShowDebugPoly(params string[] args)
        {
            MLODevTool.ShowDebugBox = !MLODevTool.ShowDebugBox;
        }
        [Command("intcorners")]
        public void ShowInteriorCorners(params string[] args)
        {
            MLODevTool.ShowCorners = !MLODevTool.ShowCorners;
        }
        [Command("intset")]
        public void SetEntitySet(params string[] args)
        {
            if (args.ToList().Count > 0)
            {
                string entitySetName = args[0].ToString();
                Debug.WriteLine($" # Set Interior EntitySet Name: {entitySetName}");
                int InteriorId = GetInteriorFromEntity(Game.PlayerPed.Handle);
                if (InteriorId != 0)
                {
                    if (IsInteriorEntitySetActive(InteriorId, entitySetName))
                    {
                        DeactivateInteriorEntitySet(InteriorId, entitySetName);
                    }
                    else
                    {
                        ActivateInteriorEntitySet(InteriorId, entitySetName);
                    }
                    RefreshInterior(InteriorId);
                }

            }
        }
        [Command("intflag")]
        public void SetInteriorFlags(params string[] args)
        {
            if (args.ToList().Count > 0)
            {
                int portalId = Convert.ToInt32(args[0]);
                int newFlags = Convert.ToInt32(args[1]);
                Debug.WriteLine($" # Set Interior Portal Id {portalId} Flags: {newFlags}");
                int InteriorId = GetInteriorFromEntity(Game.PlayerPed.Handle);
                if (InteriorId != 0)
                {
                    SetInteriorPortalFlag(InteriorId, portalId, newFlags);
                    RefreshInterior(InteriorId);
                }

            }
        }


        private async Task OnTick()
        {
            try
            {
                    int InteriorId = GetInteriorFromEntity(Game.PlayerPed.Handle);
                    var pedPosition = Game.PlayerPed.Position;
                if (InteriorId != 0)
                {
                    float ix = 0f;
                    float iy = 0f;
                    float iz = 0f;
                    GetInteriorPosition(InteriorId, ref ix, ref iy, ref iz);
                    float rotX = 0f;
                    float rotY = 0f;
                    float rotZ = 0f;
                    float rotW = 0f;
                    GetInteriorRotation(InteriorId, ref rotX, ref rotY, ref rotZ, ref rotW);

                    Vector3 YMapInteriorPos = new Vector3(ix, iy, iz);
                    Quaternion Orientation = new Quaternion(rotX, rotY, rotZ, rotW);
                    SetTextWrap(0f, 1f);
                    Utils.DrawTextOnScreen($"~w~Interior Id:~r~ {InteriorId} ~w~- Pos: ~r~{YMapInteriorPos.X}~w~ / ~r~{YMapInteriorPos.Y} ~w~/~r~ {YMapInteriorPos.Z}", 0.01f, 0.01f, 0.48f, CitizenFX.Core.UI.Alignment.Left, 6, false);
                    int countPortals = GetInteriorPortalCount(InteriorId);
                    int countRooms = GetInteriorRoomCount(InteriorId);
                    SetTextWrap(0f, 1f);
                    Utils.DrawTextOnScreen($"~w~Portals Count:~r~ {countPortals} ~w~/ Rooms Count:~r~ {countRooms}", 0.01f, 0.04f, 0.48f, CitizenFX.Core.UI.Alignment.Left, 6, false);
                    SetTextWrap(0f, 1f);
                    var roomHash = GetRoomKeyFromEntity(Game.PlayerPed.Handle);
                    var currentRoomId = GetInteriorRoomIndexByHash(InteriorId, roomHash);
                    Utils.DrawTextOnScreen($"~w~Current Room:~r~ {currentRoomId}", 0.01f, 0.07f, 0.48f, CitizenFX.Core.UI.Alignment.Left, 6, false);

                    for (int portalId = 0; portalId < countPortals; portalId++)
                    {
                        Vector3[] corners = new Vector3[4];
                        Vector3[] pureCorners = new Vector3[4];
                        for (int c = 0; c < 4; c++)
                        {
                            float cx = 0.0f;
                            float cy = 0.0f;
                            float cz = 0.0f;

                            GetInteriorPortalCornerPosition(InteriorId, portalId, c, ref cx, ref cy, ref cz);
                            Vector3 cornerPosition = YMapInteriorPos + Utils.QMultiply(Orientation, new Vector3(cx, cy, cz));
                            corners[c] = cornerPosition;
                            pureCorners[c] = new Vector3(cx, cy, cz);
                        }

                        Vector3 CrossVector = Vector3.Lerp(corners[0], corners[2], 0.5f);

                        if (ShowDebugBox)
                        {
                            DrawPoly(corners[0].X, corners[0].Y, corners[0].Z, corners[1].X, corners[1].Y, corners[1].Z, corners[2].X, corners[2].Y, corners[2].Z, 255, 0, 0, 100);
                            DrawPoly(corners[0].X, corners[0].Y, corners[0].Z, corners[2].X, corners[2].Y, corners[2].Z, corners[3].X, corners[3].Y, corners[3].Z, 255, 0, 0, 100);
                            DrawPoly(corners[3].X, corners[3].Y, corners[3].Z, corners[2].X, corners[2].Y, corners[2].Z, corners[1].X, corners[1].Y, corners[1].Z, 255, 0, 0, 100);
                            DrawPoly(corners[3].X, corners[3].Y, corners[3].Z, corners[1].X, corners[1].Y, corners[1].Z, corners[0].X, corners[0].Y, corners[0].Z, 255, 0, 0, 100);
                        }

                        if (GetDistanceBetweenCoords(pedPosition.X, pedPosition.Y, pedPosition.Z, CrossVector.X, CrossVector.Y, CrossVector.Z, true) <= 10)
                        {
                            DrawLine(corners[0].X, corners[0].Y, corners[0].Z, corners[1].X, corners[1].Y, corners[1].Z, 0, 255, 0, 255);
                            DrawLine(corners[1].X, corners[1].Y, corners[1].Z, corners[2].X, corners[2].Y, corners[2].Z, 0, 255, 0, 255);
                            DrawLine(corners[2].X, corners[2].Y, corners[2].Z, corners[3].X, corners[3].Y, corners[3].Z, 0, 255, 0, 255);
                            DrawLine(corners[3].X, corners[3].Y, corners[3].Z, corners[0].X, corners[0].Y, corners[0].Z, 0, 255, 0, 255);
                            if (ShowCorners && (GetDistanceBetweenCoords(pedPosition.X, pedPosition.Y, pedPosition.Z, CrossVector.X, CrossVector.Y, CrossVector.Z, true) <= 5))
                            {
                                Utils.Draw3DText(corners[0].X, corners[0].Y, corners[0].Z - 1.9f, $"~r~C~g~{0}~w~(~r~{pureCorners[0].X}~w~/~r~{pureCorners[0].Y}~w~/~r~{pureCorners[0].Z}~w~)", 6, 0.6f, 255, 0, 0);
                                Utils.Draw3DText(corners[1].X, corners[1].Y, corners[1].Z - 1.9f, $"~r~C~g~{1}~w~(~r~{pureCorners[1].X}~w~/~r~{pureCorners[1].Y}~w~/~r~{pureCorners[1].Z}~w~)", 6, 0.6f, 255, 0, 0);
                                Utils.Draw3DText(corners[2].X, corners[2].Y, corners[2].Z - 1.9f, $"~r~C~g~{2}~w~(~r~{pureCorners[2].X}~w~/~r~{pureCorners[2].Y}~w~/~r~{pureCorners[2].Z}~w~)", 6, 0.6f, 255, 0, 0);
                                Utils.Draw3DText(corners[3].X, corners[3].Y, corners[3].Z - 1.9f, $"~r~C~g~{3}~w~(~r~{pureCorners[3].X}~w~/~r~{pureCorners[3].Y}~w~/~r~{pureCorners[3].Z}~w~)", 6, 0.6f, 255, 0, 0);
                            }
                            Utils.Draw3DText(CrossVector.X, CrossVector.Y, CrossVector.Z - 1.5f, $"~w~Portal #~g~{portalId}", 6, 0.9f, 255, 0, 0);
                            int portalFlags = GetInteriorPortalFlag(InteriorId, portalId);
                            int portalRoomTo = GetInteriorPortalRoomTo(InteriorId, portalId);
                            int portalRoomFrom = GetInteriorPortalRoomFrom(InteriorId, portalId);
                            Utils.Draw3DText(CrossVector.X, CrossVector.Y, CrossVector.Z - 1.7f, $"~w~From ~g~{portalRoomFrom} ~w~ To ~g~{portalRoomTo}", 6, 0.9f, 255, 0, 0);
                            Utils.Draw3DText(CrossVector.X, CrossVector.Y, CrossVector.Z - 1.9f, $"~w~Flags~g~ {portalFlags}", 6, 0.9f, 255, 0, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Interior Error: {ex.Message} - {ex.StackTrace}");
            }
        }
    }
}
