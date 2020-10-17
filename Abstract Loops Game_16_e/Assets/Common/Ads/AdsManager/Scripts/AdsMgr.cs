using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsMgr : MonoBehaviour
{
    #region Variables
    private BannerAdWrapper m_BannerAd; 
    public BannerAdWrapper BannerAd { get => BannerAd; }

    private InterstitialAdWrapper m_InterstitialAd;   
    public InterstitialAdWrapper InterstitialAd { get => m_InterstitialAd; }
    private int m_GamesSinceLastInterstitialAd;
    public int GamesSinceLastInterstitialAd { get => m_GamesSinceLastInterstitialAd; set => m_GamesSinceLastInterstitialAd = value; }

    private RewardedAdWrapper m_RewardedAd;    
    public RewardedAdWrapper RewardedAd { get => m_RewardedAd; }
    #endregion

    private void Start()
    {
        MobileAds.Initialize(status => { });        
    } 
    
    public void BannerCheckAndCreate_F()
    {
        if(m_BannerAd == null)
        {
            m_BannerAd = new BannerAdWrapper("");
            m_BannerAd.BannerView.OnAdFailedToLoad += (sender, e) => m_BannerAd = null;
            m_BannerAd.LoadAndShow_F();            
        }
    }

    public void InterstitialAdCheckAndCreate_F()
    {
        if (m_InterstitialAd == null)
        {
            m_InterstitialAd = new InterstitialAdWrapper("", false);
            m_InterstitialAd.InterstitialAd.OnAdFailedToLoad += (sender, e) => m_InterstitialAd = null;
            m_InterstitialAd.InterstitialAd.OnAdOpening += (sender, e) => m_GamesSinceLastInterstitialAd = 0;
            m_InterstitialAd.InterstitialAd.OnAdClosed += (sender, e) => 
            {
                m_InterstitialAd = new InterstitialAdWrapper("", false);
                m_InterstitialAd.Load_F();
            };
            m_InterstitialAd.Load_F();
        }
    }
}

public class BannerAdWrapper
{
    #region Variables
    private BannerView m_BannerView;
    public BannerView BannerView { get => m_BannerView; }   
    #endregion

    public BannerAdWrapper(string appId)
    {
        m_BannerView = new BannerView(appId, AdSize.Banner, AdPosition.Bottom);        
    }

    public void LoadAndShow_F()
    {
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        m_BannerView.LoadAd(request);
    }
}

public class InterstitialAdWrapper
{
    #region Variables
    private InterstitialAd m_InterstitialAd;
    public InterstitialAd InterstitialAd { get => m_InterstitialAd; }
    private bool m_ShowOnLoad;
    #endregion

    public InterstitialAdWrapper(string appId, bool m_ShowOnLoad)
    {
        m_InterstitialAd = new InterstitialAd(appId);

        m_InterstitialAd.OnAdLoaded += OnAdLoaded_EF;

        this.m_ShowOnLoad = m_ShowOnLoad;
    }

    public void Load_F()
    {
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        m_InterstitialAd.LoadAd(request);
    }

    public void Show_F()
    {
        m_InterstitialAd.Show();
    }

    private void OnAdLoaded_EF(object sender, EventArgs e)
    {
        if (m_ShowOnLoad == true) m_InterstitialAd.Show();
    }
}

public class RewardedAdWrapper
{
    #region Variables
    private RewardedAd m_RewardedAd;
    public RewardedAd RewardedAd { get => m_RewardedAd; }
    private bool m_ShowOnLoad;   
    #endregion

    public RewardedAdWrapper(string appId, bool m_ShowOnLoad, EventHandler<Reward> onUserEarnedReward_E = null)
    {
        m_RewardedAd = new RewardedAd(appId);

        m_RewardedAd.OnAdLoaded += OnAdLoaded_EF;        
        m_RewardedAd.OnUserEarnedReward += onUserEarnedReward_E;

        this.m_ShowOnLoad = m_ShowOnLoad;  
    }

    public void Load_F()
    {
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        m_RewardedAd.LoadAd(request);
    }

    public void Show_F()
    {
        m_RewardedAd.Show();
    }

    private void OnAdLoaded_EF(object sender, EventArgs e)
    {
        if (m_ShowOnLoad == true) m_RewardedAd.Show();
    }
}
