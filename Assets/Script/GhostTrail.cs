using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostTrail : MonoBehaviour
{
    public float fadeSpeed = 4f;      
    public float startAlpha = 0.5f;   

    private SpriteRenderer sr;
    private Color color;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
        color.a = startAlpha;
        sr.color = color;
    }

    public void Setup(Sprite sprite, bool flipX, Color tint)
    {
        sr.sprite = sprite;
        sr.flipX = flipX;
        
        sr.color = tint;
        
    }

    private void Update()
    {
       
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color = color;

      
        if (color.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}