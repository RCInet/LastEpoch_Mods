using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    if (Config.Data.mods_config.character.Enable_channel_cost) { ability.channelCost = 0f; }
                    if (Config.Data.mods_config.character.Enable_manaCost)
                    {
                        ability.manaCost = 0f;
                        ability.minimumManaCost = 0f;
                        ability.manaCostPerDistance = 0f;
                    }
                    if (Config.Data.mods_config.character.Enable_noManaRegenWhileChanneling) { ability.noManaRegenWhileChanneling = false; }
                    if (Config.Data.mods_config.character.Enable_stopWhenOutOfMana) { ability.stopWhenOutOfMana = false; }
                    if (ability.companion)
                    {
                        if (Config.Data.mods_config.character.Enable_RemoveCooldown)
                        {
                            //Get companionmutator and remove cooldown here
                        }
                    }
                }
                if (__1 != null) //Companion ability don't have AbilityMutator
                {
                    if (Config.Data.mods_config.character.Enable_RemoveCooldown) { __1.RemoveCooldown(); }
                }

                return true;
            }
        }

    }
}
