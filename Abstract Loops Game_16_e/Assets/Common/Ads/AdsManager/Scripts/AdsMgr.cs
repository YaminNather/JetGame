using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsMgr : MonoBehaviour
{
    #region Variables
    private BannerView m_BannerView;
    #endregion

    private void Start()
    {
        MobileAds.Initialize(status => { });

        BannerRequest_F();
    }

    private void BannerRequest_F()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        m_BannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        //Adding callbacks to Banner events.
        m_BannerView.OnAdLoaded += BannerOnAdLoaded_E;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        m_BannerView.LoadAd(request);
    }

    private void BannerOnAdLoaded_E(object sender, System.EventArgs e)
    {
        Debug.Log("<color=yellow>Banner Ad Loaded</color>");
    }
}
