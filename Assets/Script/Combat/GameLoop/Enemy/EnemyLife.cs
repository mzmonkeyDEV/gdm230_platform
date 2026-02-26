using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour, IDamageable
{
    [SerializeField] private CombatConfig config;
    [SerializeField] private int currentHP;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        ResetHP();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void ResetHP()
    {
        int max = (config != null) ? config.enemyMaxHP : 3;
        currentHP = max;
    }

    public void TakeDamage(int amount, Vector3 Epos)
    {
        currentHP -= Mathf.Max(0, amount);
        StartCoroutine(Flash());
        Debug.Log($"Enemy HP: {currentHP}");
        rb.AddForce((transform.position-Epos)*2,ForceMode2D.Impulse);
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
        spriteRenderer.color = Color.white;
    }
}
