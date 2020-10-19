using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MainMenuSceneMgr : MonoBehaviour
{
    #region Variables
    private MainMenuSceneReferences mmsr;
    private GlobalDatabaseInitializer gdi;

    private Pages_EN m_PageCur;
    public Pages_EN PageCur { get => m_PageCur; }

    [SerializeField] private AudioClip m_BackgroundMusic;

    private bool m_ScoreLastGameIsBest;
    public bool ScoreLastGameIsBest { get => m_ScoreLastGameIsBest; }
    #endregion

    private void Awake()
    {
        gdi = GlobalDatabaseInitializer.INSTANCE;
        mmsr = MainMenuSceneReferences.INSTANCE;
    }

    private void Start()
    {
        gdi.m_AdsMgr.InterstitialAdCheckAndCreate_F();

        if (gdi.m_GlobalData.ScoreLastGameIsBest)
        { 
            m_ScoreLastGameIsBest = true;
            gdi.m_GlobalData.ScoreLastGameIsBest = false;
        }

        gdi.m_BackgroundMusicMgr.Play_F(m_BackgroundMusic);
        //DOTween.To(() => 0f, val => mmsr.colorMgr.Hue0 = val, 1f, 40f).SetLoops(-1).SetEase(Ease.Linear);
        StartCoroutine(Start_IEF());
    }

    private IEnumerator Start_IEF()
    {
        while (gdi.AllLoaded == false) yield return null;

        MainMenuSceneReferences.INSTANCE.mainMenuJetMgr.JetsInstantiate_F();
        PageOpen_F(Pages_EN.Main, onOpen_E: () => 
        {
            gdi.m_AdsMgr.BannerCheckAndCreate_F();
            if(gdi.m_AdsMgr.GamesSinceLastInterstitialAd == 2)
            {
                if(gdi.m_AdsMgr.InterstitialAd.IsValid_F() == false) gdi.m_AdsMgr.InterstitialAd.Show_F();
            }
        });        

        //mmsr.jetDisplayMgr.Init_F();
    }

    public void PageOpen_F(Pages_EN sceneEnum, Action onClose_E = null, Action onOpen_E = null)
    {
        Page pageToOpen = PagesENToPage_F(sceneEnum);
        if (pageToOpen == null) throw new Exception("Cannot open Page Page is null!!!");

        if (m_PageCur == Pages_EN.None) pageToOpen.Open_F(onOpen_E);
        else PagesENToPage_F(m_PageCur).Close_F(pageToOpen, onOpen:onOpen_E);

        m_PageCur = sceneEnum;
    }

    public Page PagesENToPage_F(Pages_EN sceneEnum)
    {
        return sceneEnum switch
        {
            Pages_EN.Main => mmsr.mainMenuMgr,
            Pages_EN.JetStore => mmsr.jetStoreMgr,
            Pages_EN.Settings => throw new System.Exception("Cannot convert to Enum to Settings Page"),
            _ => null
        };
    }

    public enum Pages_EN { None, Main, Settings, JetStore }

#if UNITY_EDITOR
    [MenuItem("Scenes/Main Menu")]
    public static void OpenScene_F() => EditorSceneManager.OpenScene("Assets/Main Menu Stuff/Scenes/MainMenu_0_Scene.unity");
#endif
}
