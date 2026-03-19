using UnityEngine;
using Unity.Services.LevelPlay;

public class InterstitialAdController : MonoBehaviour
{
    [SerializeField] private string interstitialAdUnitId = "YOUR_INTERSTITIAL_AD_UNIT_ID";

    private LevelPlayInterstitialAd interstitialAd;
    private bool isReady;

    public void InitializeInterstitial()
    {
        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);

        interstitialAd.OnAdLoaded += OnAdLoaded;
        interstitialAd.OnAdLoadFailed += OnAdLoadFailed;
        interstitialAd.OnAdDisplayed += OnAdDisplayed;
        interstitialAd.OnAdDisplayFailed += OnAdDisplayFailed;
        interstitialAd.OnAdClicked += OnAdClicked;
        interstitialAd.OnAdClosed += OnAdClosed;
    }

    public void LoadInterstitial()
    {
        isReady = false;
        interstitialAd.LoadAd();
    }

    public void ShowInterstitial()
    {
        if (!isReady)
        {
            Debug.LogWarning("[Interstitial] Ad not ready.");
            return;
        }

        interstitialAd.ShowAd();
    }

    private void OnAdLoaded(LevelPlayAdInfo adInfo)
    {
        isReady = true;
        Debug.Log("[Interstitial] Ad loaded: " + adInfo);
    }

    private void OnAdLoadFailed(LevelPlayAdError error)
    {
        isReady = false;
        Debug.LogError("[Interstitial] Load failed: " + error);
    }

    private void OnAdDisplayed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Interstitial] Ad displayed: " + adInfo);
    }

    private void OnAdDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.LogError("[Interstitial] Display failed: " + error);
    }

    private void OnAdClicked(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Interstitial] Ad clicked: " + adInfo);
    }

    private void OnAdClosed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Interstitial] Ad closed: " + adInfo);
        isReady = false;

        // Optional: prepare the next interstitial
        LoadInterstitial();
    }
}