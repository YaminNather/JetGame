using DG.Tweening;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReviveMgr : MonoBehaviour
{
    #region Variables
    [Header("UI References")]
    [SerializeField] private Image m_FullPulse_Image;
    [SerializeField] private Image m_NoPulse_Image;
    [SerializeField] private GameObject m_ReviveBtnGObj;
    [SerializeField] private Transform m_ReviveLblImageTrans;

    [Header("Audio Stuff")]
    private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_HeartbeatingAC;
    [SerializeField] private AudioClip m_HeartFlatlineAC;

    [Space(20)] [SerializeField] private AnimationCurve m_ReviveLblImageAnimationAC;
    [SerializeField] private int m_ReviveTime = 3;

    private Sequence m_Pulse_Seq;
    public System.Action<bool> m_OnReviveEndE;

    private RewardedAdWrapper RewardedAd => GlobalMgr.s_Instance.m_AdsMgr.RewardedAd;

    private bool IsAdClosed;
    #endregion

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void LoadAd_F()
    {
        GlobalMgr.s_Instance.m_AdsMgr.RewardedAdCheckAndCreate_F(false);
        RewardedAd.m_RewardedAd.OnAdClosed += (object sender, EventArgs e) => IsAdClosed = true;
    }

    private void OnEnable()
    {
        m_ReviveBtnGObj.SetActive(true);
        m_FullPulse_Image.gameObject.SetActive(true);
        m_NoPulse_Image.gameObject.SetActive(false);
        m_Pulse_Seq = DOTween.Sequence();
        m_FullPulse_Image.color = m_NoPulse_Image.color  = new Color(0f, 0f, 0f); 
        
        //Heartbeat Pulses.
        for(int i = 0; i < m_ReviveTime; i++)
        {
            m_Pulse_Seq.AppendCallback(() => m_AudioSource.PlayOneShot(m_HeartbeatingAC));
            m_Pulse_Seq.Append(DOTween.To(() => 0f, val => m_FullPulse_Image.color = new Color(0f, val, 0f), 1f, 0.9f));
            m_Pulse_Seq.Append(DOTween.To(() => 0f, val => m_FullPulse_Image.color = new Color(val, 1f, 0f), 1f, 0.1f));
            m_Pulse_Seq.Insert(i, m_ReviveLblImageTrans.DOScale(1.2f, 0.9f).SetEase(m_ReviveLblImageAnimationAC));
        }
        m_Pulse_Seq.AppendCallback(() =>
        {
            m_ReviveBtnGObj.SetActive(false);
            m_FullPulse_Image.gameObject.SetActive(false);
            m_NoPulse_Image.gameObject.SetActive(true);            
        });
        
        //Heartbeat Flatline.
        m_Pulse_Seq.AppendCallback(() => m_AudioSource.PlayOneShot(m_HeartFlatlineAC));
        m_Pulse_Seq.Append(DOTween.To(() => 0f, val => m_NoPulse_Image.color = new Color(0f, val, 0f), 1f, 0.9f));
        m_Pulse_Seq.Append(DOTween.To(() => 0f, val => m_NoPulse_Image.color = new Color(val, 1f, 0f), 1f, 0.1f));
        m_Pulse_Seq.Insert(m_ReviveTime, m_ReviveLblImageTrans.DOScale(0.0f,1f).SetEase(Ease.InBack));
        m_Pulse_Seq.AppendCallback(() =>
        {
            m_OnReviveEndE?.Invoke(false);
            gameObject.SetActive(false);
        });
    } 

    public void Revive_BEF()
    {
        StartCoroutine(ReviveProcess_IEF());
    }

    private IEnumerator ReviveProcess_IEF()
    {
        if (m_Pulse_Seq.IsActive())
            m_Pulse_Seq.Kill();

        Debug.Log("<color=magenta>Rewarded Ad to be shown now.</color>");

#if UNITY_EDITOR
        m_OnReviveEndE?.Invoke(true);
#else
        RewardedAd.Show_F();

        while(!IsAdClosed) yield return null;

        Debug.Log("<color=66FF00>Video Ad closed.</color>");
        Debug.Log($"Rewarded Ad == null? {RewardedAd == null}");
        m_OnReviveEndE?.Invoke(RewardedAd.RewardRecieved);
#endif
        
        gameObject.SetActive(false);
        yield break;
    }
}