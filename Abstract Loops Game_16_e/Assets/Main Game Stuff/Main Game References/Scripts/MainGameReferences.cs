using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameReferences : MonoBehaviour
{
    #region Variables
    public static MainGameReferences s_Instance;

    public MainGameMgr mainGameMgr;
    public JetPlayerController playerController;
    public JetPawn player;
    public PlayerExplosionPSMgr playerExplosionPSMgr;
    public Image LoopTransition;
    public LoopsMgr loopsMgr;
    public LevelsMgr levelsMgr;
    public ColorMgr colorMgr;
    public ScoreMgr scoreMgr;
    public ReviveMgr reviveMgr;
    public CurrencyPSMgr currencyPSMgr;
    #endregion

    private void Awake()
    {
        s_Instance = this;
    }
}
