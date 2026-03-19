using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreeCoinButton : MonoBehaviour
{
    [SerializeField] private RewardedAdController rewardedAdController;
    [SerializeField] private Button freeCoinButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private int rewardAmount = 20;

    public void OnClickFreeCoin()
    {
        Debug.Log("[FreeCoin] RewardedAd Button clicked");
        if (GameManager.I == null)
        {
            Debug.LogError("[FreeCoin] GameManager.I is null");
            SetStatus("GameManager missing");
            return;
        }

        if (rewardedAdController == null)
        {
            Debug.LogError("[FreeCoin] RewardedAdController missing");
            SetStatus("Ad system missing");
            return;
        }
        
        SetButtonInteractable(false);
        SetStatus("Checking ad...");

        bool didShow = rewardedAdController.TryShowRewarded(OnRewardGranted);
        
        if (!didShow)
        {
            SetStatus("Ad not ready");
            SetButtonInteractable(true);
        }
        else
        {
            SetStatus("Showing ad...");
        }
    }

    private void OnRewardGranted()
    {
        GameManager.I.AddCoin(rewardAmount);
        SetStatus($"+{rewardAmount} Coins!");
        SetButtonInteractable(false);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    private void SetButtonInteractable(bool value)
    {
        if (freeCoinButton != null)
            freeCoinButton.interactable = value;
    }
}
