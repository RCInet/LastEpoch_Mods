using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.Teleport
{
    [RegisterTypeInIl2Cpp]
    public class Teleport_ToScene : MonoBehaviour
    {
        public static Teleport_ToScene instance { get; private set; }
        public Teleport_ToScene(System.IntPtr ptr) : base(ptr) { }

        public static System.Collections.Generic.List<string> scene_names = new System.Collections.Generic.List<string>();
        public static bool starting_tp = false;
        public static string old_scene = "";
        public static string new_scene = "";

        void Awake()
        {
            instance = this;
            scene_names = new System.Collections.Generic.List<string>();
        }
        void Update()
        {
            if ((starting_tp) && (!Refs_Manager.player_spawn_manager.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                if (SceneManager.GetActiveScene().name != new_scene)
                {
                    try { SceneManager.SetActiveScene(SceneManager.GetSceneByName(new_scene)); }
                    catch { }
                }
                else
                {
                    starting_tp = false;
                    PlayerSpawn choosen_spawn = null;
                    Refs_Manager.player_spawn_manager.TryPlacePlayerAtSpawn(Refs_Manager.player_actor, old_scene, new_scene, 0, out choosen_spawn);
                    SceneManager.UnloadSceneAsync(old_scene);
                    old_scene = "";
                    new_scene = "";
                }
            }
        }

        public static void StartTp(int index)
        {
            if ((index > 0) && (index < scene_names.Count))
            {
                old_scene = SceneManager.GetActiveScene().name;
                new_scene = scene_names[index];
                Main.logger_instance.Warning("scene = " + new_scene);
                SceneManager.LoadSceneAsync(new_scene, LoadSceneMode.Additive);
                starting_tp = true;
            }
        }
    }
}
