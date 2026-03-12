using UnityEngine;
using static UnityEngine.SpriteMask;

public class PlayerAttack : MonoBehaviour
{
    [Header("Input")]
    private Mobile m_Mobile;
    [SerializeField] private CombatConfig config;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject hitAnim;

    float lastAttackTime;
    private void OnEnable()
    {
        m_Mobile.Enable();
        m_Mobile.Player.Attack.performed += OnAttackPerformed;
        
    }
    private void OnDisable()
    {
        m_Mobile.Disable();
        m_Mobile.Player.Attack.performed -= OnAttackPerformed;
    }
    private void OnAttackPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        TryAttack();
    }
    private void Awake()
    {
        m_Mobile = new Mobile();
    }
    public void TryAttack()
    {
        float cd = (config != null) ? config.playerAttackCooldown : 0.5f;
        if (Time.time < lastAttackTime + cd) return;
        lastAttackTime = Time.time;

        float radius = (config != null) ? config.attackRadius : 0.5f;
        int damage = (config != null) ? config.playerDamage : 1;
        Animator hit = hitAnim.GetComponent<Animator>();
        hit.SetTrigger("Slash");

        var hits = Physics2D.OverlapCircleAll(attackPoint.position, radius, enemyLayer);
        foreach (var h in hits)
        {
            var dmg = h.GetComponent<IDamageable>();
            if (dmg != null) dmg.TakeDamage(damage, transform.position);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        float radius = (config != null) ? config.attackRadius : 0.5f;
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
#endif
}
