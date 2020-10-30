using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

public class MainGameMgr : MonoBehaviour
{
    #region Variables
    static private MainGameMgr s_Instance;
    static public MainGameMgr Instance => s_Instance;

    private GlobalMgr m_globalMgr;
    private MainGameReferences mgr;

    private DifficultyEN m_Difficulty;
    public DifficultyEN Difficulty
    {
        get => m_Difficulty;
        set => m_Difficulty = value;
    }

    private int m_PlayerDeathCount;
    public int PlayerDeathCount => m_PlayerDeathCount;

    [SerializeField] private AudioClip[] m_BackgroundMusicACs;

    private AsyncOperationHandle<SceneInstance> mainMenuSceneLoadingAsyncOp;
    #endregion

    private void Awake()
    {
        m_globalMgr = GlobalMgr.s_Instance;
        mgr = MainGameReferences.INSTANCE;
    }

    private void Start()
    {                
        StartCoroutine(Start_IEF());
    }

    private IEnumerator Start_IEF()
    {
        //Setting Screen to Black, so that we can fade in once everything is loaded.
        mgr.transitionMgr.ColorSet_F(Color.black);

        GlobalMgr.s_Instance.m_ColorMgr.SetRandomColor_F();

        //Waiting for all assets to load into their database. This part will be moved somewhere else later.
        while (m_globalMgr.AllLoaded == false) yield return null;

        //Start Loading MainMenu Scene in advance.
        mainMenuSceneLoadingAsyncOp = GlobalMgr.s_Instance.m_SceneLoader.LoadScene_F(ScenesLoader.ScenesEN.MainMenu, activateOnLoad: false);
        
        //Load Ad
        MainGameReferences.INSTANCE.reviveMgr.LoadAd_F();

        //Sets the best score in ScoreMgr.
        mgr.scoreMgr.ScoreBestSet_F();

        //Play Background Music
        GlobalMgr.s_Instance.m_BackgroundMusicMgr.Play_F(m_BackgroundMusicACs[GlobalMgr.s_Instance.m_GlobalData.BackgroundMusicCur]);


        //Getting all loops and levels from their database.
        mgr.loopsMgr.LoopsFieldSetup_F();
        mgr.levelsMgr.GetAllLevelsForGame_F();

        //Spawning the first loops and levels.
        mgr.loopsMgr.LoopSpawn_F(Vector3.zero);
        mgr.levelsMgr.RandomLevelSpawn_F(new Vector3(0.0f, 0.0f, 50.0f));
        
        //Spawn player and possess it.
        JetPawn spawnedPlayer = m_globalMgr.m_JetsDatabase.JetCurInstantiate_F().GetComponent<JetPawn>();
        spawnedPlayer.transform.position = Vector3.forward * 5.0f;
        //Debug.Log($"<color=green>Spawned Player default pos = {spawnedPlayer.transform.position}</color>");
        mgr.player = spawnedPlayer;
        mgr.player.OnDeath_E += OnPlayerDeath_EF;
        mgr.playerController.Possess_F(spawnedPlayer);

        //Fading in the scene.
        mgr.transitionMgr.TransitionDo_F(0.0f, 0.5f);
    }    

    /// <summary>
    /// On player death, transition to white and then decide if player should be given the choice to revive, and if not, kill the player.
    /// </summary>
    private void OnPlayerDeath_EF()
    {
        m_PlayerDeathCount++;

        StartCoroutine(OnPlayerDeath_IEF());
    }

    private IEnumerator OnPlayerDeath_IEF()
    {
        yield return new WaitForSeconds(2f);
        if (m_PlayerDeathCount == 1) TransitionToRevive_F();
        else if (m_PlayerDeathCount > 1) TransitionToMainMenu_F();
    }

    /// <summary>
    /// Transition To the revive part by covering the screen white.
    /// </summary>
    private void TransitionToRevive_F()
    {        
        //DOTween.To(() => 0f, val =>
        //{
        //    MainGameReferences mgr = MainGameReferences.s_Instance;
        //    Image LoopTransition = mgr.transitionMgr;
        //    LoopTransition.color = LoopTransition.color.With(a: val);
        //}, 1f, 1f)
        //    .OnComplete(() =>
        //    {
        //        MainGameReferences mgr = MainGameReferences.s_Instance;
        //        mgr.levelsMgr.LevelsDespawnAll_F();
        //        ReviveMgr reviveMgr = mgr.reviveMgr;
        //        if (reviveMgr.IsAdLoaded)
        //        {
        //            reviveMgr.m_OnReviveEndE += OnReviveProcessEnd_EF;
        //            reviveMgr.gameObject.SetActive(true);
        //        }
        //        else TransitionToMainMenu_F();
        //    });       
        MainGameReferences.INSTANCE.transitionMgr.TransitionDo_F(0.0f, 1.0f, 1.0f, () =>
        {
            MainGameReferences mgr = MainGameReferences.INSTANCE;
            mgr.levelsMgr.LevelsDespawnAll_F();
            ReviveMgr reviveMgr = mgr.reviveMgr;
            RewardedAdWrapper rewardedAd = GlobalMgr.s_Instance.m_AdsMgr.RewardedAd;
            if (rewardedAd.IsValid && rewardedAd.IsLoaded)
            {
                reviveMgr.m_OnReviveEndE += OnReviveProcessEnd_EF;
                reviveMgr.gameObject.SetActive(true);
            }
            else TransitionToMainMenu_F();
        });
    }

    /// <summary>
    /// Listens for the end of revive and then based on user input decides if player should be revived or not.
    /// </summary>
    /// <param name="decision"></param>
    private void OnReviveProcessEnd_EF(bool decision)
    {
        switch(decision)
        {
            case false:
                TransitionToMainMenu_F();
                break;

            case true:
                MainGameReferences.INSTANCE.transitionMgr.TransitionDo_F(0f, 1f, PlayerRevive_F);
                break;
        }
    }

    /// <summary>
    /// Revives the player.
    /// </summary>
    private void PlayerRevive_F()
    {
        GlobalMgr.s_Instance.m_BackgroundMusicMgr.Play_F(m_BackgroundMusicACs[GlobalMgr.s_Instance.m_GlobalData.BackgroundMusicCur]);
        JetPawn player = MainGameReferences.INSTANCE.player;
        player.Revive_F();
        JetPlayerController playerController = MainGameReferences.INSTANCE.playerController;
        playerController.Possess_F(player);
        //MainGameReferences.s_Instance.levelsMgr.PlayerJustRevived = true;
        MainGameReferences.INSTANCE.levelsMgr.RandomLevelSpawn_F(new Vector3(0f, 0f, MainGameReferences.INSTANCE.player.transform.position.z));
    }

    /// <summary>
    /// Transition To the revive part after players dead by covering the screen black.
    /// </summary>
    private void TransitionToMainMenu_F()
    {
        //Debug.Log("Transitioning to Score Display");
        
        //Setting the score and currency values in Global Data and saving it.
        GlobalMgr gdi = GlobalMgr.s_Instance;
        gdi.m_GlobalData.ScoreLastGame = MainGameReferences.INSTANCE.scoreMgr.Score;
        gdi.m_GlobalData.CurrencyLastGame = MainGameReferences.INSTANCE.scoreMgr.Currency;
        gdi.m_GlobalData.CurrencyChange_F(MainGameReferences.INSTANCE.scoreMgr.Currency);
        gdi.m_GlobalData.Save_F();

        //Doing a fade out to black and when fade is done, despawning all levels and loops and then opening the Main Menu Scene.
        MainGameReferences.INSTANCE.transitionMgr.TransitionDo_F(Color.black, 2f, () =>
        {
            GlobalMgr.s_Instance.m_AdsMgr.GamesSinceLastInterstitialAd++;
            MainGameReferences.INSTANCE.levelsMgr.LevelsDespawnAll_F();
            MainGameReferences.INSTANCE.loopsMgr.LoopDespawn_F();

            StartCoroutine(ActivateMainMenuScene_IEF());
            //gdi.m_SceneLoader.LoadScene_F(ScenesLoader.ScenesEN.MainMenu);
        });
    }

    /// <summary>
    /// Activates the Preloaded MainMenuScene. Called when game ends. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateMainMenuScene_IEF()
    {
        while (mainMenuSceneLoadingAsyncOp.IsDone == false) yield return null;

        yield return mainMenuSceneLoadingAsyncOp.Result.ActivateAsync();
    }

#if UNITY_EDITOR
    [MenuItem("Scenes/Main Game")]
    public static void MainGameSceneOpen_F() => EditorSceneManager.OpenScene("Assets/Main Game Stuff/Scenes/MainGame_1_Scene.unity");
#endif

    public enum DifficultyEN { Easy, Normal, Hard }
}
