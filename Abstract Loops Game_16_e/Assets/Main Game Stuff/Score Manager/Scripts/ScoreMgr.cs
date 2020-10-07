using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMgr : MonoBehaviour
{
    #region Variables
    [SerializeField]private Text ScoreValue_Lbl;
    private int m_Score;
    public int Score { get => m_Score; }

    [SerializeField] private Text CurrencyValue_Lbl;
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
        ScoreValue_Lbl.text = "0";
        CurrencyValue_Lbl.text = "0";
    }

    public void ScoreAdd_F(int amount)
    {
        m_Score += amount;
        ScoreUIUpdate_F();
    }

    private void ScoreUIUpdate_F() => ScoreValue_Lbl.text =  "" + m_Score;

    public void CurrencyReset_F() => m_Currency = 0;

    public void CurrencyAdd_F(int amount)
    {
        m_Currency += amount;
        CurrencyUIUpdate_F();
    }

    private void CurrencyUIUpdate_F() => CurrencyValue_Lbl.text = "" + m_Currency;    
}
