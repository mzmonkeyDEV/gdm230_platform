using UnityEngine;

public class TutorialPromtUp : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform player;
    public float revealDistance = 5f;
    public float minDistance = 2f;

    [Header("Floating Settings")]
    public float floatAmplitude = 0.1f;
    public float floatFrequency = 2f;

    [Header("Pulse Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.2f;

    private SpriteRenderer spriteRenderer;
    private Vector3 startPos;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;

        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        
        float alpha = Mathf.InverseLerp(revealDistance, minDistance, distance);
        Color newColor = spriteRenderer.color;
        newColor.a = alpha;
        spriteRenderer.color = newColor;


        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);


        //float pulse = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * floatFrequency * 2f) + 1f) / 2f);
        //spriteRenderer.color = new Color(newColor.r * pulse, newColor.g * pulse, newColor.b * pulse, alpha);
    }
}
