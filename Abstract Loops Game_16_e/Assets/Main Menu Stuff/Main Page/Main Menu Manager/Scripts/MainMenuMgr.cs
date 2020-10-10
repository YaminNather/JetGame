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
        mmsr = MainMenuSceneReferences.INSTANCE;
    }

    private void OnEnable()
    {
        GlobalData gd = GlobalDatabaseInitializer.INSTANCE.m_GlobalData;
        ScoreBest_Lbl.text = "" + gd.ScoreBest;
        ScoreCur_Lbl.text = "" + gd.ScoreLastGame;

        CurrencyValue_Lbl.text = gd.Currency + ""; 
    }

    #region Button Functions
    public void Play_BEF()
    {
        GlobalDatabaseInitializer.INSTANCE.m_BackgroundMusicMgr.FadeOut_F();
        StartCoroutine(Play_IEF());
    }

    private IEnumerator Play_IEF()
    {
        yield return new WaitForSeconds(1);
        GlobalDatabaseInitializer.INSTANCE.m_ScenesDatabase.LoadScene_F(Scenes_EN.MainGame);
    }

    public void Save_BEF()
    {
        GlobalDatabaseInitializer.INSTANCE.m_GlobalData.Save_F();
    }

    public void JetStoreBtn_BEF()
    {
        MainMenuSceneReferences.INSTANCE.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.Pages_EN.JetStore);     
    }
    #endregion
}
