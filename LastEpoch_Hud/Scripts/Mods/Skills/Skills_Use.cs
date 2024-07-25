using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    public class Skills_Use
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return true;
                }
                else { return false; }
            }
            else { return false; }
        }

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
        private static Ability using_ability = null;
        [HarmonyPatch(typeof(CharacterMutator), "OnStartedUsingAbility")]
        public class OnStartedUsingAbility
        {
            [HarmonyPrefix]
            static void Prefix(CharacterMutator __instance, AbilityInfo __0, ref Ability __1, UnityEngine.Vector3 __2)
            {
                try
                {
                    if (CanRun())
                    {
                        if (__1 != null)
                        {
                            using_ability = __1;

                            //Main.logger_instance.Msg("OnStartedUsingAbility : Ability = " + __1.abilityName);

                            if (Save_Manager.instance.data.Skills.Enable_RemoveChannelCost) { __1.channelCost = 0f; }
                            if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                            {
                                __1.manaCost = 0f;
                                __1.minimumManaCost = 0f;
                                __1.manaCostPerDistance = 0f;
                            }
                            if (Save_Manager.instance.data.Skills.Enable_NoManaRegenWhileChanneling) { __1.noManaRegenWhileChanneling = false; }
                            if (Save_Manager.instance.data.Skills.Enable_StopWhenOutOfMana) { __1.stopWhenOutOfMana = false; }
                            //if (Config.Data.mods_config.character.characterstats.Enable_attack_rate) { __1.speedMultiplier = Config.Data.mods_config.character.characterstats.attack_rate; }
                            if ((__1.moveOrAttackCompatible) && (__1.moveOrAttackFallback == Ability.MoveOrAttackFallback.Move))
                            {
                                if (Save_Manager.instance.data.Skills.MovementSkills.Enable_NoTarget)
                                {
                                    __1.playerRequiresTarget = false;
                                    __1.requiredEnemyTargetMustBeAccessible = false;
                                }
                                if (Save_Manager.instance.data.Skills.MovementSkills.Enable_ImmuneDuringMovement)
                                {
                                    __1.immuneDuringMovement = true;
                                    __1.cannotDieDuringMovement = true;
                                }
                                if (Save_Manager.instance.data.Skills.MovementSkills.Disable_SimplePath)
                                {
                                    __1.limitRangeForPlayers = false;
                                    __1.requireSimplePath = false;
                                }
                            }

                            //Get Mutator
                            try
                            {
                                AbilityMutator ability_mutator = Ability_Mutator.GetMutatorFromAbility(__1); System.Type type = ability_mutator.GetType();
                                Il2CppSystem.Type il2cpp_type = ability_mutator.GetIl2CppType();

                                //Main.logger_instance.Msg("OnStartedUsingAbility Prefix : Mutator Type = " + il2cpp_type.ToString());

                                //Use Switch(il2cpp_type.ToString()) instead of if for better result (== is bad)

                                //Warpath : Fix Channel Cost
                                if (il2cpp_type.ToString() == "WarpathMutator")
                                {
                                    if (Save_Manager.instance.data.Skills.Enable_RemoveChannelCost)
                                    {
                                        ability_mutator.TryCast<WarpathMutator>().addedChannelCost = 0f;
                                        ability_mutator.TryCast<WarpathMutator>().addedChannelCostPerSecond = 0f;
                                    }
                                }

                                //Smite : Fix ManaCost
                                if (il2cpp_type.ToString() == "SmiteMutator")
                                {
                                    if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                                    {
                                        ability_mutator.TryCast<SmiteMutator>().addedManaCost = 0f;
                                        ability_mutator.TryCast<SmiteMutator>().increasedManaCost = 0f;
                                    }
                                }

                                //Sigil of Hope : Fix ManaCost
                                if (il2cpp_type.ToString() == "SigilsOfHopeMutator")
                                {
                                    if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                                    {
                                        ability_mutator.TryCast<SigilsOfHopeMutator>().addedManaCost = 0f;
                                        ability_mutator.TryCast<SigilsOfHopeMutator>().increasedManaCost = 0f;
                                    }
                                }

                                //Meteor
                                if (il2cpp_type.ToString() == "MeteorMutator")
                                {
                                    if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
                                    {
                                        ability_mutator.TryCast<MeteorMutator>().addedManaCost = 0f;
                                    }
                                }

                                //Companions
                                else if (il2cpp_type.ToString() == "SummonWolfMutator")
                                {
                                    if (Save_Manager.instance.data.Skills.Companion.Wolf.Enable_SummonLimit)
                                    {
                                        ability_mutator.TryCast<SummonWolfMutator>().wolfLimit = Save_Manager.instance.data.Skills.Companion.Wolf.SummonLimit;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "SummonScorpionMutator")
                                {
                                    if (Save_Manager.instance.data.Skills.Companion.Scorpion.Enable_BabyQuantity)
                                    {
                                        SummonScorpionMutator mutator = ability_mutator.TryCast<SummonScorpionMutator>();
                                        mutator.babyScorpionQuantity = Save_Manager.instance.data.Skills.Companion.Scorpion.BabyQuantity;
                                        mutator.babyScorpionsToSpawnOnAbilityActivation = Save_Manager.instance.data.Skills.Companion.Scorpion.BabyQuantity;
                                        mutator.increasedBabySpawnRate = 1;
                                    }
                                }

                                //Minions
                                else if (il2cpp_type.ToString() == "SummonSkeletonMutator")
                                {
                                    if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromPassives)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().additionalSkeletonsFromPassives = Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromPassives;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().onlySummonOneWarrior = false;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromSkillTree)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().additionalSkeletonsFromSkillTree = Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromSkillTree;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().onlySummonOneWarrior = false;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsPerCast)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().additionalSkeletonsPerCast = Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsPerCast;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_chanceToResummonOnDeath)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().chanceToResummonOnDeath = Save_Manager.instance.data.Skills.Minions.Skeletons.chanceToResummonOnDeath;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceArcher)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonArchers = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonWarriors = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().canSummonRogues = false;

                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceBrawler)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonArchers = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonWarriors = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().canSummonRogues = false;

                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceRogue)
                                    {
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonArchers = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().cannotSummonWarriors = true;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().canSummonRogues = true;

                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceWarrior = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceArcher = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceBrawler = false;
                                        ability_mutator.TryCast<SummonSkeletonMutator>().forceRogue = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceWarrior)
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
                                    if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_additionalMaxWraiths)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().additionalMaxWraiths = Save_Manager.instance.data.Skills.Minions.Wraiths.additionalMaxWraiths;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_delayedWraiths)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().delayedWraiths = Save_Manager.instance.data.Skills.Minions.Wraiths.delayedWraiths; //Wraiths per cast
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_limitedTo2Wraiths)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().limitedTo2Wraiths = false;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_wraithsDoNotDecay)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().wraithsDoNotDecay = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_increasedCastSpeed)
                                    {
                                        ability_mutator.TryCast<SummonWraithMutator>().increasedCastSpeed = Save_Manager.instance.data.Skills.Minions.Wraiths.increasedCastSpeed;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "SummonMageMutator")
                                //else if (type == typeof(SummonMageMutator))
                                {
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromItems)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsFromItems = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromItems;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromPassives)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsFromPassives = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromPassives;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromSkillTree)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsFromSkillTree = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromSkillTree;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsPerCast)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().additionalSkeletonsPerCast = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsPerCast;
                                    }
                                    //if (Config.Data.mods_config.character.minions.mage.Enable_onlySummonOneMage)
                                    //{
                                    //    ability_mutator.TryCast<SummonMageMutator>().onlySummonOneMage = false;                                
                                    //}
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_singleSummon)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().singleSummon = false;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_forceCryomancer)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().forceDeathKnight = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forcePyromancer = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceNoCryo = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceCryomancer = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_forceDeathKnight)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().forcePyromancer = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceCryomancer = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceDeathKnight = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_forcePyromancer)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().forceCryomancer = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceDeathKnight = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forceNoPyro = false;
                                        ability_mutator.TryCast<SummonMageMutator>().forcePyromancer = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_chanceForTwoExtraProjectiles)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().chanceForTwoExtraProjectiles = Save_Manager.instance.data.Skills.Minions.Mages.chanceForTwoExtraProjectiles;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.Mages.Enable_doubleProjectiles)
                                    {
                                        ability_mutator.TryCast<SummonMageMutator>().doubleProjectiles = true;
                                    }
                                    //ability_mutator.TryCast<SummonMageMutator>().additionalWarlords = 50;
                                }
                                else if (il2cpp_type.ToString() == "SummonBoneGolemMutator")
                                //else if (type == typeof(SummonBoneGolemMutator))
                                {
                                    if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_selfResurrectChance)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().selfResurrectChance = Save_Manager.instance.data.Skills.Minions.BoneGolems.selfResurrectChance;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_increasedFireAuraArea)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().increasedFireAuraArea = Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedFireAuraArea;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_increasedMoveSpeed)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().increasedMoveSpeed = Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedMoveSpeed;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_twins)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().twins = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_hasSlamAttack)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().hasSlamAttack = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_undeadArmorAura)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().undeadArmorAura = Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadArmorAura;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_undeadMovespeedAura)
                                    {
                                        ability_mutator.TryCast<SummonBoneGolemMutator>().undeadMovespeedAura = Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadMovespeedAura;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "SummonVolatileZombieMutator")
                                //else if (type == typeof(SummonVolatileZombieMutator))
                                {
                                    if (Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastFromMinionDeath)
                                    {
                                        ability_mutator.TryCast<SummonVolatileZombieMutator>().chanceToCastFromMinionDeath = Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastFromMinionDeath;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastInfernalShadeOnDeath)
                                    {
                                        ability_mutator.TryCast<SummonVolatileZombieMutator>().chanceToCastInfernalShadeOnDeath = Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastInfernalShadeOnDeath;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastMarrowShardsOnDeath)
                                    {
                                        ability_mutator.TryCast<SummonVolatileZombieMutator>().chanceToCastMarrowShardsOnDeath = Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastMarrowShardsOnDeath;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "DreadShadeMutator")
                                {
                                    if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_DisableLimit)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().limitTo1DreadShade = false;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Duration)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().increasedDuration = Save_Manager.instance.data.Skills.Minions.DreadShades.Duration;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_DisableHealthDrain)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().noHealthDrain = true;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Max)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().addedMaxShades = Save_Manager.instance.data.Skills.Minions.DreadShades.max;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_ReduceDecay)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().reducedDecayRate = Save_Manager.instance.data.Skills.Minions.DreadShades.decay;
                                    }
                                    if (Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Radius)
                                    {
                                        ability_mutator.TryCast<DreadShadeMutator>().increasedRadius = Save_Manager.instance.data.Skills.Minions.DreadShades.radius;
                                    }
                                }
                                else if (il2cpp_type.ToString() == "FlameWardMutator")
                                {
                                    if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
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
                    if (CanRun())
                    {
                        if (__1 != null)
                        {
                            AbilityMutator ability_mutator = Ability_Mutator.GetMutatorFromAbility(__1);
                            //Il2CppSystem.Type il2cpp_type = ability_mutator.GetIl2CppType();
                            //Main.logger_instance.Msg("OnStartedUsingAbility Postfix : Mutator Type = " + il2cpp_type.ToString());

                            if (Save_Manager.instance.data.Skills.Enable_RemoveCooldown) { ability_mutator.RemoveCooldown(); }
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
                if (CanRun())
                {
                    try
                    {
                        System.Type type = __1.GetType();
                        Il2CppSystem.Type il2cpp_type = __1.GetIl2CppType();
                        //Main.logger_instance.Msg("OnAbilityUse Prefix : Mutator Type = " + il2cpp_type.ToString());

                        //Use Switch(il2cpp_type.ToString()) instead of if for better result (== is bad)

                        //Sigil of Hope : Fix ManaCost
                        if (il2cpp_type.ToString() == "HolyAuraMutator")
                        {
                            if (Save_Manager.instance.data.Skills.Enable_RemoveManaCost)
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
                if (CanRun())
                {
                    try
                    {
                        if (Save_Manager.instance.data.Skills.Enable_RemoveCooldown) { __1.RemoveCooldown(); }
                    }
                    catch { } //Sometimes we have Portal error
                    
                    /*try
                    {
                        System.Type type = __1.GetType();
                        //Il2CppSystem.Type il2cpp_type = __1.GetIl2CppType();
                        //Main.logger_instance.Msg("OnAbilityUse PostFix : Mutator Type = " + il2cpp_type.ToString());

                        if (Save_Manager.instance.data.Skills.Enable_RemoveCooldown) { __1.RemoveCooldown(); }
                    }
                    catch { }*/
                }
            }
        }
    }
}
