using Il2Cpp;
using Il2CppItemFiltering;
using Il2CppLE.Factions;
using Il2CppLE.Services.Visuals;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts
{
    [RegisterTypeInIl2Cpp]
    public class Refs_Manager : MonoBehaviour
    {
        public Refs_Manager(System.IntPtr ptr) : base(ptr) { }
        public static Refs_Manager instance { get; private set; }

        public static bool online = true;

        public static UIBase game_uibase = null;
        public static EpochInputManager epoch_input_manager = null; //Use to block input
        public static CharacterSelect character_select = null;
        public static SceneList scene_list = null;
        public static InventoryPanelUI InventoryPanelUI = null;
        public static EternityCachePanelUI EternityCachePanelUI = null;
        public static GameObject BlessingsPanel = null;
        public static Actor player_actor = null;
        public static ActorVisuals player_visuals = null;
        public static Il2CppLE.Data.CharacterData player_data = null;
        public static CharacterDataTracker player_data_tracker = null;
        public static PlayerHealth player_health = null;
        public static HealthPotion health_potion = null;
        public static Stats player_stats = null;
        public static GoldTracker player_gold_tracker = null;
        public static LocalTreeData player_treedata = null;
        public static CharacterClassList character_class_list = null;
        public static ExperienceTracker exp_tracker = null;
        public static GroundItemManager ground_item_manager = null;
        public static ItemContainersManager item_containers_manager = null;
        public static ItemList item_list = null;
        public static UniqueList unique_list = null;
        public static SetBonusesList set_bonuses_list = null;
        public static QuestList quest_list = null;
        public static PlayerQuestListHolder player_quest_list = null;
        public static ItemFilterManager filter_manager = null;
        public static CameraManager camera_manager = null;
        public static CraftingSlotManager craft_slot_manager = null;
        public static UIPanel craft_materials_holder = null;
        public static CraftingPanelUI crafting_panel_ui = null;
        public static ProtectionClass player_protection_class = null;
        public static GlobalDataTracker player_golbal_data_tracker = null;
        public static MonolithZoneManager monolith_zone_manager = null;
        public static MovingPlayer player_moving = null;
        public static AbilityManager ability_manager = null;
        public static FactionTracker faction_tracker = null;
        public static CharacterMutator character_mutator = null;
        public static UsingAbilityPlayer using_ability_player = null;
        public static SummonTracker summon_tracker = null;
        public static MapPanel map_panel = null;
        public static StashPanelUI stash_panel_ui = null;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if ((game_uibase.IsNullOrDestroyed()) && (!UIBase.instance.IsNullOrDestroyed())) { game_uibase = UIBase.instance; }
            if ((epoch_input_manager.IsNullOrDestroyed()) && (!EpochInputManager.instance.IsNullOrDestroyed())) { epoch_input_manager = EpochInputManager.instance; }
            if ((character_class_list.IsNullOrDestroyed()) && (!CharacterClassList.instance.IsNullOrDestroyed())) { character_class_list = CharacterClassList.instance; }
            if ((item_list.IsNullOrDestroyed()) && (!ItemList.instance.IsNullOrDestroyed())) { item_list = ItemList.instance; }
            if (unique_list.IsNullOrDestroyed())
            {
                if (UniqueList.instance.IsNullOrDestroyed()) { UniqueList.getUnique(0); }
                if (!UniqueList.instance.IsNullOrDestroyed()) { unique_list = UniqueList.instance; }
            }
            if (set_bonuses_list.IsNullOrDestroyed())
            {
                if (SetBonusesList.instance.IsNullOrDestroyed()) { SetBonusesList.getEntry(0); }
                if (!SetBonusesList.instance.IsNullOrDestroyed()) { set_bonuses_list = SetBonusesList.instance; }
            }
            if ((quest_list.IsNullOrDestroyed()) && (!QuestList.instance.IsNullOrDestroyed())) { quest_list = QuestList.instance; }
            if ((scene_list.IsNullOrDestroyed()) && (!SceneList.instance.IsNullOrDestroyed())) { scene_list = SceneList.instance; }
            if ((character_select.IsNullOrDestroyed()) && (!CharacterSelect.instance.IsNullOrDestroyed())) { character_select = CharacterSelect.instance; }
            //if ((!character_select.IsNullOrDestroyed()) && (character_select.OnOnlineTabChange.IsNullOrDestroyed())) { character_select.OnOnlineTabChange = Action_SetOnline; }
            if ((craft_slot_manager.IsNullOrDestroyed())) // && (!CraftingSlotManager.instance.IsNullOrDestroyed()))
            {
                foreach (CraftingSlotManager obj in Resources.FindObjectsOfTypeAll<CraftingSlotManager>())
                {
                    craft_slot_manager = obj;
                    break;
                }
                //craft_slot_manager = CraftingSlotManager.instance;
            }
            if (ability_manager.IsNullOrDestroyed()) { ability_manager = AbilityManager.instance; }
            if (player_data_tracker.IsNullOrDestroyed()) { player_data_tracker = PlayerFinder.getPlayerDataTracker(); }

            if (Scenes.IsGameScene())
            {
                if (!game_uibase.IsNullOrDestroyed())
                {
                    /*if ((InventoryPanelUI.IsNullOrDestroyed()))// && (!game_uibase.inventoryPanel.IsNullOrDestroyed()))
                    {
                        if (!game_uibase.inventoryPanel.instance.IsNullOrDestroyed())
                        {
                            InventoryPanelUI = game_uibase.inventoryPanel.instance.GetComponent<InventoryPanelUI>();
                        }
                    }
                    if ((EternityCachePanelUI.IsNullOrDestroyed()))// && (!game_uibase.eternityCachePanel.IsNullOrDestroyed()))
                    {
                        if (game_uibase.eternityCachePanel.instance.IsNullOrDestroyed())
                        {
                            game_uibase.eternityCachePanel.GetInstance();
                        }
                        EternityCachePanelUI = game_uibase.eternityCachePanel.instance.GetComponent<EternityCachePanelUI>();
                    }
                    if ((crafting_panel_ui.IsNullOrDestroyed())) // && (!game_uibase.craftingPanel.IsNullOrDestroyed()))
                    {
                        //if (!game_uibase.craftingPanel.instance.IsNullOrDestroyed()) { crafting_panel_ui = game_uibase.craftingPanel.instance.GetComponent<CraftingPanelUI>(); }
                    }
                    if ((craft_materials_holder.IsNullOrDestroyed()))// && (!game_uibase.craftingMaterialsPanel.IsNullOrDestroyed()))
                    {
                        //craft_materials_holder = game_uibase.craftingMaterialsPanel;
                    }*/
                    if ((BlessingsPanel.IsNullOrDestroyed()) && (!InventoryPanelUI.IsNullOrDestroyed())) { BlessingsPanel = InventoryPanelUI.blessingPanel; }
                }
                if ((InventoryPanelUI.IsNullOrDestroyed()) && (!InventoryPanelUI.instance.IsNullOrDestroyed())) { InventoryPanelUI = InventoryPanelUI.instance; }
                if ((EternityCachePanelUI.IsNullOrDestroyed()) && (!EternityCachePanelUI.instance.IsNullOrDestroyed())) { EternityCachePanelUI = EternityCachePanelUI.instance; }
                if ((crafting_panel_ui.IsNullOrDestroyed()))
                {
                    foreach (CraftingPanelUI obj in Resources.FindObjectsOfTypeAll<CraftingPanelUI>())
                    {
                        crafting_panel_ui = obj;
                        break;
                    }
                }
                if ((stash_panel_ui.IsNullOrDestroyed()) && (!StashPanelUI.Instance.IsNullOrDestroyed())) { stash_panel_ui = StashPanelUI.Instance; }

                if ((ground_item_manager.IsNullOrDestroyed()) && (!GroundItemManager.instance.IsNullOrDestroyed())) { ground_item_manager = GroundItemManager.instance; }
                if ((item_containers_manager.IsNullOrDestroyed()) && (!ItemContainersManager.Instance.IsNullOrDestroyed())) { item_containers_manager = ItemContainersManager.Instance; }
                if (player_actor.IsNullOrDestroyed()) { player_actor = PlayerFinder.getPlayerActor(); }
                if (player_visuals.IsNullOrDestroyed()) { player_visuals = PlayerFinder.getPlayerVisuals(); }
                if (player_data.IsNullOrDestroyed()) { player_data = PlayerFinder.getPlayerData(); }                
                if ((faction_tracker.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { faction_tracker = player_actor.gameObject.GetComponent<FactionTracker>(); }
                if ((player_quest_list.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { player_quest_list = player_actor.gameObject.GetComponent<PlayerQuestListHolder>(); }
                if (player_health.IsNullOrDestroyed()) { player_health = PlayerFinder.getLocalPlayerHealth(); }
                if ((player_moving.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { player_moving = player_actor.gameObject.GetComponent<MovingPlayer>(); }
                if ((player_protection_class.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { player_protection_class = player_actor.gameObject.GetComponent<ProtectionClass>(); }
                if ((using_ability_player.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { using_ability_player = player_actor.gameObject.GetComponent<UsingAbilityPlayer>(); }
                if ((summon_tracker.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { summon_tracker = player_actor.gameObject.GetComponent<SummonTracker>(); }
                if ((health_potion.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { health_potion = player_actor.gameObject.GetComponent<HealthPotion>(); }
                if ((character_mutator.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { character_mutator = player_actor.gameObject.GetComponent<CharacterMutator>(); }
                if (player_stats.IsNullOrDestroyed()) { player_stats = PlayerFinder.getLocalPlayerStats(); }
                if (exp_tracker.IsNullOrDestroyed()) { exp_tracker = PlayerFinder.getExperienceTracker(); }
                if (player_treedata.IsNullOrDestroyed()) { player_treedata = PlayerFinder.getLocalTreeData(); }
                if (player_gold_tracker.IsNullOrDestroyed()) { player_gold_tracker = PlayerFinder.getLocalGoldTracker(); }
                if (player_golbal_data_tracker.IsNullOrDestroyed()) { player_golbal_data_tracker = PlayerFinder.getGlobalDataTracker(); }
                if ((filter_manager.IsNullOrDestroyed()) && (!ItemFilterManager.Instance.IsNullOrDestroyed())) { filter_manager = ItemFilterManager.Instance; }
                if ((camera_manager.IsNullOrDestroyed()) && (!CameraManager.instance.IsNullOrDestroyed())) { camera_manager = CameraManager.instance; }
                if (map_panel.IsNullOrDestroyed() && (!MapPanel.instance.IsNullOrDestroyed()))
                {
                    map_panel = MapPanel.instance;
                    /*foreach (MapPanel obj in Resources.FindObjectsOfTypeAll<MapPanel>())
                    {
                        if (obj.name.Contains("Clone"))
                        {
                            map_panel = obj;
                            break;
                        }
                    }*/
                }
            }
            else
            {
                if (!player_data.IsNullOrDestroyed()) { player_data = null; }
                if (!map_panel.IsNullOrDestroyed()) {  map_panel = null; }
            }
        }

        /*private static readonly System.Action<bool> Action_SetOnline = new System.Action<bool>(SetOnline);
        private static void SetOnline(bool result)
        {
            result = true;
            if (!character_select.IsNullOrDestroyed()) { result = character_select.isOnlineTabShowing; }
            if (online != result)
            {
                Main.logger_instance?.Msg("Refs Manager : Online = " + result);
                online = result;
                if (!Mods_Manager.instance.IsNullOrDestroyed()) { Mods_Manager.instance.SetActive(result); }
            }
        }*/
    }
}
