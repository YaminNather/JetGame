using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuMgr : Page
{
    #region Variables
    [Space(50)]
    private MainMenuSceneReferences mmsr;


    [SerializeField] private Text ScoreBest_Lbl;
    [SerializeField] private Text ScoreCur_Lbl;
    [SerializeField] private Text CurrencyValue_Lbl; 
    #endregion

    private void Awake()
    {
        mmsr = MainMenuSceneReferences.s_Instance;
    }

    private void OnEnable()
    {
        GlobalData gd = GlobalDatabaseInitializer.s_Instance.globalData;
        ScoreBest_Lbl.text = "" + gd.ScoreBest;
        ScoreCur_Lbl.text = "" + gd.ScoreLastGame;

        CurrencyValue_Lbl.text = gd.Currency + ""; 
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
        MainMenuSceneReferences.s_Instance.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.Pages_EN.JetStore);     
    }
    #endregion
}
