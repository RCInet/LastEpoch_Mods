using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Camera
{
    public class Camera_Override
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) &&
                (!Refs_Manager.camera_manager.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed()) { return true; }
                else { return false; }
            }
            else { return false; }
        }
        public static void Set()
        {
            if (CanRun())
            {
                if (Save_Manager.instance.data.Scenes.Camera.Enable_ZoomMinimum) { Refs_Manager.camera_manager.zoomMin = Save_Manager.instance.data.Scenes.Camera.ZoomMinimum; }
                if (Save_Manager.instance.data.Scenes.Camera.Enable_ZoomPerScroll) { Refs_Manager.camera_manager.zoomPerScroll = Save_Manager.instance.data.Scenes.Camera.ZoomPerScroll; }
                if (Save_Manager.instance.data.Scenes.Camera.Enable_ZoomSpeed) { Refs_Manager.camera_manager.zoomSpeed = Save_Manager.instance.data.Scenes.Camera.ZoomSpeed; }
                if (Save_Manager.instance.data.Scenes.Camera.Enable_DefaultRotation) { Refs_Manager.camera_manager.cameraRotationDefault = Save_Manager.instance.data.Scenes.Camera.DefaultRotation; }
                if (Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMinimum) { Refs_Manager.camera_manager.offsetMin = Save_Manager.instance.data.Scenes.Camera.OffsetMinimum; }
                if (Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMaximum) { Refs_Manager.camera_manager.offsetMax = Save_Manager.instance.data.Scenes.Camera.OffsetMaximum; }
                if (Save_Manager.instance.data.Scenes.Camera.Enable_AngleMinimum) { Refs_Manager.camera_manager.cameraAngleMin = Save_Manager.instance.data.Scenes.Camera.AngleMinimum; }
                if (Save_Manager.instance.data.Scenes.Camera.Enable_AngleMaximum) { Refs_Manager.camera_manager.cameraAngleMax = Save_Manager.instance.data.Scenes.Camera.AngleMaximum; }                    
            }
        }
        public static void ResetToDefault()
        {
            if (CanRun())
            {
                Save_Manager.instance.data.Scenes.Camera.ZoomMinimum = -7f;
                Save_Manager.instance.data.Scenes.Camera.ZoomPerScroll = 0.15f;
                Save_Manager.instance.data.Scenes.Camera.ZoomSpeed = 2.5f;
                Save_Manager.instance.data.Scenes.Camera.DefaultRotation = 95f;
                Save_Manager.instance.data.Scenes.Camera.OffsetMinimum = -1f;
                Save_Manager.instance.data.Scenes.Camera.OffsetMaximum = 0.31f;
                Save_Manager.instance.data.Scenes.Camera.AngleMinimum = 35f;
                Save_Manager.instance.data.Scenes.Camera.AngleMaximum = 49f;
                Save_Manager.instance.Save();

                //force set
                Refs_Manager.camera_manager.zoomMin = Save_Manager.instance.data.Scenes.Camera.ZoomMinimum;
                Refs_Manager.camera_manager.zoomPerScroll = Save_Manager.instance.data.Scenes.Camera.ZoomPerScroll;
                Refs_Manager.camera_manager.zoomSpeed = Save_Manager.instance.data.Scenes.Camera.ZoomSpeed;
                Refs_Manager.camera_manager.cameraRotationDefault = Save_Manager.instance.data.Scenes.Camera.DefaultRotation;
                Refs_Manager.camera_manager.offsetMin = Save_Manager.instance.data.Scenes.Camera.OffsetMinimum;
                Refs_Manager.camera_manager.offsetMax = Save_Manager.instance.data.Scenes.Camera.OffsetMaximum;
                Refs_Manager.camera_manager.cameraAngleMin = Save_Manager.instance.data.Scenes.Camera.AngleMinimum;
                Refs_Manager.camera_manager.cameraAngleMax = Save_Manager.instance.data.Scenes.Camera.AngleMaximum;                
            }
        }
    }
}
