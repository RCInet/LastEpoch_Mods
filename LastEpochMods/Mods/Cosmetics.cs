using UnityEngine;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Cosmetics
    {
        public class MtxStore
        {
            private static UnityEngine.GameObject game_object = null;
            public static bool IsOpen = false;
            public static void Get()
            {
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
                {
                    if (obj.name == "MTXShopPanel(Clone)")
                    {
                        game_object = obj.TryCast<UnityEngine.GameObject>().gameObject;
                        break;
                    }
                }
            }
            public static void Open()
            {
                if (game_object != null)
                {                    
                    Hide_Unwanted();
                    Show_AllFilters();
                    game_object.active = true;
                    IsOpen = true;
                }
            }
            public static void Close()
            {
                if (game_object != null)
                {
                    game_object.active = false;
                    IsOpen = false;
                }
            }
            private static void Hide_Unwanted()
            {
                UnityEngine.GameObject header = null;
                for (int i = 0; i < game_object.transform.childCount; i++)
                {
                    string name = game_object.transform.GetChild(i).gameObject.name;
                    if (name == "Header")
                    {
                        header = game_object.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                if (header != null)
                {
                    for (int i = 0; i < header.transform.childCount; i++)
                    {
                        string name = header.transform.GetChild(i).gameObject.name;
                        if ((name == "Armory") || (name == "GetPoints") || (name == "EP"))
                        {
                            header.transform.GetChild(i).gameObject.active = false;                            
                        }
                    }
                }
            }
            private static void Show_AllFilters()
            {
                GameObject panels = null;
                if (game_object != null)
                {
                    for (int i = 0; i < game_object.transform.childCount; i++)
                    {
                        string name = game_object.transform.GetChild(i).gameObject.name;
                        if (name == "Panels")
                        {
                            panels = game_object.transform.GetChild(i).gameObject;
                            break;
                        }
                    }
                }
                GameObject allpanels = null;
                if (panels != null)
                {
                    for (int i = 0; i < panels.TryCast<UnityEngine.GameObject>().transform.childCount; i++)
                    {
                        string name = panels.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject.name;
                        if (name == "All Panel")
                        {
                            allpanels = panels.transform.GetChild(i).gameObject;
                            break;
                        }
                    }
                }
                GameObject filters = null;
                if (allpanels != null)
                {
                    for (int i = 0; i < allpanels.TryCast<UnityEngine.GameObject>().transform.childCount; i++)
                    {
                        string name = allpanels.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject.name;
                        if (name == "Filters")
                        {
                            filters = allpanels.transform.GetChild(i).gameObject;
                            break;
                        }
                    }
                }
                GameObject CategorieFilters = null;
                if (filters != null)
                {
                    for (int i = 0; i < filters.TryCast<UnityEngine.GameObject>().transform.childCount; i++)
                    {
                        string name = filters.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject.name;
                        if (name == "Category Filters")
                        {
                            CategorieFilters = filters.transform.GetChild(i).gameObject;
                            break;
                        }
                    }
                }
                if (CategorieFilters != null)
                {
                    for (int i = 0; i < CategorieFilters.TryCast<UnityEngine.GameObject>().transform.childCount; i++)
                    {
                        string obj_name = CategorieFilters.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject.name;
                        if (obj_name != "FilterHeader")
                        {
                            GameObject g = CategorieFilters.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject;
                            g.GetComponent<MTXFilterToggle>().disabled = false;
                        }
                    }
                }
            }            
        }
        public class Manager
        {
            public static CosmeticsManager __instance = null;
        }
        public class Inventory
        {
            public static InventoryPanelUI __instance = null;
            public static TabUIElement tabUIElement = null;
            public static UnityEngine.CanvasGroup canvasGroup = null;
            public static bool IsOpen = false;
            public static void Open()
            {
                if (__instance != null)
                {
                    __instance.gameObject.active = true;
                    Panel.IsOpen = true;
                }
            }
            public static void Close()
            {
                if (__instance != null)
                {
                    __instance.gameObject.active = false;
                    Panel.IsOpen = false;
                }
            }            
            public static void ShowHideTab()
            {
                bool disable = true;
                if (Config.Data.mods_config.character.cosmetic.Enable_Cosmetic_Btn) { disable = false; }
                if (tabUIElement != null) { tabUIElement.isDisabled = disable; }
                if (canvasGroup != null) { canvasGroup.enabled = disable; }
            }
        }
        public class Panel
        {
            public static CosmeticPanelUI __instance = null;
            public static bool IsOpen = false;
            public static void Hide_Text_GetPoint(CosmeticPanelUI instance)
            {
                if (instance != null)
                {
                    for (int i = 0; i < instance.gameObject.TryCast<UnityEngine.GameObject>().transform.childCount; i++)
                    {
                        string obj_name = instance.gameObject.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject.name;
                        if (obj_name == "EP")
                        {
                            instance.gameObject.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject.active = false;
                            break;
                        }
                    }
                }
            }
            public static void Hide_Btn_GetPoint(CosmeticPanelUI instance)
            {
                if (instance != null) { instance.getCoinsButton.gameObject.active = false; }
            }
            public static void Hide_Btn_OpenShop(CosmeticPanelUI instance)
            {
                if (instance != null) { instance.openShopButton.gameObject.active = false; }
            }
        }
        public class ItemSlot
        {
            public static CosmeticItemSlot selected_slot;
        }
    }
}
