using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMgr : MonoBehaviour
{
    #region Variables
    [Header("Score Stuff")]
    [SerializeField] private Text m_ScoreValue_Lbl;
    private int m_Score;
    public int Score => m_Score; 
    private Tweener m_ScoreValueUpdateT;
    [SerializeField] private AnimationCurve m_ScoreValueLblUpdateTEaseAC;
    public System.Action<int> ScoreOnUpdate_E;

    private int m_ScoreBest;
    private ScoreBestCheckpointMgr m_ScoreBestCheckpointMgr;

    [Header("Currency Stuff")]
    [SerializeField] private Text m_CurrencyValue_Lbl;
    private int m_Currency;
    public int Currency { get => m_Currency; set => m_Currency = value; }
    #endregion

    private void Awake()
    {
        m_ScoreValue_Lbl.text = "0";
        m_ScoreValueUpdateT = DOTween.To(() => 1f, val => m_ScoreValue_Lbl.transform.localScale = new Vector3(val, val, val), 1.5f, 0.5f)
            .SetEase(m_ScoreValueLblUpdateTEaseAC)
            .SetAutoKill(false)
            .Pause();
        m_ScoreBestCheckpointMgr = GetComponentInChildren<ScoreBestCheckpointMgr>(true);
        
        m_CurrencyValue_Lbl.text = "0";

    }

    public void ScoreBestSet_F() => m_ScoreBest = GlobalMgr.INSTANCE.m_GlobalData.ScoreBest;

    public void ScoreAdd_F(int amount)
    {
        m_Score += amount;
        if (m_Score == 20)
            MainGameReferences.INSTANCE.mainGameMgr.Difficulty = MainGameMgr.DifficultyEN.Normal;
        else if(m_Score == 10)
            MainGameReferences.INSTANCE.mainGameMgr.Difficulty = MainGameMgr.DifficultyEN.Hard;

        ScoreOnUpdate_E?.Invoke(m_Score);

        if (m_Score == m_ScoreBest - 10)
        {
            m_ScoreBestCheckpointMgr.gameObject.SetActive(true);
            m_ScoreBestCheckpointMgr.CountdownStart_F(m_ScoreBest);
        }

        ScoreUIUpdate_F();
    }

    private void ScoreUIUpdate_F()
    {
        
        m_ScoreValue_Lbl.text = "" + m_Score;

        m_ScoreValueUpdateT.Restart();
    }

    public void CurrencyReset_F() => m_Currency = 0;

    public void CurrencyAdd_F(int amount)
    {
        m_Currency += amount;
        CurrencyUIUpdate_F();
    }

    private void CurrencyUIUpdate_F() => m_CurrencyValue_Lbl.text = "" + m_Currency;
}
