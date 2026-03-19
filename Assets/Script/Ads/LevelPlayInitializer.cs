using UnityEngine;
using Unity.Services.LevelPlay;

public class LevelPlayInitializer : MonoBehaviour
{
    [SerializeField] private string appKey = "25a3146e5";

    public static bool IsInitialized { get; private set; }

    [SerializeField] private BannerAdController bannerController;
    [SerializeField] private InterstitialAdController interstitialAdController;
    [SerializeField] private RewardedAdController rewardedAdController;
    private void Start()
    {
        Debug.Log("[LevelPlay] Initializing SDK...");

        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;

        LevelPlay.Init(appKey);
    }

    private void OnInitSuccess(LevelPlayConfiguration configuration)
    {
        IsInitialized = true;
        Debug.Log("[LevelPlay] SDK initialized successfully.");

        // Create Banner / Interstitial / Rewarded objects after this point
        if (bannerController != null)
        {
            bannerController.InitializeBanner();
            bannerController.LoadBanner();
            bannerController.ShowBanner();
        }
        if (interstitialAdController != null) 
        {
            interstitialAdController.InitializeInterstitial();
            interstitialAdController.LoadInterstitial();
        }
        if (rewardedAdController != null)
        {
            rewardedAdController.InitializeRewarded();
            rewardedAdController.LoadRewarded();
        }
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        IsInitialized = false;
        Debug.LogError("[LevelPlay] SDK initialization failed: " + error);
    }

    private void OnDestroy()
    {
        LevelPlay.OnInitSuccess -= OnInitSuccess;
        LevelPlay.OnInitFailed -= OnInitFailed;
    }
}
