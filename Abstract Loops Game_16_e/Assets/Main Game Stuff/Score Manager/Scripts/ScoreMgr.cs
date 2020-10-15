using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMgr : MonoBehaviour
{
    #region Variables
    [SerializeField]private Text m_ScoreValue_Lbl;
    private int m_Score;
    public int Score { get => m_Score; }
    private Tweener m_ScoreValueUpdateT;
    [SerializeField] private AnimationCurve m_ScoreValueLblUpdateTEaseAC;

    [SerializeField] private Text m_CurrencyValue_Lbl;
    private int m_Currency;
    public int Currency { get => m_Currency; set => m_Currency = value; }

    private bool m_IsRecording;
    public bool IsRecording { get => m_IsRecording; }
    private Vector3 m_RecordingStartPos;
    private Pawn m_player;

    [SerializeField] private int ScoreModifier;
    #endregion

    private void Awake()
    {
        m_ScoreValue_Lbl.text = "0";
        m_ScoreValueUpdateT = DOTween.To(() => 1f, val => m_ScoreValue_Lbl.transform.localScale = new Vector3(val, val, val), 1.5f, 0.5f)
            .SetEase(m_ScoreValueLblUpdateTEaseAC)
            .SetAutoKill(false)
            .Pause();
        
        m_CurrencyValue_Lbl.text = "0";

    }

    public void ScoreAdd_F(int amount)
    {
        m_Score += amount;
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
