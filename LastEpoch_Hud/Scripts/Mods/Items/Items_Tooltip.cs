using HarmonyLib;
using LE.Data;
using System;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Tooltip
    {
        public static UnityEngine.UI.Image legendary_icon = null;
        public static Vector2 backup;
        public static Sprite Up_icon = null;
        public static Sprite Down_icon = null;

        public static void LoadAssets()
        {
            if (!Hud_Manager.asset_bundle.IsNullOrDestroyed())
            {
                foreach (string name in Hud_Manager.asset_bundle.GetAllAssetNames())
                {
                    if (name.Contains("/tooltip/"))
                    {
                        if (Functions.Check_Texture(name))
                        {
                            Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                            if ((name.Contains("up")) && (Up_icon.IsNullOrDestroyed()))
                            {
                                Up_icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                //UnityEngine.Object.DontDestroyOnLoad(Up_icon);
                            }
                            if ((name.Contains("down")) && (Down_icon.IsNullOrDestroyed()))
                            {
                                Down_icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                //UnityEngine.Object.DontDestroyOnLoad(Down_icon);
                            }
                        }
                    }
                }
            }
        }
        public static int SavedItems_LegendaryPotencial(ItemDataUnpacked item)
        {
            int legendary_potencial = 0;
            if (!Refs_Manager.player_data.IsNullOrDestroyed())
            {
                foreach (ItemLocationPair item_loc_pair in Refs_Manager.player_data.SavedItems)
                {
                    if (legendary_potencial > 3) { break; }
                    if ((item.rarity == 7) && (item_loc_pair.Data.Count > 9))
                    {
                        byte item_type = item_loc_pair.Data[1];
                        byte item_id = item_loc_pair.Data[2];
                        byte item_rarity = item_loc_pair.Data[3];
                        int item_unique_id = (item_loc_pair.Data[8] * 256) + item_loc_pair.Data[9];
                        byte item_legendary_potencial = item_loc_pair.Data[item_loc_pair.Data.Count - 1];
                        if ((item_type == item.itemType) && (item_id == item.subType) &&
                            (item_rarity > 6) && (item_unique_id == item.uniqueID))
                        {
                            if (item_legendary_potencial > legendary_potencial) { legendary_potencial = item_legendary_potencial; }
                        }
                    }
                }
            }

            return legendary_potencial;
        }
        /*public static bool IsIdol(int item_type)
        {
            bool result = false;
            if ((item_type > 24) && (item_type < 34)) { result = true; }

            return result;
        }*/

        
        [HarmonyPatch(typeof(TooltipItemManager), "OpenTooltip", new Type[] { typeof(ItemDataUnpacked), typeof(TooltipItemManager.SlotType), typeof(UnityEngine.Vector2), typeof(UnityEngine.Vector3), typeof(UnityEngine.GameObject) })]
        public class TooltipItemManager_OpenTooltip
        {
            [HarmonyPostfix]
            static void Postfix(ref TooltipItemManager __instance)
            {
                if (!legendary_icon.IsNullOrDestroyed()) { legendary_icon.gameObject.active = false; }
                if ((legendary_icon.IsNullOrDestroyed()) && (!UITooltipItem.instance.IsNullOrDestroyed()))
                {
                    if (!UITooltipItem.instance.legendaryPotential.IsNullOrDestroyed())
                    {
                        GameObject label_obj = Functions.GetChild(UITooltipItem.instance.legendaryPotential, "Label");
                        if (!label_obj.IsNullOrDestroyed())
                        {
                            string obj_name = "legendary_icon";
                            GameObject legendary_instance_obj = Functions.GetChild(label_obj, obj_name);
                            if (legendary_instance_obj.IsNullOrDestroyed())
                            {
                                Main.logger_instance.Msg("Create legendary icon in tooltip");
                                legendary_instance_obj = new GameObject { name = obj_name };
                                legendary_instance_obj.AddComponent<UnityEngine.UI.Image>();
                                legendary_instance_obj.transform.SetParent(label_obj.transform);                                
                                RectTransform recttransform = legendary_instance_obj.GetComponent<RectTransform>();
                                recttransform.offsetMax = new Vector2 { x = 100, y = 25 };
                                recttransform.offsetMin = new Vector2 { x = 50, y = -25 };
                                legendary_icon = legendary_instance_obj.GetComponent<UnityEngine.UI.Image>();
                                legendary_icon.gameObject.active = false;
                            }
                            else { legendary_icon = legendary_instance_obj.GetComponent<UnityEngine.UI.Image>(); }
                        }
                    }
                }
                else if (UITooltipItem.instance.IsNullOrDestroyed()) { Main.logger_instance.Error("UITooltipItem.instance is null"); }
                if ((!__instance.activeContent.IsNullOrDestroyed()) && (!legendary_icon.IsNullOrDestroyed()))
                {
                    if ((__instance.activeContent.slotType == TooltipItemManager.SlotType.GROUND) &&
                        (!__instance.activeParameters.Item.IsNullOrDestroyed()))
                    {
                        if ((__instance.activeParameters.Item.rarity == 7) && (__instance.activeParameters.Item.itemType < 34))
                        {
                            if ((Up_icon.IsNullOrDestroyed()) || (Down_icon.IsNullOrDestroyed())) { LoadAssets(); }
                            if ((!Up_icon.IsNullOrDestroyed()) && (!Down_icon.IsNullOrDestroyed()))
                            {
                                int legendary_potencial_owned = SavedItems_LegendaryPotencial(__instance.activeParameters.Item);
                                if (legendary_potencial_owned > 0)
                                {
                                    if (__instance.activeParameters.Item.legendaryPotential > legendary_potencial_owned)
                                    {
                                        legendary_icon.sprite = Up_icon;
                                        legendary_icon.gameObject.active = true;
                                    }
                                    else if (__instance.activeParameters.Item.legendaryPotential < legendary_potencial_owned)
                                    {
                                        legendary_icon.sprite = Down_icon;
                                        legendary_icon.gameObject.active = true;
                                    }
                                }
                            }
                            else { Main.logger_instance.Error("Icons are null"); }
                        }                        
                    }
                    else if (__instance.activeParameters.Item.IsNullOrDestroyed()) { Main.logger_instance.Error("Item is null"); }
                }
                else if (__instance.activeContent.IsNullOrDestroyed()) { Main.logger_instance.Error("activeContent is null"); }
                else if (legendary_icon.IsNullOrDestroyed()) { Main.logger_instance.Error("legendary_icon is null"); }
            }
        }
    }
}
