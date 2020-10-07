using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class MainGameMgr : MonoBehaviour
{
    #region Variables
    private GlobalDatabaseInitializer gdi;
    private MainGameReferences mgr;

    private int m_PlayerDeathCount;
    public int PlayerDeathCount { get => m_PlayerDeathCount; }
    #endregion

    private void Awake()
    {
        gdi = GlobalDatabaseInitializer.s_Instance;
        mgr = MainGameReferences.INSTANCE;
    }

    private void Start()
    {                
        StartCoroutine(Start_IEF());
    }

    private IEnumerator Start_IEF()
    {
        //TESTING- Waiting for all assets to load into their database. This part will be moved somewhere else later.
        while (gdi.AllLoaded == false) yield return null;

        //First Spawn player and possess it because loops and levels are spawned from player position.
        AsyncOperationHandle<GameObject> asyncOpHandle_0 = Addressables.InstantiateAsync(gdi.globalData.JetCur);
        yield return asyncOpHandle_0;
        JetPawn spawnedPlayer = asyncOpHandle_0.Result.GetComponent<JetPawn>();
        spawnedPlayer.transform.position = Vector3.forward * 5;
        Debug.Log($"<color=green>Spawned Player default pos = {spawnedPlayer.transform.position}</color>");
        mgr.player = spawnedPlayer;
        mgr.player.OnDeath_E += OnPlayerDeath_EF;
        mgr.playerController.Possess_F(spawnedPlayer);

        //Getting all loops and levels from their database.
        mgr.loopsMgr.LoopsFieldSetup_F();
        mgr.levelsMgr.LevelsAssignAll_F();

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
        else if (m_PlayerDeathCount > 1) TransitionToScoreDisplay_F();
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
                reviveMgr.m_OnReviveEnd_E += OnReviveProcessEnd_EF;
                reviveMgr.gameObject.SetActive(true);
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
                TransitionToScoreDisplay_F();
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
    private void TransitionToScoreDisplay_F()
    {
        Debug.Log("Transitioning to Score Display");
        GlobalDatabaseInitializer gdi = GlobalDatabaseInitializer.s_Instance;
        gdi.globalData.ScoreLastGame = MainGameReferences.INSTANCE.scoreMgr.Score;
        gdi.globalData.CurrencyLastGame = MainGameReferences.INSTANCE.scoreMgr.Currency;
        gdi.globalData.CurrencyChange_F(MainGameReferences.INSTANCE.scoreMgr.Currency);
        gdi.globalData.Save_F();
        MainGameReferences.INSTANCE.LoopTransition.DOColor(Color.black, 2f).OnComplete(() =>
        {
            MainGameReferences.INSTANCE.levelsMgr.LevelsDespawnAll_F();
            MainGameReferences.INSTANCE.loopsMgr.LoopsAllDespawn_F();
            gdi.globalData.m_MainMenuPageToOpen = MainMenuSceneMgr.Pages_EN.ScoreBoard;
            gdi.scenesDatabase.LoadScene_F(Scenes_EN.MainMenu);
        });
    }
}
