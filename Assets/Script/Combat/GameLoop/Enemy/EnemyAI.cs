using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private CombatConfig config;
    [SerializeField] private Transform player;

    Rigidbody2D rb;
    float lastAttackTime;
    Vector2 patrolDir = Vector2.right;

    enum State { Patrol, Chase, Attack }
    State state;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state = State.Patrol;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float detect = (config != null) ? config.detectionRange : 5f;
        float atkRange = (config != null) ? config.attackRange : 1f;
        float dist = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (dist < detect) state = State.Chase;
                break;

            case State.Chase:
                Chase();
                if (dist < atkRange) state = State.Attack;
                else if (dist > detect) state = State.Patrol;
                break;

            case State.Attack:
                Attack();
                if (dist > atkRange) state = State.Chase;
                break;
        }
    }

    void Patrol()
    {
        float s = (config != null) ? config.patrolSpeed : 2f;
        rb.linearVelocity = patrolDir * s;
    }

    void Chase()
    {
        float s = (config != null) ? config.chaseSpeed : 3f;
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * s;
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;

        float cd = (config != null) ? config.enemyAttackCooldown : 1f;
        if (Time.time < lastAttackTime + cd) return;
        lastAttackTime = Time.time;

        int dmg = (config != null) ? config.enemyDamage : 1;
        var damageable = player.GetComponent<IDamageable>();
        if (damageable != null) damageable.TakeDamage(dmg,transform.position);
    }
}
