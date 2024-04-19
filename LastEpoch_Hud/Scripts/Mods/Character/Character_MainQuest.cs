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
                SceneDetails monolith_scene_details = null;
                foreach (Quest quest in Refs_Manager.quest_list.quests)
                {
                    if (quest.mainLineQuest)
                    {
                        QuestState state = QuestState.NotStarted;
                        foreach (LE.Data.SavedQuest saved_quest in Refs_Manager.player_data.SavedQuests)
                        {
                            if (saved_quest.QuestID == quest.id) { state = saved_quest.State; break; }
                        }

                        if (state == QuestState.NotStarted) { quest.startQuest(); }
                        if (state != QuestState.Completed) { quest.completeQuest(Refs_Manager.player_actor); }

                        int count = 306; //Need to be fix
                        for (int i = 0; i < count; i++)
                        {
                            SceneDetails scene_details = null;
                            try { scene_details = Refs_Manager.scene_list.scenes[i]; }
                            catch { break; }

                            if (scene_details != null)
                            {
                                if (Refs_Manager.scene_list.scenes[i].chapter == quest.chapter)
                                {
                                    bool already = Refs_Manager.player_data.UnlockedWaypointScenes.Contains(Refs_Manager.scene_list.scenes[i].Name);
                                    if (!already)
                                    {
                                        Refs_Manager.player_data.UnlockedWaypointScenes.Add(Refs_Manager.scene_list.scenes[i].Name);
                                    }
                                }
                                if (Refs_Manager.scene_list.scenes[i].Name == monolith_scene) { monolith_scene_details = Refs_Manager.scene_list.scenes[i]; }
                            }
                        }
                    }
                }
                UnlockPortalInteraction.unlockPortal(Refs_Manager.player_actor.gameObject);
                bool monolith_waypoint_unlock = Refs_Manager.player_data.UnlockedWaypointScenes.Contains(monolith_scene);
                if (!monolith_waypoint_unlock) { Refs_Manager.player_data.UnlockedWaypointScenes.Add(monolith_scene); }
                Character_Level.LevelUpToLevel(58); //Level up to 58                
                Refs_Manager.player_data.SaveData(); //Save Player Data
                                                     //Tp to Monolith
                                                     //Hud_Base.Resume_Click(); //Close Hud
                if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
                {
                    Refs_Manager.game_uibase.openMap();
                    if (!Refs_Manager.game_uibase.map.instance.IsNullOrDestroyed())
                    {
                        MapPanel map_panel = Refs_Manager.game_uibase.map.instance.GetComponent<MapPanel>();
                        map_panel.OpenEra(map_panel.eras[map_panel.eras.Count - 1].era, false);
                        UIWaypointStandard waypoint = GetWaypoint(monolith_scene, 0);
                        if (waypoint == null) { Main.logger_instance.Error("Waypoint not found for " + monolith_scene + " scene"); }
                        else { waypoint.LoadWaypointScene(); }
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
