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

    private bool m_IsOpeningMainGameScene;

    private bool m_InterstitialAdShowIsDone;
    #endregion

    private void Awake()
    {
        mmsr = MainMenuSceneReferences.INSTANCE;
        OnOpen_E += OnOpen_EF;
    }

    private void OnOpen_EF()
    {
        GlobalData globalData = GlobalMgr.s_Instance.m_GlobalData;
        Debug.Log($"<color=#66FF00>Games Played since last interstitial Ad = {globalData.GamesPlayedSinceLastInterstitialAd}</color>");
        if (globalData.GamesPlayedSinceLastInterstitialAd > 3)
        {
            Debug.Log($"<color=magenta>Interstitial ad should be here now</color>");
            AdsMgr adsMgr = GlobalMgr.s_Instance.m_AdsMgr;
            if (adsMgr.InterstitialAd.IsValid_F() && adsMgr.InterstitialAd.IsLoaded && !m_InterstitialAdShowIsDone)
            {
                adsMgr.InterstitialAd.Show_F();
                globalData.GamesPlayedSinceLastInterstitialAd = 0;
                m_InterstitialAdShowIsDone = true;
            }
        }
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
        GlobalData gd = GlobalMgr.s_Instance.m_GlobalData;
        m_ScoreBest_Lbl.text = "" + gd.ScoreBest;
        m_ScoreCur_Lbl.text = "" + gd.ScoreLastGame;
        m_CurrencyValue_Lbl.text = "" + gd.Currency;
        m_NewBestScoreLblGObj.gameObject.SetActive(false);

        //Refresh LoopSelect Btns.
        foreach(LoopSelectBtnMgr btn in GetComponentsInChildren<LoopSelectBtnMgr>()) btn.Refresh_F();
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
        if (m_IsOpeningMainGameScene == true) return;

        m_IsOpeningMainGameScene = true;
        StartCoroutine(Play_IEF());
    }

    private IEnumerator Play_IEF()
    {

        //Jet Blast Off.
        MainMenuSceneReferences.INSTANCE.mainMenuJetMgr.BlastOff_F();

        //Fading out the Scene.
        MainMenuSceneReferences.INSTANCE.transitionImage.transform.parent.gameObject.SetActive(true);
        MainMenuSceneReferences.INSTANCE.transitionImage.DOFade(1.0f, 1.0f);

        //Fading out the Background Music.
        GlobalMgr.s_Instance.m_BackgroundMusicMgr.FadeOut_F();
        yield return new WaitForSeconds(1.0f);

        //Checking if MainGameSceneIsLoading or waiting for it to finish loading.
        while (MainMenuSceneMgr.Instance.MainGameSceneLoadingAsyncOp.IsDone == false) yield return null;

        //Activating MainGameScene.
        AsyncOperation activateSceneAsyncOp = MainMenuSceneMgr.Instance.MainGameSceneLoadingAsyncOp.Result.ActivateAsync();
        yield return activateSceneAsyncOp;

        m_IsOpeningMainGameScene = false;
    }

    public void JetStoreBtn_BEF()
    {
        if(m_IsOpeningMainGameScene == true) return;
        
        MainMenuSceneReferences.INSTANCE.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.PagesEN.JetStore);     
    }

    public void SettingsBtn_BEF()
    {
        ////Shader.SetGlobalFloat("_Hue0", Shader.GetGlobalFloat("_Hue0") + 0.1f * Time.deltaTime);
        //GameObject GObj_0 = FindObjectOfType<Volume>(true).gameObject;
        //GObj_0.SetActive(!GObj_0.activeSelf);
        if (m_IsOpeningMainGameScene == true) return;

        mmsr.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.PagesEN.Settings);
    }
    #endregion
}
