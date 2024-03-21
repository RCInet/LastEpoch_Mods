using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Scenes
{
    public class Minimap
    {
        [HarmonyPatch(typeof(MinimapFogOfWar), "Initialize")]
        public class MinimapFogOfWar_Initialize
        {
            [HarmonyPrefix]
            static bool Prefix(MinimapFogOfWar __instance, MinimapFogOfWar.QuadScale __0, UnityEngine.Vector3 __1)
            {
                if (Save_Manager.Data.UserData.Scene.Minimap.Remove_FogOfWar) { __instance.discoveryDistance = float.MaxValue; }
                else { __instance.discoveryDistance = 20f; } //Default Value
                return true;
            }
        }

        [HarmonyPatch(typeof(DMMapZoom), "ZoomOutMinimap")]
        public class DMMapZoom_ZoomOutMinimap
        {
            public static void Prefix(ref DMMapZoom __instance)
            {
                if (Save_Manager.Data.UserData.Scene.Minimap.Enable_MaxZoomOut)
                {
                    __instance.maxMinimapZoom = float.MaxValue;
                }
                else { __instance.maxMinimapZoom = 37.5f; }
            }
        }
        
        /*public struct MapIcon
        {
            public string Name;
            public ItemDataUnpacked ItemDataUnpacked;
            public Vector3 MapPosition;
            public uint Id;
        }

        [HarmonyPatch(typeof(GroundItemLabel), "initialise")]
        public class GroundItemLabel_
        {
            [HarmonyPostfix]
            public static void Postfix(GroundItemLabel __instance, ref GroundItemVisuals __0)
            {
                if ((__0.label.emphasized) && (__0.itemData.rarity < 7))
                {
                    //__0.itemData

                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    Properties.Resources.STAR_UNIQUE.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    Texture2D icon = new Texture2D(1, 1);
                    ImageConversion.LoadImage(icon, stream.ToArray(), true);
                    Sprite sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);

                    BaseDMMapIcon BaseMapIcon = new BaseDMMapIcon();
                    BaseMapIcon.name = __0.label.name;                    
                    BaseMapIcon.scaleWithZoom = true;
                    BaseMapIcon.icon = sprite;
                    BaseMapIcon.scaleMultiplier = 1f;
                    BaseMapIcon.rotationOffset = -90f;
                    BaseMapIcon.rotateWithMap = false;
                    //BaseMapIcon.TryCast<DMMapIcon>().mapPosition = 

                    DMMap.Instance.icons.Add(BaseMapIcon);
                }
            }
        }*/
    }
}
