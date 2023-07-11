using HarmonyLib;
using UniverseLib;

namespace LastEpochMods.Hooks
{
    public class Character_Mutator
    {
        [HarmonyPatch(typeof(CharacterMutator), "OnAbilityUse")]
        public class OnAbilityUse
        {
            [HarmonyPrefix]
            static bool Prefix(CharacterMutator __instance, AbilityInfo __0, ref AbilityMutator __1, ref float __2, UnityEngine.Vector3 __3, bool __4)
            {
                if (__0 != null)
                {
                    Ability ability = __0.getAbility();
                    if (Config.Data.mods_config.character.skills.Enable_channel_cost) { ability.channelCost = 0f; }
                    if (Config.Data.mods_config.character.skills.Enable_manaCost)
                    {
                        ability.manaCost = 0f;
                        ability.minimumManaCost = 0f;
                        ability.manaCostPerDistance = 0f;
                    }
                    if (Config.Data.mods_config.character.skills.Enable_noManaRegenWhileChanneling) { ability.noManaRegenWhileChanneling = false; }
                    if (Config.Data.mods_config.character.skills.Enable_stopWhenOutOfMana) { ability.stopWhenOutOfMana = false; }                    
                }
                if (__1 != null)
                {                    
                    if (Config.Data.mods_config.character.skills.Enable_RemoveCooldown) { __1.RemoveCooldown(); }

                    //Companions
                    System.Type type = __1.GetActualType();
                    if (type ==  typeof(SummonWolfMutator))
                    {
                        if (Config.Data.mods_config.character.companions.wolf.Enable_override_limit)
                        {
                            __1.TryCast<SummonWolfMutator>().wolfLimit = Config.Data.mods_config.character.companions.wolf.summon_limit;
                        }
                    }                    
                    else if (type == typeof(SummonScorpionMutator))
                    {
                        if (Config.Data.mods_config.character.companions.scorpion.Enable_baby_quantity)
                        {
                            SummonScorpionMutator mutator = __1.TryCast<SummonScorpionMutator>();
                            mutator.babyScorpionQuantity = Config.Data.mods_config.character.companions.scorpion.baby_quantity;
                            mutator.babyScorpionsToSpawnOnAbilityActivation = Config.Data.mods_config.character.companions.scorpion.baby_quantity;
                            mutator.increasedBabySpawnRate = 1;
                        }
                    }
                    /*else if(type == typeof(SummonRaptorMutator))
                    {
                        
                    }
                    else if (type == typeof(SummonBearMutator))
                    {

                    }
                    else if (type == typeof(SummonSabertoothMutator))
                    {
                        __1.TryCast<SummonSabertoothMutator>().quantityLimit = 100;
                    }*/
                }

                return true;
            }
        }

    }
}
