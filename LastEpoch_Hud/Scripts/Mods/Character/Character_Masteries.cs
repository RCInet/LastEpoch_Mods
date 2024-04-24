using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_Masteries : MonoBehaviour
    {
        public static Character_Masteries instance { get; private set; }
        public Character_Masteries(System.IntPtr ptr) : base(ptr) { }
        public static readonly string choose_masterie = "Choose Masterie";
        public static readonly string reset_masterie = "Reset Masterie";

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if (CanRun())
            {
                if (Refs_Manager.player_treedata.chosenMastery > 0) { Hud_Manager.Content.Character.Cheats.masterie_text.text = reset_masterie; }
                else { Hud_Manager.Content.Character.Cheats.masterie_text.text = choose_masterie; }
            }
        }
        
        public static void ResetChooseMasterie()
        {
            if (CanRun())
            {
                if (Hud_Manager.Content.Character.Cheats.masterie_text.text == reset_masterie) { ResetMasterie(); }
                else if (Hud_Manager.Content.Character.Cheats.masterie_text.text == choose_masterie) { ChooseMasterie(); }
            }
        }
        private static void ResetMasterie()
        {
            if (CanRun())
            {
                Refs_Manager.player_treedata.chosenMastery = 0;
                Refs_Manager.player_data_tracker.charData.ChosenMastery = 0;
                Refs_Manager.player_data_tracker.charData.SaveData();
                Hud_Manager.Content.Character.Cheats.masterie_text.text = choose_masterie;
            }
        }
        private static void ChooseMasterie()
        {
            Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
            if (!Refs_Manager.game_uibase.IsNullOrDestroyed()) { Refs_Manager.game_uibase.openMasteryPanel(false); }
        }
        private static bool CanRun()
        {
            if ((Scenes.IsGameScene()) &&
                (!Refs_Manager.player_treedata.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_data_tracker.IsNullOrDestroyed()) &&
                (!Hud_Manager.Content.Character.Cheats.masterie_text.IsNullOrDestroyed()))
            {
                return true;
            }
            else { return false; }
        }
    }
}
