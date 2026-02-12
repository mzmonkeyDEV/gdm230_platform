using UnityEngine;
using TMPro;
using System.Collections;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text msgText;

    [Header("Message Settings")]
    [SerializeField] private float autoHideDelay = 1.0f;

    Coroutine _msgRoutine;
    
    public void SetCoins(int current, int target)
    {
        if (coinText != null)
            coinText.text = $"{current}/{target}";
    }
    
    public void ShowWin()
    {
        ShowMessage("YOU WIN!", false);
    }
    
    public void ShowDead()
    {
        ShowMessage("RESPAWNING...", true);
    }
    
    public void ShowMessage(string message, bool autoHide)
    {
        if (msgText == null) return;

        if (_msgRoutine != null)
            StopCoroutine(_msgRoutine);

        msgText.gameObject.SetActive(true);
        msgText.text = message;

        if (autoHide)
            _msgRoutine = StartCoroutine(HideAfterDelay(autoHideDelay));
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearMessage();
    }

    public void ClearMessage()
    {
        if (msgText == null) return;

        msgText.text = "";
        msgText.gameObject.SetActive(false);
    }
}
