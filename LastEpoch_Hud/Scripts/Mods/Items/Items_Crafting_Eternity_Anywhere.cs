// https://github.com/RolandSolymosi/LastEpoch_Mods/blob/master/LastEpoch_Hud/Scripts/Mods/Items/Items_Crafting_Eternity_Anywhere.cs

using HarmonyLib;
using MelonLoader;
using UnityEngine;
using Il2Cpp;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    [RegisterTypeInIl2Cpp]
    public class Items_Crafting_Eternity_Anywhere : MonoBehaviour
    {
        public static Items_Crafting_Eternity_Anywhere instance { get; private set; }
        public Items_Crafting_Eternity_Anywhere(System.IntPtr ptr) : base(ptr) { }

        private static bool isRunning = false;
        private static EternityCachePanelUI panel = null;
        private static Button close_btn = null;
        private static Button.ButtonClickedEvent backup_event = null;
        private static readonly System.Action OpenCloseAction = new System.Action(OpenClose);

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if (Scenes.IsGameScene() && !Refs_Manager.EternityCachePanelUI.IsNullOrDestroyed())
            {
                if (panel.IsNullOrDestroyed() || close_btn.IsNullOrDestroyed())
                {
                    panel = Refs_Manager.EternityCachePanelUI;
                    GameObject CloseBtnGameObject = Functions.GetChild(panel.gameObject, "Close_Button");
                    if (!CloseBtnGameObject.IsNullOrDestroyed())
                    {
                        close_btn = CloseBtnGameObject.GetComponent<Button>();
                        backup_event = close_btn.onClick;
                    }                    
                }
                else if (Input.GetKeyDown(Save_Manager.instance.data.KeyBinds.EternityCache)) { OpenClose(); }
            }
            else
            {
                isRunning = false;
                panel = null;
                close_btn = null;
            }
        }
        private static void OpenClose()
        {
            panel.gameObject.active = !panel.gameObject.active;
            isRunning = panel.gameObject.active;
            if (isRunning)
            {                
                close_btn.onClick = new Button.ButtonClickedEvent();
                close_btn.onClick.AddListener(OpenCloseAction);
                panel.Open();
            }
            else
            {                
                close_btn.onClick = backup_event;
                panel.Close();
            }
        }

        [HarmonyPatch(typeof(EternityCachePanelUI), "seal")]
        public class EternityCachePanelUI_seal
        {
            [HarmonyPrefix]
            static bool Prefix(ref EternityCachePanelUI __instance)
            {
                bool result = false;
                if (isRunning)
                {                    
                    if (!__instance.beforeMain.IsNullOrDestroyed() && !__instance.beforeOther.IsNullOrDestroyed())
                    {
                        if (!__instance.beforeMain.Container.IsNullOrDestroyed() && !__instance.beforeOther.Container.IsNullOrDestroyed())
                        {
                            if (__instance.beforeMain.Container.GetContent().Count > 0 && __instance.beforeOther.Container.GetContent().Count > 0)
                            {
                                var unique = __instance.beforeMain.Container.GetContent()[0].data;
                                var exalted = __instance.beforeOther.Container.GetContent()[0].data;
                                if (!unique.IsNullOrDestroyed() && !exalted.IsNullOrDestroyed())
                                {
                                    unique.absorb4ModExaltedItemToBecomeLegendary(exalted);
                                    result = true;
                                }
                            }
                        }
                    }               
                }
                return result;
            }
        }
    }
}
