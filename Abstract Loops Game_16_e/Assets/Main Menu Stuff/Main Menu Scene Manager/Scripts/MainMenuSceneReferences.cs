using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneReferences : MonoBehaviour
{
    #region Variables
    static public MainMenuSceneReferences s_Instance;

    public MainMenuMgr mainMenuMgr;
    public ScoreBoardMgr scoreBoardMgr;
    public JetDisplayMgr jetDisplayMgr;
    public JetStoreMgr jetStoreMgr;
    public ColorMgr colorMgr;
    #endregion

    private void Awake()
    {
        s_Instance = this;        
    }    
}
