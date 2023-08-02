using HarmonyLib;
using UniverseLib;

namespace LastEpochMods.Hooks
{
    public class Character_Mutator
    {
        [HarmonyPatch(typeof(CharacterMutator), "OnStartedUsingAbility")]
        public class OnStartedUsingAbility
        {
            [HarmonyPostfix]
            static void Postfix(CharacterMutator __instance, AbilityInfo __0, ref Ability __1, UnityEngine.Vector3 __2)
            {
                if (__1 != null)
                {
                    //Main.logger_instance.Msg("Ability name = " + __1.abilityName); //Debug
                    if (Config.Data.mods_config.character.skills.Enable_channel_cost) { __1.channelCost = 0f; }
                    if (Config.Data.mods_config.character.skills.Enable_manaCost)
                    {
                        __1.manaCost = 0f;
                        __1.minimumManaCost = 0f;
                        __1.manaCostPerDistance = 0f;
                    }
                    if (Config.Data.mods_config.character.skills.Enable_noManaRegenWhileChanneling) { __1.noManaRegenWhileChanneling = false; }
                    if (Config.Data.mods_config.character.skills.Enable_stopWhenOutOfMana) { __1.stopWhenOutOfMana = false; }
                    if (Config.Data.mods_config.character.characterstats.Enable_attack_rate) { __1.speedMultiplier = Config.Data.mods_config.character.characterstats.attack_rate; }
                    if ((__1.moveOrAttackCompatible) && (__1.moveOrAttackFallback == Ability.MoveOrAttackFallback.Move))
                    {
                        if (Config.Data.mods_config.character.skills.Movements.Enable_NoTarget)
                        {
                            __1.playerRequiresTarget = false;
                            __1.requiredEnemyTargetMustBeAccessible = false;
                        }
                        if (Config.Data.mods_config.character.skills.Movements.Enable_ImmuneDuringMovement)
                        {
                            __1.immuneDuringMovement = true;
                            __1.cannotDieDuringMovement = true;
                        }
                        if (Config.Data.mods_config.character.skills.Movements.Disable_SimplePath)
                        {
                            __1.limitRangeForPlayers = false;
                            __1.requireSimplePath = false;
                        }
                    }

                    //Get Mutator
                    AbilityMutator ability_mutator = OnSceneChanged.Ability_Mutator.GetMutatorFromAbility(__1);

                    if (Config.Data.mods_config.character.skills.Enable_RemoveCooldown) { ability_mutator.RemoveCooldown(); }
                    System.Type type = ability_mutator.GetActualType();
                    //Main.logger_instance.Msg("Mutator type = " + type.ToString()); //Debug
                    //Acolyte
                    if (type == typeof(TransplantMutator))
                    {
                        if (Config.Data.mods_config.character.skills.HealCost.Enable_Transplant)
                        {
                            ability_mutator.TryCast<TransplantMutator>().moreHealthCost = -1;
                        }
                    }
                    else if (type == typeof(MarrowShardsMutator))
                    {
                        if (Config.Data.mods_config.character.skills.HealCost.Enable_MarrowShards)
                        {
                            ability_mutator.TryCast<MarrowShardsMutator>().noHealthCost = true;
                        }
                    }
                    else if (type == typeof(ReaperFormMutator))
                    {
                        if (Config.Data.mods_config.character.skills.HealCost.Enable_ReaperForm)
                        {
                            ability_mutator.TryCast<ReaperFormMutator>().increasedHealthDrainSpeed = -255;
                        }
                    }
                    /*else if (type == typeof(ReapMutator))
                    {
                        //if (Config.Data.mods_config.character.skills.HealCost.Enable_ReaperForm)
                        //{
                            __1.TryCast<ReapMutator>().increasedMoveDistance = 5;                            
                        //}
                    }*/
                    //Companions
                    else if (type == typeof(SummonWolfMutator))
                    {
                        if (Config.Data.mods_config.character.companions.wolf.Enable_override_limit)
                        {
                            ability_mutator.TryCast<SummonWolfMutator>().wolfLimit = Config.Data.mods_config.character.companions.wolf.summon_limit;
                        }
                    }
                    else if (type == typeof(SummonScorpionMutator))
                    {
                        if (Config.Data.mods_config.character.companions.scorpion.Enable_baby_quantity)
                        {
                            SummonScorpionMutator mutator = ability_mutator.TryCast<SummonScorpionMutator>();
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

                    //Minions
                    else if (type == typeof(SummonSkeletonMutator))
                    {
                        if (Config.Data.mods_config.character.minions.skeleton.Enable_additionalSkeletonsFromPassives)
                        {
                            ability_mutator.TryCast<SummonSkeletonMutator>().additionalSkeletonsFromPassives = Config.Data.mods_config.character.minions.skeleton.additionalSkeletonsFromPassives;
                        }
                        if (Config.Data.mods_config.character.minions.skeleton.Enable_additionalSkeletonsFromSkillTree)
                        {
                            ability_mutator.TryCast<SummonSkeletonMutator>().additionalSkeletonsFromSkillTree = Config.Data.mods_config.character.minions.skeleton.additionalSkeletonsFromSkillTree;
                        }
                        if (Config.Data.mods_config.character.minions.skeleton.Enable_additionalSkeletonsPerCast)
                        {
                            ability_mutator.TryCast<SummonSkeletonMutator>().additionalSkeletonsPerCast = Config.Data.mods_config.character.minions.skeleton.additionalSkeletonsPerCast;
                        }
                        if (Config.Data.mods_config.character.minions.skeleton.Enable_chanceToResummonOnDeath)
                        {
                            ability_mutator.TryCast<SummonSkeletonMutator>().chanceToResummonOnDeath = Config.Data.mods_config.character.minions.skeleton.chanceToResummonOnDeath;
                        }
                        if (Config.Data.mods_config.character.minions.skeleton.Enable_forceArcher)
                        {
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = true;
                        }
                        if (Config.Data.mods_config.character.minions.skeleton.Enable_forceBrawler)
                        {
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = true;
                        }
                        if (Config.Data.mods_config.character.minions.skeleton.Enable_forceRogue)
                        {
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = true;
                        }
                        if (Config.Data.mods_config.character.minions.skeleton.Enable_forceWarrior)
                        {
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = false;
                            ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = true;
                        }
                    }
                    else if (type == typeof(SummonWraithMutator))
                    {
                        if (Config.Data.mods_config.character.minions.wraith.Enable_additionalMaxWraiths)
                        {
                            ability_mutator.TryCast<SummonWraithMutator>().additionalMaxWraiths = Config.Data.mods_config.character.minions.wraith.additionalMaxWraiths;
                        }
                        if (Config.Data.mods_config.character.minions.wraith.Enable_delayedWraiths)
                        {
                            ability_mutator.TryCast<SummonWraithMutator>().delayedWraiths = Config.Data.mods_config.character.minions.wraith.delayedWraiths; //Wraiths per cast
                        }
                        /*if (Config.Data.mods_config.character.minions.wraith.)
                        {
                            //__1.TryCast<SummonWraithMutator>().bloodWraithChance = 255;
                        }
                        if (Config.Data.mods_config.character.minions.wraith)
                        {
                            //__1.TryCast<SummonWraithMutator>().flameWraithChance = 255;
                        }
                        if (Config.Data.mods_config.character.minions.wraith)
                        {
                            //__1.TryCast<SummonWraithMutator>().putridWraithChance = 255;
                        }*/
                        if (Config.Data.mods_config.character.minions.wraith.Enable_limitedTo2Wraiths)
                        {
                            ability_mutator.TryCast<SummonWraithMutator>().limitedTo2Wraiths = false;
                        }
                        if (Config.Data.mods_config.character.minions.wraith.Enable_wraithsDoNotDecay)
                        {
                            ability_mutator.TryCast<SummonWraithMutator>().wraithsDoNotDecay = true;
                        }
                        if (Config.Data.mods_config.character.minions.wraith.Enable_increasedCastSpeed)
                        {
                            ability_mutator.TryCast<SummonWraithMutator>().increasedCastSpeed = Config.Data.mods_config.character.minions.wraith.increasedCastSpeed;
                        }
                    }
                    else if (type == typeof(SummonMageMutator))
                    {
                        if (Config.Data.mods_config.character.minions.mage.Enable_additionalSkeletonsFromItems)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsFromItems = Config.Data.mods_config.character.minions.mage.additionalSkeletonsFromItems;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_additionalSkeletonsFromPassives)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsFromPassives = Config.Data.mods_config.character.minions.mage.additionalSkeletonsFromPassives;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_additionalSkeletonsFromSkillTree)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsFromSkillTree = Config.Data.mods_config.character.minions.mage.additionalSkeletonsFromSkillTree;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_additionalSkeletonsPerCast)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsPerCast = Config.Data.mods_config.character.minions.mage.additionalSkeletonsPerCast;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_onlySummonOneMage)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().onlySummonOneMage = false;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_singleSummon)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().singleSummon = false;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_forceCryomancer)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().forceDeathKnight = false;
                            ability_mutator.TryCast<SummonMageMutator>().forcePyromancer = false;
                            ability_mutator.TryCast<SummonMageMutator>().forceCryomancer = true;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_forceDeathKnight)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().forcePyromancer = false;
                            ability_mutator.TryCast<SummonMageMutator>().forceCryomancer = false;
                            ability_mutator.TryCast<SummonMageMutator>().forceDeathKnight = true;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_forcePyromancer)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().forceCryomancer = false;
                            ability_mutator.TryCast<SummonMageMutator>().forceDeathKnight = false;
                            ability_mutator.TryCast<SummonMageMutator>().forcePyromancer = true;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_chanceForTwoExtraProjectiles)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().chanceForTwoExtraProjectiles = Config.Data.mods_config.character.minions.mage.chanceForTwoExtraProjectiles;
                        }
                        if (Config.Data.mods_config.character.minions.mage.Enable_doubleProjectiles)
                        {
                            ability_mutator.TryCast<SummonMageMutator>().doubleProjectiles = true;
                        }
                        //ability_mutator.TryCast<SummonMageMutator>().additionalWarlords = 50;
                    }
                    else if (type == typeof(SummonBoneGolemMutator))
                    {
                        if (Config.Data.mods_config.character.minions.bone_golem.Enable_selfResurrectChance)
                        {
                            ability_mutator.TryCast<SummonBoneGolemMutator>().selfResurrectChance = Config.Data.mods_config.character.minions.bone_golem.selfResurrectChance;
                        }
                        if (Config.Data.mods_config.character.minions.bone_golem.Enable_increasedFireAuraArea)
                        {
                            ability_mutator.TryCast<SummonBoneGolemMutator>().increasedFireAuraArea = Config.Data.mods_config.character.minions.bone_golem.increasedFireAuraArea;
                        }
                        if (Config.Data.mods_config.character.minions.bone_golem.Enable_increasedMoveSpeed)
                        {
                            ability_mutator.TryCast<SummonBoneGolemMutator>().increasedMoveSpeed = Config.Data.mods_config.character.minions.bone_golem.increasedMoveSpeed;
                        }
                        if (Config.Data.mods_config.character.minions.bone_golem.Enable_twins)
                        {
                            ability_mutator.TryCast<SummonBoneGolemMutator>().twins = true;
                        }
                        if (Config.Data.mods_config.character.minions.bone_golem.Enable_hasSlamAttack)
                        {
                            ability_mutator.TryCast<SummonBoneGolemMutator>().hasSlamAttack = true;
                        }
                        if (Config.Data.mods_config.character.minions.bone_golem.Enable_undeadArmorAura)
                        {
                            ability_mutator.TryCast<SummonBoneGolemMutator>().undeadArmorAura = Config.Data.mods_config.character.minions.bone_golem.undeadArmorAura;
                        }
                        if (Config.Data.mods_config.character.minions.bone_golem.Enable_undeadMovespeedAura)
                        {
                            ability_mutator.TryCast<SummonBoneGolemMutator>().undeadMovespeedAura = Config.Data.mods_config.character.minions.bone_golem.undeadMovespeedAura;
                        }
                    }
                    else if (type == typeof(SummonVolatileZombieMutator))
                    {
                        if (Config.Data.mods_config.character.minions.volatile_zombie.Enable_chanceToCastFromMinionDeath)
                        {
                            ability_mutator.TryCast<SummonVolatileZombieMutator>().chanceToCastFromMinionDeath = Config.Data.mods_config.character.minions.volatile_zombie.chanceToCastFromMinionDeath;
                        }
                        if (Config.Data.mods_config.character.minions.volatile_zombie.Enable_chanceToCastInfernalShadeOnDeath)
                        {
                            ability_mutator.TryCast<SummonVolatileZombieMutator>().chanceToCastInfernalShadeOnDeath = Config.Data.mods_config.character.minions.volatile_zombie.chanceToCastInfernalShadeOnDeath;
                        }
                        if (Config.Data.mods_config.character.minions.volatile_zombie.Enable_chanceToCastMarrowShardsOnDeath)
                        {
                            ability_mutator.TryCast<SummonVolatileZombieMutator>().chanceToCastMarrowShardsOnDeath = Config.Data.mods_config.character.minions.volatile_zombie.chanceToCastMarrowShardsOnDeath;
                        }
                    }
                    else if (type == typeof(DreadShadeMutator))
                    {
                        if (Config.Data.mods_config.character.minions.dread_shade.Enable_DisableLimit)
                        {
                            ability_mutator.TryCast<DreadShadeMutator>().limitTo1DreadShade = false;
                        }
                        if (Config.Data.mods_config.character.minions.dread_shade.Enable_Duration)
                        {
                            ability_mutator.TryCast<DreadShadeMutator>().increasedDuration = Config.Data.mods_config.character.minions.dread_shade.Duration;
                        }
                        if (Config.Data.mods_config.character.minions.dread_shade.Enable_DisableHealthDrain)
                        {
                            ability_mutator.TryCast<DreadShadeMutator>().noHealthDrain = true;
                        }
                        if (Config.Data.mods_config.character.minions.dread_shade.Enable_Max)
                        {
                            ability_mutator.TryCast<DreadShadeMutator>().addedMaxShades = Config.Data.mods_config.character.minions.dread_shade.max;
                        }
                        if (Config.Data.mods_config.character.minions.dread_shade.Enable_ReduceDecay)
                        {
                            ability_mutator.TryCast<DreadShadeMutator>().reducedDecayRate = Config.Data.mods_config.character.minions.dread_shade.decay;
                        }
                        if (Config.Data.mods_config.character.minions.dread_shade.Enable_Radius)
                        {
                            ability_mutator.TryCast<DreadShadeMutator>().increasedRadius = Config.Data.mods_config.character.minions.dread_shade.radius;
                        }
                    }
                }
            }
        }
    }
}
