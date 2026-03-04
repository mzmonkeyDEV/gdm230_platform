using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class PlayerLife : MonoBehaviour, IDamageable
{
    [Header("Combat Config")]
    [SerializeField] private CombatConfig config;

    [Header("HP")]
    [SerializeField] private int currentHP;
    private int maxHP;

    [Header("UI & Feedback")]
    [SerializeField] private GameObject[] heartIcons;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound; // damage sound here
    private AudioSource sfxSource;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private Rigidbody2D rb;

    [Header("Invincibility")]
    [SerializeField] private float invincibleTime = 0f;
    public bool IsInvincible => _invincible;

    bool _invincible;
    Coroutine _coInv;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Awake()
    {
        // Automatically create the AudioSource and make it 2D so it's loud and clear
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.spatialBlend = 0f;
    }

    public void Start()
    {
        ResetHP();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Kill()
    {
        if (_invincible) return;
        currentHP = 0;
        UpdateHealthUI();
        GameManager.I?.PlayerDied();
    }

    public void SetControlEnabled(bool enabled)
    {
        if (playerController != null)
            playerController.enabled = enabled;
    }

    public void StopMotion()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    public void TeleportTo(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetInvincible(float seconds)
    {
        if (_coInv != null) StopCoroutine(_coInv);
        if (seconds <= 0f) { _invincible = false; return; }
        _coInv = StartCoroutine(CoInvincible(seconds));
    }

    IEnumerator CoInvincible(float seconds)
    {
        _invincible = true;
        yield return new WaitForSeconds(seconds);
        _invincible = false;
        _coInv = null;
    }

    public void TakeDamage(int amount, Vector3 p)
    {
        if (_invincible) return;

        currentHP -= Mathf.Max(0, amount);

        // --- NEW: Play Damage Sound ---
        if (damageSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(damageSound);
        }

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        UpdateHealthUI();

        StartCoroutine(Flash());
        Debug.Log($"Player HP: {currentHP}");

        if (currentHP <= 0) { Kill(); return; }
        if (invincibleTime > 0f) SetInvincible(invincibleTime);
    }

    public void ResetHP()
    {
        maxHP = (config != null) ? config.playerMaxHP : 5;
        currentHP = maxHP;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (heartIcons == null || heartIcons.Length == 0) return;

        float healthPercent = (float)currentHP / maxHP;
        int activeHearts = Mathf.CeilToInt(healthPercent * heartIcons.Length);
        activeHearts = Mathf.Clamp(activeHearts, 0, heartIcons.Length);

        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < activeHearts);
        }
    }

    IEnumerator Flash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}