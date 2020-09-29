using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuMgr : Page
{
    #region Variables
    [Space(50)]
    private MainMenuSceneReferences mmsr;

    [SerializeField] private Text CurrencyValue_Lbl; 
    #endregion

    private void Awake()
    {
        mmsr = MainMenuSceneReferences.s_Instance;
    }

    private void OnEnable()
    {
        CurrencyValue_Lbl.text = GlobalDatabaseInitializer.s_Instance.globalData.Currency + "";
    }

    #region Button Functions
    public void Play_BEF()
    {
        GlobalDatabaseInitializer.s_Instance.scenesDatabase.LoadScene_F(Scenes_EN.MainGame);
    }

    public void Save_BEF()
    {
        GlobalDatabaseInitializer.s_Instance.globalData.Save_F();
    }

    public void JetStoreBtn_BEF()
    {
        Close_F(mmsr.jetStoreMgr);        
    }
    #endregion
}
