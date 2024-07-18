using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_MainQuest
    {
        public static void Complete()
        {
            if ((!Refs_Manager.quest_list.IsNullOrDestroyed()) &&
                (!Refs_Manager.scene_list.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_data.IsNullOrDestroyed()))
            {
                Refs_Manager.player_data.ReachedTown = true;
                string monolith_scene = "EoT";
                //SceneDetails monolith_scene_details = null;

                Main.logger_instance.Msg("Add all Quest to player");
                foreach (Quest quest in Refs_Manager.quest_list.quests)
                {
                    if (quest.mainLineQuest)
                    {
                        
                        QuestState state = QuestState.NotStarted;
                        bool found = false;
                        foreach (LE.Data.SavedQuest saved_quest in Refs_Manager.player_data.SavedQuests)
                        {
                            if (saved_quest.QuestID == quest.id)
                            {
                                state = saved_quest.State;
                                found = true;
                                break;
                            }
                        }
                        Main.logger_instance.Msg("Quest Name = " + quest.name + " Id = " + quest.id + ", Type = " + quest.questType.ToString() + ", State = " + state.ToString() + ", Chapter = " + quest.chapter);
                        if (!found)
                        {
                            Main.logger_instance.Msg("Quest Added to player");
                            LE.Data.SavedQuest q = new LE.Data.SavedQuest
                            {
                                QuestID = quest.id,
                                State = QuestState.NotStarted
                            };
                            Refs_Manager.player_data.SavedQuests.Add(q);
                        }
                        if (state == QuestState.NotStarted)
                        {
                            Main.logger_instance.Msg("Start Quest");
                            try { quest.startQuest(); }
                            catch { Main.logger_instance.Error("Error when trying to start quest"); }
                        }
                        Main.logger_instance.Msg("Complete Quest");
                        try { quest.completeQuest(Refs_Manager.player_actor); }
                        catch { Main.logger_instance.Error("Error when trying to complete quest"); }

                        //Unlock all waypoints in this scene
                        int count = 306; //Need to be fix                        
                        for (int i = 0; i < count; i++)
                        {
                            try
                            {
                                SceneDetails scene_details = Refs_Manager.scene_list.scenes[i];
                                if (scene_details != null)
                                {
                                    if (scene_details.chapter == quest.chapter)
                                    {
                                        bool already = Refs_Manager.player_data.UnlockedWaypointScenes.Contains(scene_details.Name);
                                        if (!already)
                                        {
                                            Main.logger_instance.Msg("Unlock Waypoint Scene = " + scene_details.Name);
                                            Refs_Manager.player_data.UnlockedWaypointScenes.Add(scene_details.Name);
                                        }
                                    }
                                }
                            }
                            catch { break; }
                        }
                    }
                }
                
                Main.logger_instance.Msg("Unlock Portal");
                UnlockPortalInteraction.unlockPortal(Refs_Manager.player_actor.gameObject);
                bool monolith_waypoint_unlock = Refs_Manager.player_data.UnlockedWaypointScenes.Contains(monolith_scene);
                if (!monolith_waypoint_unlock) { Refs_Manager.player_data.UnlockedWaypointScenes.Add(monolith_scene); }
                
                Main.logger_instance.Msg("Level Character to 58");
                Character_Level.LevelUpToLevel(58); //Level up to 58                

                Main.logger_instance.Msg("Save Character");
                Refs_Manager.player_data.SaveData(); //Save Player Data
                                                     //Tp to Monolith
                                                     //Hud_Base.Resume_Click(); //Close Hud
                if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
                {
                    Main.logger_instance.Msg("Open Map");
                    Refs_Manager.game_uibase.openMap();
                    if (!Refs_Manager.game_uibase.map.instance.IsNullOrDestroyed())
                    {
                        Main.logger_instance.Msg("Open Era");
                        MapPanel map_panel = Refs_Manager.game_uibase.map.instance.GetComponent<MapPanel>();
                        map_panel.OpenEra(map_panel.eras[map_panel.eras.Count - 1].era, false);

                        Main.logger_instance.Msg("Get Waypoint");
                        UIWaypointStandard waypoint = GetWaypoint(monolith_scene, 0);
                        if (waypoint == null) { Main.logger_instance.Error("Waypoint not found for " + monolith_scene + " scene"); }
                        else
                        {
                            Main.logger_instance.Msg("Load Waypoint Scene");
                            waypoint.LoadWaypointScene();
                        }
                    }
                    else { Main.logger_instance.Error("Map instance is null"); }
                }
            }
        }
        private static UIWaypointStandard GetWaypoint(string name, byte gate)
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
