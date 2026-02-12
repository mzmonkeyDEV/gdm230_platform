using UnityEngine;

public class Checkpoint : MonoBehaviour
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
        GameManager.I?.RegisterCheckpoint(transform.position);
    }
}
