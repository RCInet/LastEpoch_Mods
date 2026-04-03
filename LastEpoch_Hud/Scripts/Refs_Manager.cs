using Il2Cpp;
using Il2CppItemFiltering;
using Il2CppLE.Factions;
//using Il2CppLE.Services.Visuals;
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
        public static EpochInputManager epoch_input_manager = null;
        public static SceneList scene_list = null;
        public static InventoryPanelUI InventoryPanelUI = null;
        public static GameObject BlessingsPanel = null;
        public static Actor player_actor = null;
        public static PlayerSpawnManager player_spawn_manager = null;
        //public static ActorVisuals player_visuals = null;
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
        public static QuestList quest_list = null;
        public static ItemFilterManager filter_manager = null;
        public static CameraManager camera_manager = null;
        //public static UIPanel craft_materials_holder = null;
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
            if ((epoch_input_manager.IsNullOrDestroyed()) && (!EpochInputManager.instance.IsNullOrDestroyed())) { epoch_input_manager = EpochInputManager.instance; }                               //Used to block input
            if ((character_class_list.IsNullOrDestroyed()) && (!CharacterClassList.instance.IsNullOrDestroyed())) { character_class_list = CharacterClassList.instance; }                           //Hud, Maxroll
            if ((item_list.IsNullOrDestroyed()) && (!ItemList.instance.IsNullOrDestroyed())) { item_list = ItemList.instance; }                                                                     //Hud, Blessings, Materials, Req, Sockets, NewItems
            if (unique_list.IsNullOrDestroyed())
            {
                if (UniqueList.instance.IsNullOrDestroyed()) { UniqueList.getUnique(0); }                                                                                                           //Force initialize Unique list
                if (!UniqueList.instance.IsNullOrDestroyed()) { unique_list = UniqueList.instance; }                                                                                                //NewItems
            }
            if (ability_manager.IsNullOrDestroyed()) { ability_manager = AbilityManager.instance; }                                                                                                 //Mjolner
            if (player_data_tracker.IsNullOrDestroyed()) { player_data_tracker = PlayerFinder.getPlayerDataTracker(); }                                                                             //Hud
            if ((stash_panel_ui.IsNullOrDestroyed()) && (!StashPanelUI.Instance.IsNullOrDestroyed())) { stash_panel_ui = StashPanelUI.Instance; }                                                   //Hud, QuadStash

            if (Scenes.IsGameScene())
            {
                if (player_spawn_manager.IsNullOrDestroyed()) { player_spawn_manager = PlayerSpawnManager.instance; }                                                                                             //
                if ((quest_list.IsNullOrDestroyed()) && (!QuestList.instance.IsNullOrDestroyed())) { quest_list = QuestList.instance; }                                                             //Complete MainQuest
                if ((scene_list.IsNullOrDestroyed()) && (!SceneList.instance.IsNullOrDestroyed())) { scene_list = SceneList.instance; }                                                             //Complete MainQuest
                //craft_materials_holder //Need to fix for LE 1.4
                if ((InventoryPanelUI.IsNullOrDestroyed()) && (!InventoryPanelUI.instance.IsNullOrDestroyed())) { InventoryPanelUI = InventoryPanelUI.instance; }                                   //AutoStore
                if ((BlessingsPanel.IsNullOrDestroyed()) && (!InventoryPanelUI.IsNullOrDestroyed())) { BlessingsPanel = InventoryPanelUI.blessingPanel; }                                           //Blessings
                if ((ground_item_manager.IsNullOrDestroyed()) && (!GroundItemManager.instance.IsNullOrDestroyed())) { ground_item_manager = GroundItemManager.instance; }                           //Hud
                if ((item_containers_manager.IsNullOrDestroyed()) && (!ItemContainersManager.Instance.IsNullOrDestroyed())) { item_containers_manager = ItemContainersManager.Instance; }           //Unlock Idols, Items Update
                if (player_actor.IsNullOrDestroyed()) { player_actor = PlayerFinder.getPlayerActor(); }                                                                                             //Hud, MainQuest, Materials, MemoryAmber, PermanentBuffs, AutoPickup, RangePickup, Maxroll, MinimapIcons, Monolith options, NewsItems, TimeBeast, DamageMeter
                //if (player_visuals.IsNullOrDestroyed()) { player_visuals = PlayerFinder.getPlayerVisuals(); }                                                                                     //PlayerVisuals (Have a make a fix for LE 1.4)
                if (player_data.IsNullOrDestroyed()) { player_data = PlayerFinder.getPlayerData(); }                                                                                                //Hud, MainQuest, TooltipLegendaryVisual, Maxroll              
                if ((faction_tracker.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { faction_tracker = player_actor.gameObject.GetComponent<FactionTracker>(); }                     //Hud
                if (player_health.IsNullOrDestroyed()) { player_health = PlayerFinder.getLocalPlayerHealth(); }                                                                                     //AutoPotions, GodMode
                if ((player_moving.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { player_moving = player_actor.gameObject.GetComponent<MovingPlayer>(); }                           //Monolith Complete Objective
                if ((player_protection_class.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { player_protection_class = player_actor.gameObject.GetComponent<ProtectionClass>(); }    //Essentia Sanguis
                if ((using_ability_player.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { using_ability_player = player_actor.gameObject.GetComponent<UsingAbilityPlayer>(); }       //TimeBeast
                if ((summon_tracker.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { summon_tracker = player_actor.gameObject.GetComponent<SummonTracker>(); }                        //PermanentBuffs, Headhunter, Summon Options, DamageMeter
                if ((health_potion.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { health_potion = player_actor.gameObject.GetComponent<HealthPotion>(); }                           //AutoPotions, PotionReplenishment, AutoPickupPot
                if ((character_mutator.IsNullOrDestroyed()) && (!player_actor.IsNullOrDestroyed())) { character_mutator = player_actor.gameObject.GetComponent<CharacterMutator>(); }               //TwoHandShield
                if (player_stats.IsNullOrDestroyed()) { player_stats = PlayerFinder.getLocalPlayerStats(); }                                                                                        //LowLife
                if (exp_tracker.IsNullOrDestroyed()) { exp_tracker = PlayerFinder.getExperienceTracker(); }                                                                                         //Hud
                if (player_treedata.IsNullOrDestroyed()) { player_treedata = PlayerFinder.getLocalTreeData(); }                                                                                     //Hud, Masteries, MaxrollPassives, SkillLevel
                if (player_gold_tracker.IsNullOrDestroyed()) { player_gold_tracker = PlayerFinder.getLocalGoldTracker(); }                                                                          //AutoPickupGold
                if (player_golbal_data_tracker.IsNullOrDestroyed()) { player_golbal_data_tracker = PlayerFinder.getGlobalDataTracker(); }                                                           //AutoPickupItems
                if ((filter_manager.IsNullOrDestroyed()) && (!ItemFilterManager.Instance.IsNullOrDestroyed())) { filter_manager = ItemFilterManager.Instance; }                                     //AutoPickupItems, MinimapIcons
                if ((camera_manager.IsNullOrDestroyed()) && (!CameraManager.instance.IsNullOrDestroyed())) { camera_manager = CameraManager.instance; }                                             //CameraOverride
                if (map_panel.IsNullOrDestroyed() && (!MapPanel.instance.IsNullOrDestroyed())) { map_panel = MapPanel.instance; }                                                                   //MainQuest, TpSafe
            }
            else
            {
                if (!player_data.IsNullOrDestroyed()) { player_data = null; }
                if (!map_panel.IsNullOrDestroyed()) {  map_panel = null; }
            }
        }
    }
}
