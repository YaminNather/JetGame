using DG.Tweening;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveMgr : MonoBehaviour
{
    #region Variables
    [Header("UI References")]
    [SerializeField] private Image m_FullPulse_Image;
    [SerializeField] private Image m_NoPulse_Image;
    [SerializeField] private GameObject m_ReviveBtnGObj;

    [Header("Audio Stuff")]
    private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_HeartbeatingAC;
    [SerializeField] private AudioClip m_HeartFlatlineAC;

    [Space(20)]

    [SerializeField] private int m_ReviveTime = 3;

    private Sequence m_Pulse_Seq;
    public System.Action<bool> m_OnReviveEndE;

    private RewardedAdWrapper m_RewardedAd;
    public bool IsAdLoaded => m_RewardedAd != null;

    #endregion

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(RewardedAdTryLoad_IEF());        
    }

    private IEnumerator RewardedAdTryLoad_IEF()
    {
        int triesCount = 0;
        tryToLoad_EF();
        yield break;
        
        void tryToLoad_EF()
        {
            if (triesCount < 2)
            {
                m_RewardedAd = new RewardedAdWrapper("", false);
                m_RewardedAd.Load_F();
                m_RewardedAd.RewardedAd.OnAdFailedToLoad += (sender, e) => tryToLoad_EF();
                m_RewardedAd.RewardedAd.OnAdClosed += (sender, e) => m_OnReviveEndE.Invoke(true);
                triesCount++;
            }
            else
            {
                m_RewardedAd = null;
            }
        }
    }

    private void OnEnable()
    {
        m_ReviveBtnGObj.SetActive(true);
        m_FullPulse_Image.gameObject.SetActive(true);
        m_NoPulse_Image.gameObject.SetActive(false);
        m_Pulse_Seq = DOTween.Sequence();
        m_FullPulse_Image.color = m_NoPulse_Image.color  = new Color(0f, 0f, 0f);        
        for(int i = 0; i < m_ReviveTime; i++)
        {
            m_Pulse_Seq.AppendCallback(() => m_AudioSource.PlayOneShot(m_HeartbeatingAC));
            m_Pulse_Seq.Append(DOTween.To(() => 0f, val => m_FullPulse_Image.color = new Color(0f, val, 0f), 1f, 0.9f));
            m_Pulse_Seq.Append(DOTween.To(() => 0f, val => m_FullPulse_Image.color = new Color(val, 1f, 0f), 1f, 0.1f));
        }
        m_Pulse_Seq.AppendCallback(() =>
        {
            m_ReviveBtnGObj.SetActive(false);
            m_FullPulse_Image.gameObject.SetActive(false);
            m_NoPulse_Image.gameObject.SetActive(true);            
        });
        m_Pulse_Seq.AppendCallback(() => m_AudioSource.PlayOneShot(m_HeartFlatlineAC));
        m_Pulse_Seq.Append(DOTween.To(() => 0f, val => m_NoPulse_Image.color = new Color(0f, val, 0f), 1f, 0.9f));
        m_Pulse_Seq.Append(DOTween.To(() => 0f, val => m_NoPulse_Image.color = new Color(val, 1f, 0f), 1f, 0.1f));
        m_Pulse_Seq.AppendCallback(() =>
        {
            m_OnReviveEndE?.Invoke(false);
            gameObject.SetActive(false);
        });
    } 

    public void Revive_BEF()
    {
        if (m_Pulse_Seq.IsActive())
            m_Pulse_Seq.Kill();
        gameObject.SetActive(false);

#if UNITY_EDITOR
        m_OnReviveEndE?.Invoke(true);
#else
        m_RewardedAd.Show_F();
#endif
    }    
}
