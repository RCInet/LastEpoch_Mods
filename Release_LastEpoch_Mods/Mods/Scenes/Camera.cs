using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Scenes
{
    public class Camera
    {
        public static void LoadOnStart()
        {
            if (Save_Manager.Data.UserData.Scene.Camera.Enable_OnLoad)
            {
                if (!CameraManager.instance.IsNullOrDestroyed()) { Update(); }
            }
        }
        public static void Update()
        {
            if (!CameraManager.instance.IsNullOrDestroyed())
            {
                CameraManager.instance.zoomMin = Save_Manager.Data.UserData.Scene.Camera.ZoomMin;
                CameraManager.instance.zoomPerScroll = Save_Manager.Data.UserData.Scene.Camera.ZoomPerScroll;
                CameraManager.instance.zoomSpeed = Save_Manager.Data.UserData.Scene.Camera.ZoomSpeed;
                CameraManager.instance.cameraRotationDefault = Save_Manager.Data.UserData.Scene.Camera.Rotation;
                CameraManager.instance.offsetMin = Save_Manager.Data.UserData.Scene.Camera.OffsetMin;
                CameraManager.instance.offsetMax = Save_Manager.Data.UserData.Scene.Camera.OffsetMax;
                CameraManager.instance.cameraAngleMin = Save_Manager.Data.UserData.Scene.Camera.AngleMin;
                CameraManager.instance.cameraAngleMax = Save_Manager.Data.UserData.Scene.Camera.AngleMax;
            }
        }
        public static void ResetToDefault()
        {
            Save_Manager.Data.UserData.Scene.Camera.ZoomMin = -7f;
            Save_Manager.Data.UserData.Scene.Camera.ZoomPerScroll = 0.15f;
            Save_Manager.Data.UserData.Scene.Camera.ZoomSpeed = 2.5f;
            Save_Manager.Data.UserData.Scene.Camera.Rotation = 95f;
            Save_Manager.Data.UserData.Scene.Camera.OffsetMin = -1f;
            Save_Manager.Data.UserData.Scene.Camera.OffsetMax = 0.31f;
            Save_Manager.Data.UserData.Scene.Camera.AngleMin = 35f;
            Save_Manager.Data.UserData.Scene.Camera.AngleMax = 49f;
            Save_Manager.Save.Mods();
            Update();
        }
    }
}
