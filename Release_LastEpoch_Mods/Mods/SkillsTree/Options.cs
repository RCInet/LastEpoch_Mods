using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.SkillsTree
{
    public class Options
    {
        //Ref
        public static LocalTreeData Local_Tree_Data = null;
        [HarmonyPatch(typeof(LocalTreeData), "Awake")]
        public class LocalTreeData_Awake
        {
            [HarmonyPostfix]
            static void Postfix(ref LocalTreeData __instance)
            {
                Local_Tree_Data = __instance;
            }
        }

        //Specialization Slots
        [HarmonyPatch(typeof(SkillsPanelManager), "OnEnable")]
        public class SkillsPanelManager_OnEnable
        {
            [HarmonyPrefix]
            static void Prefix(ref SkillsPanelManager __instance)
            {
                if (Save_Manager.Data.UserData.Skills.SkillTree.Enable_Slots)
                {
                    SpecialisedAbilityManager.getNumberOfSpecialisationSlots((int)PlayerFinder.localPlayerLevel());

                }
            }
        }

        [HarmonyPatch(typeof(SpecialisedAbilityManager), "getNumberOfSpecialisationSlots")]
        public class SpecialisedAbilityManager_getNumberOfSpecialisationSlots
        {
            [HarmonyPrefix]
            static bool Prefix(ref byte __result, int __0)
            {
                if (Save_Manager.Data.UserData.Skills.SkillTree.Enable_Slots)
                {
                    __result = Save_Manager.Data.UserData.Skills.SkillTree.Slots;
                    return false;
                }
                else { return true; }
            }
        }   

        //Remove Nodes Req
        [HarmonyPatch(typeof(LocalTreeData), "fulfilledRequirementExists")]
        public class LocalTreeData_FulfilledRequirementExists
        {
            [HarmonyPrefix]
            static bool Prefix(LocalTreeData __instance, ref bool __result, GlobalTreeData.TreeData __0, ref GlobalTreeData.NodeData __1, LocalTreeData.TreeData __2, LocalTreeData.NodeData __3)
            {
                if (Save_Manager.Data.UserData.Skills.Disable_NodeRequirement)
                {
                    __result = true;
                    return false;
                }
                else { return true; }
            }
        }

        //Spend Skill Points
        [HarmonyPatch(typeof(LocalTreeData), "tryToSpendSkillPoint")]
        public class TryToSpendSkillPoint
        {
            private static bool Added_MaxPoint = false;
            private static byte Backup_MaxPoint = 0;

            [HarmonyPrefix]
            static void Prefix(ref LocalTreeData __instance, bool __result, Ability __0, byte __1)
            {
                Added_MaxPoint = false;
                if ((!__result) && (Save_Manager.Data.UserData.Skills.Disable_NodeRequirement))
                {
                    foreach (LocalTreeData.SkillTreeData tree in __instance.specialisedSkillTrees)
                    {
                        if (tree.ability == __0)
                        {
                            Added_MaxPoint = true;
                            Backup_MaxPoint = tree.additionalMaxPointsFromStats;
                            tree.additionalMaxPointsFromStats = 255;
                            break;
                        }
                    }
                }
            }
            [HarmonyPostfix]
            static void Postfix(ref LocalTreeData __instance, bool __result, Ability __0, byte __1)
            {
                if (Added_MaxPoint)
                {
                    foreach (LocalTreeData.SkillTreeData tree in __instance.specialisedSkillTrees)
                    {
                        if (tree.ability == __0)
                        {
                            tree.additionalMaxPointsFromStats = Backup_MaxPoint;
                            Added_MaxPoint = false;
                            break;
                        }
                    }
                }
            }
        }

        //Set Skill Level
        [HarmonyPatch(typeof(SkillsPanelManager), "openSkillTree")]
        public class SkillsPanelManager_OpenSkillTree
        {
            [HarmonyPrefix]
            static void Prefix(ref SkillsPanelManager __instance, Ability __0)
            {
                if (Save_Manager.Data.UserData.Skills.SkillTree.Enable_Level)
                {
                    foreach (LocalTreeData.SkillTreeData skill_tree_data in Local_Tree_Data.specialisedSkillTrees)
                    {
                        if (skill_tree_data.ability.abilityName == __0.abilityName)
                        {
                            skill_tree_data.level = Save_Manager.Data.UserData.Skills.SkillTree.Level;
                            break;
                        }
                    }
                }
                __instance.updateVisuals();
            }
        }

        //Set Passive Points        
        [HarmonyPatch(typeof(PassivePanelManager), "onTreeOpened")]
        public class PassivePanelManager_onTreeOpened
        {
            [HarmonyPrefix]
            static void Prefix(PassivePanelManager __instance, CharacterClass __0, byte __1)
            {
                if (Save_Manager.Data.UserData.Skills.PassiveTree.Enable_PointsEarnt)
                {
                    Local_Tree_Data.passiveTree.pointsEarnt = Save_Manager.Data.UserData.Skills.PassiveTree.PointsEarnt;
                }
            }
        }

        //Spend Passive Points
        [HarmonyPatch(typeof(LocalTreeData), "tryToSpendPassivePoint")]
        public class LocalTreeData_TryToSpendPassivePoint
        {
            private static bool Added_Level = false;
            private static UnhollowerBaseLib.Il2CppStructArray<byte> backup_masteries_level = new UnhollowerBaseLib.Il2CppStructArray<byte>(4);
            [HarmonyPrefix]
            static void Prefix(LocalTreeData __instance, ref bool __result, CharacterClass __0, byte __1)
            {
                Added_Level = false;
                if (Scenes_Manager.GameScene())
                {
                    backup_masteries_level = __instance.masteryLevels;
                    if ((!__result) && (Save_Manager.Data.UserData.Skills.Disable_NodeRequirement))
                    {
                        UnhollowerBaseLib.Il2CppStructArray<byte> results = new UnhollowerBaseLib.Il2CppStructArray<byte>(4);
                        for (int i = 0; i < results.Length; i++)
                        {
                            results[i] = 255;
                        }
                        __instance.masteryLevels = results;
                        Added_Level = true;
                    }
                }
            }
            [HarmonyPostfix]
            static void Postfix(ref LocalTreeData __instance, ref bool __result, CharacterClass __0, byte __1)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (Added_Level) { __instance.masteryLevels = backup_masteries_level; }
                }
            }
        }

        //AbilityStats
        [HarmonyPatch(typeof(AbilityStatsMutatorManager), "OnStatsUpdate")]
        public class AbilityStatsMutatorManager_OnStatsUpdate
        {
            [HarmonyPostfix]
            static void Postfix(ref AbilityStatsMutatorManager __instance)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (Save_Manager.Data.UserData.Skills.Companion.Wolf.Enable_SummonMax) { __instance.canSummonWolvesUpToMaxCompanions = true; }
                    if (Save_Manager.Data.UserData.Skills.Minions.BoneGolems.Enable_addedGolemsPer4Skeletons)
                    {
                        __instance.addedGolemsPer4Skeletons = Save_Manager.Data.UserData.Skills.Minions.BoneGolems.addedGolemsPer4Skeletons;
                    }
                    if (Save_Manager.Data.UserData.Skills.Companion.Wolf.Enable_StunImmunity) { __instance.wolfStunImmunity = true; }
                    else { __instance.wolfStunImmunity = false; }
                }
            }
        }

        //Unlock Skills
        [HarmonyPatch(typeof(CharacterClass), "getLevelUnlockedBaseClassAbilities")]
        public class CharacterClass_getLevelUnlockedBaseClassAbilities
        {
            [HarmonyPrefix]
            static void Prefix(ref CharacterClass __instance, Il2CppSystem.Collections.Generic.List<Ability> __result)
            {
                if (Save_Manager.Data.UserData.Skills.Enable_AllSkills)
                {
                    foreach (var a in __instance.unlockableAbilities) { a.level = 0; }
                    foreach (Mastery m in __instance.masteries)
                    {
                        foreach (var b in m.abilities) { b.level = 0; }
                    }
                }
            }
        }
          
        /*[HarmonyPatch(typeof(CharacterClass), "isBasePassiveAbilityUnlocked")]
        public class CharacterClass_isBasePassiveAbilityUnlocked
        {
            [HarmonyPrefix]
            static void Prefix(CharacterClass __instance, ref bool __result, Ability __0, ref byte __1)
            {
                if (Save_Manager.Data.UserData.Skills.Enable_AllSkills)
                {
                    //__1 = 100;
                    foreach (var a in __instance.unlockableAbilities) { a.level = 0; }
                    foreach (Mastery m in __instance.masteries)
                    {
                        foreach (var b in m.abilities) { b.level = 0; }
                    }
                }
            }
        }*/

        /*[HarmonyPatch(typeof(CharacterClass), "isLevelAbilityUnlocked")]
        public class CharacterClass_isLevelAbilityUnlocked
        {
            [HarmonyPrefix]
            static void Prefix(CharacterClass __instance, ref bool __result, Ability __0, ref int __1)
            {                
                if (Save_Manager.Data.UserData.Skills.Enable_AllSkills)
                {
                    __1 = 100;
                }
            }
        }*/

        /*[HarmonyPatch(typeof(CharacterClass), "getUnlockedAbilities")]
        public class CharacterClass_getUnlockedAbilities
        {
            [HarmonyPrefix]
            static bool Prefix(CharacterClass __instance, ref Il2CppSystem.Collections.Generic.List<Ability> __result, int __0, UnhollowerBaseLib.Il2CppStructArray<byte> __1, byte __2)
            {
                if (Save_Manager.Data.UserData.Skills.Enable_AllSkills)
                {
                    Il2CppSystem.Collections.Generic.List<Ability> abilities = new Il2CppSystem.Collections.Generic.List<Ability>();
                    foreach (AbilityAndLevel ab_level in __instance.unlockableAbilities)
                    {
                        abilities.Add(ab_level.ability);
                    }
                    foreach (Mastery masterie in __instance.masteries)
                    {
                        foreach (AbilityAndLevel ab_level in masterie.abilities)
                        {
                            abilities.Add(ab_level.ability);
                        }
                        if (masterie.masteryAbility != null)
                        {
                            abilities.Add(masterie.masteryAbility);
                        }
                    }

                    return false;
                }
                else { return true; }
            }
        }        
        */

        /*[HarmonyPatch(typeof(CharacterClass), "getUnlockedMasteryAbilities")]
        public class CharacterClass_getUnlockedMasteryAbilities
        {
            [HarmonyPrefix]
            static void Prefix(CharacterClass __instance, Il2CppSystem.Collections.Generic.List<Ability> __result, ref byte __0, ref byte __1, ref byte __2)
            {
                if (Save_Manager.Data.UserData.Skills.Enable_AllSkills)
                {
                    __1 = 100;
                    __2 = __0;
                }
            }
        }*/

        /*[HarmonyPatch(typeof(CharacterClass), "getLockedMasteryAbilities")]
        public class CharacterClass_getLockedMasteryAbilities
        {
            [HarmonyPrefix]
            static void Prefix(CharacterClass __instance, ref Il2CppSystem.Collections.Generic.List<Ability> __result, ref byte __0, ref byte __1, ref byte __2)
            {
                if (Save_Manager.Data.UserData.Skills.Enable_AllSkills)
                {
                    __1 = 100;
                    __2 = __0;
                }
            }
        }*/

        /*[HarmonyLib.HarmonyPatch(typeof(AbilityPanelIcon), "Init")]
        public class AbilityPanelIcon_Init
        {
            [HarmonyLib.HarmonyPostfix]
            static void Postfix(ref AbilityPanelIcon __instance, Ability __0, AbilityPanelIcon.AbilityUnlockType __1, bool __2, string __3)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (Save_Manager.Data.UserData.Skills.Enable_AllSkills)
                    {
                        __instance.SetAbilityLockedState(false);
                    }
                    else if (Local_Tree_Data != null)
                    {
                        try
                        {
                            string level_str = __instance.UnlockReqTMP.text;
                            int level = 0;
                            if (level_str != "") { level = Il2CppSystem.Convert.ToInt32(level_str); }
                            int masterie = Local_Tree_Data.chosenMastery;
                            int masterie_level = Local_Tree_Data.masteryLevels[masterie];
                            if (__1 == AbilityPanelIcon.AbilityUnlockType.BaseClass)
                            {
                                bool skill_found = false;
                                foreach (AbilityAndLevel ab in Local_Tree_Data.characterClass.masteries[0].abilities)
                                {
                                    if (ab.ability == __0)
                                    {
                                        skill_found = true;
                                        break;
                                    }
                                }
                                if (skill_found)
                                {
                                    if (Local_Tree_Data.masteryLevels[0] >= level)
                                    {
                                        __instance.SetAbilityLockedState(false);
                                    }
                                }
                                else
                                {
                                    if (Local_Tree_Data.characterLevel >= level)
                                    {
                                        __instance.SetAbilityLockedState(false);
                                    }
                                }
                            }
                            else if (__1 == AbilityPanelIcon.AbilityUnlockType.Mastering)
                            {
                                if (Local_Tree_Data.characterClass.masteries[masterie].masteryAbility == __0)
                                {
                                    __instance.SetAbilityLockedState(false);
                                }
                            }
                            else if (__1 == AbilityPanelIcon.AbilityUnlockType.MasteryPoints)
                            {
                                foreach (AbilityAndLevel ablevel in Local_Tree_Data.characterClass.masteries[masterie].abilities)
                                {
                                    if (ablevel.ability == __0)
                                    {
                                        if (masterie_level >= level)
                                        {
                                            __instance.SetAbilityLockedState(false);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
        }*/

        //Set AbilityMutator List
        public class Ability_Mutator
        {
            private static System.Collections.Generic.List<AbilityMutator> ability_mutators = null;
            public static void Init()
            {
                ability_mutators = new System.Collections.Generic.List<AbilityMutator>();
                foreach (UnityEngine.Object obj in UnityEngine.Object.FindObjectsOfType<AbilityMutator>())
                {
                    ability_mutators.Add(obj.TryCast<AbilityMutator>());
                }
            }
            public static AbilityMutator GetMutatorFromAbility(Ability ability)
            {
                AbilityMutator mutator = new AbilityMutator();
                bool found = false;
                foreach (AbilityMutator obj in ability_mutators)
                {
                    if (obj.ability == ability)
                    {
                        mutator = obj;
                        found = true;
                        break;
                    }
                }
                if ((!found) && (ability.abilityName != "Attack"))
                {
                    Main.logger_instance.Msg("Ability Mutator Not Found for this Ability : " + ability.abilityName);
                }

                return mutator;
            }
        }

        //Max Companions
        [HarmonyPatch(typeof(CharacterStats), "getMaximumCompanions")]
        public class CharacterStats_GetMaximumCompanions
        {
            [HarmonyPostfix]
            static void Postfix(CharacterStats __instance, ref int __result)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (Save_Manager.Data.UserData.Skills.Companion.Enable_Limit)
                    {
                        __result = Save_Manager.Data.UserData.Skills.Companion.Limit;
                    }
                }
            }
        }

        //Ability Use
        private static Ability using_ability = null;
        [HarmonyPatch(typeof(CharacterMutator), "OnStartedUsingAbility")]
        public class OnStartedUsingAbility
        {
            [HarmonyPrefix]
            static void Prefix(CharacterMutator __instance, AbilityInfo __0, ref Ability __1, UnityEngine.Vector3 __2)
            {
                try
                {
                    if (Scenes_Manager.GameScene())
                    {
                        if (__1 != null)
                        {
                            using_ability = __1;
                            
                            //Main.logger_instance.Msg("OnStartedUsingAbility : Ability = " + __1.abilityName);
                            
                            if (Save_Manager.Data.UserData.Skills.Enable_RemoveChannelCost) { __1.channelCost = 0f; }
                            if (Save_Manager.Data.UserData.Skills.Enable_RemoveManaCost)
                            {
                                __1.manaCost = 0f;
                                __1.minimumManaCost = 0f;
                                __1.manaCostPerDistance = 0f;
                            }
                            if (Save_Manager.Data.UserData.Skills.Enable_NoManaRegenWhileChanneling) { __1.noManaRegenWhileChanneling = false; }
                            if (Save_Manager.Data.UserData.Skills.Enable_StopWhenOutOfMana) { __1.stopWhenOutOfMana = false; }
                            //if (Config.Data.mods_config.character.characterstats.Enable_attack_rate) { __1.speedMultiplier = Config.Data.mods_config.character.characterstats.attack_rate; }
                            if ((__1.moveOrAttackCompatible) && (__1.moveOrAttackFallback == Ability.MoveOrAttackFallback.Move))
                            {
                                if (Save_Manager.Data.UserData.Skills.MovementSkills.Enable_NoTarget)
                                {
                                    __1.playerRequiresTarget = false;
                                    __1.requiredEnemyTargetMustBeAccessible = false;
                                }
                                if (Save_Manager.Data.UserData.Skills.MovementSkills.Enable_ImmuneDuringMovement)
                                {
                                    __1.immuneDuringMovement = true;
                                    __1.cannotDieDuringMovement = true;
                                }
                                if (Save_Manager.Data.UserData.Skills.MovementSkills.Disable_SimplePath)
                                {
                                    __1.limitRangeForPlayers = false;
                                    __1.requireSimplePath = false;
                                }
                            }

                            //Get Mutator
                            try
                            {
                                AbilityMutator ability_mutator = Ability_Mutator.GetMutatorFromAbility(__1);System.Type type = ability_mutator.GetType();
                                Il2CppSystem.Type il2cpp_type = ability_mutator.GetIl2CppType();

                                //Main.logger_instance.Msg("OnStartedUsingAbility Prefix : Mutator Type = " + il2cpp_type.ToString());

                                //Warpath : Fix Channel Cost
                                if (il2cpp_type.ToString() == "WarpathMutator")
                                {
                                    if (Save_Manager.Data.UserData.Skills.Enable_RemoveChannelCost)
                                    {
                                        ability_mutator.TryCast<WarpathMutator>().addedChannelCost = 0f;
                                        ability_mutator.TryCast<WarpathMutator>().addedChannelCostPerSecond = 0f;
                                    }
                                }

                                //Smite : Fix ManaCost
                                if (il2cpp_type.ToString() == "SmiteMutator")
                                {
                                    if (Save_Manager.Data.UserData.Skills.Enable_RemoveManaCost)
                                    {
                                        ability_mutator.TryCast<SmiteMutator>().addedManaCost = 0f;
                                        ability_mutator.TryCast<SmiteMutator>().increasedManaCost = 0f;
                                    }
                                }

                                //Sigil of Hope : Fix ManaCost
                                if (il2cpp_type.ToString() == "SigilsOfHopeMutator")
                                {
                                    if (Save_Manager.Data.UserData.Skills.Enable_RemoveManaCost)
                                    {
                                        ability_mutator.TryCast<SigilsOfHopeMutator>().addedManaCost = 0f;
                                        ability_mutator.TryCast<SigilsOfHopeMutator>().increasedManaCost = 0f;
                                    }
                                }

                                //Meteor
                                if (il2cpp_type.ToString() == "MeteorMutator")
                                {
                                    if (Save_Manager.Data.UserData.Skills.Enable_RemoveManaCost)
                                    {
                                        ability_mutator.TryCast<MeteorMutator>().addedManaCost = 0f;
                                    }
                                }                                

                                //Companions
                                else if (il2cpp_type.ToString() == "SummonWolfMutator")
                                {
                                    if (Save_Manager.Data.UserData.Skills.Companion.Wolf.Enable_SummonLimit)
                                    {
                                        ability_mutator.TryCast<SummonWolfMutator>().wolfLimit = Save_Manager.Data.UserData.Skills.Companion.Wolf.SummonLimit;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "SummonScorpionMutator")
                                {
                                    if (Save_Manager.Data.UserData.Skills.Companion.Scorpion.Enable_BabyQuantity)
                                    {
                                        SummonScorpionMutator mutator = ability_mutator.TryCast<SummonScorpionMutator>();
                                        mutator.babyScorpionQuantity = Save_Manager.Data.UserData.Skills.Companion.Scorpion.BabyQuantity;
                                        mutator.babyScorpionsToSpawnOnAbilityActivation = Save_Manager.Data.UserData.Skills.Companion.Scorpion.BabyQuantity;
                                        mutator.increasedBabySpawnRate = 1;
                                    }
                                }

                                //Minions
                                else if (il2cpp_type.ToString() == "SummonSkeletonMutator")
                                {
                                    if (Save_Manager.Data.UserData.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromPassives)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().additionalSkeletonsFromPassives = Save_Manager.Data.UserData.Skills.Minions.Skeletons.additionalSkeletonsFromPassives;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().onlySummonOneWarrior = false;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromSkillTree)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().additionalSkeletonsFromSkillTree = Save_Manager.Data.UserData.Skills.Minions.Skeletons.additionalSkeletonsFromSkillTree;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().onlySummonOneWarrior = false;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Skeletons.Enable_additionalSkeletonsPerCast)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().additionalSkeletonsPerCast = Save_Manager.Data.UserData.Skills.Minions.Skeletons.additionalSkeletonsPerCast;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Skeletons.Enable_chanceToResummonOnDeath)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().chanceToResummonOnDeath = Save_Manager.Data.UserData.Skills.Minions.Skeletons.chanceToResummonOnDeath;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Skeletons.Enable_forceArcher)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonArchers = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonWarriors = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().canSummonRogues = false;

                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Skeletons.Enable_forceBrawler)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonArchers = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonWarriors = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().canSummonRogues = false;

                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Skeletons.Enable_forceRogue)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonArchers = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonWarriors = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().canSummonRogues = true;

                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Skeletons.Enable_forceWarrior)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonArchers = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonWarriors = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().canSummonRogues = false;

                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = true;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "SummonWraithMutator")
                                //else if (type == typeof(SummonWraithMutator))
                                {
                                    if (Save_Manager.Data.UserData.Skills.Minions.Wraiths.Enable_additionalMaxWraiths)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().additionalMaxWraiths = Save_Manager.Data.UserData.Skills.Minions.Wraiths.additionalMaxWraiths;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Wraiths.Enable_delayedWraiths)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().delayedWraiths = Save_Manager.Data.UserData.Skills.Minions.Wraiths.delayedWraiths; //Wraiths per cast
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Wraiths.Enable_limitedTo2Wraiths)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().limitedTo2Wraiths = false;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Wraiths.Enable_wraithsDoNotDecay)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().wraithsDoNotDecay = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Wraiths.Enable_increasedCastSpeed)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().increasedCastSpeed = Save_Manager.Data.UserData.Skills.Minions.Wraiths.increasedCastSpeed;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "SummonMageMutator")
                                //else if (type == typeof(SummonMageMutator))
                                {
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_additionalSkeletonsFromItems)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsFromItems = Save_Manager.Data.UserData.Skills.Minions.Mages.additionalSkeletonsFromItems;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_additionalSkeletonsFromPassives)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsFromPassives = Save_Manager.Data.UserData.Skills.Minions.Mages.additionalSkeletonsFromPassives;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_additionalSkeletonsFromSkillTree)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsFromSkillTree = Save_Manager.Data.UserData.Skills.Minions.Mages.additionalSkeletonsFromSkillTree;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_additionalSkeletonsPerCast)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsPerCast = Save_Manager.Data.UserData.Skills.Minions.Mages.additionalSkeletonsPerCast;
                                    }
                                    //if (Config.Data.mods_config.character.minions.mage.Enable_onlySummonOneMage)
                                    //{
                                    //    ability_mutator.TryCast<SummonMageMutator>().onlySummonOneMage = false;                                
                                    //}
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_singleSummon)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().singleSummon = false;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_forceCryomancer)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().forceDeathKnight = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forcePyromancer = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceNoCryo = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceCryomancer = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_forceDeathKnight)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().forcePyromancer = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceCryomancer = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceDeathKnight = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_forcePyromancer)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().forceCryomancer = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceDeathKnight = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceNoPyro = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forcePyromancer = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_chanceForTwoExtraProjectiles)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().chanceForTwoExtraProjectiles = Save_Manager.Data.UserData.Skills.Minions.Mages.chanceForTwoExtraProjectiles;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.Mages.Enable_doubleProjectiles)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().doubleProjectiles = true;
                                    }
                                    //ability_mutator.TryCast<SummonMageMutator>().additionalWarlords = 50;
                                }
                                else if (il2cpp_type.ToString() == "SummonBoneGolemMutator")
                                //else if (type == typeof(SummonBoneGolemMutator))
                                {
                                    if (Save_Manager.Data.UserData.Skills.Minions.BoneGolems.Enable_selfResurrectChance)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().selfResurrectChance = Save_Manager.Data.UserData.Skills.Minions.BoneGolems.selfResurrectChance;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.BoneGolems.Enable_increasedFireAuraArea)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().increasedFireAuraArea = Save_Manager.Data.UserData.Skills.Minions.BoneGolems.increasedFireAuraArea;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.BoneGolems.Enable_increasedMoveSpeed)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().increasedMoveSpeed = Save_Manager.Data.UserData.Skills.Minions.BoneGolems.increasedMoveSpeed;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.BoneGolems.Enable_twins)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().twins = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.BoneGolems.Enable_hasSlamAttack)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().hasSlamAttack = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.BoneGolems.Enable_undeadArmorAura)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().undeadArmorAura = Save_Manager.Data.UserData.Skills.Minions.BoneGolems.undeadArmorAura;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.BoneGolems.Enable_undeadMovespeedAura)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().undeadMovespeedAura = Save_Manager.Data.UserData.Skills.Minions.BoneGolems.undeadMovespeedAura;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "SummonVolatileZombieMutator")
                                //else if (type == typeof(SummonVolatileZombieMutator))
                                {
                                    if (Save_Manager.Data.UserData.Skills.Minions.VolatileZombies.Enable_chanceToCastFromMinionDeath)
                                    {
                                        ability_mutator.TryCast<SummonVolatileZombieMutator>().chanceToCastFromMinionDeath = Save_Manager.Data.UserData.Skills.Minions.VolatileZombies.chanceToCastFromMinionDeath;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.VolatileZombies.Enable_chanceToCastInfernalShadeOnDeath)
                                    {
                                        ability_mutator.TryCast<SummonVolatileZombieMutator>().chanceToCastInfernalShadeOnDeath = Save_Manager.Data.UserData.Skills.Minions.VolatileZombies.chanceToCastInfernalShadeOnDeath;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.VolatileZombies.Enable_chanceToCastMarrowShardsOnDeath)
                                    {
                                        ability_mutator.TryCast<SummonVolatileZombieMutator>().chanceToCastMarrowShardsOnDeath = Save_Manager.Data.UserData.Skills.Minions.VolatileZombies.chanceToCastMarrowShardsOnDeath;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "DreadShadeMutator")
                                //else if (type == typeof(DreadShadeMutator))
                                {
                                    if (Save_Manager.Data.UserData.Skills.Minions.DreadShades.Enable_DisableLimit)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().limitTo1DreadShade = false;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.DreadShades.Enable_Duration)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().increasedDuration = Save_Manager.Data.UserData.Skills.Minions.DreadShades.Duration;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.DreadShades.Enable_DisableHealthDrain)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().noHealthDrain = true;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.DreadShades.Enable_Max)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().addedMaxShades = Save_Manager.Data.UserData.Skills.Minions.DreadShades.max;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.DreadShades.Enable_ReduceDecay)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().reducedDecayRate = Save_Manager.Data.UserData.Skills.Minions.DreadShades.decay;
                                    }
                                    if (Save_Manager.Data.UserData.Skills.Minions.DreadShades.Enable_Radius)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().increasedRadius = Save_Manager.Data.UserData.Skills.Minions.DreadShades.radius;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "FlameWardMutator")
                                {
                                    if (Save_Manager.Data.UserData.Skills.Enable_RemoveManaCost)
                                    {
                                        ability_mutator.TryCast<FlameWardMutator>().addedManaCost = 0f;
                                    }
                                }
                            }
                            catch { }
                        }
                        else { Main.logger_instance.Msg("Ability is null"); }
                    }
                }
                catch { }
            }

            [HarmonyPostfix]
            static void Postfix(CharacterMutator __instance, AbilityInfo __0, ref Ability __1, UnityEngine.Vector3 __2)
            {
                try
                {
                    if (Scenes_Manager.GameScene())
                    {
                        if (__1 != null)
                        {
                            AbilityMutator ability_mutator = Ability_Mutator.GetMutatorFromAbility(__1);
                            //Il2CppSystem.Type il2cpp_type = ability_mutator.GetIl2CppType();
                            //Main.logger_instance.Msg("OnStartedUsingAbility Postfix : Mutator Type = " + il2cpp_type.ToString());
                            
                            if (Save_Manager.Data.UserData.Skills.Enable_RemoveCooldown) { ability_mutator.RemoveCooldown(); }
                        }
                    }
                }
                catch { }
            }
        }

        [HarmonyPatch(typeof(CharacterMutator), "OnAbilityUse")]
        public class OnAbilityUse
        {
            [HarmonyPrefix]
            static void Prefix(CharacterMutator __instance, AbilityInfo __0, ref AbilityMutator __1, float __2, UnityEngine.Vector3 __3, bool __4)
            {
                if (Scenes_Manager.GameScene())
                {
                    try
                    {
                        System.Type type = __1.GetType();
                        Il2CppSystem.Type il2cpp_type = __1.GetIl2CppType();
                        //Main.logger_instance.Msg("OnAbilityUse Prefix : Mutator Type = " + il2cpp_type.ToString());

                        //Sigil of Hope : Fix ManaCost
                        if (il2cpp_type.ToString() == "HolyAuraMutator")
                        {
                            if (Save_Manager.Data.UserData.Skills.Enable_RemoveManaCost)
                            {
                                __1.TryCast<HolyAuraMutator>().ability.manaCost = 0f;
                                __1.TryCast<HolyAuraMutator>().ability.minimumManaCost = 0f;
                                __1.TryCast<HolyAuraMutator>().ability.manaCostPerDistance = 0f;
                            }
                        }
                    }
                    catch { }
                }
            }

            [HarmonyPostfix]
            static void PostFix(CharacterMutator __instance, AbilityInfo __0, ref AbilityMutator __1, float __2, UnityEngine.Vector3 __3, bool __4)
            {
                if (Scenes_Manager.GameScene())
                {
                    try
                    {
                        System.Type type = __1.GetType();
                        //Il2CppSystem.Type il2cpp_type = __1.GetIl2CppType();
                        //Main.logger_instance.Msg("OnAbilityUse PostFix : Mutator Type = " + il2cpp_type.ToString());

                        if (Save_Manager.Data.UserData.Skills.Enable_RemoveCooldown) { __1.RemoveCooldown(); }
                    }
                    catch { }
                }
            }
        }
    }
}
