using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
public static GameManager I { get; private set; }

    [Header("References")]
    [SerializeField] private HUDController hud;
    [SerializeField] private PlayerLife player;

    [Header("Rules")]
    [SerializeField] private int coinsToWin = 5;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 0.6f;
    [SerializeField] private float postRespawnInvincible = 1.0f;

    public GameState State { get; private set; } = GameState.Playing;
    public int Coins { get; private set; } = 0;

    // default spawn point
    public Vector3 RespawnPoint { get; private set; }

    void Awake()
    {
        //if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        State = GameState.Playing;

        if (player != null)
            RespawnPoint = player.transform.position; 

        hud?.SetCoins(Coins, coinsToWin);
        hud?.ClearMessage();
    }

    public bool IsPlaying => State == GameState.Playing;

    public void RegisterCheckpoint(Vector3 p)
    {
        RespawnPoint = p;
        Debug.Log($"Respawn point: {p}");
    }

    public void AddCoin(int amount)
    {
        if (!IsPlaying) return;

        Coins += amount;
        hud?.SetCoins(Coins, coinsToWin);

        if (Coins >= coinsToWin)
            Win();
    }

    public void Win()
    {
        if (State == GameState.Win) return;

        State = GameState.Win;
        hud?.ShowWin();
        
        if (player != null)
        {
            player.SetControlEnabled(false);
        }
    }
    
    public void PlayerDied()
    {
        if (!IsPlaying) return;

        State = GameState.Dead;
        hud?.ShowDead();

        if (player != null)
        {
            player.SetControlEnabled(false);
            player.StopMotion();
        }

        StartCoroutine(CoRespawn());
    }

    IEnumerator CoRespawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (player != null)
        {
            player.TeleportTo(RespawnPoint);
            player.StopMotion();
            player.SetControlEnabled(true);
            player.SetInvincible(postRespawnInvincible);
        }

        hud?.ShowDead();
        
        if (State != GameState.Win)
            State = GameState.Playing;
    }
}
