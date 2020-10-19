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
using UnityEngine.UI;

public class MainGameMgr : MonoBehaviour
{
    #region Variables
    private GlobalDatabaseInitializer m_gdi;
    private MainGameReferences mgr;

    private int m_PlayerDeathCount;
    public int PlayerDeathCount { get => m_PlayerDeathCount; }
    
    [SerializeField] private AudioClip m_BackgroundMusicAC;
    #endregion

    private void Awake()
    {
        m_gdi = GlobalDatabaseInitializer.INSTANCE;
        mgr = MainGameReferences.INSTANCE;
    }

    private void Start()
    {                
        StartCoroutine(Start_IEF());
    }

    private IEnumerator Start_IEF()
    {
        //TESTING- Waiting for all assets to load into their database. This part will be moved somewhere else later.
        while (m_gdi.AllLoaded == false) yield return null;

        //Play Background Music
        GlobalDatabaseInitializer.INSTANCE.m_BackgroundMusicMgr.Play_F(m_BackgroundMusicAC);

        //First Spawn player and possess it because loops and levels are spawned from player position.        
        JetPawn spawnedPlayer = m_gdi.m_JetsDatabase.JetCurInstantiate_F().GetComponent<JetPawn>();
        spawnedPlayer.transform.position = Vector3.forward * 5;
        //Debug.Log($"<color=green>Spawned Player default pos = {spawnedPlayer.transform.position}</color>");
        mgr.player = spawnedPlayer;
        mgr.player.OnDeath_E += OnPlayerDeath_EF;
        mgr.playerController.Possess_F(spawnedPlayer);

        //Getting all loops and levels from their database.
        mgr.loopsMgr.LoopsFieldSetup_F();
        mgr.levelsMgr.GetAllLevelsForGame_F();

        //Spawning the first loops and levels.
        mgr.loopsMgr.RandomLoopSpawn_F();
        mgr.levelsMgr.RandomLevelSpawn_F();        
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
        DOTween.To(() => 0f, val =>
        {
            MainGameReferences mgr = MainGameReferences.INSTANCE;
            Image LoopTransition = mgr.LoopTransition;
            LoopTransition.color = LoopTransition.color.With(a: val);
        }, 1f, 1f)
            .OnComplete(() =>
            {
                MainGameReferences mgr = MainGameReferences.INSTANCE;
                mgr.levelsMgr.LevelsDespawnAll_F();
                ReviveMgr reviveMgr = mgr.reviveMgr;
                if (reviveMgr.IsAdLoaded)
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
                MainGameReferences.INSTANCE.LoopTransition.DOFade(0f, 1f).OnComplete(PlayerRevive_F);
                break;
        }
    }

    /// <summary>
    /// Revives the player.
    /// </summary>
    private void PlayerRevive_F()
    {
        GlobalDatabaseInitializer.INSTANCE.m_BackgroundMusicMgr.Play_F(m_BackgroundMusicAC);
        JetPawn player = MainGameReferences.INSTANCE.player;
        player.Revive_F();
        JetPlayerController playerController = MainGameReferences.INSTANCE.playerController;
        playerController.Possess_F(player);
        MainGameReferences.INSTANCE.levelsMgr.PlayerJustRevived = true;
        MainGameReferences.INSTANCE.levelsMgr.RandomLevelSpawn_F();
    }

    /// <summary>
    /// Transition To the revive part after players dead by covering the screen black.
    /// </summary>
    private void TransitionToMainMenu_F()
    {
        //Debug.Log("Transitioning to Score Display");
        
        //Setting the score and currency values in Global Data and saving it.
        GlobalDatabaseInitializer gdi = GlobalDatabaseInitializer.INSTANCE;
        gdi.m_GlobalData.ScoreLastGame = MainGameReferences.INSTANCE.scoreMgr.Score;
        gdi.m_GlobalData.CurrencyLastGame = MainGameReferences.INSTANCE.scoreMgr.Currency;
        gdi.m_GlobalData.CurrencyChange_F(MainGameReferences.INSTANCE.scoreMgr.Currency);
        gdi.m_GlobalData.Save_F();

        //Doing a fade out to black and when fade is done, despawning all levels and loops and then opening the Main Menu Scene.
        MainGameReferences.INSTANCE.LoopTransition.DOColor(Color.black, 2f).OnComplete(() =>
        {
            GlobalDatabaseInitializer.INSTANCE.m_AdsMgr.GamesSinceLastInterstitialAd++;
            MainGameReferences.INSTANCE.levelsMgr.LevelsDespawnAll_F();
            MainGameReferences.INSTANCE.loopsMgr.LoopsAllDespawn_F();           
            gdi.m_ScenesDatabase.LoadScene_F(Scenes_EN.MainMenu);
        });
    }

#if UNITY_EDITOR
    [PostProcessScene]
    public static void PostProcessScene_F()
    {
        Debug.Log("<color=green>Post Processed</color>");
    }

    [MenuItem("Scenes/Main Game")]
    public static void MainGameSceneOpen_F()
    {
        EditorSceneManager.OpenScene("Assets/Main Game Stuff/Scenes/MainGame_1_Scene.unity");
    }
#endif
}
