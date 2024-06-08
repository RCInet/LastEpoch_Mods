using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Tooltip_LegendaryPotencial
    {
        public class Icons
        {
            public static Sprite Up_icon = null;
            public static Sprite Down_icon = null;

            public static void Load()
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
                                if (!texture.IsNullOrDestroyed())
                                {
                                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                    if ((name.Contains("up")) && (Up_icon.IsNullOrDestroyed())) { Up_icon = sprite; }
                                    if ((name.Contains("down")) && (Down_icon.IsNullOrDestroyed())) { Down_icon = sprite; }
                                }
                            }
                        }
                    }
                }
            }
        }
        public class Image
        {
            public static UnityEngine.UI.Image icon = null;
            
            public static void Add()
            {
                if (!UITooltipItem.instance.IsNullOrDestroyed())
                {
                    if (!UITooltipItem.instance.legendaryPotential.IsNullOrDestroyed())
                    {
                        GameObject label_obj = Functions.GetChild(UITooltipItem.instance.legendaryPotential, "Label");
                        if (!label_obj.IsNullOrDestroyed())
                        {
                            string obj_name = "legendary_icon";
                            GameObject obj = Functions.GetChild(label_obj, obj_name);
                            if (obj.IsNullOrDestroyed())
                            {
                                obj = new GameObject { name = obj_name };
                                obj.AddComponent<UnityEngine.UI.Image>();
                                obj.transform.SetParent(label_obj.transform);
                                RectTransform recttransform = obj.GetComponent<RectTransform>();
                                recttransform.offsetMax = new Vector2 { x = 100, y = 25 };
                                recttransform.offsetMin = new Vector2 { x = 50, y = -25 };
                                icon = obj.GetComponent<UnityEngine.UI.Image>();
                                icon.gameObject.active = false;
                            }
                            else { icon = obj.GetComponent<UnityEngine.UI.Image>(); }
                        }
                    }
                }
                else { Main.logger_instance.Error("UITooltipItem.instance is null"); }
            }
        }
        public class Check
        {
            public static int SavedItems(ItemDataUnpacked item)
            {
                int legendary_potencial = 0;
                if (!Refs_Manager.player_data.IsNullOrDestroyed())
                {
                    foreach (LE.Data.ItemLocationPair item_loc_pair in Refs_Manager.player_data.SavedItems)
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
                                (item_rarity == item.rarity) && (item_unique_id == item.uniqueID))
                            {
                                if (item_legendary_potencial > legendary_potencial) { legendary_potencial = item_legendary_potencial; }
                            }
                        }
                    }
                }

                return legendary_potencial;
            }
        }
        
        [HarmonyPatch(typeof(TooltipItemManager), "OpenTooltip", new System.Type[] { typeof(ItemDataUnpacked), typeof(TooltipItemManager.SlotType), typeof(Vector2), typeof(Vector3), typeof(GameObject) })]
        public class TooltipItemManager_OpenTooltip
        {
            [HarmonyPostfix]
            static void Postfix(ref TooltipItemManager __instance)
            {
                if (!Image.icon.IsNullOrDestroyed()) { Image.icon.gameObject.active = false; }
                if (Image.icon.IsNullOrDestroyed()) { Image.Add(); }                
                if ((Icons.Up_icon.IsNullOrDestroyed()) || (Icons.Down_icon.IsNullOrDestroyed())) { Icons.Load(); }

                if ((!__instance.activeContent.IsNullOrDestroyed()) && (!__instance.activeParameters.IsNullOrDestroyed()) &&
                    (!Image.icon.IsNullOrDestroyed()) && (!Icons.Up_icon.IsNullOrDestroyed()) && (!Icons.Down_icon.IsNullOrDestroyed()))
                {
                    ItemDataUnpacked item = __instance.activeParameters.Item;
                    if ((__instance.activeContent.slotType == TooltipItemManager.SlotType.GROUND) && (!item.IsNullOrDestroyed()))
                    {
                        if ((item.rarity == 7) && (item.itemType < 34))
                        {
                            int legendary_potencial_owned = Check.SavedItems(__instance.activeParameters.Item);
                            if (legendary_potencial_owned > 0)
                            {
                                if (item.legendaryPotential > legendary_potencial_owned)
                                {
                                    Image.icon.sprite = Icons.Up_icon;
                                    Image.icon.gameObject.active = true;
                                }
                                else if (item.legendaryPotential < legendary_potencial_owned)
                                {
                                    Image.icon.sprite = Icons.Down_icon;
                                    Image.icon.gameObject.active = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
