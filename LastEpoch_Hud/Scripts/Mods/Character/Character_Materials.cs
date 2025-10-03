using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using System.Text.Json;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_Materials
    {
        public static void GetAllRunesX99()
        {
            if (!Refs_Manager.item_list.IsNullOrDestroyed())
            {
                Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                int item_type = 102;
                ItemList.BaseNonEquipmentItem items = GetItemList(item_type);
                if (!items.IsNullOrDestroyed())
                {
                    foreach (ItemList.NonEquipmentItem item in items.subItems)
                    {
                        ForceDrop(item_type, item.subTypeID, 99);
                    }
                }                
            }
        }
        public static void GetAllGlyphsX99()
        {
            if (!Refs_Manager.item_list.IsNullOrDestroyed())
            {
                Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                int item_type = 103;
                foreach (ItemList.NonEquipmentItem item in GetItemList(item_type).subItems)
                {
                    ForceDrop(item_type, item.subTypeID, 99);
                }
            }
        }
        public static void GetAllShardsX10()
        {
            if (!Refs_Manager.item_list.IsNullOrDestroyed())
            {
                Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                int item_type = 101;
                foreach (ItemList.NonEquipmentItem item in GetItemList(item_type).subItems)
                {
                    ForceDrop(item_type, item.subTypeID, 10);
                }
            }
        }
        public static void GetAddAncienBonesX10000()
        {
            if ((!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                Refs_Manager.ground_item_manager.dropAncientBoneForPlayer(Refs_Manager.player_actor, 10000, Refs_Manager.player_actor.position(), false, false);
            }
        }
        public static void ImportJson()
        {
            try
            {
                if (Refs_Manager.ground_item_manager.IsNullOrDestroyed() || Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    return;
                }

                string text = GUIUtility.systemCopyBuffer;

                using (System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(text))
                {
                    if (doc.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
                    {
                        return;
                    }

                    foreach (var category in doc.RootElement.EnumerateObject())
                    {
                        switch (category.Name)
                        {
                            case "idols":
                                {
                                    if (category.Value.ValueKind != System.Text.Json.JsonValueKind.Array)
                                    {
                                        break;
                                    }
                                    foreach (var item in category.Value.EnumerateArray())
                                    {
                                        if (item.ValueKind == JsonValueKind.Object)
                                        {
                                            CreateItemFromJson(item);
                                        }
                                    }
                                    break;
                                }
                            case "items":
                                {
                                    if (category.Value.ValueKind != System.Text.Json.JsonValueKind.Object)
                                    {
                                        break;
                                    }
                                    foreach (var itemPair in category.Value.EnumerateObject())
                                    {
                                        CreateItemFromJson(itemPair.Value);
                                    }
                                    break;
                                }
                            case "mastery":
                                {
                                    if (Refs_Manager.player_data.CharacterClass != doc.RootElement.GetProperty("class").GetInt32())
                                    {
                                        break;
                                    }
                                    Refs_Manager.player_actor.localTreeData.ReceiveRespecAllCommand(Refs_Manager.player_data.GetCharacterClass(), 0);
                                    Refs_Manager.player_actor.localTreeData.resetChosenMastery();
                                    Refs_Manager.player_actor.localTreeData.receiveChooseMasteriesCommand(category.Value.GetByte());
                                    if (doc.RootElement.TryGetProperty("passives", out var passiveJson))
                                    {
                                        foreach (var e in passiveJson.GetProperty("history").EnumerateArray())
                                        {
                                            if (Refs_Manager.player_actor.localTreeData.getPassiveTreeData().getUnspentPoints() > 0)
                                            {
                                                if (!Refs_Manager.player_actor.localTreeData.receiveSpendPassivePointCommand(Refs_Manager.player_data.GetCharacterClass(), e.GetByte()))
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case "skillTrees":
                                {
                                    var ApplySkillJson = (Ability ability, JsonElement skillIdList) =>
                                    {
                                        foreach (var idJson in skillIdList.EnumerateArray())
                                        {
                                            if (!Refs_Manager.player_actor.localTreeData.receiveSpendSkillPointCommand(ability, idJson.GetByte()))
                                            {
                                                break;
                                            }
                                        }
                                    };
                                    foreach (var abilityJson in category.Value.EnumerateObject())
                                    {
                                        // try to relearn a specialized skill
                                        bool found = false;
                                        foreach (var ability in Refs_Manager.player_actor.localTreeData.getSpecialisedAbilities())
                                        {
                                            if (ability.playerAbilityID == abilityJson.Name)
                                            {
                                                Refs_Manager.player_actor.localTreeData.receiveDespecialiseCommand(ability);
                                                Refs_Manager.player_actor.localTreeData.receiveSpecialiseCommand(ability, false, 0);
                                                Refs_Manager.player_actor.localTreeData.ApplyAbilityXp(100000000, true);
                                                if (abilityJson.Value.TryGetProperty("history", out var historyJson))
                                                {
                                                    ApplySkillJson(ability, historyJson);
                                                }
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (found)
                                        {
                                            break;
                                        }
                                        // delete all if no slots available
                                        if (Refs_Manager.player_actor.localTreeData.getFreeSlots().Count < 1)
                                        {
                                            foreach (var ability in Refs_Manager.player_actor.localTreeData.getSpecialisedAbilities())
                                            {
                                                Refs_Manager.player_actor.localTreeData.receiveDespecialiseCommand(ability);
                                            }
                                        }
                                        // add the new skill
                                        foreach (var ability in Refs_Manager.ability_manager.abilities)
                                        {
                                            if (ability && ability.playerAbilityID == abilityJson.Name)
                                            {
                                                Refs_Manager.player_actor.localTreeData.receiveSpecialiseCommand(ability, false, 0);
                                                Refs_Manager.player_actor.localTreeData.ApplyAbilityXp(100000000, true);
                                                if (abilityJson.Value.TryGetProperty("history", out var historyJson))
                                                {
                                                    ApplySkillJson(ability, historyJson);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Main.logger_instance?.Error(e.ToString());
            }
        }
        private static void CreateItemFromJson(System.Text.Json.JsonElement itemJson)
        {
            byte itemType = itemJson.GetProperty("itemType").GetByte();
            ushort subType = itemJson.GetProperty("subType").GetUInt16();

            var item = new ItemDataUnpacked();

            item.itemType = itemType;
            item.subType = subType;
            item.SetAllImplicitRolls(255);
            item.RefreshIDAndValues();

            // order matters here: unique, affixes, primordial, sealed
            List<Stats.Stat> changes = new List<Stats.Stat>();
            if (itemJson.TryGetProperty("uniqueID", out var uniqueIdJson))
            {
                ushort uniqueId = uniqueIdJson.GetUInt16();
                List<double> rolls = new List<double>();
                foreach (var roll in itemJson.GetProperty("uniqueRolls").EnumerateArray())
                {
                    rolls.Add(roll.GetDouble());
                }
                item.uniqueID = uniqueId;
                item.rarity = (byte)(UniqueList.getUnique(uniqueId).isSetItem ? 8 : 7);
                for (int i = 0; i < rolls.Count && i < item.uniqueRolls.Count; ++i)
                {
                    item.uniqueRolls[i] = (byte)(255 * rolls[i]);
                }
                item.RefreshIDAndValues();
            }
            if (itemJson.TryGetProperty("affixes", out var affixesJson))
            {
                if (item.isUnique() && affixesJson.GetArrayLength() > 0)
                {
                    item.rarity = 9;
                }
                foreach (var affixJson in affixesJson.EnumerateArray())
                {
                    int affixId = affixJson.GetProperty("id").GetInt32();
                    byte tier = (byte)(affixJson.GetProperty("tier").GetByte() - 1);
                    double roll = affixJson.GetProperty("roll").GetDouble();
                    int count = item.GetNonSealedAffixes() != null ? item.GetNonSealedAffixes().Count : 0;
                    if (count < 4)
                    {
                        Il2CppSystem.Nullable<byte> v = new Il2CppSystem.Nullable<byte>((byte)(255 * roll));
                        item.AddAffixNoCostOrChecks(affixId, false, tier, ref changes, v);
                    }
                }
                if (!item.isUniqueSetOrLegendary())
                {
                    item.setRarityFromAffixesForNormalMagicOrRareItem();
                    item.forgingPotential = 40;
                }
            }
            if (itemJson.TryGetProperty("primordialAffix", out var primordialJson))
            {
                int affixId = primordialJson.GetProperty("id").GetInt32();
                byte tier = (byte)(primordialJson.GetProperty("tier").GetByte() - 1);
                double roll = primordialJson.GetProperty("roll").GetDouble();
                int count = item.GetNonSealedAffixes().Count;
                Il2CppSystem.Nullable<byte> v = new Il2CppSystem.Nullable<byte>((byte)(255 * roll));
                item.AddAffixNoCostOrChecks(affixId, true, tier, ref changes, v);
                foreach (var affix in item.affixes)
                {
                    if (affix.affixId == affixId)
                    {
                        // the function that makes an affix primordial doesn't seem to work but this does
                        affix.affixTier = 7;
                        affix.sealedAffixType = SealedAffixType.Primordial;
                        item.hasSealedPrimordialAffix = true;
                        break;
                    }
                }
            }
            if (itemJson.TryGetProperty("sealedAffix", out var sealedJson))
            {
                int affixId = sealedJson.GetProperty("id").GetInt32();
                byte tier = (byte)(sealedJson.GetProperty("tier").GetByte() - 1);
                double roll = sealedJson.GetProperty("roll").GetDouble();
                int count = item.GetNonSealedAffixes().Count;
                Il2CppSystem.Nullable<byte> v = new Il2CppSystem.Nullable<byte>((byte)(255 * roll));
                item.AddAffixNoCostOrChecks(affixId, true, tier, ref changes, v);
                ItemAffix foundAffix = null;
                foreach (var affix in item.affixes)
                {
                    if (affix.affixId == affixId)
                    {
                        foundAffix = affix;
                        break;
                    }
                }
                if (foundAffix != null)
                {
                    item.SealAffix(foundAffix);
                    item.hasSealedRegularAffix = true;
                }
            }

            item.RefreshIDAndValues();
            Refs_Manager.ground_item_manager.dropItemForPlayer(Refs_Manager.player_actor, item.TryCast<ItemData>(), Refs_Manager.player_actor.position(), false);
        }
        private static void ForceDrop(int type, int subtype, int quantity)
        {
            if ((!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                ItemDataUnpacked item = new ItemDataUnpacked
                {
                    LvlReq = 0,
                    classReq = ItemList.ClassRequirement.Any,
                    itemType = (byte)type,
                    subType = (ushort)subtype,
                    rarity = (byte)0,
                    sockets = (byte)0,
                    uniqueID = (ushort)0
                };
                item.RefreshIDAndValues();
                ItemData final_item = item.TryCast<ItemData>();
                for (int i = 0; i < quantity; i++)
                {
                    Refs_Manager.ground_item_manager.dropItemForPlayer(Refs_Manager.player_actor, final_item, Refs_Manager.player_actor.position(), false);
                }
            }
            else { Main.logger_instance?.Error("Ground Item Manager Not Found"); }
        }
        private static ItemList.BaseNonEquipmentItem GetItemList(int type_id)
        {
            ItemList.BaseNonEquipmentItem result = null;            
            if (!Refs_Manager.item_list.IsNullOrDestroyed())
            {
                int index = 0;
                foreach (ItemList.BaseNonEquipmentItem n_item in Refs_Manager.item_list.nonEquippableItems)
                {
                    if (n_item.baseTypeID == type_id)
                    {
                        result = Refs_Manager.item_list.nonEquippableItems[index];
                        break;
                    }
                    index++;
                }
                if (result.IsNullOrDestroyed()) { Main.logger_instance?.Error("Character_Materials : List with type = " + type_id + " not found"); }
            }
            else { Main.logger_instance?.Error("Character_Materials : Itemlist is null"); }
            
            return result;
        }
    }
}
