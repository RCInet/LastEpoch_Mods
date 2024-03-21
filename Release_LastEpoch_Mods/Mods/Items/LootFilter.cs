using DMM;
using HarmonyLib;
using ItemFiltering;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpochMods.Mods.Items
{
    public class LootFilter
    {
        /*[HarmonyPatch(typeof(GroundItemVisuals), "initialise", new Type[] { typeof(ItemDataUnpacked), typeof(uint), typeof(GroundItemLabel), typeof(GroundItemRarityVisuals), typeof(bool) })]
        private static class GroundItemVisuals_initialise
        {
            [HarmonyPostfix]
            private static void Postfix(ref GroundItemVisuals __instance, ItemDataUnpacked __0, uint __1, ref GroundItemLabel __2, GroundItemRarityVisuals __3, bool __4)
            {
                if (!__0.isUniqueSetOrLegendary())
                {
                    ItemFilter filter = ItemFilterManager.Instance.Filter;
                    if (filter != null)
                    {
                        bool Show = false;
                        foreach (Rule rule in filter.rules)
                        {
                            if (rule.isEnabled) { if (rule.Match(__0)) { Show = true; break; } }
                        }
                        if (Show)
                        {
                            Main.logger_instance.Msg("Item Match Rules");

                            DMM.SceneChangeableDMMapIcon MapIcon = new SceneChangeableDMMapIcon(); 
                            
                            GameObject gameObject = new GameObject("Icon");
                            try
                            {
                                gameObject.transform.SetParent(LootFilter.MapIcon.transform);
                                gameObject.transform.localPosition = Vector3.zero;
                                gameObject.transform.localScale = Vector3.one;
                                Image vector2 = gameObject.AddComponent<Image>();
                                vector2.rectTransform.sizeDelta = new Vector2(24f, 24f);
                                Image image = LootFilter.MapIcon.AddComponent<Image>();
                                image.rectTransform.sizeDelta = new Vector2(24f, 24f);
                            }
                            catch { }

                            try
                            {
                                Vector3 game_objet_position = __instance.gameObject.transform.position;
                                Main.logger_instance.Msg("Game Object Position : X = " + game_objet_position.x +
                                    ", Y = " + game_objet_position.y + ", Z = " + game_objet_position.z);


                                Vector3 ui_position = DMMap.Instance.WorldtoUI(game_objet_position);
                                Main.logger_instance.Msg("UI Position : X = " + ui_position.x +
                                    ", Y = " + ui_position.y + ", Z = " + ui_position.z);

                                MapIcon = new SceneChangeableDMMapIcon();
                                MapIcon.dormantParent = __3.transform;
                                MapIcon.distance = new Vector3(0, 0, 0);
                                MapIcon.iconGO = new GameObject();
                                MapIcon.img = new UnityEngine.UI.Image();
                                MapIcon.playerPos = new Vector3(0, 0, 0);
                                MapIcon.mapPosition = ui_position;
                                //MapIcon.Start();

                            }
                            catch { Main.logger_instance.Error("Error Setup"); }
                            
                            
                            try
                            {
                                //MapIcon.img.sprite = ItemList.instance.experimentalExaltedBackgroundSprite;
                            }
                            catch { Main.logger_instance.Error("Error Sprite"); }
                            try
                            {
                                //DMMap.Instance.icons.Add(MapIcon);
                                SceneChangeableDMMapIcon __icon = UnityEngine.Object.Instantiate<SceneChangeableDMMapIcon>(MapIcon, DMMap.Instance.iconContainer.transform);
                                __icon.gameObject.SetActive(true);
                            }
                            catch { Main.logger_instance.Error("Error Add Icon to Map"); }


                            //MapIcon.img.sprite = ItemList.instance.experimentalExaltedBackgroundSprite;

                            //string lower = ItemList.instance.GetBaseTypeName((int)__0.itemType).Replace(" ", "_").ToLower();
                            //string baseNameForTooltipSprite = __0.BaseNameForTooltipSprite;
                            //Sprite sprite = Resources.Load<Sprite>(string.Concat("gear/", lower, "/", baseNameForTooltipSprite));
                            //MapIcon.GetComponent<DMMapWorldIcon>().sprite.sprite = ItemList.instance.experimentalExaltedBackgroundSprite;
                            //MapIcon.transform.GetChild(0).GetComponent<Image>().sprite = (sprite == null ? LootFilter.DefaultIcon : sprite);
                            
                        }
                    }
                    else { Main.logger_instance.Msg("No LootFilter Selected"); }
                }
            }
        }*/

        /*
        private static GameObject MapIcon;
        private static Sprite DefaultIcon;
        private static LootFilter _thistype;
        public void OnInitializeMelon()
        {
            LootFilter._thistype = this;
            ClassInjector.RegisterTypeInIl2Cpp<LootFilter.CustomIconProcessor>();
            LootFilter.MapIcon = new GameObject("kg_CustomMapIcon")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            LootFilter.MapIcon.SetActive(false);

            GameObject gameObject = new GameObject("Icon");
            gameObject.transform.SetParent(LootFilter.MapIcon.transform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            Image vector2 = gameObject.AddComponent<Image>();
            vector2.rectTransform.sizeDelta = new Vector2(24f, 24f);
            Image image = LootFilter.MapIcon.AddComponent<Image>();
            image.rectTransform.sizeDelta = new Vector2(24f, 24f);

            LootFilter.MapIcon.AddComponent<CanvasGroup>().ignoreParentGroups = true;
            GameObject gameObject1 = new GameObject("Text");
            gameObject1.transform.SetParent(LootFilter.MapIcon.transform);
            gameObject1.transform.localPosition = Vector3.zero;
            Text builtinResource = gameObject1.AddComponent<Text>();
            builtinResource.fontSize = 15;
            builtinResource.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            builtinResource.alignment = TextAnchor.MiddleLeft;
            builtinResource.rectTransform.anchoredPosition = new Vector2(64f, 0f);
            builtinResource.horizontalOverflow = HorizontalWrapMode.Overflow;
            builtinResource.verticalOverflow = VerticalWrapMode.Overflow;
            builtinResource.AddComponent<Outline>().effectColor = Color.black;
            LootFilter.MapIcon.AddComponent<LootFilter.CustomIconProcessor>();
            byte[] numArray = Convert.FromBase64String(BaseIcon.Base64);
            Texture2D texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, numArray);
            LootFilter.DefaultIcon = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
            //(new Harmony("kg.LastEpoch.FilterIcons")).PatchAll();
        }

        private class CustomIconProcessor : MonoBehaviour
        {
            public GameObject _trackable;
            private Text _text;
            private RectTransform thisTransform;
            private GroundItemLabel _label;

            private static LootFilter.CustomIconProcessor showingAffix;

            public CustomIconProcessor()
            {
            }

            private void Awake()
            {
                this._text = base.transform.GetChild(1).GetComponent<Text>();
            }

            private void FixedUpdate()
            {
                if (!this._trackable || !this._trackable.activeSelf)
                {
                    UnityEngine.Object.Destroy(base.gameObject);
                    return;
                }
                if (LootFilter.CustomIconProcessor.showingAffix == this && (!RectTransformUtility.RectangleContainsScreenPoint(this.thisTransform, Input.mousePosition) || !Input.GetKey(KeyCode.LeftShift)))
                {
                    LootFilter.CustomIconProcessor.showingAffix = null;
                    this.PointerExit();
                }
                if ((!LootFilter.CustomIconProcessor.showingAffix || LootFilter.CustomIconProcessor.showingAffix == this) && Input.GetKey(KeyCode.LeftShift) && RectTransformUtility.RectangleContainsScreenPoint(this.thisTransform, Input.mousePosition))
                {
                    LootFilter.CustomIconProcessor.showingAffix = this;
                    this.PointerEnter();
                }
                base.transform.localPosition = DMMap.Instance.WorldtoUI(this._trackable.transform.position);
            }

            public void Init(GameObject toTrack, ItemDataUnpacked item, GroundItemLabel label)
            {
                this.thisTransform = base.transform.GetComponent<RectTransform>();
                base.transform.localPosition = DMMap.Instance.WorldtoUI(toTrack.transform.position);
                this._trackable = toTrack;
                this._label = label;
            }

            private void PointerEnter()
            {
                if (this._label != null && this._label && this._label.tooltipItem)
                {
                    this._label.tooltipItem.OnPointerEnter(null);
                }
            }

            private void PointerExit()
            {
                if (this._label != null && this._label && this._label.tooltipItem)
                {
                    this._label.tooltipItem.OnPointerExit(null);
                }
            }

            public void ShowLegendaryPotential(int lp, int ww)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
                if (lp > 0)
                {
                    Text text = this._text;
                    string str = text.text;
                    defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
                    defaultInterpolatedStringHandler.AppendFormatted<int>(lp);
                    text.text = string.Concat(str, defaultInterpolatedStringHandler.ToStringAndClear());
                    this._text.color = new Color(1f, 0.5f, 0f);
                    return;
                }
                if (ww > 0)
                {
                    Text text1 = this._text;
                    string str1 = text1.text;
                    defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
                    defaultInterpolatedStringHandler.AppendFormatted<int>(ww);
                    text1.text = string.Concat(str1, defaultInterpolatedStringHandler.ToStringAndClear());
                    this._text.color = new Color(1f, 0.05f, 0.77f);
                }
            }
        }
        
        [HarmonyPatch(typeof(GroundItemVisuals), "initialise", new Type[] { typeof(ItemDataUnpacked), typeof(uint), typeof(GroundItemLabel), typeof(GroundItemRarityVisuals), typeof(bool) })]        
        private static class GroundItemVisuals_initialise_Patch2
        {
            private static void Postfix(GroundItemVisuals __instance, ItemDataUnpacked itemData, GroundItemLabel label)
            {
                ItemFilter filter = ItemFilterManager.Instance.Filter;
                if (filter != null)
                {
                    List<Rule>.Enumerator enumerator = filter.rules.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Rule current = enumerator.Current;
                        if (!current.isEnabled || current.type == Rule.RuleOutcome.HIDE || !current.nameOverride.Contains("@show") || !current.Match(itemData))
                        {
                            continue;
                        }
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LootFilter.MapIcon, DMMap.Instance.iconContainer.transform);
                        gameObject.SetActive(true);
                        gameObject.GetComponent<CustomIconProcessor>().Init(__instance.gameObject, itemData, label);
                        string lower = ItemList.instance.GetBaseTypeName((int)itemData.itemType).Replace(" ", "_").ToLower();
                        string baseNameForTooltipSprite = itemData.BaseNameForTooltipSprite;
                        if (itemData.isUniqueSetOrLegendary())
                        {
                            gameObject.GetComponent<CustomIconProcessor>().ShowLegendaryPotential((int)itemData.legendaryPotential, (int)itemData.weaversWill);
                            ushort num = itemData.uniqueID;
                            UniqueList.Entry entry = null;
                            List<UniqueList.Entry>.Enumerator enumerator1 = UniqueList.instance.uniques.GetEnumerator();
                            while (enumerator1.MoveNext())
                            {
                                UniqueList.Entry current1 = enumerator1.Current;
                                if (current1.uniqueID != num)
                                {
                                    continue;
                                }
                                entry = current1;
                                break;
                            }
                            if (entry != null)
                            {
                                lower = "uniques";
                                baseNameForTooltipSprite = entry.name.Replace(" ", "_");
                            }
                        }
                        Sprite sprite = Resources.Load<Sprite>(string.Concat("gear/", lower, "/", baseNameForTooltipSprite));
                        gameObject.GetComponent<Image>().sprite = ItemList.instance.experimentalExaltedBackgroundSprite;
                        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = (sprite == null ? LootFilter.DefaultIcon : sprite);
                        return;
                    }
                }
            }
        }
        */
    }  
}
