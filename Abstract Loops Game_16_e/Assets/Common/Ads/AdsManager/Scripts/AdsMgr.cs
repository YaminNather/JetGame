using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public class AdsMgr : MonoBehaviour
{
    #region Variables
    private BannerAdWrapper m_BannerAd; 
    public BannerAdWrapper BannerAd 
    {
        get
        {
            return m_BannerAd;
        }
    }

    private InterstitialAdWrapper m_InterstitialAd;
    public InterstitialAdWrapper InterstitialAd
    {
        get
        {
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
            return m_RewardedAd;
        }
    }
    #endregion

    private void Start()
    {
        MobileAds.Initialize(status => Debug.Log("<color=cyan>Initialization Finished</color>"));
    } 
    
    public void BannerCheckAndCreate_F()
    {
        if (m_BannerAd.IsValid_F() && m_BannerAd.IsLoaded == false) return;

        Debug.Log($"<color=#66FF00>Banner Ad Loading</color>");
#if UNITY_ANDROID
        const string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        const string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        const string adUnitId = "unexpected_platform";
#endif
        m_BannerAd = new BannerAdWrapper(adUnitId);
    }

    public void InterstitialAdCheckAndCreate_F(bool showOnLoad)
    {
        if (m_InterstitialAd.IsValid_F() && m_RewardedAd.IsLoaded == false) return;

        Debug.Log($"<color=#66FF00>Interstitial Ad Loading</color>");
#if UNITY_ANDROID
        const string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
            const string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
            const string adUnitId = "unexpected_platform";
#endif
            m_InterstitialAd = new InterstitialAdWrapper(adUnitId, showOnLoad);
    }

    public void RewardedAdCheckAndCreate_F(bool showOnLoad, EventHandler<Reward> onUserEarnedReward = null)
    {
        if (m_RewardedAd.IsValid_F() && m_RewardedAd.IsLoaded == false) return;

        Debug.Log($"<color=#66FF00>Video Ad Loading</color>");
        string appId = "ca-app-pub-3940256099942544/5224354917";
        m_RewardedAd = new RewardedAdWrapper(appId, showOnLoad);
    }
}

public class BannerAdWrapper
{
    #region Variables
    private bool m_IsValid;
    public bool IsValid => m_IsValid;

    private bool m_IsLoaded;
    public bool IsLoaded => m_IsLoaded;

    private readonly BannerView m_BannerView;
    public BannerView BannerView => m_BannerView;

    #endregion

    public BannerAdWrapper(string appId)
    {
        m_BannerView = new BannerView(appId, AdSize.Banner, AdPosition.Bottom);
        m_BannerView.OnAdLoaded += (sender, args) => m_IsLoaded = true;
        m_BannerView.OnAdFailedToLoad += (sender, e) => m_IsValid = false;
        m_BannerView.OnAdClosed += (sender, e) => m_IsValid = false;

        m_IsValid = true;
        LoadAndShow_F();
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

    private bool m_IsLoaded;
    public bool IsLoaded => m_IsLoaded;

    private readonly InterstitialAd m_InterstitialAd;
    public InterstitialAd InterstitialAd => m_InterstitialAd;
    private readonly bool m_ShowOnLoad;
    #endregion

    public InterstitialAdWrapper(string appId, bool m_ShowOnLoad)
    {
        m_InterstitialAd = new InterstitialAd(appId);
        m_InterstitialAd.OnAdLoaded += OnAdLoaded_EF;
        m_InterstitialAd.OnAdFailedToLoad += (sender, e) => m_IsValid = false;
        m_InterstitialAd.OnAdClosed += (sender, e) => m_IsValid = false;
        
        this.m_ShowOnLoad = m_ShowOnLoad;

        m_IsValid = true;
        Load_F();
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
        m_IsLoaded = true;
        if (m_IsValid && m_ShowOnLoad) m_InterstitialAd.Show();
    }
}

public class RewardedAdWrapper
{
    #region Variables
    private bool m_IsValid;
    public bool IsValid => m_IsValid;

    private bool m_IsLoaded;
    public bool IsLoaded => m_IsLoaded;

    public readonly RewardedAd m_RewardedAd;
    public RewardedAd RewardedAd => m_RewardedAd;

    private readonly bool m_ShowOnLoad;

    private bool m_RewardRecieved;
    public bool RewardRecieved => m_RewardRecieved;

    #endregion

    public RewardedAdWrapper(string appId, bool m_ShowOnLoad)
    {
        m_RewardedAd = new RewardedAd(appId);

        m_RewardedAd.OnAdLoaded += OnAdLoaded_EF;
        m_RewardedAd.OnAdFailedToLoad += (object sender, AdErrorEventArgs e) =>
        {
            Debug.Log("<color=#66FF00>Rewarded Ad failed to load</color>");
            m_IsValid = false;
        };
        m_RewardedAd.OnUserEarnedReward += (object sender, Reward reward) => m_RewardRecieved = true;
        m_RewardedAd.OnAdClosed += (object sender, EventArgs e) => m_IsValid = false;

        this.m_ShowOnLoad = m_ShowOnLoad;
        m_IsValid = true;

        Load_F();
    }

    public void Load_F()
    {
        if (!m_IsValid) return;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        m_RewardedAd.LoadAd(request);
    }

    public void Show_F()
    {
        if (!m_IsValid) return;

        m_RewardedAd.Show();
    }

    private void OnAdLoaded_EF(object sender, EventArgs e)
    {
        Debug.Log("<color=#66FF00>Rewarded Ad Loaded</color>");
        m_IsLoaded = true;
        
        if (m_IsValid == false || m_ShowOnLoad == false) return;
        m_RewardedAd.Show();
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

    public static bool IsValid_F(this RewardedAdWrapper ad)
    {
        return ad?.IsValid ?? false;
    }
}