using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsMgr : MonoBehaviour
{
    #region Variables
    private BannerAdWrapper m_BannerAd; 
    public BannerAdWrapper BannerAd 
    {
        get
        {
            if (m_BannerAd != null && m_BannerAd.IsValid == false) m_BannerAd = null;
            return m_BannerAd;
        }
    }

    private InterstitialAdWrapper m_InterstitialAd;   
    public InterstitialAdWrapper InterstitialAd
    {
        get
        {
            if(m_InterstitialAd != null && m_InterstitialAd.IsValid == false) m_InterstitialAd = null;
            return m_InterstitialAd;
        }
    }
    private int m_GamesSinceLastInterstitialAd;
    public int GamesSinceLastInterstitialAd { get => m_GamesSinceLastInterstitialAd; set => m_GamesSinceLastInterstitialAd = value; }

    private RewardedAdWrapper m_RewardedAd;    
    public RewardedAdWrapper RewardedAd 
    {
        get
        {
            if (m_RewardedAd != null && m_RewardedAd.IsValid == false) m_RewardedAd = null;
            return m_RewardedAd;
        }
    }
    #endregion

    private void Start()
    {
        MobileAds.Initialize(status => { });        
    } 
    
    public void BannerCheckAndCreate_F()
    {
        if(m_BannerAd.IsValid_F() == false)
        {
            m_BannerAd = new BannerAdWrapper("");
            m_BannerAd.LoadAndShow_F();            
        }
    }

    public void InterstitialAdCheckAndCreate_F()
    {
        if (m_InterstitialAd.IsValid_F() == false)
        {
            m_InterstitialAd = new InterstitialAdWrapper("", false);
            m_InterstitialAd.InterstitialAd.OnAdOpening += (sender, e) => m_GamesSinceLastInterstitialAd = 0;
            m_InterstitialAd.InterstitialAd.OnAdClosed += (sender, e) => InterstitialAdCheckAndCreate_F();
            m_InterstitialAd.Load_F();
        }
    }    
}

public class BannerAdWrapper
{
    #region Variables
    private bool m_IsValid;
    public bool IsValid => m_IsValid;

    private BannerView m_BannerView;
    public BannerView BannerView => m_BannerView;

    #endregion

    public BannerAdWrapper(string appId)
    {
        m_BannerView = new BannerView(appId, AdSize.Banner, AdPosition.Bottom);
        
        m_BannerView.OnAdFailedToLoad += (sender, e) => m_IsValid = false;
        m_BannerView.OnAdClosed += (sender, e) => m_IsValid = false;

        m_IsValid = true;
    }

    public void LoadAndShow_F()
    {
        if (IsValid == false) return;        
        
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        m_BannerView.LoadAd(request);
    }    
}

public class InterstitialAdWrapper
{
    #region Variables
    private bool m_IsValid;
    public bool IsValid => m_IsValid;

    private InterstitialAd m_InterstitialAd;
    public InterstitialAd InterstitialAd => m_InterstitialAd;
    private bool m_ShowOnLoad;
    #endregion

    public InterstitialAdWrapper(string appId, bool m_ShowOnLoad)
    {
        m_InterstitialAd = new InterstitialAd(appId);

        m_InterstitialAd.OnAdLoaded += OnAdLoaded_EF;
        m_InterstitialAd.OnAdFailedToLoad += (sender, e) => m_IsValid = true;
        m_InterstitialAd.OnAdClosed += (sender, e) => m_IsValid = true;

        this.m_ShowOnLoad = m_ShowOnLoad;
        m_IsValid = true;
    }

    public void Load_F()
    {
        if (m_IsValid == false) return;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        m_InterstitialAd.LoadAd(request);
    }

    public void Show_F()
    {
        if (m_IsValid == false) return;

        m_InterstitialAd.Show();
    }

    private void OnAdLoaded_EF(object sender, EventArgs e)
    {
        if (m_IsValid == true && m_ShowOnLoad == true) m_InterstitialAd.Show();
    }
}

public class RewardedAdWrapper
{
    #region Variables
    private bool m_IsValid;
    public bool IsValid => m_IsValid;

    private RewardedAd m_RewardedAd;
    public RewardedAd RewardedAd => m_RewardedAd;
    private bool m_ShowOnLoad;   
    #endregion

    public RewardedAdWrapper(string appId, bool m_ShowOnLoad, EventHandler<Reward> onUserEarnedReward_E = null)
    {
        m_RewardedAd = new RewardedAd(appId);

        m_RewardedAd.OnAdLoaded += OnAdLoaded_EF;        
        m_RewardedAd.OnUserEarnedReward += onUserEarnedReward_E;

        this.m_ShowOnLoad = m_ShowOnLoad;
        m_IsValid = true;
    }

    public void Load_F()
    {
        if (m_IsValid == true) return;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        m_RewardedAd.LoadAd(request);
    }

    public void Show_F()
    {
        if (m_IsValid == true) return;

        m_RewardedAd.Show();
    }

    private void OnAdLoaded_EF(object sender, EventArgs e)
    {
        if (m_IsValid == true && m_ShowOnLoad == true) m_RewardedAd.Show();
    }
}

public static class AdWrappersExtensionMethods
{
    public static bool IsValid_F(this BannerAdWrapper ad)
    {
        if (ad == null) return false;
        else return ad.IsValid;
    }

    public static bool IsValid_F(this InterstitialAdWrapper ad)
    {
        if (ad == null) return false;
        else return ad.IsValid;
    }
}