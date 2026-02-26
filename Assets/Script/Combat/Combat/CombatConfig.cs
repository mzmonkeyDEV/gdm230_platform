using UnityEngine;

[CreateAssetMenu(fileName = "CombatConfig", menuName = "Scriptable Objects/CombatConfig")]
public class CombatConfig : ScriptableObject
{
    [Header("Health")]
    public int playerMaxHP = 5;
    public int enemyMaxHP = 3;

    [Header("Player Attack")]
    public int playerDamage = 1;
    public float playerAttackCooldown = 0.5f;
    public float attackRadius = 0.5f;

    [Header("Enemy AI")]
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public int enemyDamage = 1;
    public float enemyAttackCooldown = 1f;
}
