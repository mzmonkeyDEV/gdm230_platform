using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    private Mobile m_Mobile;

    [Header("References")]
    [SerializeField] private HUDController hud;
    [SerializeField] private PlayerLife player;
    [SerializeField] private SpriteRenderer playerSprite;
    public GameObject spawnCam;
    [SerializeField] private InterstitialAdController interstitialAdController;

    [Header("Rules")]
    [SerializeField] private int coinsToWin = 5;
    public int totalPartsToFind = 3;

    [Header("Respawn & Feedback")]
    [SerializeField] private float respawnDelay = 0.6f;
    [SerializeField] private float postRespawnInvincible = 1.0f;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float flashSpeed = 0.05f;

    public GameState State { get; private set; } = GameState.Playing;
    public int Coins { get; private set; } = 0;

    public bool[] HasParts { get; private set; } = new bool[3];
    public int PartsCollected { get; private set; } = 0;

    public event Action<int> OnPartCollected;

    public Vector3 RespawnPoint { get; private set; }

    private AudioSource sfxSource;

    // NEW: Property to check if conditions are met (but doesn't trigger win yet)
    public bool CanWin => Coins >= coinsToWin && PartsCollected >= totalPartsToFind;

    void Awake()
    {
        m_Mobile = new Mobile();
        I = this;
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.spatialBlend = 0f;
    }

    void Start()
    {
        State = GameState.Playing;

        if (player != null)
            RespawnPoint = player.transform.position;

        if (playerSprite == null && player != null)
        {
            playerSprite = player.GetComponent<SpriteRenderer>();
        }

        hud?.SetCoins(Coins, coinsToWin);
        hud?.ClearMessage();
    }

    public bool IsPlaying => State == GameState.Playing;

    public void RegisterCheckpoint(Vector3 p)
    {
        RespawnPoint = p;
    }

    public void AddCoin(int amount)
    {
        if (!IsPlaying) return;

        Coins += amount;
        hud?.SetCoins(Coins, coinsToWin);

        // Removed automatic CheckWinCondition()
    }

    public void CollectPart(int partIndex)
    {
        if (!IsPlaying || partIndex < 0 || partIndex >= HasParts.Length || HasParts[partIndex]) return;

        HasParts[partIndex] = true;
        PartsCollected++;

        OnPartCollected?.Invoke(partIndex);

        // Removed automatic CheckWinCondition()
    }

    public void SetPlayerControl(bool enabled)
    {
        if (player != null)
        {
            player.SetControlEnabled(enabled);
            if (!enabled) player.StopMotion();
        }
    }

    public void Win()
    {
        if (State == GameState.Win) return;

        State = GameState.Win;
        hud?.ShowWin();

        SetPlayerControl(false);
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

        if (deathSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(deathSound);
        }

        if (interstitialAdController != null)
        {
            interstitialAdController.ShowInterstitial();
        }
        StartCoroutine(CoRespawn());
    }

    IEnumerator CoRespawn()
    {
        float elapsed = 0f;
        bool isSpriteVisible = true;

        while (elapsed < respawnDelay)
        {
            if (playerSprite != null)
            {
                isSpriteVisible = !isSpriteVisible;
                playerSprite.enabled = isSpriteVisible;
                playerSprite.color = isSpriteVisible ? Color.red : Color.white;
            }

            yield return new WaitForSeconds(flashSpeed);
            elapsed += flashSpeed;
        }

        if (playerSprite != null)
        {
            playerSprite.enabled = true;
            playerSprite.color = Color.white;
        }

        if (player != null)
        {
            player.TeleportTo(RespawnPoint);
            player.StopMotion();
            player.SetControlEnabled(true);
            player.ResetHP();
            player.SetInvincible(postRespawnInvincible);
            spawnCam.SetActive(true);
        }

        hud?.ShowDead();

        if (State != GameState.Win)
            State = GameState.Playing;
    }
}