using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_TpSafe : MonoBehaviour
    {
        //You have to unlock Portal first to be able to use this

        bool mod_enable = true;                 //Set here if you want this mod to start
        KeyCode key_0 = KeyCode.LeftControl;    //Left Ctrl
        KeyCode key_1 = KeyCode.Q;              //Q
        //Era can be edit line 52, change "map_panel.eras.Count - 1" to the desired era, default = latest era for monoliths 
        string tp_waypoint = "EoT";            //Monolith Waypoint
        byte tp_gate = 0;

        public static Character_TpSafe instance { get; private set; }
        public Character_TpSafe(System.IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if (CanRun())
            {
                if ((Input.GetKey(key_0)) && (Input.GetKey(key_1))) { TpSafe(); }
            }
        }
        bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed())  &&
                (!Refs_Manager.game_uibase.IsNullOrDestroyed()) && (mod_enable))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed()) { return true; }
                else { return false; }
            }
            else { return false; }
        }
        void TpSafe()
        {
            bool backup_godmode = Save_Manager.instance.data.Character.Cheats.Enable_GodMode;
            Save_Manager.instance.data.Character.Cheats.Enable_GodMode = true;
            if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
            {
                Refs_Manager.game_uibase.openMap();
                if (!Refs_Manager.game_uibase.map.instance.IsNullOrDestroyed())
                {
                    MapPanel map_panel = Refs_Manager.game_uibase.map.instance.GetComponent<MapPanel>();
                    map_panel.OpenEra(map_panel.eras[map_panel.eras.Count - 1].era, false);
                    UIWaypointStandard waypoint = GetWaypoint(tp_waypoint, tp_gate); //Monolith
                    if (waypoint != null)
                    {
                        bool backup_unlock_waypoint = Save_Manager.instance.data.Character.Cheats.Enable_WaypointsUnlock;
                        Save_Manager.instance.data.Character.Cheats.Enable_WaypointsUnlock = true;
                        waypoint.LoadWaypointScene();
                        Save_Manager.instance.data.Character.Cheats.Enable_WaypointsUnlock = backup_unlock_waypoint;
                    }
                    else { Main.logger_instance.Error("Waypoint is null"); }
                }
                else { Main.logger_instance.Error("Map instance is null"); }
            }
            Save_Manager.instance.data.Character.Cheats.Enable_GodMode = backup_godmode;
        }
        UIWaypointStandard GetWaypoint(string name, byte gate)
        {
            UIWaypointStandard result = null;
            foreach (UIWaypointStandard waypoint in Object.FindObjectsOfType<UIWaypointStandard>())
            {
                if ((waypoint.sceneName == name) && (waypoint.gate == gate)) { result = waypoint; break; }
            }

            return result;
        }
    }
}
