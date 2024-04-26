using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.Minimap
{
    [RegisterTypeInIl2Cpp]
    public class Minimap_Icons : MonoBehaviour
    {
        public static Minimap_Icons instance { get; private set; }
        public Minimap_Icons(System.IntPtr ptr) : base(ptr) { }
        public struct objects_structure
        {
            public string scene_name;
            public uint id;
            public GameObject base_object;
        }

        void Awake()
        {
            instance = this;
            SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!items_in_map.IsNullOrDestroyed())
            {
                if (!Scenes.IsGameScene())
                {
                    foreach (objects_structure obj_struct in items_in_map)
                    {
                        if (!obj_struct.base_object.IsNullOrDestroyed()) { Object.Destroy(obj_struct.base_object); }
                    }
                    items_in_map = null;
                }
                else
                {
                    System.Collections.Generic.List<objects_structure> temp = new System.Collections.Generic.List<objects_structure>();
                    foreach (objects_structure obj_struct in items_in_map)
                    {
                        if (obj_struct.scene_name != Scenes.SceneName) { Object.Destroy(obj_struct.base_object); }
                        else { temp.Add(obj_struct); }
                    }
                    items_in_map = temp;
                }
            }
        }

        private static System.Collections.Generic.List<objects_structure> items_in_map;                
        private static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) && (!instance.IsNullOrDestroyed()))
            {
                if ((!Save_Manager.instance.data.IsNullOrDestroyed()) && (!instance.gameObject.IsNullOrDestroyed()))
                {
                    return instance.gameObject.active;
                }
                else { return false; }
            }
            else { return false; }
        }
        private static bool ItemMatchFilter(ItemDataUnpacked item)
        {
            bool result = false;
            if ((!Refs_Manager.player_actor.IsNullOrDestroyed()) && (!Refs_Manager.filter_manager.IsNullOrDestroyed()))
            {
                if (!Refs_Manager.filter_manager.Filter.IsNullOrDestroyed())
                {
                    foreach (ItemFiltering.Rule rule in Refs_Manager.filter_manager.Filter.rules)
                    {
                        if ((rule.isEnabled) && (rule.Match(item.TryCast<ItemDataUnpacked>())) &&
                            (((rule.levelDependent) && (rule.LevelInBounds(Refs_Manager.player_actor.stats.level))) ||
                            (!rule.levelDependent)))
                        {
                            if (rule.type == ItemFiltering.Rule.RuleOutcome.SHOW)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }
        
        [HarmonyPatch(typeof(GroundItemVisuals), "initialise", new System.Type[] { typeof(ItemDataUnpacked), typeof(uint), typeof(GroundItemLabel), typeof(bool) })]
        public class GroundItemVisuals_initialise
        {
            [HarmonyPostfix]
            static void Postfix(ref GroundItemVisuals __instance, ItemDataUnpacked __0, uint __1) //, GroundItemLabel __2, bool __3)
            {
                if ((CanRun()) && (!DMM.DMMap.Instance.IsNullOrDestroyed()))
                {
                    if (items_in_map.IsNullOrDestroyed()) { items_in_map = new System.Collections.Generic.List<objects_structure>(); }
                    if (__0.rarity < 7)
                    {
                        if ((Save_Manager.instance.data.Scenes.Minimap.Enable_ShowAllItems) ||
                            ((Save_Manager.instance.data.Scenes.Minimap.Enable_ShowItemsFromFilter) &&
                            (ItemMatchFilter(__0))))
                        {
                            bool remove = false;
                            int i = 0;
                            foreach (objects_structure obj_struct in items_in_map)
                            {
                                if ((obj_struct.id == __1) && (obj_struct.scene_name == Scenes.SceneName))
                                {
                                    remove = true;
                                    break;
                                }
                                i++;
                            }
                            if ((remove) && (i < items_in_map.Count))
                            {
                                if (!items_in_map[i].base_object.IsNullOrDestroyed()) { Object.Destroy(items_in_map[i].base_object); }
                                items_in_map.Remove(items_in_map[i]);
                            }

                            GameObject base_object = Object.Instantiate(new GameObject(name: Scenes.SceneName + "_icon_" + __1), Vector3.zero, Quaternion.identity);
                            Object.DontDestroyOnLoad(base_object);
                            base_object.transform.position = DMM.DMMap.Instance.WorldtoUI(__instance.gameObject.transform.position);
                            base_object.transform.localPosition = __instance.gameObject.transform.localPosition;
                            base_object.AddComponent<DMM.DMMapIcon>();
                            base_object.AddComponent<Minimap_Icons_UI>();
                            base_object.GetComponent<Minimap_Icons_UI>().icon = UITooltipItem.SetItemSprite(__0);

                            items_in_map.Add(new objects_structure { scene_name = Scenes.SceneName, id = __1, base_object = base_object });
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(GroundItemManager), "pickupItem", new System.Type[] { typeof(Actor), typeof(uint) })]
        public class GroundItemManager_pickupItem
        {
            [HarmonyPostfix]
            static void Postfix(uint __1)
            {
                if ((CanRun()) && (!DMM.DMMap.Instance.IsNullOrDestroyed()) &&
                    (!items_in_map.IsNullOrDestroyed()))
                {
                    bool found = false;
                    int i = 0;                    
                    foreach (objects_structure obj_struct in items_in_map)
                    {
                        if ((obj_struct.id == __1) && (obj_struct.scene_name == Scenes.SceneName)) { found = true; break; }
                        i++;
                    }

                    if ((found) && (i < items_in_map.Count))
                    {
                        Object.Destroy(items_in_map[i].base_object);
                        items_in_map.Remove(items_in_map[i]);
                    }
                }
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    public class Minimap_Icons_UI : MonoBehaviour
    {
        public static Minimap_Icons_UI instance { get; private set; }
        public Minimap_Icons_UI(System.IntPtr ptr) : base(ptr) { }
                
        DMM.DMMapIcon map_icon = null;
        public Sprite icon = null;
        bool initialized = false;

        void Awake()
        {
            instance = this;
            map_icon = this.gameObject.GetComponent<DMM.DMMapIcon>();
            map_icon.scaleMultiplier = 1f;
            if (!Save_Manager.instance.IsNullOrDestroyed())
            {
                if ((Save_Manager.instance.initialized) && (Save_Manager.instance.data.Scenes.Minimap.Icons_Scale > 0))
                {
                    map_icon.scaleMultiplier = Save_Manager.instance.data.Scenes.Minimap.Icons_Scale;
                }
            }
            map_icon.scaleWithZoom = true;
            map_icon.rotationOffset = -90f;
            map_icon.rotateWithMap = false;
        }
        void Update()
        {
            if ((!map_icon.IsNullOrDestroyed()) && (!icon.IsNullOrDestroyed()) && (!initialized))
            {
                if (!map_icon.img.IsNullOrDestroyed())
                {
                    map_icon.img.sprite = icon;
                    initialized = true;
                }
            }
        }
    }
}
