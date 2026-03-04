using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private CombatConfig config;
    [SerializeField] private Transform player;

    [Header("Flee Settings")]
    [SerializeField] private int fleeHealthThreshold = 1;
    [SerializeField] private float fleeSpeed = 4f;

    private EnemyLife enemyLife;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // For flipping the sprite
    private float lastAttackTime;
    private Vector2 patrolDir = Vector2.right;
    private float stunTimer = 0f;

    enum State { Patrol, Chase, Attack, Flee }
    State state;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyLife = GetComponent<EnemyLife>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Auto-grab the sprite renderer
        state = State.Patrol;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    public void ApplyStun(float duration)
    {
        stunTimer = duration;
    }

    void Update()
    {
        // 1. KNOCKBACK CHECK
        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            return;
        }

        if (player == null) return;

        // 2. LOW HEALTH CHECK
        bool isLowHealth = (enemyLife != null && enemyLife.CurrentHP <= fleeHealthThreshold);

        if (isLowHealth)
        {
            state = State.Flee;
        }
        else if (state == State.Flee && !isLowHealth)
        {
            state = State.Patrol;
        }

        // 3. NORMAL AI LOGIC
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

            case State.Flee:
                Flee();
                break;
        }

        // 4. VISUAL UPDATE
        UpdateFacingDirection();
    }

    //  Handles left/right flipping
    void UpdateFacingDirection()
    {
        if (spriteRenderer == null) return;

        
        if (rb.linearVelocity.x > 0.05f)
        {
            spriteRenderer.flipX = false; // Moving right
        }
        else if (rb.linearVelocity.x < -0.05f)
        {
            spriteRenderer.flipX = true;  // Moving left
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

        
        rb.linearVelocity = new Vector2(dir.x * s, rb.linearVelocity.y);
    }

    void Flee()
    {
        float s = fleeSpeed;
        Vector2 dir = (transform.position - player.position).normalized;

        dir.y = 0;
        dir = dir.normalized;

        rb.linearVelocity = new Vector2(dir.x * s, rb.linearVelocity.y);
    }

    void Attack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving horizontally to attack

        float cd = (config != null) ? config.enemyAttackCooldown : 1f;
        if (Time.time < lastAttackTime + cd) return;
        lastAttackTime = Time.time;

        int dmg = (config != null) ? config.enemyDamage : 1;
        var damageable = player.GetComponent<IDamageable>();
        if (damageable != null) damageable.TakeDamage(dmg, transform.position);
    }
}