using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_MainQuest
    {
        private static readonly string monolith_scene = "EoT";

        //Complete all MainQuest, Idols Reward Quest, Passices Reward Quest, Unlock all Waypoints in all era except "End of Time"
        public static void Complete()
        {
            if ((!Refs_Manager.quest_list.IsNullOrDestroyed()) &&
                (!Refs_Manager.scene_list.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_data.IsNullOrDestroyed()))
            {
                Refs_Manager.player_data.ReachedTown = true;
                Main.logger_instance.Msg("Add all Quest to player");
                foreach (Quest quest in Refs_Manager.quest_list.quests)
                {
                    if ((quest.mainLineQuest) || (quest.idolUnlockReward > 0) || (quest.passivePointsReward > 0))
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
                            LE.Data.SavedQuest q = new LE.Data.SavedQuest
                            {
                                QuestID = quest.id,
                                State = QuestState.NotStarted
                            };
                            Refs_Manager.player_data.SavedQuests.Add(q);
                            Main.logger_instance.Msg("Added to player");
                        }
                        if (state == QuestState.NotStarted)
                        {
                            try
                            {
                                quest.startQuest();
                                Main.logger_instance.Msg("Started");
                            }
                            catch { Main.logger_instance.Error("Error when trying to start quest"); }
                        }

                        if (quest.id != 148) //Escape the Draal
                        {
                            try
                            {
                                quest.completeQuest(Refs_Manager.player_actor);
                                Main.logger_instance.Msg("Completed");
                            }
                            catch { Main.logger_instance.Error("Error when trying to complete quest"); }
                        }

                        //Unlock all waypoints in this scene
                        int count = 306; //Need to be fix                        
                        for (int i = 0; i < count; i++)
                        {
                            //try
                            //{
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
                            //}
                            //catch { break; }
                        }
                    }
                }

                //Complete All quest
                /*Main.logger_instance.Msg("Complete all quest");
                Actor actor = PlayerFinder.getPlayerActor();
                if (!actor.IsNullOrDestroyed())
                {
                    Main.logger_instance.Msg("Player");
                    PlayerQuestListHolder player_component = actor.gameObject.GetComponent<PlayerQuestListHolder>();
                    if (!player_component.IsNullOrDestroyed())
                    {
                        if (!player_component.statefulQuestList.IsNullOrDestroyed())
                        {
                            if (!player_component.statefulQuestList.trackedQuests.IsNullOrDestroyed())
                            {
                                for (int i = 0; i < player_component.statefulQuestList.trackedQuests.Count; i++)
                                {
                                    StatefulQuest statefulquest = player_component.statefulQuestList.trackedQuests[i];
                                    if (!statefulquest.IsNullOrDestroyed())
                                    {
                                        try { statefulquest.quest.completeQuest(actor); Main.logger_instance.Msg("Complete quest[" + i + "] : " + statefulquest.quest.name); }
                                        catch { Main.logger_instance.Error("Complete quest[" + i + "] : " + statefulquest.quest.name); }
                                    }
                                    else { Main.logger_instance.Error("statefulquest[" + i + "] is null"); }                                    
                                }
                            }
                            else { Main.logger_instance.Error("trackedQuests is null"); }
                        }
                        else { Main.logger_instance.Error("player_component is null"); }
                    }

                    Main.logger_instance.Msg("Stash");
                    StashPlayerQuestListHolder stash_component = actor.gameObject.GetComponent<StashPlayerQuestListHolder>();
                    if (!stash_component.IsNullOrDestroyed())
                    {
                        if (!stash_component.statefulQuestList.trackedQuests.IsNullOrDestroyed())
                        {
                            for (int i = 0; i < stash_component.statefulQuestList.trackedQuests.Count; i++)
                            {
                                StatefulQuest statefulquest = stash_component.statefulQuestList.trackedQuests[i];
                                if (!statefulquest.IsNullOrDestroyed())
                                {
                                    try { statefulquest.quest.completeQuest(actor); Main.logger_instance.Msg("Complete quest[" + i + "] : " + statefulquest.quest.name); }
                                    catch { Main.logger_instance.Error("Complete quest[" + i + "] : " + statefulquest.quest.name); }
                                }
                                else { Main.logger_instance.Error("statefulquest[" + i + "] is null"); }
                            }
                        }
                        else { Main.logger_instance.Error("trackedQuests is null"); }
                    }

                    Main.logger_instance.Msg("Monolith");
                    PlayerMonolithQuestListHolder monolith_component = actor.gameObject.GetComponent<PlayerMonolithQuestListHolder>();
                    if (!monolith_component.IsNullOrDestroyed())
                    {
                        for (int i = 0; i < monolith_component.statefulQuestList.trackedQuests.Count; i++)
                        {
                            StatefulQuest statefulquest = monolith_component.statefulQuestList.trackedQuests[i];
                            if (!statefulquest.IsNullOrDestroyed())
                            {
                                try { statefulquest.quest.completeQuest(actor); Main.logger_instance.Msg("Complete quest[" + i + "] : " + statefulquest.quest.name); }
                                catch { Main.logger_instance.Error("Complete quest[" + i + "] : " + statefulquest.quest.name); }
                            }
                            else { Main.logger_instance.Error("statefulquest[" + i + "] is null"); }
                        }
                    }
                }*/

                Main.logger_instance.Msg("Unlock Portal");
                UnlockPortalInteraction.unlockPortal(Refs_Manager.player_actor.gameObject);
                bool monolith_waypoint_unlock = Refs_Manager.player_data.UnlockedWaypointScenes.Contains(monolith_scene);
                if (!monolith_waypoint_unlock) { Refs_Manager.player_data.UnlockedWaypointScenes.Add(monolith_scene); }
                
                Main.logger_instance.Msg("Level Character to 58");
                Character_Level.LevelUpToLevel(58);             

                Main.logger_instance.Msg("Save Character");
                Refs_Manager.player_data.SaveData();

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
                            Main.logger_instance.Msg("Tp to Waypoint");
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
