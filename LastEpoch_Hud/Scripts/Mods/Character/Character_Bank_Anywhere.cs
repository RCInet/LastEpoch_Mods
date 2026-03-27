using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
	[RegisterTypeInIl2Cpp]
	public class Character_Bank_Anywhere : MonoBehaviour
	{
		public static Character_Bank_Anywhere instance { get; private set; }
		public Character_Bank_Anywhere(System.IntPtr ptr) : base(ptr) { }
			
		void Awake()
		{
			instance = this;
		}
		void Update()
		{
			if ((Scenes.IsGameScene()) && (!Refs_Manager.game_uibase.IsNullOrDestroyed()) && (!Save_Manager.instance.IsNullOrDestroyed()))
			{
				if (Input.GetKeyDown(Save_Manager.instance.data.KeyBinds.BankStashs))
				{
					if (IsOpen()) { Refs_Manager.game_uibase.closeStash(false); }
					else { Refs_Manager.game_uibase.openStash(true, false); }
                }					
			}
		}
		bool IsOpen()
		{
			bool result = false;
			if (!Refs_Manager.stash_panel_ui.IsNullOrDestroyed()) { result = Refs_Manager.stash_panel_ui.gameObject.active; }
            //if (!Refs_Manager.game_uibase.IsNullOrDestroyed()) { result = Refs_Manager.game_uibase.stashPanel.instance.active; }

            return result;
        }
	}
}
