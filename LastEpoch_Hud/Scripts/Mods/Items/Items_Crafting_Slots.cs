using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Crafting_Slots
    {
        public class Slots
        {
            public static bool Initialized = false;
            public static bool Initializing = false;
            public static void Init()
            {                
                Initializing = true;
                GameObject main_content = Functions.GetChild(Refs_Manager.crafting_panel_ui.gameObject, "MainContent");
                if (!main_content.IsNullOrDestroyed())
                {
                    Slot0.game_object = Functions.GetChild(main_content, "ModSlot");
                    if (!Slot0.game_object.IsNullOrDestroyed())
                    {
                        Slot0.rect_transform = Slot0.game_object.GetComponent<RectTransform>();
                        Slot0.affix_slot_forge = Slot0.game_object.GetComponent<AffixSlotForge>();

                        Slot0.Shard.game_object = Functions.GetChild(Slot0.game_object, "Shard");
                        if (!Slot0.Shard.game_object.IsNullOrDestroyed())
                        {
                            Slot0.Shard.rect_transform = Slot0.Shard.game_object.GetComponent<RectTransform>();

                            Slot0.Shard.AddShardButton.game_object = Functions.GetChild(Slot0.Shard.game_object, "addShardButton");
                            if (!Slot0.Shard.AddShardButton.game_object.IsNullOrDestroyed())
                            {
                                Slot0.Shard.AddShardButton.rect_transform = Slot0.Shard.AddShardButton.game_object.GetComponent<RectTransform>();
                                Slot0.Shard.AddShardButton.canvas_renderer = Slot0.Shard.AddShardButton.game_object.GetComponent<CanvasRenderer>();
                                Slot0.Shard.AddShardButton.image = Slot0.Shard.AddShardButton.game_object.GetComponent<UnityEngine.UI.Image>();
                                Slot0.Shard.AddShardButton.button = Slot0.Shard.AddShardButton.game_object.GetComponent<UnityEngine.UI.Button>();
                                Slot0.Shard.AddShardButton.button_sounds = Slot0.Shard.AddShardButton.game_object.GetComponent<Il2CppLE.Audio.ButtonSounds>();
                            }

                            Slot0.Shard.ShardIcon.game_object = Functions.GetChild(Slot0.Shard.game_object, "ShardIcon");
                            if (!Slot0.Shard.ShardIcon.game_object.IsNullOrDestroyed())
                            {
                                Slot0.Shard.ShardIcon.rect_transform = Slot0.Shard.ShardIcon.game_object.GetComponent<RectTransform>();
                                Slot0.Shard.ShardIcon.canvas_renderer = Slot0.Shard.ShardIcon.game_object.GetComponent<CanvasRenderer>();
                                Slot0.Shard.ShardIcon.image = Slot0.Shard.ShardIcon.game_object.GetComponent<UnityEngine.UI.Image>();
                            }

                            Slot0.Shard.GlassLense.game_object = Functions.GetChild(Slot0.Shard.game_object, "GlassLense");
                            if (!Slot0.Shard.GlassLense.game_object.IsNullOrDestroyed())
                            {
                                Slot0.Shard.GlassLense.rect_transform = Slot0.Shard.GlassLense.game_object.GetComponent<RectTransform>();
                                Slot0.Shard.GlassLense.canvas_renderer = Slot0.Shard.GlassLense.game_object.GetComponent<CanvasRenderer>();
                                Slot0.Shard.GlassLense.image = Slot0.Shard.GlassLense.game_object.GetComponent<UnityEngine.UI.Image>();
                            }

                            Slot0.Shard.SLAM.game_object = Functions.GetChild(Slot0.Shard.game_object, "SLAM");
                            if (!Slot0.Shard.SLAM.game_object.IsNullOrDestroyed())
                            {
                                Slot0.Shard.SLAM.rect_transform = Slot0.Shard.SLAM.game_object.GetComponent<RectTransform>();
                                Slot0.Shard.SLAM.canvas_renderer = Slot0.Shard.SLAM.game_object.GetComponent<CanvasRenderer>();
                                Slot0.Shard.SLAM.layout_element = Slot0.Shard.SLAM.game_object.GetComponent<UnityEngine.UI.LayoutElement>();
                                Slot0.Shard.SLAM.deactivate_onenable = Slot0.Shard.SLAM.game_object.GetComponent<DeactivateOnEnable>();
                            }
                        }

                        Slot0.AvailableShardSlotType.game_object = Functions.GetChild(Slot0.game_object, "AvailableShardsofSlottedType");
                        if (!Slot0.AvailableShardSlotType.game_object.IsNullOrDestroyed())
                        {
                            Slot0.AvailableShardSlotType.rect_transform = Slot0.AvailableShardSlotType.game_object.GetComponent<RectTransform>();
                            Slot0.AvailableShardSlotType.canvas_renderer = Slot0.AvailableShardSlotType.game_object.GetComponent<CanvasRenderer>();
                            Slot0.AvailableShardSlotType.image = Slot0.AvailableShardSlotType.game_object.GetComponent<UnityEngine.UI.Image>();

                            Slot0.AvailableShardSlotType.AvailableShardCoundType.game_object = Functions.GetChild(Slot0.AvailableShardSlotType.game_object, "Available Shard Count TMP");
                            if (!Slot0.AvailableShardSlotType.AvailableShardCoundType.game_object.IsNullOrDestroyed())
                            {
                                Slot0.AvailableShardSlotType.AvailableShardCoundType.rect_transform = Slot0.AvailableShardSlotType.AvailableShardCoundType.game_object.GetComponent<RectTransform>();
                                Slot0.AvailableShardSlotType.AvailableShardCoundType.canvas_renderer = Slot0.AvailableShardSlotType.AvailableShardCoundType.game_object.GetComponent<CanvasRenderer>();
                                Slot0.AvailableShardSlotType.AvailableShardCoundType.textmeshpro_gui = Slot0.AvailableShardSlotType.AvailableShardCoundType.game_object.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
                            }
                        }

                        Slot0.UpgradeAvailable.game_object = Functions.GetChild(Slot0.game_object, "upgradeAvailable");
                        if (!Slot0.UpgradeAvailable.game_object.IsNullOrDestroyed())
                        {
                            Slot0.UpgradeAvailable.rect_transform = Slot0.UpgradeAvailable.game_object.GetComponent<RectTransform>();
                            Slot0.UpgradeAvailable.canvas_renderer = Slot0.UpgradeAvailable.game_object.GetComponent<CanvasRenderer>();
                            Slot0.UpgradeAvailable.image = Slot0.UpgradeAvailable.game_object.GetComponent<UnityEngine.UI.Image>();
                            Slot0.UpgradeAvailable.button = Slot0.UpgradeAvailable.game_object.GetComponent<UnityEngine.UI.Button>();
                            Slot0.UpgradeAvailable.upgrade_button = Slot0.UpgradeAvailable.game_object.GetComponent<CraftingUpgradeButton>();

                            Slot0.UpgradeAvailable.UpgradeAvailableIndicator.game_object = Functions.GetChild(Slot0.UpgradeAvailable.game_object, "Upgrade Available Indicator");
                            if (!Slot0.UpgradeAvailable.UpgradeAvailableIndicator.game_object.IsNullOrDestroyed())
                            {
                                Slot0.UpgradeAvailable.UpgradeAvailableIndicator.rect_transform = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.game_object.GetComponent<RectTransform>();
                                Slot0.UpgradeAvailable.UpgradeAvailableIndicator.canvas_renderer = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.game_object.GetComponent<CanvasRenderer>();
                                Slot0.UpgradeAvailable.UpgradeAvailableIndicator.image = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.game_object.GetComponent<UnityEngine.UI.Image>();
                                Slot0.UpgradeAvailable.UpgradeAvailableIndicator.layout_element = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.game_object.GetComponent<UnityEngine.UI.LayoutElement>();
                            }
                        }

                        Slot0.AffixDescHolder.game_object = Functions.GetChild(Slot0.game_object, "affixDescHolder");
                        if (!Slot0.AffixDescHolder.game_object.IsNullOrDestroyed())
                        {
                            Slot0.AffixDescHolder.rect_transform = Slot0.AffixDescHolder.game_object.GetComponent<RectTransform>();
                            Slot0.AffixDescHolder.canvas_renderer = Slot0.AffixDescHolder.game_object.GetComponent<CanvasRenderer>();
                            Slot0.AffixDescHolder.vertical_layout_group = Slot0.AffixDescHolder.game_object.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
                            Slot0.AffixDescHolder.content_size_filter = Slot0.AffixDescHolder.game_object.GetComponent<UnityEngine.UI.ContentSizeFitter>();

                            Slot0.AffixDescHolder.DropShadow.game_object = Functions.GetChild(Slot0.AffixDescHolder.game_object, "dropshadow");
                            if (!Slot0.AffixDescHolder.DropShadow.game_object.IsNullOrDestroyed())
                            {
                                Slot0.AffixDescHolder.DropShadow.rect_transform = Slot0.AffixDescHolder.DropShadow.game_object.GetComponent<RectTransform>();
                                Slot0.AffixDescHolder.DropShadow.canvas_renderer = Slot0.AffixDescHolder.DropShadow.game_object.GetComponent<CanvasRenderer>();
                                Slot0.AffixDescHolder.DropShadow.image = Slot0.AffixDescHolder.DropShadow.game_object.GetComponent<UnityEngine.UI.Image>();
                                Slot0.AffixDescHolder.DropShadow.layout_element = Slot0.AffixDescHolder.DropShadow.game_object.GetComponent<UnityEngine.UI.LayoutElement>();
                            }

                            Slot0.AffixDescHolder.TierLevel.game_object = Functions.GetChild(Slot0.AffixDescHolder.game_object, "tierLevel");
                            if (!Slot0.AffixDescHolder.TierLevel.game_object.IsNullOrDestroyed())
                            {
                                Slot0.AffixDescHolder.TierLevel.rect_transform = Slot0.AffixDescHolder.TierLevel.game_object.GetComponent<RectTransform>();
                                Slot0.AffixDescHolder.TierLevel.canvas_renderer = Slot0.AffixDescHolder.TierLevel.game_object.GetComponent<CanvasRenderer>();
                                Slot0.AffixDescHolder.TierLevel.textmeshpro_gui = Slot0.AffixDescHolder.TierLevel.game_object.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
                            }

                            Slot0.AffixDescHolder.Separator.game_object = Functions.GetChild(Slot0.AffixDescHolder.game_object, "separator");
                            if (!Slot0.AffixDescHolder.Separator.game_object.IsNullOrDestroyed())
                            {
                                Slot0.AffixDescHolder.Separator.rect_transform = Slot0.AffixDescHolder.Separator.game_object.GetComponent<RectTransform>();
                                Slot0.AffixDescHolder.Separator.canvas_renderer = Slot0.AffixDescHolder.Separator.game_object.GetComponent<CanvasRenderer>();
                                Slot0.AffixDescHolder.Separator.image = Slot0.AffixDescHolder.Separator.game_object.GetComponent<UnityEngine.UI.Image>();
                                Slot0.AffixDescHolder.Separator.layout_element = Slot0.AffixDescHolder.Separator.game_object.GetComponent<UnityEngine.UI.LayoutElement>();
                            }

                            Slot0.AffixDescHolder.AffixName.game_object = Functions.GetChild(Slot0.AffixDescHolder.game_object, "AffixName");
                            if (!Slot0.AffixDescHolder.AffixName.game_object.IsNullOrDestroyed())
                            {
                                Slot0.AffixDescHolder.AffixName.rect_transform = Slot0.AffixDescHolder.AffixName.game_object.GetComponent<RectTransform>();
                                Slot0.AffixDescHolder.AffixName.canvas_renderer = Slot0.AffixDescHolder.AffixName.game_object.GetComponent<CanvasRenderer>();
                                Slot0.AffixDescHolder.AffixName.textmeshpro_gui = Slot0.AffixDescHolder.AffixName.game_object.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
                                Slot0.AffixDescHolder.AffixName.layout_element = Slot0.AffixDescHolder.AffixName.game_object.GetComponent<UnityEngine.UI.LayoutElement>();
                            }
                        }
                    }

                    GameObject slot_1 = Functions.GetChild(main_content, "ModSlot (1)");
                    if (!slot_1.IsNullOrDestroyed()) { Slot1.GameObject = slot_1; }

                    GameObject slot_2 = Functions.GetChild(main_content, "ModSlot (2)");
                    if (!slot_2.IsNullOrDestroyed()) { Slot2.GameObject = slot_2; }

                    GameObject slot_3 = Functions.GetChild(main_content, "ModSlot (3)");
                    if (!slot_3.IsNullOrDestroyed()) { Slot3.GameObject = slot_3; ; }

                    //Slot 4
                    Slot4.GameObject = Slots.Add(main_content, 4, (slot_1.transform.position - (Slot0.game_object.transform.position - slot_1.transform.position)));

                    //Slot 5
                    Slot5.GameObject = Slots.Add(main_content, 5, (slot_3.transform.position - (slot_2.transform.position - slot_3.transform.position)));

                    GameObject seal_affix_holder = Functions.GetChild(main_content, "SealedAffixInfoHolder");
                    if (!seal_affix_holder.IsNullOrDestroyed())
                    {
                        Slots.Seal.game_object = seal_affix_holder;

                        //Move Seal
                    }
                }
                Initialized = true;
                Initializing = false;
            }
            public static GameObject Add(GameObject main_content, int slot, Vector3 position)
            {
                GameObject slot_obj = new GameObject { name = "ModSlot (" + slot + ")" };
                slot_obj.active = false;
                slot_obj.transform.SetParent(main_content.transform);
                slot_obj.transform.position = position;
                slot_obj.AddComponent<UnityEngine.RectTransform>();
                slot_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.rect_transform.offsetMin;
                slot_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.rect_transform.offsetMax;
                slot_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.rect_transform.sizeDelta;
                slot_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.rect_transform.localScale;
                slot_obj.AddComponent<AffixSlotForge>();

                GameObject shards_obj = new GameObject { name = "Shard" };
                shards_obj.active = false;
                shards_obj.transform.SetParent(slot_obj.transform);
                //shards_obj.transform.position = position - ;
                shards_obj.AddComponent<UnityEngine.RectTransform>();
                shards_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.Shard.rect_transform.offsetMin;
                shards_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.Shard.rect_transform.offsetMax;
                shards_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.Shard.rect_transform.sizeDelta;
                shards_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.Shard.rect_transform.localScale;

                GameObject add_shards_btn_obj = new GameObject { name = "addShardButton" };
                add_shards_btn_obj.active = false;
                add_shards_btn_obj.transform.SetParent(shards_obj.transform);
                //shards_obj.transform.position = position - ;
                add_shards_btn_obj.AddComponent<UnityEngine.RectTransform>();
                add_shards_btn_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.Shard.AddShardButton.rect_transform.offsetMin;
                add_shards_btn_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.Shard.AddShardButton.rect_transform.offsetMax;
                add_shards_btn_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.Shard.AddShardButton.rect_transform.sizeDelta;
                add_shards_btn_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.Shard.AddShardButton.rect_transform.localScale;
                add_shards_btn_obj.AddComponent<UnityEngine.UI.Image>();
                add_shards_btn_obj.GetComponent<UnityEngine.UI.Image>().sprite = Slot0.Shard.AddShardButton.image.sprite;
                add_shards_btn_obj.GetComponent<UnityEngine.UI.Image>().material = Slot0.Shard.AddShardButton.image.material;
                add_shards_btn_obj.AddComponent<UnityEngine.UI.Button>();
                //add_shards_btn_obj.GetComponent<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                //add_shards_btn_obj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Slot_OnClick_Action);
                add_shards_btn_obj.AddComponent<Il2CppLE.Audio.ButtonSounds>();
                //add_shards_btn_obj.GetComponent<LE.Audio.ButtonSounds>() = ModSlot.Shard.AddShardButton.button_sounds;

                GameObject shards_ico_obj = new GameObject { name = "ShardIcon" };
                shards_ico_obj.active = false;
                shards_ico_obj.transform.SetParent(shards_obj.transform);
                //shards_ico_obj.transform.position = position - shards_btn_diff;
                shards_ico_obj.AddComponent<UnityEngine.RectTransform>();
                shards_ico_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.Shard.ShardIcon.rect_transform.offsetMin;
                shards_ico_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.Shard.ShardIcon.rect_transform.offsetMax;
                shards_ico_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.Shard.ShardIcon.rect_transform.sizeDelta;
                shards_ico_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.Shard.ShardIcon.rect_transform.localScale;
                shards_ico_obj.AddComponent<UnityEngine.CanvasRenderer>();
                shards_ico_obj.AddComponent<UnityEngine.UI.Image>();
                shards_ico_obj.GetComponent<UnityEngine.UI.Image>().sprite = Slot0.Shard.ShardIcon.image.sprite;
                shards_ico_obj.GetComponent<UnityEngine.UI.Image>().material = Slot0.Shard.ShardIcon.image.material;

                GameObject glass_lense_obj = new GameObject { name = "GlassLense" };
                glass_lense_obj.active = false;
                glass_lense_obj.transform.SetParent(shards_obj.transform);
                //glass_lense_obj.transform.position = position - shards_btn_diff;
                glass_lense_obj.AddComponent<UnityEngine.RectTransform>();
                glass_lense_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.Shard.GlassLense.rect_transform.offsetMin;
                glass_lense_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.Shard.GlassLense.rect_transform.offsetMax;
                glass_lense_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.Shard.GlassLense.rect_transform.sizeDelta;
                glass_lense_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.Shard.GlassLense.rect_transform.localScale;
                glass_lense_obj.AddComponent<UnityEngine.CanvasRenderer>();
                glass_lense_obj.AddComponent<UnityEngine.UI.Image>();
                glass_lense_obj.GetComponent<UnityEngine.UI.Image>().sprite = Slot0.Shard.GlassLense.image.sprite;
                glass_lense_obj.GetComponent<UnityEngine.UI.Image>().material = Slot0.Shard.GlassLense.image.material;

                GameObject slam_obj = new GameObject { name = "SLAM" };
                slam_obj.active = false;
                slam_obj.transform.SetParent(shards_obj.transform);
                //slam_obj.transform.position = position - shards_btn_diff;
                slam_obj.AddComponent<UnityEngine.RectTransform>();
                slam_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.Shard.SLAM.rect_transform.offsetMin;
                slam_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.Shard.SLAM.rect_transform.offsetMax;
                slam_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.Shard.SLAM.rect_transform.sizeDelta;
                slam_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.Shard.SLAM.rect_transform.localScale;
                slam_obj.AddComponent<UnityEngine.CanvasRenderer>();
                slam_obj.AddComponent<UnityEngine.UI.LayoutElement>();
                slam_obj.AddComponent<DeactivateOnEnable>();


                GameObject availables_obj = new GameObject { name = "AvailableShardsofSlottedType" };
                availables_obj.active = false;
                availables_obj.transform.SetParent(slot_obj.transform);
                //availables_obj.transform.position = position - shards_diff;
                availables_obj.AddComponent<UnityEngine.RectTransform>();
                availables_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.AvailableShardSlotType.rect_transform.offsetMin;
                availables_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.AvailableShardSlotType.rect_transform.offsetMax;
                availables_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.AvailableShardSlotType.rect_transform.sizeDelta;
                availables_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.AvailableShardSlotType.rect_transform.localScale;
                availables_obj.AddComponent<UnityEngine.CanvasRenderer>();
                availables_obj.AddComponent<UnityEngine.UI.Image>();
                availables_obj.GetComponent<UnityEngine.UI.Image>().sprite = Slot0.AvailableShardSlotType.image.sprite;
                availables_obj.GetComponent<UnityEngine.UI.Image>().material = Slot0.AvailableShardSlotType.image.material;

                GameObject availables_count_obj = new GameObject { name = "Available Shard Count TMP" };
                availables_count_obj.active = false;
                availables_count_obj.transform.SetParent(availables_obj.transform);
                //availables_obj.transform.position = position - ;
                availables_count_obj.AddComponent<UnityEngine.RectTransform>();
                availables_count_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.AvailableShardSlotType.AvailableShardCoundType.rect_transform.offsetMin;
                availables_count_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.AvailableShardSlotType.AvailableShardCoundType.rect_transform.offsetMax;
                availables_count_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.AvailableShardSlotType.AvailableShardCoundType.rect_transform.sizeDelta;
                availables_count_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.AvailableShardSlotType.AvailableShardCoundType.rect_transform.localScale;
                availables_count_obj.AddComponent<UnityEngine.CanvasRenderer>();
                availables_count_obj.AddComponent<Il2CppTMPro.TextMeshProUGUI>();

                GameObject active_pathing_obj = new GameObject { name = "activePathing" };
                active_pathing_obj.active = false;
                active_pathing_obj.transform.SetParent(slot_obj.transform);
                //active_pathing_obj.transform.position = position - ;
                active_pathing_obj.AddComponent<UnityEngine.RectTransform>();
                active_pathing_obj.AddComponent<UnityEngine.CanvasRenderer>();
                active_pathing_obj.AddComponent<UnityEngine.UI.Image>();
                active_pathing_obj.AddComponent<UnityEngine.CanvasGroup>();


                GameObject upgrade_obj = new GameObject { name = "upgradeAvailable" };
                upgrade_obj.active = false;
                upgrade_obj.transform.SetParent(slot_obj.transform);
                //upgrade_obj.transform.position = position - ;
                upgrade_obj.AddComponent<UnityEngine.RectTransform>();
                upgrade_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.UpgradeAvailable.rect_transform.offsetMin;
                upgrade_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.UpgradeAvailable.rect_transform.offsetMax;
                upgrade_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.UpgradeAvailable.rect_transform.sizeDelta;
                upgrade_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.UpgradeAvailable.rect_transform.localScale;
                upgrade_obj.AddComponent<UnityEngine.CanvasRenderer>();
                upgrade_obj.AddComponent<UnityEngine.UI.Image>();
                upgrade_obj.GetComponent<UnityEngine.UI.Image>().sprite = Slot0.UpgradeAvailable.image.sprite;
                upgrade_obj.GetComponent<UnityEngine.UI.Image>().material = Slot0.UpgradeAvailable.image.material;
                upgrade_obj.AddComponent<UnityEngine.UI.Button>();
                upgrade_obj.AddComponent<CraftingUpgradeButton>();

                GameObject upgrade_indicator_obj = new GameObject { name = "Upgrade Available Indicator" };
                upgrade_indicator_obj.active = false;
                upgrade_indicator_obj.transform.SetParent(upgrade_obj.transform);
                //upgrade_indicator_obj.transform.position = position - ;
                upgrade_indicator_obj.AddComponent<UnityEngine.RectTransform>();
                upgrade_indicator_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.rect_transform.offsetMin;
                upgrade_indicator_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.rect_transform.offsetMax;
                upgrade_indicator_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.rect_transform.sizeDelta;
                upgrade_indicator_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.rect_transform.localScale;
                upgrade_indicator_obj.AddComponent<UnityEngine.CanvasRenderer>();
                upgrade_indicator_obj.AddComponent<UnityEngine.UI.Image>();
                upgrade_indicator_obj.GetComponent<UnityEngine.UI.Image>().sprite = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.image.sprite;
                upgrade_indicator_obj.GetComponent<UnityEngine.UI.Image>().material = Slot0.UpgradeAvailable.UpgradeAvailableIndicator.image.material;
                upgrade_indicator_obj.AddComponent<UnityEngine.UI.LayoutElement>();

                GameObject affix_desc_obj = new GameObject { name = "affixDescHolder" };
                affix_desc_obj.active = false;
                affix_desc_obj.transform.SetParent(slot_obj.transform);
                //affix_desc_obj.transform.position = position - shards_diff;
                affix_desc_obj.AddComponent<UnityEngine.RectTransform>();
                affix_desc_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.AffixDescHolder.rect_transform.offsetMin;
                affix_desc_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.AffixDescHolder.rect_transform.offsetMax;
                affix_desc_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.AffixDescHolder.rect_transform.sizeDelta;
                affix_desc_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.AffixDescHolder.rect_transform.localScale;
                affix_desc_obj.AddComponent<UnityEngine.CanvasRenderer>();
                affix_desc_obj.AddComponent<UnityEngine.UI.LayoutGroup>();
                affix_desc_obj.AddComponent<UnityEngine.UI.ContentSizeFitter>();

                GameObject shadow_obj = new GameObject { name = "dropshadow" };
                shadow_obj.active = false;
                shadow_obj.transform.SetParent(affix_desc_obj.transform);
                //shadow_obj.transform.position = position - shards_diff;
                shadow_obj.AddComponent<UnityEngine.RectTransform>();
                shadow_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.AffixDescHolder.DropShadow.rect_transform.offsetMin;
                shadow_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.AffixDescHolder.DropShadow.rect_transform.offsetMax;
                shadow_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.AffixDescHolder.DropShadow.rect_transform.sizeDelta;
                shadow_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.AffixDescHolder.DropShadow.rect_transform.localScale;
                shadow_obj.AddComponent<UnityEngine.CanvasRenderer>();
                shadow_obj.AddComponent<UnityEngine.UI.Image>();
                shadow_obj.GetComponent<UnityEngine.UI.Image>().sprite = Slot0.AffixDescHolder.DropShadow.image.sprite;
                shadow_obj.GetComponent<UnityEngine.UI.Image>().material = Slot0.AffixDescHolder.DropShadow.image.material;
                shadow_obj.AddComponent<UnityEngine.UI.LayoutElement>();


                GameObject tier_obj = new GameObject { name = "tierLevel" };
                tier_obj.active = false;
                tier_obj.transform.SetParent(affix_desc_obj.transform);
                //tier_obj.transform.position = position - shards_diff;
                tier_obj.AddComponent<UnityEngine.RectTransform>();
                tier_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.AffixDescHolder.TierLevel.rect_transform.offsetMin;
                tier_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.AffixDescHolder.TierLevel.rect_transform.offsetMax;
                tier_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.AffixDescHolder.TierLevel.rect_transform.sizeDelta;
                tier_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.AffixDescHolder.TierLevel.rect_transform.localScale;
                tier_obj.AddComponent<UnityEngine.CanvasRenderer>();
                tier_obj.AddComponent<Il2CppTMPro.TextMeshProUGUI>();
                if (slot == 4) { Slot4.AffixTier = tier_obj.GetComponent<Il2CppTMPro.TextMeshProUGUI>(); }
                else if (slot == 5) { Slot5.AffixTier = tier_obj.GetComponent<Il2CppTMPro.TextMeshProUGUI>(); }

                GameObject separator_obj = new GameObject { name = "separator" };
                separator_obj.active = false;
                separator_obj.transform.SetParent(affix_desc_obj.transform);
                //separator_obj.transform.position = position - shards_diff;
                separator_obj.AddComponent<UnityEngine.RectTransform>();
                separator_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.AffixDescHolder.Separator.rect_transform.offsetMin;
                separator_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.AffixDescHolder.Separator.rect_transform.offsetMax;
                separator_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.AffixDescHolder.Separator.rect_transform.sizeDelta;
                separator_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.AffixDescHolder.Separator.rect_transform.localScale;
                separator_obj.AddComponent<UnityEngine.CanvasRenderer>();
                separator_obj.AddComponent<UnityEngine.UI.Image>();
                separator_obj.GetComponent<UnityEngine.UI.Image>().sprite = Slot0.AffixDescHolder.Separator.image.sprite;
                separator_obj.GetComponent<UnityEngine.UI.Image>().material = Slot0.AffixDescHolder.Separator.image.material;
                separator_obj.AddComponent<UnityEngine.UI.LayoutElement>();

                GameObject affix_name_obj = new GameObject { name = "AffixName" };
                affix_name_obj.active = false;
                affix_name_obj.transform.SetParent(affix_desc_obj.transform);
                //affix_name_obj.transform.position = position - shards_diff;
                affix_name_obj.AddComponent<UnityEngine.RectTransform>();
                affix_name_obj.GetComponent<UnityEngine.RectTransform>().offsetMin = Slot0.AffixDescHolder.AffixName.rect_transform.offsetMin;
                affix_name_obj.GetComponent<UnityEngine.RectTransform>().offsetMax = Slot0.AffixDescHolder.AffixName.rect_transform.offsetMax;
                affix_name_obj.GetComponent<UnityEngine.RectTransform>().sizeDelta = Slot0.AffixDescHolder.AffixName.rect_transform.sizeDelta;
                affix_name_obj.GetComponent<UnityEngine.RectTransform>().localScale = Slot0.AffixDescHolder.AffixName.rect_transform.localScale;
                affix_name_obj.AddComponent<UnityEngine.CanvasRenderer>();
                affix_name_obj.AddComponent<Il2CppTMPro.TextMeshProUGUI>();
                affix_name_obj.AddComponent<UnityEngine.UI.LayoutElement>();
                if (slot == 4) { Slot4.AffixName = affix_name_obj.GetComponent<Il2CppTMPro.TextMeshProUGUI>(); }
                else if (slot == 5) { Slot5.AffixName = affix_name_obj.GetComponent<Il2CppTMPro.TextMeshProUGUI>(); }

                //Main.logger_instance.Msg("Init AffixSlotForge");
                AffixSlotForge asf = slot_obj.GetComponent<AffixSlotForge>();
                asf.background = affix_desc_obj;
                asf.countTMP = availables_count_obj.GetComponent<TextMeshProUGUI>();
                asf.glassLense = glass_lense_obj;
                asf.glowPath = active_pathing_obj.GetComponent<UnityEngine.UI.Image>();
                asf.nameTMP = affix_name_obj.GetComponent<TextMeshProUGUI>();
                asf.shardCountHolder = availables_obj;
                asf.shardIcon = shards_ico_obj.GetComponent<UnityEngine.UI.Image>();
                if (slot == 4) { asf.slotID = AffixSlotForge.AffixSlotID.PREFIX_TWO; }
                else if (slot == 5) { asf.slotID = AffixSlotForge.AffixSlotID.SUFFIX_TWO; }
                asf.tierTMP = tier_obj.GetComponent<TextMeshProUGUI>();
                asf.tierVFXObject = slam_obj;
                asf.upgradeButton = upgrade_obj.GetComponent<CraftingUpgradeButton>();
                
                return slot_obj;
            }
            public static void UpdateSlots(ItemData item)
            {
                /*int nb_prefix = 0;
                int nb_suffix = 0;
                if (!item.IsNullOrDestroyed())
                {
                    foreach (ItemAffix affix in item.affixes)
                    {
                        if (!affix.isSealedAffix)
                        {
                            if (affix.affixType == AffixList.AffixType.PREFIX)
                            {
                                nb_prefix++;
                                if (nb_prefix == 3)
                                {
                                    Slots.Slot4.AffixTier.text = (affix.affixTier + 1).ToString();
                                    Slots.Slot4.AffixName.text = affix.affixName;
                                }
                            }
                            else if (affix.affixType == AffixList.AffixType.SUFFIX)
                            {
                                nb_suffix++;
                                if (nb_suffix == 3)
                                {
                                    Slots.Slot5.AffixTier.text = (affix.affixTier + 1).ToString();
                                    Slots.Slot5.AffixName.text = affix.affixName;
                                }
                            }
                        }
                    }

                    if (nb_prefix < 2) { Slot4.GameObject.active = false; }
                    else if (nb_prefix == 2)
                    {
                        Slot4.GameObject.active = false;                     //Show Slot 5
                        Get_ButtonFromSlot(4).enabled = true;                               //Enable btn (add affix)
                        Get_AffixHolder(4).active = false;                                  //Hide Name, Text
                        Get_ImageFromSlot(4).gameObject.active = false;                     //Hide affix icon
                        Get_GlassLenseFromSlot(4).active = false;                           //Hide glass lense
                        Get_UpgradeAvailable(4).active = false;                             //Hide upgrade button
                        Get_UpgradeAvailableIndicator(4).active = false;                     //Hide upgrade indicator
                    }
                    else if (nb_prefix > 2)
                    {
                        Get_ButtonFromSlot(4).gameObject.active = true;                     //Show Slot 5
                        Get_ButtonFromSlot(4).enabled = false;                              //Disable btn
                        Get_UpgradeAvailable(4).active = true;                             //Show upgrade button
                                                                                           //Get_UpgradeAvailableIndicator(4).active = true;                     //Show upgrade indicator

                        slot5_Name_Text.text = Slot5_Name;                                  //Set Name
                        slot5_Tier_Text.text = "T" + (Slot5_Tier + 1);                      //Set Tier
                        Get_AffixHolder(4).active = true;                                   //Show Text

                        UnityEngine.UI.Image img = Get_ImageFromSlot(4);
                        img.sprite = Get_AffixImgFromAffixList(Slot5_Id);                   //Set Affix icon
                        img.gameObject.active = true;                                       //Enable Affix icon

                        Get_GlassLenseFromSlot(4).active = true;                            //Show Glass Lense
                    }

                    if (nb_suffix < 2) { ShowSlot(5, false); }
                    else if (nb_suffix == 2)
                    {
                        Get_ButtonFromSlot(5).gameObject.active = true;                     //Show Shard
                        Get_ButtonFromSlot(5).enabled = true;                               //Enable btn (add affix)
                        Get_AffixHolder(5).active = false;                                  //Hide Name, Text
                        Get_ImageFromSlot(5).gameObject.active = false;                     //Hide affix icon
                        Get_GlassLenseFromSlot(5).active = false;                           //Hide glass lense
                        Get_UpgradeAvailable(5).active = false;                             //Hide upgrade button
                        Get_UpgradeAvailableIndicator(5).active = false;                     //Hide upgrade indicator
                    }
                    else if (nb_suffix > 2)
                    {
                        Get_ButtonFromSlot(5).gameObject.active = true;                     //Show Shard
                        Get_ButtonFromSlot(5).enabled = false;                              //Disable btn
                        Get_UpgradeAvailable(5).active = true;                             //Show upgrade button
                                                                                           //Get_UpgradeAvailableIndicator(5).active = true;                     //Show upgrade indicator

                        slot5_Name_Text.text = Slot6_Name;                                  //Set Name
                        slot5_Tier_Text.text = "T" + (Slot6_Tier + 1);                      //Set Tier
                        Get_AffixHolder(5).active = true;                                   //Show Text

                        UnityEngine.UI.Image img = Get_ImageFromSlot(5);
                        img.sprite = Get_AffixImgFromAffixList(Slot5_Id);                   //Set Affix icon
                        img.gameObject.active = true;                                       //Enable Affix icon

                        Get_GlassLenseFromSlot(5).active = true;                            //Show Glass Lense
                    }
                }*/
            }

            public class Slot0
            {
                public static GameObject game_object;
                public static RectTransform rect_transform;
                public static AffixSlotForge affix_slot_forge;

                public class Shard
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;

                    public class AddShardButton
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static UnityEngine.UI.Image image;
                        public static UnityEngine.UI.Button button;
                        public static Il2CppLE.Audio.ButtonSounds button_sounds;
                    }
                    public class ShardIcon
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static UnityEngine.UI.Image image;
                    }
                    public class GlassLense
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static UnityEngine.UI.Image image;
                    }
                    public class SLAM
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static UnityEngine.UI.LayoutElement layout_element;
                        public static DeactivateOnEnable deactivate_onenable;
                    }
                }
                public class AvailableShardSlotType
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.Image image;

                    public class AvailableShardCoundType
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static Il2CppTMPro.TextMeshProUGUI textmeshpro_gui;
                    }
                }
                public class UpgradeAvailable
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.Image image;
                    public static UnityEngine.UI.Button button;
                    public static CraftingUpgradeButton upgrade_button;

                    public class UpgradeAvailableIndicator
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static UnityEngine.UI.Image image;
                        public static UnityEngine.UI.LayoutElement layout_element;
                    }
                }
                public class AffixDescHolder
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.VerticalLayoutGroup vertical_layout_group;
                    public static UnityEngine.UI.ContentSizeFitter content_size_filter;

                    public class DropShadow
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static UnityEngine.UI.Image image;
                        public static UnityEngine.UI.LayoutElement layout_element;
                    }
                    public class TierLevel
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static Il2CppTMPro.TextMeshProUGUI textmeshpro_gui;
                    }
                    public class Separator
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static UnityEngine.UI.Image image;
                        public static UnityEngine.UI.LayoutElement layout_element;
                    }
                    public class AffixName
                    {
                        public static GameObject game_object;
                        public static RectTransform rect_transform;
                        public static UnityEngine.CanvasRenderer canvas_renderer;
                        public static Il2CppTMPro.TextMeshProUGUI textmeshpro_gui;
                        public static UnityEngine.UI.LayoutElement layout_element;
                    }
                }
            }
            public class Slot1
            {
                public static GameObject GameObject;
            }
            public class Slot2
            {
                public static GameObject GameObject;
            }
            public class Slot3
            {
                public static GameObject GameObject;
            }
            public class Slot4
            {
                public static GameObject GameObject;
                public static Il2CppTMPro.TextMeshProUGUI AffixName;
                public static Il2CppTMPro.TextMeshProUGUI AffixTier;
            }
            public class Slot5
            {
                public static GameObject GameObject;
                public static Il2CppTMPro.TextMeshProUGUI AffixName;
                public static Il2CppTMPro.TextMeshProUGUI AffixTier;
            }
            public class Seal
            {
                public static GameObject game_object;
            }
        }

        /*public class ModSlot
        {
            public static GameObject game_object;
            public static RectTransform rect_transform;
            public static AffixSlotForge affix_slot_forge;

            public class Shard
            {
                public static GameObject game_object;
                public static RectTransform rect_transform;

                public class AddShardButton
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.Image image;
                    public static UnityEngine.UI.Button button;
                    public static LE.Audio.ButtonSounds button_sounds;
                }
                public class ShardIcon
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.Image image;
                }
                public class GlassLense
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.Image image;
                }
                public class SLAM
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.LayoutElement layout_element;
                    public static DeactivateOnEnable deactivate_onenable;
                }
            }
            public class AvailableShardSlotType
            {
                public static GameObject game_object;
                public static RectTransform rect_transform;
                public static UnityEngine.CanvasRenderer canvas_renderer;
                public static UnityEngine.UI.Image image;

                public class AvailableShardCoundType
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static TMPro.TextMeshProUGUI textmeshpro_gui;
                }
            }
            public class UpgradeAvailable
            {
                public static GameObject game_object;
                public static RectTransform rect_transform;
                public static UnityEngine.CanvasRenderer canvas_renderer;
                public static UnityEngine.UI.Image image;
                public static UnityEngine.UI.Button button;
                public static CraftingUpgradeButton upgrade_button;

                public class UpgradeAvailableIndicator
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.Image image;
                    public static UnityEngine.UI.LayoutElement layout_element;
                }
            }
            public class AffixDescHolder
            {
                public static GameObject game_object;
                public static RectTransform rect_transform;
                public static UnityEngine.CanvasRenderer canvas_renderer;
                public static UnityEngine.UI.VerticalLayoutGroup vertical_layout_group;
                public static UnityEngine.UI.ContentSizeFitter content_size_filter;

                public class DropShadow
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.Image image;
                    public static UnityEngine.UI.LayoutElement layout_element;
                }
                public class TierLevel
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static TMPro.TextMeshProUGUI textmeshpro_gui;
                }
                public class Separator
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static UnityEngine.UI.Image image;
                    public static UnityEngine.UI.LayoutElement layout_element;
                }
                public class AffixName
                {
                    public static GameObject game_object;
                    public static RectTransform rect_transform;
                    public static UnityEngine.CanvasRenderer canvas_renderer;
                    public static TMPro.TextMeshProUGUI textmeshpro_gui;
                    public static UnityEngine.UI.LayoutElement layout_element;
                }
            }
        }*/
    }
}
