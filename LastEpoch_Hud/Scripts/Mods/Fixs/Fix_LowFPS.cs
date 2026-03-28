using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Fixs
{
    [RegisterTypeInIl2Cpp]
    public class Fix_LowFPS : MonoBehaviour
    {
        public Fix_LowFPS(System.IntPtr ptr) : base(ptr) { }
        public static Fix_LowFPS instance { get; private set; }

        public static bool check_fps = false;
        public static bool init_timer = false;
        public static bool need_fix = false;
        public static bool fixing = false;
        public static System.DateTime StartTime;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if (!Scenes.IsGameScene()) { Init(); }
            else
            {
                if (init_timer)
                {
                    init_timer = false;
                    StartTime = System.DateTime.Now;
                }
                if ((check_fps) && (!Refs_Manager.game_uibase.IsNullOrDestroyed()))
                {
                    int fps = GetFps();
                    if ((fps > -1) && (fps < 20))
                    {
                        check_fps = false;
                        need_fix = true;
                        //Main.logger_instance.Warning("Fix : LowFPS");
                    }
                    else if (GetElapsedTime() > 10) { check_fps = false; }
                }

                if ((need_fix) && (!Refs_Manager.game_uibase.IsNullOrDestroyed()) && (!Hud_Manager.hud_object.IsNullOrDestroyed()))
                {
                    if (!Hud_Manager.IsPauseOpen())
                    {
                        fixing = true;
                        Hud_Manager.hud_object.active = false;
                        Refs_Manager.game_uibase.openMenu(false);                        
                    }
                    else if (fixing)
                    {
                        fixing = false;
                        need_fix = false;                        
                        Hud_Manager.Hud_Base.Btn_Resume.onClick.Invoke();
                        Hud_Manager.hud_object.active = true;                        
                        Init();
                    }
                }
            }
        }
        void Init()
        {
            init_timer = true;
            check_fps = true;
        }
        double GetElapsedTime()
        {
            System.DateTime LastTime = System.DateTime.Now;
            var elaspedTime = LastTime - StartTime;

            return elaspedTime.TotalSeconds;
        }
        int GetFps()
        {
            int fps = -1;
            if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
            {
                if (!Refs_Manager.game_uibase.fpsDisplay.IsNullOrDestroyed())
                {
                    Refs_Manager.game_uibase.fpsDisplay.UpdateUI();
                    if (Refs_Manager.game_uibase.fpsDisplay.display.text.Split(' ').Length > 2)
                    {
                        //<color=#aaa>FPS:</color> 74 <color=#aaa>Min:</color> 59 <color=#aaa>Max: </color>75<color=#aaa> Avg: </color>68
                        string s = Refs_Manager.game_uibase.fpsDisplay.display.text.Split(' ')[3];
                        fps = System.Convert.ToInt32(s);
                    }
                    else if (Refs_Manager.game_uibase.fpsDisplay.display.text.Split(' ').Length > 0)
                    {
                        //<color=#aaa>FPS:</color> 10
                        string s = Refs_Manager.game_uibase.fpsDisplay.display.text.Split(' ')[1];
                        fps = System.Convert.ToInt32(s);                        
                    }
                }
            }

            return fps;
        }
    }
}
