using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Monoliths
{
    public class Monoliths_BlessingSlots
    {
        //public static MonolithRun Run = null;
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Scenes.Monoliths.Enable_BlessingSlots;
                }
                else { return false; }
            }
            else { return false; }
        }

        //Fix by Silver-D for LastEpoch v1.1.2 (see issue "Setting Blessings to 5 results in Null Reference Exception on reward panel")
        [HarmonyPatch(typeof(LE.Gameplay.Monolith.MonolithUIManager), "OpenBlessingsRewardPanelAfterDelay")]
        public class MonolithUIManager_OpenBlessingsRewardPanelAfterDelay
        {
            [HarmonyPrefix]
            static void Prefix(Cysharp.Threading.Tasks.UniTaskVoid __result, TimelineID __0, int __1, float __2, ref int __3)
            {
                if (CanRun())
                {
                    int blessingSlots = (int)Save_Manager.instance.data.Scenes.Monoliths.BlessingSlots;
                    if (__3 > blessingSlots) { blessingSlots = __3; } // in case we unlocked more slots due to difficulty/corruption, but we chose a lesser value for a minimum
                    __3 = blessingSlots;
                }
            }
        }

        [HarmonyPatch(typeof(ItemContainersManager), "populateBlessingOptions")]
        public class ItemContainersManager_populateBlessingOptions
        {
            [HarmonyPrefix]
            static void Prefix(ref ItemContainersManager __instance, TimelineID __0, int __1, ref int __2, ref int __3)
            {
                if (CanRun())
                {
                    int numStandardBlessings = 3;
                    int numExtraBlessings = (int)(Save_Manager.instance.data.Scenes.Monoliths.BlessingSlots - numStandardBlessings);
                    if (__3 > numExtraBlessings) { numExtraBlessings = __3; } // in case we unlocked more slots due to difficulty/corruption, but we chose a lesser value for a minimum
                    __2 = numStandardBlessings;
                    __3 = numExtraBlessings;
                }
            }
        }

        //Old code
        /*private static System.Collections.Generic.List<byte> GetAllTimelineBlessings(TimelineID timeline_id, int difficulty)
        {
            System.Collections.Generic.List<byte> result = new System.Collections.Generic.List<byte>();
            if (Run != null)
            {
                foreach (System.Int32 b in Run.timeline.difficulties[difficulty].anySlotBlessings) { result.Add((byte)b); }
                foreach (System.Int32 b in Run.timeline.difficulties[difficulty].otherSlotBlessings) { result.Add((byte)b); }
            }

            return result;
        }*/

        /*[HarmonyPatch(typeof(MonolithRun), "calculateIncreasedRarityAndExperienceFromMods")]
        public class MonolithRun_calculateIncreasedRarityAndExperienceFromMods
        {
            [HarmonyPostfix]
            static void Postfix(ref MonolithRun __instance)
            {
                Run = __instance;
            }
        }*/

        /*[HarmonyPatch(typeof(BlessingRewardPanelManager), "OnOptionsPopulated")]
        public class BlessingRewardPanelManager_OnOptionsPopulated
        {
            [HarmonyPostfix]
            static void Postfix(ref BlessingRewardPanelManager __instance, TimelineID __0, int __1, int __2)
            {
                if (CanRun())
                {
                    System.Collections.Generic.List<byte> available_blessings = GetAllTimelineBlessings(__0, __1);
                    if (available_blessings.Count > 0)
                    {
                        System.Collections.Generic.List<byte> already_list = new System.Collections.Generic.List<byte>();
                        foreach (BlessingOptionContainerUI b_container in __instance.blessingOptions)
                        {
                            if (b_container.container.HasContent())
                            {
                                already_list.Add(b_container.container.GetContent()[0].data.id[2]);
                            }
                            else
                            {
                                System.Collections.Generic.List<byte> new_list = new System.Collections.Generic.List<byte>();
                                foreach (byte available in available_blessings)
                                {
                                    bool found = false;
                                    foreach (byte already in already_list)
                                    {
                                        if (available == already)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found) { new_list.Add(available); }
                                }
                                byte new_blessing_id = new_list[UnityEngine.Random.Range(0, new_list.Count - 1)];
                                b_container.container.TryAddItem(PlayerFinder.getPlayerActor().generateItems.initialiseRandomItemData(false, 100, false, ItemLocationTag.None, 34, new_blessing_id, 0, 0, 0, false, 0), 1, Context.DEFAULT);
                                already_list.Add(new_blessing_id);
                            }
                        }
                    }
                }
            }
        }*/
    }
}
