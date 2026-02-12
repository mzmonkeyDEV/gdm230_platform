using UnityEngine;

public class CollectibleCoin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private string playerTag = "Player";

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        GameManager.I?.AddCoin(value);
        gameObject.SetActive(false);
    }
}
