using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Generate_Items
    {
        [HarmonyPatch(typeof(GenerateItems), "RollRarity")]
        public class RollRarity
        {
            [HarmonyPostfix]
            static void Postfix(ref byte __result, int __0)
            {
                if (Config.Data.mods_config.items.Enable_Rarity) { __result = Config.Data.mods_config.items.GenerateItem_Rarity; }
            }
        }

        [HarmonyPatch(typeof(GenerateItems), "RollAffixes")]
        public class RollAffixes
        {
            [HarmonyPostfix]
            static void Postfix(GenerateItems __instance, ref int __result, ref ItemDataUnpacked __0, int __1, ref bool __2, ref bool __3, ref Il2CppSystem.Boolean __4)
            {
                if (Config.Data.mods_config.items.Enable_RollImplicit)
                {
                    for (int j = 0; j < __0.implicitRolls.Count; j++) //Work only for basic item
                    {
                        __0.implicitRolls[j] = Config.Data.mods_config.items.Roll_Implicit;
                    }
                }
                if ((Config.Data.mods_config.items.Enable_AffixsTier) | (Config.Data.mods_config.items.Enable_AffixsValue))
                {
                    int tier_result = System.Convert.ToInt32(Config.Data.mods_config.items.Roll_AffixTier) - 1;
                    for (int i = 0; i < __0.affixes.Count; i++)
                    {
                        if (Config.Data.mods_config.items.Enable_AffixsTier) { __0.affixes[i].affixTier = (byte)tier_result; }
                        if (Config.Data.mods_config.items.Enable_AffixsValue) { __0.affixes[i].affixRoll = Config.Data.mods_config.items.Roll_AffixValue; }
                    }
                    __2 = true; //Can roll uncraftable Tier
                    __3 = false; //force Exalted
                }
            }
        }
        
        [HarmonyPatch(typeof(GenerateItems), "rollForgingPotential")]
        public class rollForgingPotential
        {
            [HarmonyPostfix]
            static void Postfix(ref int __result, ItemDataUnpacked __0, int __1, bool __2, GenerateItems.VendorType __3)
            {
                if (Config.Data.mods_config.items.Enable_ForgingPotencial)
                {
                    __result = Config.Data.mods_config.items.Roll_ForgingPotencial;
                }                
            }
        }

        //Use to populate shop, gambling, ...
        [HarmonyPatch(typeof(GenerateItems), "initialiseRandomItemData")]
        public class initialiseRandomItemData
        {
            [HarmonyPrefix]
            static bool Prefix(GenerateItems __instance, ref ItemDataUnpacked __result, bool __0, ref int __1, bool __2, ItemLocationTag __3, int __4, int __5, ref int __6, ref int __7, ref int __8, ref bool __9, ref int __10)
            {
                //Main.logger_instance.Msg("GenerateItems : initialiseRandomItemData : Prefix");
                //__0 =  //giftable
                //__1 = 100; //ilvl
                //_2 = //Require class compatibility
                //__3 = //Location
                //__4 = //base type
                //__5 = //subtype
                if (Config.Data.mods_config.items.Enable_Rarity) //rarity
                {
                    __6 = Config.Data.mods_config.items.GenerateItem_Rarity;
                }
                //__7 = 4; //sockets
                //int unique_id = 0;
                //Make Random
                //foreach (UniqueList.Entry unique in UniqueList.get().uniques)
                //{
                //    if ((!unique.isSetItem) && (unique.baseType == __result.itemType))
                //    {
                //        unique_id = unique.uniqueID;
                //        break;
                //    }
                //}
                //__8 = unique_id;
                //__9 = false; //force exalted
                //__10 = 255; //minimum legendary potencial

                return true;
            }
        }
    }
}
