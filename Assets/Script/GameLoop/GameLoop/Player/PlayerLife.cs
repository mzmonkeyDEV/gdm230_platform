using UnityEngine;
using System.Collections;

public class PlayerLife : MonoBehaviour
{
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
    }

    public void Kill()
    {
        if (_invincible) return;
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
}
