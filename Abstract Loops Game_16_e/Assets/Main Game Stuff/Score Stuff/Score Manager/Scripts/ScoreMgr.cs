using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMgr : MonoBehaviour
{
    #region Variables
    [Header("Score Stuff")]
    [SerializeField]private Text ScoreValue_Lbl;
    private int m_Score;
    public int Score { get => m_Score; }

    [Header("Currency Stuff")]
    [SerializeField] private Text CurrencyValue_Lbl;
    private int m_Currency;
    public int Currency { get => m_Currency; }       
    #endregion

    private void Awake()
    {
        ScoreValue_Lbl.text = "0";
        CurrencyValue_Lbl.text = "0";
    }

    /// <summary>
    /// Resets the score back to zero. 
    /// </summary>
    public void ScoreRecordingReset_F() => m_Score = 0;

    /// <summary>
    /// Adds to the score.
    /// </summary>
    /// <param name="amount"></param>
    public void ScoreAdd_F(int amount)
    {
        m_Score += amount;
        ScoreUIRefresh_F();
    }

    /// <summary>
    /// Refreshes the Score UI to match the m_Score value.
    /// </summary>
    private void ScoreUIRefresh_F() => ScoreValue_Lbl.text = "" + m_Score;

    /// <summary>
    /// Resets the currency back to zero. 
    /// </summary>
    public void CurrencyReset_F() => m_Currency = 0;

    /// <summary>
    /// Adds to the currency.
    /// </summary>
    /// <param name="amount"></param>
    public void CurrencyAdd_F(int amount)
    {
        m_Currency += amount;
        CurrencyUIRefresh_F();
    }

    /// <summary>
    /// Refreshes the Currency UI to match the m_Currency value.
    /// </summary>
    private void CurrencyUIRefresh_F() => CurrencyValue_Lbl.text = "" + m_Currency;    
}
