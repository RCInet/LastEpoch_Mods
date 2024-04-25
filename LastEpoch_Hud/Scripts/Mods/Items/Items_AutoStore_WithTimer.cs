using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    [RegisterTypeInIl2Cpp]
    public class Items_AutoStore_WithTimer : MonoBehaviour
    {
        public static Items_AutoStore_WithTimer instance { get; private set; }
        public Items_AutoStore_WithTimer(System.IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if (CanRun())
            {
                if (!running) { Start(); }
                if (running)
                {
                    if (GetElapsedTime() > 10)
                    {
                        Refs_Manager.InventoryPanelUI.StoreMaterialsButtonPress();
                        running = false;
                    }
                }
            }
            else { running = false; }
        }
                
        public static System.DateTime StartTime;
        public static System.DateTime LastTime;
        public static bool running;

        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) &&
                (!Refs_Manager.InventoryPanelUI.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_All10Sec;
                }
                else { return false; }
            }
            else { return false; }
        }
        public static void Start()
        {
            StartTime = System.DateTime.Now;
            running = true;
        }
        public static double GetElapsedTime()
        {
            LastTime = System.DateTime.Now;
            var elaspedTime = LastTime - StartTime;

            return elaspedTime.TotalSeconds;
        }
    }
}
