using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public static HudManager instance { get; private set; }
	
    public Canvas Game_Hud;   
    void Start()
    {
        instance = this;
        //Game_Hud.enabled = false;
    }
	
	#region Base
    public Button BaseBtn_Resume;
	public Button BaseBtn_Settings;
	public Button BaseBtn_GameGuide;
	public Button BaseBtn_LeaveGame;
	public Button BaseBtn_ExitDesktop;
	
	public void BaseBtn_Resume_Click()
    {
        
    }	
    public void BaseBtn_Settings_Click()
    {
        
    }
    public void BaseBtn_GameGuide_Click()
    {
        
    }
    public void BaseBtn_LeaveGame_Click()
    {
        
    }
	public void BaseBtn_ExitDesktop_Click()
    {
        
    }
    #endregion
	
    #region Ressources
    
    #endregion	
    #region Skills
    
    #endregion
}
