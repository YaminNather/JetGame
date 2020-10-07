using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMgr : MonoBehaviour
{
    #region Variables
    [SerializeField]private Text ScoreValue_Lbl;
    private int m_AccumulatedScorePrev;
    private int m_AccumulatedScoreCur;
    public int Score { get => m_AccumulatedScorePrev + m_AccumulatedScoreCur; }

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

    private void Update()
    {
        if(m_IsRecording)
        {
            m_AccumulatedScoreCur = (int)(m_player.transform.position.z - m_RecordingStartPos.z) / ScoreModifier;
            ScoreUIUpdate_F();
        }
    }

    public void ScoreRecordingStart_F(JetPlayerController playerController)
    {
        Pawn possessedPlayer = playerController.PossessedPawn;
        if (m_IsRecording == true || possessedPlayer == null)
            return;

        m_IsRecording = true;
        m_player = possessedPlayer;
        m_RecordingStartPos = possessedPlayer.transform.position;
    }    

    public void ScoreRecordingStop_F()
    {
        if (!m_IsRecording)
            return;

        m_AccumulatedScorePrev += m_AccumulatedScoreCur;
        m_player = null;
        m_IsRecording = false;
    }

    public void ScoreRecordingReset_F()
    {
        if (m_IsRecording) ScoreRecordingStop_F();

        m_AccumulatedScorePrev = m_AccumulatedScoreCur = 0;
    }

    private void ScoreUIUpdate_F()
    {
        ScoreValue_Lbl.text =  "" + (m_AccumulatedScorePrev + m_AccumulatedScoreCur);
    }

    public void CurrencyReset_F() => m_Currency = 0;

    public void CurrencyAdd_F(int amount)
    {
        m_Currency += amount;
        CurrencyUIUpdate_F();
    }

    private void CurrencyUIUpdate_F() => CurrencyValue_Lbl.text = "" + m_Currency;    
}
