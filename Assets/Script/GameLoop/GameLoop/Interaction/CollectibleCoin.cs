using UnityEngine;

public class CollectibleCoin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private string playerTag = "Player";
    public AudioSource audioSource;

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        GameManager.I?.AddCoin(value);
        
        playSound();
        gameObject.SetActive(false);
        
    }

    void playSound()
    {
        AudioSource.PlayClipAtPoint(audioSource.clip,transform.position);
       
    }
}
