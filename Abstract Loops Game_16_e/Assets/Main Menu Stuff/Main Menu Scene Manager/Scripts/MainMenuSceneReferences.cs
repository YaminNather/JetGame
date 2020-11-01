using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSceneReferences : MonoBehaviour
{
    #region Variables
    static public MainMenuSceneReferences INSTANCE;

    public MainMenuSceneMgr mainMenuSceneMgr;
    public MainMenuJetMgr mainMenuJetMgr;
    public MainMenuMgr mainMenuMgr;
    public JetDisplayMgr jetDisplayMgr;
    public JetStoreMgr jetStoreMgr;
    public SettingsPageMgr settingsPageMgr;
    public Image transitionImage;
    public ColorMgr colorMgr;
    #endregion

    private void Awake()
    {
        INSTANCE = this;        
    }    
}
