using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Drop_Rarity
    {
        public static float R = 1f;
        public static float G = 0.416666f;
        public static float B = 0f;
        public static float A = 1f;
        public static Color color;
        public static Texture2D texture;
        public static Sprite sprite;

        public static bool CanRun()
        {
            bool res = false;
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed()) { res = true; }
            }

            return res;
        }
        public static void Update_ColorAndTexture()
        {
            color = new Color(R, G, B, A);
            if (texture.IsNullOrDestroyed()) { Initialize_Texture(color); }
            if (!texture.IsNullOrDestroyed())
            {
                if (sprite.IsNullOrDestroyed()) { Initialize_Sprite(texture); }
                if (sprite.IsNullOrDestroyed()) { Main.logger_instance.Error("Sprite create Fail"); }
            }
            else { Main.logger_instance.Error("Can't create sprite, texture is null"); }
        }
        public static void Initialize_Texture(Color color)
        {
            texture = new Texture2D(100, 100);
            Color[] pixels = new Color[100 * 100];
            for (int i = 0; i < pixels.Length; i++) { pixels[i] = color; }
            texture.SetPixels(pixels);
            texture.Apply();
        }
        public static void Initialize_Sprite(Texture2D texture)
        {
            sprite = Sprite.Create(new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2), 100.0f, texture);
        }
        public static bool IsNewRarity(ItemDataUnpacked item)
        {
            int nb_affix = item.affixes.Count;
            if ((!item.IsNullOrDestroyed()) && (nb_affix > 4)) { return true; }
            else {  return false; }
        }

        //Unlock rarity for 5 and 6 affixs
        [HarmonyPatch(typeof(GenerateItems), "RollRarity")]
        public class GenerateItems_RollRarity
        {
            [HarmonyPrefix]
            static bool Prefix(ref byte __result, int __0)
            {
                if (CanRun())
                {
                    if (Save_Manager.instance.data.Items.Drop.Enable_ForceUnique) { __result = 7; return false; }
                    else if (Save_Manager.instance.data.Items.Drop.Enable_ForceSet) { __result = 8; return false; }
                    else if (Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary) { __result = 9; return false; }
                    else if (Save_Manager.instance.data.Items.Drop.Enable_AffixCount)
                    {
                        int min = (int)Save_Manager.instance.data.Items.Drop.AffixCount_Min;
                        int max = (int)Save_Manager.instance.data.Items.Drop.AffixCount_Max;
                        if ((min == 0) && (max == 4)) { return true; }
                        else
                        {                            
                            if (min < 0) { min = 0; Save_Manager.instance.data.Items.Drop.AffixCount_Min = 0f; }
                            else if (min > 6) { min = 6; Save_Manager.instance.data.Items.Drop.AffixCount_Min = 6f; }
                            if (max < 0) { max = 0; Save_Manager.instance.data.Items.Drop.AffixCount_Max = 0f; }
                            else if (max > 6) { max = 6; Save_Manager.instance.data.Items.Drop.AffixCount_Max = 6f; }
                            __result = (byte)UnityEngine.Random.RandomRangeInt(min, (max + 1)); //Should be replace
                            return false;
                        }
                    }
                    else { return true; }
                }
                else { return true; }                
            }
        }

        //New rarity on Tooltip
        private static bool UpdateTooltip = false;
        private static bool ItemBackground_Done = false;
        private static bool Ornamentation_Done = false;

        [HarmonyPatch(typeof(TooltipItemManager), "OpenTooltip", new System.Type[] { typeof(ItemDataUnpacked), typeof(TooltipItemManager.SlotType), typeof(Vector2), typeof(Vector3), typeof(GameObject) })]
        public class TooltipItemManager_OpenTooltip
        {
            [HarmonyPostfix]
            static void Postfix(ref TooltipItemManager __instance)
            {
                bool res = false;
                if (Scenes.IsGameScene())
                {
                    if (!__instance.activeParameters.IsNullOrDestroyed())
                    {
                        if (IsNewRarity(__instance.activeParameters.Item)) { res = true; }
                    }
                }
                UpdateTooltip = res;
            }
        }

        [HarmonyPatch(typeof(TooltipItemManager), "CloseTooltip")]
        public class TooltipItemManager_CloseTooltip
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                UpdateTooltip = false;
                ItemBackground_Done = false;
                Ornamentation_Done = false;
            }
        }
            
        [HarmonyPatch(typeof(UITooltipItem), "OnUpdateTick")]
        public class UITooltipItem_OnUpdateTick
        {
            [HarmonyPostfix]
            static void Postfix(ref UITooltipItem __instance)
            {
                if ((Scenes.IsGameScene()) && (UpdateTooltip))
                {
                    Main.logger_instance.Msg("UITooltipItem:OnUpdateTick");
                    Update_ColorAndTexture();

                    if (!__instance.itemBackground.IsNullOrDestroyed())
                    {
                        if (__instance.itemBackground.color != color) { __instance.itemBackground.color = color; }
                        if (__instance.itemBackground.color == color) { ItemBackground_Done = true; }
                    }

                    if (!__instance.ornamentation.IsNullOrDestroyed())
                    {
                        if (__instance.ornamentation.color != color) { __instance.ornamentation.color = color; }
                        if (__instance.ornamentation.color == color) { Ornamentation_Done = true; }
                    }

                    if ((ItemBackground_Done) && (Ornamentation_Done)) { UpdateTooltip = false; }
                }
            }
        }

        //New rarity on Ground
        [HarmonyPatch(typeof(GroundItemLabel), "initialise", new System.Type[] { typeof(GroundItemVisuals) })]
        public class GroundItemLabel_initialise
        {
            [HarmonyPostfix]
            static void Postfix(ref GroundItemLabel __instance, ref GroundItemVisuals __0)
            {
                if (IsNewRarity(__0.itemData))
                {
                    Main.logger_instance.Msg("GroundItemLabel:initialise");
                    Update_ColorAndTexture();

                    //Text Color
                    TMPro.TextMeshProUGUI text = __instance.itemText;
                    text.faceColor = color; //Text color

                    GameObject image_obj = Functions.GetChild(__instance.gameObject, "Image");
                    if (!image_obj.IsNullOrDestroyed()) { image_obj.active = true; }

                    //Border
                    /*UnityEngine.UI.Image background_image = __instance.borderBase.GetComponent<UnityEngine.UI.Image>();
                    background_image.sprite = sprite;
                    background_image.color = color;*/
                }
            }
        }
    }
}
