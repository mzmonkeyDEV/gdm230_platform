using UnityEngine;
using Unity.Services.LevelPlay;

public class BannerAdController : MonoBehaviour
{
    [SerializeField] private string bannerAdUnitId = "t2psy8se78wz802e";
    private LevelPlayBannerAd bannerAd;
    
    public void InitializeBanner()
    {
        Debug.Log("[Banner] Create banner object: " + bannerAdUnitId);
        // create banner object here
        // register callbacks here
        var configBuilder = new LevelPlayBannerAd.Config.Builder();
        configBuilder.SetSize(LevelPlayAdSize.CreateAdaptiveAdSize());
        configBuilder.SetPosition(LevelPlayBannerPosition.BottomCenter);
        configBuilder.SetDisplayOnLoad(true);
        configBuilder.SetRespectSafeArea(true); // Android only

        var bannerConfig = configBuilder.Build();
        bannerAd = new LevelPlayBannerAd(bannerAdUnitId, bannerConfig);

        // Register events before LoadAd()
        bannerAd.OnAdLoaded += BannerOnAdLoaded;
        bannerAd.OnAdLoadFailed += BannerOnAdLoadFailed;
        bannerAd.OnAdDisplayed += BannerOnAdDisplayed;
        bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailed;
        bannerAd.OnAdClicked += BannerOnAdClicked;
    }

    public void LoadBanner()
    {
        Debug.Log("[Banner] Load banner");
        // load banner here
        bannerAd.LoadAd();
    }

    public void ShowBanner()
    {
        Debug.Log("[Banner] Show banner");
        // show banner here
        bannerAd.ShowAd();
    }

    public void HideBanner()
    {
        Debug.Log("[Banner] Hide banner");
        // hide banner here
        bannerAd.HideAd();
    }
    
    public void DestroyBanner()
    {
        bannerAd.DestroyAd();
    }

    private void BannerOnAdLoaded(LevelPlayAdInfo adInfo) { }
    private void BannerOnAdLoadFailed(LevelPlayAdError error) { }
    private void BannerOnAdDisplayed(LevelPlayAdInfo adInfo) { }
    private void BannerOnAdDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error) { }

    private void BannerOnAdClicked(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Banner] Ad clicked: " + adInfo);
        bannerAd.HideAd();
    }
}
