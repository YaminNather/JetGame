using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MainMenuMgr : Page
{
    #region Variables
    [Space(50)]
    private MainMenuSceneReferences mmsr;

    [SerializeField] private Text m_ScoreBest_Lbl;
    [SerializeField] private Text m_ScoreCur_Lbl;
    [SerializeField] private Text m_CurrencyValue_Lbl;
    [SerializeField] private GameObject m_NewBestScoreLblGObj;

    private Tweener m_NewBestScoreLblAnimT;
    #endregion

    private void Awake()
    {
        mmsr = MainMenuSceneReferences.INSTANCE;
        OnOpen_E += OnOpen_EF;
    }

    private void OnOpen_EF()
    {
        //If last game's score was the best then enable the NewBestScore Lbl.
        if (mmsr.mainMenuSceneMgr.ScoreLastGameIsBest)
        {
            m_NewBestScoreLblGObj.gameObject.SetActive(true);
            m_NewBestScoreLblAnimT = m_NewBestScoreLblGObj.transform.DOScale(1.2f, 1f).SetEase(Ease.Flash, 2).SetLoops(-1);
        }
    }

    private void OnEnable()
    {
        //Get the score and currency from GlobalData and assign it to the labels.
        GlobalData gd = GlobalDatabaseInitializer.INSTANCE.m_GlobalData;
        m_ScoreBest_Lbl.text = "" + gd.ScoreBest;
        m_ScoreCur_Lbl.text = "" + gd.ScoreLastGame;
        m_CurrencyValue_Lbl.text = "" + gd.Currency;
        m_NewBestScoreLblGObj.gameObject.SetActive(false);
    }

    public override void Close_F(Page page = null, Action onClose_E = null, Action onOpen = null)
    {
        if (m_NewBestScoreLblAnimT.IsActive()) m_NewBestScoreLblAnimT.Kill();
        m_NewBestScoreLblGObj.gameObject.SetActive(false);
        base.Close_F(page, onClose_E);
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
        MainMenuSceneReferences.INSTANCE.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.PagesEN.JetStore);     
    }

    public void SettingsBtn_BEF()
    {
        ////Shader.SetGlobalFloat("_Hue0", Shader.GetGlobalFloat("_Hue0") + 0.1f * Time.deltaTime);
        //GameObject GObj_0 = FindObjectOfType<Volume>(true).gameObject;
        //GObj_0.SetActive(!GObj_0.activeSelf);

        mmsr.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.PagesEN.Settings);
    }
    #endregion
}
