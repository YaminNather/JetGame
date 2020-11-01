using DG.Tweening;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class MainMenuSceneMgr : MonoBehaviour
{
    #region Variables
    private static MainMenuSceneMgr s_Instance;
    public static MainMenuSceneMgr Instance => s_Instance;

    private MainMenuSceneReferences mmsr;
    private GlobalMgr gdi;

    private PagesEN m_PageCur;
    public PagesEN PageCur => m_PageCur;

    [SerializeField] private AudioClip m_BackgroundMusic;

    private bool m_ScoreLastGameIsBest;
    public bool ScoreLastGameIsBest => m_ScoreLastGameIsBest;

    private AsyncOperationHandle<SceneInstance> m_MainGameSceneLoadingAsyncOp;
    public AsyncOperationHandle<SceneInstance> MainGameSceneLoadingAsyncOp => m_MainGameSceneLoadingAsyncOp;
    #endregion

    private void Awake()
    {
        s_Instance = this;
        gdi = GlobalMgr.s_Instance;
        mmsr = MainMenuSceneReferences.INSTANCE;
    }

    private void Start()
    {
        StartCoroutine(Start_IEF());
    }

    private IEnumerator Start_IEF()
    {

        while (gdi.AllLoaded == false) yield return null;
        
        //Set a random Global Color in the Menu.
        GlobalMgr.s_Instance.m_ColorMgr.SetRandomColor_F();

        //Create an Interstitial Ad and hold for display.
        gdi.m_AdsMgr.InterstitialAdCheckAndCreate_F(false);

        //Check if last games Score was the high score to set flags for displaying the New HighScore Label.
        if (gdi.m_GlobalData.ScoreLastGameIsBest)
        {
            m_ScoreLastGameIsBest = true;
            gdi.m_GlobalData.ScoreLastGameIsBest = false;
        }

        //Play Menu Background Music.
        gdi.m_BackgroundMusicMgr.Play_F(m_BackgroundMusic);

        //DOTween.To(() => 0f, val => mmsr.colorMgr.Hue0 = val, 1f, 40f).SetLoops(-1).SetEase(Ease.Linear);

        //Start Loading MainGame right away.
        m_MainGameSceneLoadingAsyncOp = GlobalMgr.s_Instance.m_SceneLoader.LoadScene_F(ScenesLoader.ScenesEN.MainGame,
            loadSceneMode:LoadSceneMode.Single, activateOnLoad:false);

        //Instantiate the Main Menu Jet.
        MainMenuSceneReferences.INSTANCE.mainMenuJetMgr.JetsInstantiate_F();
        
        //Open the Main Game and check if to load ads.
        PageOpen_F(PagesEN.Main, onOpen_E: () => 
        {
            gdi.m_AdsMgr.BannerCheckAndCreate_F();
            if(gdi.m_AdsMgr.GamesSinceLastInterstitialAd == 2)
            {
                if(gdi.m_AdsMgr.InterstitialAd.IsValid_F() == false) gdi.m_AdsMgr.InterstitialAd.Show_F();
            }
        });        

        //mmsr.jetDisplayMgr.Init_F();
    }

    public void PageOpen_F(PagesEN sceneEnum, Action onClose_E = null, Action onOpen_E = null)
    {
        Page pageToOpen = sceneEnum.ToPage_F();
        if (pageToOpen == null) throw new Exception("Cannot open Page Page is null!!!");

        if (m_PageCur == PagesEN.None) pageToOpen.Open_F(onOpen_E);
        else m_PageCur.ToPage_F().Close_F(pageToOpen, onOpen:onOpen_E);

        m_PageCur = sceneEnum;
    }

    public enum PagesEN { None, Main, Settings, JetStore }

#if UNITY_EDITOR
    [MenuItem("Scenes/Main Menu")]
    public static void OpenScene_F() => EditorSceneManager.OpenScene("Assets/Main Menu Stuff/Scenes/MainMenu_1_Scene.unity");
#endif
}

//PagesEN
public static partial class ExtensionMethods
{
    public static Page ToPage_F(this MainMenuSceneMgr.PagesEN p)
    {
        MainMenuSceneReferences mmsr = MainMenuSceneReferences.INSTANCE;
        return p switch
        {
            MainMenuSceneMgr.PagesEN.Main => mmsr.mainMenuMgr,
            MainMenuSceneMgr.PagesEN.JetStore => mmsr.jetStoreMgr,
            MainMenuSceneMgr.PagesEN.Settings => mmsr.settingsPageMgr,
            _ => null
        };
    }
}
