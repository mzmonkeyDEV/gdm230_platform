using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class CollectiblePart : MonoBehaviour
{
    [Tooltip("Set to 0, 1, or 2 for the three distinct parts.")]
    [SerializeField] private int partIndex = 0;
    [SerializeField] private string playerTag = "Player";

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        GameManager.I?.CollectPart(partIndex);

        if (audioSource.clip != null)
        {
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
        }

        gameObject.SetActive(false);
    }
}