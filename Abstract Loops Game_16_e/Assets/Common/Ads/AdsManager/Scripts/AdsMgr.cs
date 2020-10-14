using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsMgr : MonoBehaviour
{
    #region Variables
    private BannerView m_BannerView;
    private InterstitialAd m_InterstitialAd;
    #endregion

    private void Start()
    {
        MobileAds.Initialize(status => { });

        //BannerViewRequest_F();
        //InterstitialRequest_F();
    }

    private void BannerViewRequest_F()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        m_BannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        //Adding callbacks to Banner events.
        m_BannerView.OnAdLoaded += BannerViewOnAdLoaded_E;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        m_BannerView.LoadAd(request);
    }

    private void BannerViewOnAdLoaded_E(object sender, System.EventArgs e)
    {
        Debug.Log("<color=yellow>Banner Ad Loaded</color>");
    }

    private void InterstitialRequest_F()
    {
        string adUnitId;

#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        adUnitId = "unexpected_platform";
#endif

        m_InterstitialAd = new InterstitialAd(adUnitId);

        m_InterstitialAd.OnAdLoaded += InterstitialAdOnAdLoaded_E;

        AdRequest adRequest = new AdRequest.Builder().Build();
        m_InterstitialAd.LoadAd(adRequest);
    }

    private void InterstitialAdOnAdLoaded_E(object sender, System.EventArgs e)
    {
        Debug.Log("<color=yellow>Interstitial Ad loaded, gonna show it now.</color>");
        Debug.Log($"<color=yellow>InterstitialAdOnAdLoaded function parameter sender = {sender}</color>");
        m_InterstitialAd.Show();
    }
}
