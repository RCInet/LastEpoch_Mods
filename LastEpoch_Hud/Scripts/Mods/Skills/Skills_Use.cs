using HarmonyLib;
using Il2Cpp;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    public class Skills_Use
    {
        [HarmonyPatch(typeof(CharacterMutator), "OnAbilityUse")]
        public class OnAbilityUse
        {
            [HarmonyPrefix]
            static void Prefix(CharacterMutator __instance, AbilityInfo __0, ref AbilityMutator __1, float __2, UnityEngine.Vector3 __3, bool __4)
            {
                if ((Scenes.IsGameScene()) && (!__1.IsNullOrDestroyed()) && (!Save_Manager.instance.IsNullOrDestroyed()))
                {
                    //Ability
                    Ability ability = null;
                    try { ability = __1.getAbility(); }
                    catch { Main.logger_instance?.Error("OnAbilityUse Prefix : Can't get Ability"); }

                    if (!ability.IsNullOrDestroyed())
                    {
                        //Main.logger_instance.Msg("Ability = " + ability.abilityName);

                        if (Save_Manager.instance.data.Skills.Enable_RemoveChannelCost) { ability.channelCost = 0f; }
                        if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                        {
                            ability.manaCost = 0f;
                            ability.minimumManaCost = 0f;
                            ability.manaCostPerDistance = 0f;
                        }
                        if (Save_Manager.instance.data.Skills.Enable_NoManaRegenWhileChanneling) { ability.noManaRegenWhileChanneling = false; }
                        if (Save_Manager.instance.data.Skills.Enable_StopWhenOutOfMana) { ability.stopWhenOutOfMana = false; }
                        if ((ability.moveOrAttackCompatible) && (ability.moveOrAttackFallback == Ability.MoveOrAttackFallback.Move))
                        {
                            if (Save_Manager.instance.data.Skills.MovementSkills.Enable_NoTarget)
                            {
                                ability.playerRequiresTarget = false;
                                ability.requiredEnemyTargetMustBeAccessible = false;
                            }
                            if (Save_Manager.instance.data.Skills.MovementSkills.Enable_ImmuneDuringMovement)
                            {
                                ability.immuneDuringMovement = true;
                                ability.cannotDieDuringMovement = true;
                            }
                            if (Save_Manager.instance.data.Skills.MovementSkills.Disable_SimplePath)
                            {
                                ability.limitRangeForPlayers = false;
                                ability.requireSimplePath = false;
                            }
                        }
                    }

                    //Mutators
                    Il2CppSystem.Type il2cpp_type = null;
                    try { il2cpp_type = __1.GetIl2CppType(); }
                    catch { Main.logger_instance?.Error("OnAbilityUse Prefix : Can't get Mutator type"); }
                    
                    if (!il2cpp_type.IsNullOrDestroyed())
                    {
                        //Main.logger_instance.Msg("Mutator = " + il2cpp_type.ToString());

                        //Use Switch(il2cpp_type.ToString()) instead of if for better result (== is bad)

                        //Fireball : Fix ManaCost
                        if (il2cpp_type.ToString() == "FireballMutator")
                        {
                            var ab = __1.TryCast<FireballMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                                {
                                    ab.addedManaCost = 0f;
                                    ab.increasedManaCost = 0f;
                                }
                            }
                        }

                        //ErasingStrike : Fix ManaCost
                        if (il2cpp_type.ToString() == "ErasingStrikeMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                            {
                                var ab = __1.TryCast<ErasingStrikeMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.addedManaCost = 0f;
                                    ab.increasedManaCost = 0f;
                                    if (!ab.abyssalEchoes.IsNullOrDestroyed()) { ab.abyssalEchoes.manaCost = 0f; } //AbyssalEchoes
                                    if (!ab.erasingStrikeHit.IsNullOrDestroyed()) { ab.erasingStrikeHit.manaCost = 0f; } //Hit
                                }
                            }
                        }

                        //Holy Aura : Fix ManaCost
                        if (il2cpp_type.ToString() == "HolyAuraMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                            {
                                var ab = __1.TryCast<HolyAuraMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.ability.manaCost = 0f;
                                    ab.ability.minimumManaCost = 0f;
                                    ab.ability.manaCostPerDistance = 0f;
                                }
                            }
                        }

                        //Warpath : Fix Channel Cost
                        if (il2cpp_type.ToString() == "WarpathMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Enable_RemoveChannelCost)
                            {
                                var ab = __1.TryCast<WarpathMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.addedChannelCost = 0f;
                                    ab.addedChannelCostPerSecond = 0f;
                                }
                            }
                        }

                        //Smite : Fix ManaCost
                        if (il2cpp_type.ToString() == "SmiteMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                            {
                                var ab = __1.TryCast<SmiteMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.addedManaCost = 0f;
                                    ab.increasedManaCost = 0f;
                                }
                            }
                        }

                        //Judgement : Fix ManaCost                     
                        if (il2cpp_type.ToString() == "JudgementMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                            {
                                var ab = __1.TryCast<JudgementMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.addedManaCost = 0f;
                                    ab.increasedManaCost = 0f;
                                    ab.percentCurrentManaConsumed = 0f;
                                }
                            }
                        }
                        if (il2cpp_type.ToString() == "JudgementAoEMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                            {
                                var ab = __1.TryCast<JudgementAoEMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.increasedManaCost = 0f;
                                }
                            }
                        }

                        //Sigil of Hope : Fix ManaCost
                        if (il2cpp_type.ToString() == "SigilsOfHopeMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                            {
                                var ab = __1.TryCast<SigilsOfHopeMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.addedManaCost = 0f;
                                    ab.increasedManaCost = 0f;
                                }
                            }
                        }

                        //Meteor : Fix ManaCost
                        if (il2cpp_type.ToString() == "MeteorMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                            {
                                var ab = __1.TryCast<MeteorMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.addedManaCost = 0f;
                                }
                            }
                        }

                        //Companions
                        else if (il2cpp_type.ToString() == "SummonWolfMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Companion.Wolf.Enable_SummonLimit)
                            {
                                var ab = __1.TryCast<SummonWolfMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.wolfLimit = Save_Manager.instance.data.Skills.Companion.Wolf.SummonLimit;
                                }
                            }
                        }
                        else if (il2cpp_type.ToString() == "SummonScorpionMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Companion.Scorpion.Enable_BabyQuantity)
                            {
                                var ab = __1.TryCast<SummonScorpionMutator>();
                                if (!ab.IsNullOrDestroyed())
                                {
                                    ab.babyScorpionQuantity = Save_Manager.instance.data.Skills.Companion.Scorpion.BabyQuantity;
                                    ab.babyScorpionsToSpawnOnAbilityActivation = Save_Manager.instance.data.Skills.Companion.Scorpion.BabyQuantity;
                                    //ab.increasedBabySpawnRate = 1;
                                }
                            }
                        }

                        //Minions
                        else if (il2cpp_type.ToString() == "SummonSkeletonMutator")
                        {
                            var ab = __1.TryCast<SummonSkeletonMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromPassives)
                                {
                                    ab.additionalSkeletonsFromPassives = Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromPassives;
                                    ab.onlySummonOneWarrior = false;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromSkillTree)
                                {
                                    ab.additionalSkeletonsFromSkillTree = Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromSkillTree;
                                    ab.onlySummonOneWarrior = false;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsPerCast)
                                {
                                    ab.additionalSkeletonsPerCast = Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsPerCast;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_chanceToResummonOnDeath)
                                {
                                    ab.chanceToResummonOnDeath = Save_Manager.instance.data.Skills.Minions.Skeletons.chanceToResummonOnDeath;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceArcher)
                                {
                                    ab.cannotSummonArchers = false;
                                    ab.cannotSummonWarriors = true;
                                    ab.canSummonRogues = false;

                                    ab.forceBrawler = false;
                                    ab.forceRogue = false;
                                    ab.forceWarrior = false;
                                    ab.forceArcher = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceBrawler)
                                {
                                    ab.cannotSummonArchers = true;
                                    ab.cannotSummonWarriors = true;
                                    ab.canSummonRogues = false;

                                    ab.forceRogue = false;
                                    ab.forceWarrior = false;
                                    ab.forceArcher = false;
                                    ab.forceBrawler = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceRogue)
                                {
                                    ab.cannotSummonArchers = true;
                                    ab.cannotSummonWarriors = true;
                                    ab.canSummonRogues = true;

                                    ab.forceWarrior = false;
                                    ab.forceArcher = false;
                                    ab.forceBrawler = false;
                                    ab.forceRogue = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceWarrior)
                                {
                                    ab.cannotSummonArchers = true;
                                    ab.cannotSummonWarriors = false;
                                    ab.canSummonRogues = false;

                                    ab.forceArcher = false;
                                    ab.forceBrawler = false;
                                    ab.forceRogue = false;
                                    ab.forceWarrior = true;
                                }
                            }
                        }
                        else if (il2cpp_type.ToString() == "SummonWraithMutator")
                        {
                            var ab = __1.TryCast<SummonWraithMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_additionalMaxWraiths)
                                {
                                    ab.additionalMaxWraiths = Save_Manager.instance.data.Skills.Minions.Wraiths.additionalMaxWraiths;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_delayedWraiths)
                                {
                                    ab.delayedWraiths = Save_Manager.instance.data.Skills.Minions.Wraiths.delayedWraiths; //Wraiths per cast
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_limitedTo2Wraiths)
                                {
                                    ab.limitedTo2Wraiths = false;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_wraithsDoNotDecay)
                                {
                                    ab.wraithsDoNotDecay = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_increasedCastSpeed)
                                {
                                    ab.increasedCastSpeed = Save_Manager.instance.data.Skills.Minions.Wraiths.increasedCastSpeed;
                                }
                            }
                        }
                        else if (il2cpp_type.ToString() == "SummonMageMutator")
                        {
                            var ab = __1.TryCast<SummonMageMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                //remove in LastEpoch 1.2
                                /*if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromItems)
                                {
                                    __1.TryCast<SummonMageMutator>().additionalSkeletonsFromItems = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromItems;
                                }*/
                                if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromPassives)
                                {
                                    ab.additionalSkeletonsFromPassives = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromPassives;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromSkillTree)
                                {
                                    ab.additionalSkeletonsFromSkillTree = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromSkillTree;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsPerCast)
                                {
                                    ab.additionalSkeletonsPerCast = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsPerCast;
                                }
                                //if (Config.Data.mods_config.character.minions.mage.Enable_onlySummonOneMage)
                                //{
                                //    ability_mutator.TryCast<SummonMageMutator>().onlySummonOneMage = false;                                
                                //}
                                if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_singleSummon)
                                {
                                    ab.singleSummon = false;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_forceCryomancer)
                                {
                                    ab.forceDeathKnight = false;
                                    ab.forcePyromancer = false;
                                    ab.forceNoCryo = false;
                                    ab.forceCryomancer = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_forceDeathKnight)
                                {
                                    ab.forcePyromancer = false;
                                    ab.forceCryomancer = false;
                                    ab.forceDeathKnight = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_forcePyromancer)
                                {
                                    ab.forceCryomancer = false;
                                    ab.forceDeathKnight = false;
                                    ab.forceNoPyro = false;
                                    ab.forcePyromancer = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_chanceForTwoExtraProjectiles)
                                {
                                    ab.chanceForTwoExtraProjectiles = Save_Manager.instance.data.Skills.Minions.Mages.chanceForTwoExtraProjectiles;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_doubleProjectiles)
                                {
                                    ab.doubleProjectiles = true;
                                }
                                //ability_mutator.TryCast<SummonMageMutator>().additionalWarlords = 50;
                            }
                        }
                        else if (il2cpp_type.ToString() == "SummonBoneGolemMutator")
                        {
                            var ab = __1.TryCast<SummonBoneGolemMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_selfResurrectChance)
                                {
                                    //ab.selfResurrectChance = Save_Manager.instance.data.Skills.Minions.BoneGolems.selfResurrectChance;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_increasedFireAuraArea)
                                {
                                    ab.increasedFireAuraArea = Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedFireAuraArea;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_increasedMoveSpeed)
                                {
                                    //ab.increasedMoveSpeed = Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedMoveSpeed;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_twins)
                                {
                                    ab.twins = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_hasSlamAttack)
                                {
                                    //ab.hasSlamAttack = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_undeadArmorAura)
                                {
                                    ab.undeadArmorAura = Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadArmorAura;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_undeadMovespeedAura)
                                {
                                    ab.undeadMovespeedAura = Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadMovespeedAura;
                                }
                            }
                        }
                        else if (il2cpp_type.ToString() == "SummonVolatileZombieMutator")
                        {
                            var ab = __1.TryCast<SummonVolatileZombieMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastFromMinionDeath)
                                {
                                    ab.chanceToCastFromMinionDeath = Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastFromMinionDeath;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastInfernalShadeOnDeath)
                                {
                                    ab.chanceToCastInfernalShadeOnDeath = Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastInfernalShadeOnDeath;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastMarrowShardsOnDeath)
                                {
                                    ab.chanceToCastMarrowShardsOnDeath = Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastMarrowShardsOnDeath;
                                }
                            }
                        }
                        else if (il2cpp_type.ToString() == "DreadShadeMutator")
                        {
                            var ab = __1.TryCast<DreadShadeMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_DisableLimit)
                                {
                                    ab.limitTo1DreadShade = false;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Duration)
                                {
                                    ab.increasedDuration = Save_Manager.instance.data.Skills.Minions.DreadShades.Duration;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_DisableHealthDrain)
                                {
                                    ab.noHealthDrain = true;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Max)
                                {
                                    ab.addedMaxShades = Save_Manager.instance.data.Skills.Minions.DreadShades.max;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_ReduceDecay)
                                {
                                    ab.reducedDecayRate = Save_Manager.instance.data.Skills.Minions.DreadShades.decay;
                                }
                                if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Radius)
                                {
                                    ab.increasedRadius = Save_Manager.instance.data.Skills.Minions.DreadShades.radius;
                                }
                            }
                        }
                        else if (il2cpp_type.ToString() == "FlameWardMutator")
                        {
                            var ab = __1.TryCast<FlameWardMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                                {
                                    ab.addedManaCost = 0f;
                                }
                            }
                        }
                    }
                }
            }

            [HarmonyPostfix]
            static void PostFix(CharacterMutator __instance, AbilityInfo __0, ref AbilityMutator __1, float __2, UnityEngine.Vector3 __3, bool __4)
            {
                if ((Scenes.IsGameScene()) && (!__1.IsNullOrDestroyed()) && (!Save_Manager.instance.IsNullOrDestroyed()))
                {
                    if (Save_Manager.instance.data.Skills.Enable_RemoveCooldown) { __1.RemoveCooldown(); }
                }
            }
        }

        //Fix ManaCost for ErasingStrike on Echo
        [HarmonyPatch(typeof(AbilityCoroutineHandler), "CreateVoidKnightEchoAfterDelay")]
        public class AbilityCoroutineHandler_CreateVoidKnightEchoAfterDelay
        {
            [HarmonyPrefix]
            //static void Prefix(float __0, Il2Cpp.Actor __1, UnityEngine.Vector3 __2, UnityEngine.Vector3 __3, Il2Cpp.AbilityAnimation __4, int __5, Il2Cpp.AbilityObjectConstructor __6, Il2Cpp.Ability __7, UnityEngine.Vector3 __8, UnityEngine.Vector3 __9, float __10, Il2Cpp.UseType __11, Il2CppSystem.Collections.Generic.List<Il2Cpp.Stats.Stat> __12, byte __13, bool __14, ref bool __15, ref int __16)
            static void Prefix(ref bool __15, ref int __16)
            {
                if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
                {
                    if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                    {
                        __15 = false; //consumesMana
                        //__16 = 0; //manaConsumed
                    }
                }
            }
        }
    }
}
