using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class TimeMachine : MonoBehaviour
{
    [Header("UI & Dialogue")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public string line1 = "I need to fix my time machine to go back home.";
    public string line2 = "I have to find these parts. They're scattered around here somewhere.";
    public string winLine = "All systems go! Initiating time jump..."; // NEW: Victory text
    public float timePerLine = 3.0f;

    [Header("Cameras")]
    public GameObject mainCamera;
    public GameObject[] partCameras = new GameObject[3];
    public float cameraViewTime = 2.0f;

    [Header("Visuals")]
    public GameObject[] floatingIcons = new GameObject[3];

    private bool hasTriggered = false;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (dialogueText != null) dialogueText.gameObject.SetActive(false);

        foreach (var icon in floatingIcons)
        {
            if (icon != null) icon.SetActive(false);
        }

        foreach (var cam in partCameras)
        {
            if (cam != null) cam.SetActive(false);
        }
    }

    void Start()
    {
        if (GameManager.I != null)
        {
            GameManager.I.OnPartCollected += HandlePartCollected;
        }
    }

    void OnDestroy()
    {
        if (GameManager.I != null)
        {
            GameManager.I.OnPartCollected -= HandlePartCollected;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // SCENARIO 1: First time meeting (Intro Cutscene)
        if (!hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(CutsceneRoutine());
        }
        // SCENARIO 2: Player returned (Check for Win)
        else
        {
            if (GameManager.I.CanWin)
            {
                // Trigger the win sequence
                StartCoroutine(WinRoutine());
            }
            else
            {
                // Optional: Feedback if they came back too early
                // Debug.Log("You still need more parts!");
            }
        }
    }

    private IEnumerator WinRoutine()
    {
        // 1. Freeze Player
        GameManager.I?.SetPlayerControl(false);

        // 2. Final Dialogue
        if (dialogueBox != null) dialogueBox.SetActive(true);
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(true);
            dialogueText.text = winLine;
        }

        yield return new WaitForSeconds(3f);

        // 3. Trigger Game Manager Win (Show UI)
        GameManager.I?.Win();
    }

    private IEnumerator CutsceneRoutine()
    {
        GameManager.I?.SetPlayerControl(false);

        if (dialogueBox != null) dialogueBox.SetActive(true);
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(true);
            dialogueText.text = line1;
        }
        yield return new WaitForSeconds(timePerLine);

        if (dialogueText != null) dialogueText.text = line2;
        yield return new WaitForSeconds(timePerLine);

        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (dialogueText != null) dialogueText.gameObject.SetActive(false);

        if (mainCamera != null) mainCamera.SetActive(false);

        for (int i = 0; i < partCameras.Length; i++)
        {
            if (partCameras[i] != null)
            {
                partCameras[i].SetActive(true);
                yield return new WaitForSeconds(cameraViewTime);
                partCameras[i].SetActive(false);
            }
        }

        if (mainCamera != null) mainCamera.SetActive(true);

        for (int i = 0; i < floatingIcons.Length; i++)
        {
            if (floatingIcons[i] != null && !GameManager.I.HasParts[i])
            {
                floatingIcons[i].SetActive(true);
            }
        }

        GameManager.I?.SetPlayerControl(true);
    }

    private void HandlePartCollected(int partIndex)
    {
        if (hasTriggered && partIndex >= 0 && partIndex < floatingIcons.Length)
        {
            if (floatingIcons[partIndex] != null)
            {
                floatingIcons[partIndex].SetActive(false);
            }
        }
    }
}