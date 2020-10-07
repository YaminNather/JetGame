using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardMgr : Page
{
    #region Variables
    [SerializeField] private Text ScoreLastGameValue_Lbl;
    [SerializeField] private Text ScoreBestValue_Lbl;
    #endregion

    private void OnEnable()
    {
        Refresh_F();
    }

    public void Refresh_F()
    {
        GlobalData globalData = GlobalDatabaseInitializer.s_Instance.globalData;
        ScoreLastGameValue_Lbl.text = "" + globalData.ScoreLastGame;
        ScoreBestValue_Lbl.text = "" + globalData.ScoreBest;
    }

    #region Button Functions
    public void RestartBtn_BEF()
    {
        GlobalDatabaseInitializer.s_Instance.scenesDatabase.LoadScene_F(Scenes_EN.MainGame);
    }

    public void MainMenuBtn_BEF()
    {
        gameObject.SetActive(false);
        MainMenuSceneReferences.s_Instance.mainMenuMgr.gameObject.SetActive(true);
    }
    #endregion
}
