using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    private void Update()
    {
        if (spriteRenderer != null && Mathf.Abs(moveInput.x) > 0.01f)
        {
            spriteRenderer.flipX = moveInput.x < 0f;
            animator.SetBool("running", true);
        }
        else
        {
            animator.SetBool("running", false);
        }
           
        animator.SetFloat(SpeedHash, Mathf.Abs(moveInput.x));
        
    }
    private void FixedUpdate()
    {
        // Physics move
        Vector2 v = rb.linearVelocity;
        v.x = moveInput.x * moveSpeed;
        rb.linearVelocity = v;
    }
}
