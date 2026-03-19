using UnityEngine;
using Unity.Services.LevelPlay;
using System;

public class RewardedAdController : MonoBehaviour
{
[SerializeField] private string rewardedAdUnitId = "YOUR_REWARDED_AD_UNIT_ID";

    private LevelPlayRewardedAd rewardedAd;
    private bool isReady;
    private Action pendingRewardAction;

    public void InitializeRewarded()
    {
        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);

        rewardedAd.OnAdLoaded += OnAdLoaded;
        rewardedAd.OnAdLoadFailed += OnAdLoadFailed;
        rewardedAd.OnAdDisplayed += OnAdDisplayed;
        rewardedAd.OnAdDisplayFailed += OnAdDisplayFailed;
        rewardedAd.OnAdRewarded += OnAdRewarded;
        rewardedAd.OnAdClicked += OnAdClicked;
        rewardedAd.OnAdClosed += OnAdClosed;
    }

    public void LoadRewarded()
    {
        isReady = false;
        rewardedAd.LoadAd();
    }

    public bool TryShowRewarded(Action onRewardGranted)
    {
        if (!isReady)
        {
            Debug.LogWarning("[Rewarded] Ad not ready.");
            return false;
        }

        pendingRewardAction = onRewardGranted;
        rewardedAd.ShowAd();
        return true;
    }

    private void OnAdLoaded(LevelPlayAdInfo adInfo)
    {
        isReady = true;
        Debug.Log("[Rewarded] Ad loaded: " + adInfo);
    }

    private void OnAdLoadFailed(LevelPlayAdError error)
    {
        isReady = false;
        Debug.LogError("[Rewarded] Load failed: " + error);
    }

    private void OnAdDisplayed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Rewarded] Ad displayed: " + adInfo);
    }

    private void OnAdDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.LogError("[Rewarded] Display failed: " + error);
        pendingRewardAction = null;
    }

    private void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log("[Rewarded] Reward callback received: " + reward);
        pendingRewardAction?.Invoke();
        pendingRewardAction = null;
    }

    private void OnAdClicked(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Rewarded] Ad clicked: " + adInfo);
    }

    private void OnAdClosed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[Rewarded] Ad closed: " + adInfo);
        isReady = false;
        LoadRewarded();
    }
}
