using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        var life = other.GetComponent<PlayerLife>();
        if (life != null) life.Kill();
        else GameManager.I?.PlayerDied();
    }
}
