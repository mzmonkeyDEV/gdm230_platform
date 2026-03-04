using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private CombatConfig config;
    [SerializeField] private int currentHP;

    // NEW: Expose HP so the AI script can read it safely
    public int CurrentHP => currentHP;

    [Header("Combat Feedback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackStunDuration = 0.3f; // NEW: How long the AI pauses
    [SerializeField] private AudioClip hitSound;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        ResetHP();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void ResetHP()
    {
        int max = (config != null) ? config.enemyMaxHP : 3;
        currentHP = max;
    }

    public void TakeDamage(int amount, Vector3 attackerPos)
    {
        currentHP -= Mathf.Max(0, amount);

        if (hitSound != null)
        {
            Vector3 soundPos = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(hitSound, soundPos);
        }

        Vector2 knockbackDir = (transform.position - attackerPos).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // --- NEW: Stun the AI so the knockback actually works ---
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.ApplyStun(knockbackStunDuration);
        }

        StartCoroutine(Flash());
        Debug.Log($"Enemy HP: {currentHP}");

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator Flash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }
}