using UnityEngine;
using System.Collections;
using TMPro; // REQUIRED for Text Mesh Pro

[RequireComponent(typeof(Collider2D))]
public class TimeMachine : MonoBehaviour
{
    [Header("UI & Dialogue")]
    public GameObject dialogueBox; // The background bubble (optional)
    public TMP_Text dialogueText;  // Changed to TMP_Text
    public string line1 = "I need to fix my time machine to go back home.";
    public string line2 = "I have to find these parts. They're scattered around here somewhere.";
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
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(CutsceneRoutine());
        }
    }

    private IEnumerator CutsceneRoutine()
    {
        // 1. Freeze Player
        GameManager.I?.SetPlayerControl(false);

        // 2. Show Dialogue
        if (dialogueBox != null) dialogueBox.SetActive(true);
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(true);
            dialogueText.text = line1;
        }
        yield return new WaitForSeconds(timePerLine);

        if (dialogueText != null) dialogueText.text = line2;
        yield return new WaitForSeconds(timePerLine);

        // Hide Dialogue
        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (dialogueText != null) dialogueText.gameObject.SetActive(false);

        // 3. Camera Sequence
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

        // 4. Show Missing Parts Icons
        for (int i = 0; i < floatingIcons.Length; i++)
        {
            // Only show the icon if the player HAS NOT collected that part yet
            if (floatingIcons[i] != null && !GameManager.I.HasParts[i])
            {
                floatingIcons[i].SetActive(true);
            }
        }

        // 5. Unfreeze Player
        GameManager.I?.SetPlayerControl(true);
    }

    private void HandlePartCollected(int partIndex)
    {
        // When the GameManager announces a part was collected, turn off its floating icon
        if (hasTriggered && partIndex >= 0 && partIndex < floatingIcons.Length)
        {
            if (floatingIcons[partIndex] != null)
            {
                floatingIcons[partIndex].SetActive(false);
            }
        }
    }
}