using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.QuickSearch;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ScoreBestCheckpointMgr : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject m_ScoreBestDiffHolder;
    [SerializeField] private Text m_ScoreBestDiffValueLbl;
    [SerializeField] private AnimationCurve m_ScoreBestDiffUpdateAC;
    [SerializeField] private RectTransform m_LRStreakImageRT;
    [SerializeField] private RectTransform m_RLStreakImageRT;
    private int m_ScoreBest;

    [SerializeField] private Text m_ScoreBestCrossedLbl;
    #endregion

    public void CountdownStart_F(int scoreBest)
    {
        Debug.Log("<color=yellow>Countdown Started</color>");
        m_ScoreBest = scoreBest;
        m_ScoreBestDiffValueLbl.text = 10 + "";
        
        m_ScoreBestDiffHolder.transform.localScale = Vector3.zero;
        m_ScoreBestDiffHolder.transform.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack);
        MainGameReferences.INSTANCE.scoreMgr.ScoreOnUpdate_E += ScoreOnUpdate_EF;
    }

    private void ScoreOnUpdate_EF(int score)
    {
        int diff = m_ScoreBest - score;
        if (diff != 0)
        {
            m_ScoreBestDiffValueLbl.text = diff + "";
            m_ScoreBestDiffHolder.transform.localScale = Vector3.one;
            m_ScoreBestDiffHolder.transform.DOScale(1.2f, 0.5f).SetEase(m_ScoreBestDiffUpdateAC);
        }
        else
            ShowScoreBestCrossedLbl_F();
    }

    private void ShowScoreBestCrossedLbl_F()
    {
        MainGameReferences.INSTANCE.scoreMgr.ScoreOnUpdate_E -= ScoreOnUpdate_EF;
        m_ScoreBestDiffHolder.SetActive(false);
        m_ScoreBestCrossedLbl.gameObject.SetActive(true);
        m_LRStreakImageRT.gameObject.SetActive(true);
        m_RLStreakImageRT.gameObject.SetActive(true);

        m_ScoreBestCrossedLbl.transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        float time = 0.0f;
        sequence.Append(m_ScoreBestCrossedLbl.transform.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack));
        time = 0.5f;
        for (int i = 0; i < 1; i++)
        {
            sequence.Append(m_RLStreakImageRT.DOAnchorPosX(m_LRStreakImageRT.anchoredPosition.x, 0.5f).SetEase(Ease.OutBack));
            sequence.Insert(0.5f,m_LRStreakImageRT.DOAnchorPosX(m_RLStreakImageRT.anchoredPosition.x, 0.5f).SetEase(Ease.OutBack));
            time += 0.5f;
            sequence.Append(m_RLStreakImageRT.DOAnchorPosX(Screen.width * 2.0f, 0.5f).SetEase(Ease.OutBack));
            sequence.Insert(time, m_LRStreakImageRT.DOAnchorPosX(-Screen.width * 2.0f, 0.5f).SetEase(Ease.OutBack));
        }
        sequence.Insert(time, m_ScoreBestCrossedLbl.transform.DOScale(0.0f, 0.1f).SetEase(Ease.InBack));
        time += 0.5f;
        sequence.AppendCallback(() => gameObject.SetActive(false));
    }
}
