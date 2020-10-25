using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
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

    private bool IsOpeningMainGameScene;
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
        GlobalData gd = GlobalMgr.INSTANCE.m_GlobalData;
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
        if (IsOpeningMainGameScene == true) return;

        IsOpeningMainGameScene = true;
        GlobalMgr.INSTANCE.m_BackgroundMusicMgr.FadeOut_F();
        StartCoroutine(Play_IEF());
    }

    private IEnumerator Play_IEF()
    {
        yield return new WaitForSeconds(1);

        while (MainMenuSceneMgr.Instance.MainGameSceneLoadingAsyncOp.IsDone == false) yield return null;

        AsyncOperation activateSceneAsyncOp = MainMenuSceneMgr.Instance.MainGameSceneLoadingAsyncOp.Result.ActivateAsync();
        yield return activateSceneAsyncOp;

        IsOpeningMainGameScene = false;
    }

    public void JetStoreBtn_BEF()
    {
        if(IsOpeningMainGameScene == true) return;
        
        MainMenuSceneReferences.INSTANCE.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.PagesEN.JetStore);     
    }

    public void SettingsBtn_BEF()
    {
        ////Shader.SetGlobalFloat("_Hue0", Shader.GetGlobalFloat("_Hue0") + 0.1f * Time.deltaTime);
        //GameObject GObj_0 = FindObjectOfType<Volume>(true).gameObject;
        //GObj_0.SetActive(!GObj_0.activeSelf);
        if (IsOpeningMainGameScene == true) return;

        mmsr.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.PagesEN.Settings);
    }
    #endregion
}
