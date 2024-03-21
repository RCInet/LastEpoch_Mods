using UnityEngine;

namespace LastEpochMods.Mods.Character
{
    public class Campaign
    {
        private static UIWaypointStandard GetWaypoint(string name, byte gate)
        {
            UIWaypointStandard result = null;
            foreach (UIWaypointStandard waypoint in Object.FindObjectsOfType<UIWaypointStandard>())
            {
                if ((waypoint.sceneName == name) && (waypoint.gate == gate)) { result = waypoint; break; }
            }

            return result;
        }

        public static void CompleteCampaign()
        {
            if (!QuestList.instance.IsNullOrDestroyed())
            {
                Actor player = PlayerFinder.getPlayerActor();
                LE.Data.CharacterData player_data = PlayerFinder.getPlayerData();
                try { player_data.ReachedTown = true; }
                catch { Main.logger_instance.Error("Error Setting Reached Town"); }

                string monolith_scene = "EoT";
                SceneDetails monolith_scene_details = null;
                foreach (Quest quest in QuestList.instance.quests)
                {
                    if(quest.mainLineQuest)
                    {
                        QuestState state = QuestState.NotStarted;
                        foreach (LE.Data.SavedQuest saved_quest in player_data.SavedQuests)
                        {
                            if (saved_quest.QuestID == quest.id) { state = saved_quest.State; break; }
                        }
                        try { if (state == QuestState.NotStarted) { quest.startQuest(); } }
                        catch { Main.logger_instance.Error("Error Starting Quest : Id = " + quest.id + ", DisplayName = " + quest.displayName); }
                        try { if (state != QuestState.Completed) { quest.completeQuest(player); } }
                        catch { Main.logger_instance.Error("Error Completing Quest : Id = " + quest.id + ", DisplayName = " + quest.displayName); }                        
                        try
                        {
                            if (!SceneList.instance.IsNullOrDestroyed())
                            {
                                int count = 306;
                                for (int i = 0; i < count;  i++)
                                {
                                    try
                                    {
                                        SceneDetails scene_details = SceneList.instance.scenes[i];
                                        if (scene_details.chapter == quest.chapter)
                                        {
                                            bool already = player_data.UnlockedWaypointScenes.Contains(scene_details.Name);
                                            if (!already)
                                            {
                                                player_data.UnlockedWaypointScenes.Add(scene_details.Name);
                                            }
                                        }
                                        if (scene_details.Name == monolith_scene) { monolith_scene_details = scene_details; }
                                    }
                                    catch { break; }                                    
                                }
                            }
                        }
                        catch { Main.logger_instance.Error("Error Unlocking Waypoints"); }
                    }
                }
                UnlockPortalInteraction.unlockPortal(player.gameObject);                
                bool monolith_waypoint_unlock = player_data.UnlockedWaypointScenes.Contains(monolith_scene);
                if (!monolith_waypoint_unlock) { player_data.UnlockedWaypointScenes.Add(monolith_scene); }                
                Cheats.LevelUp.ToLevel(58); //Level up to 58                
                player_data.SaveData(); //Save Player Data
                //Tp to Monolith
                if (!Managers.GUI_Manager.Base.Refs.Game_UIBase.IsNullOrDestroyed())
                {
                    Managers.GUI_Manager.Base.Refs.Game_UIBase.openMap();
                    if (!Managers.GUI_Manager.Base.Refs.Game_UIBase.map.instance.IsNullOrDestroyed())
                    {
                        MapPanel map_panel = Managers.GUI_Manager.Base.Refs.Game_UIBase.map.instance.GetComponent<MapPanel>();
                        map_panel.OpenEra(map_panel.eras[map_panel.eras.Count - 1].era, false);
                        UIWaypointStandard waypoint = GetWaypoint(monolith_scene, 0);
                        if (waypoint == null) { Main.logger_instance.Error("Waypoint not found for " + monolith_scene + " scene"); }
                        else { waypoint.LoadWaypointScene(); }
                    }
                    else { Main.logger_instance.Error("Map instance is null"); }
                }
            }
        }
    }
}
