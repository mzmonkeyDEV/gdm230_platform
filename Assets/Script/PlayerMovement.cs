using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ghost Trail Settings")]
    public GameObject ghostPrefab;
    public float ghostSpawnDelay = 0.05f;

    [Header("Visual Effects")]
    public float tiltAngle = 10f;
    public float tiltSpeed = 15f;
    public float spinDuration = 0.5f;
    [Range(1, 10)]
    public int numberOfSpins = 2;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    private bool isGrounded;
    private bool canDoubleJump;
    private bool isSpinning;
    private bool isDashing;
    private bool canDash = true;

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

    private void OnJump(InputValue value)
    {
        if (isDashing) return;

        if (value.isPressed)
        {
            if (isGrounded)
            {
                PerformJump();
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                PerformJump();
                animator.SetTrigger("jump");

                StopCoroutine("DoSpin");
                StartCoroutine("DoSpin");

                canDoubleJump = false;
            }
        }
    }

    private void OnDash(InputValue value)
    {
        if (value.isPressed && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    private void PerformJump()
    {
        Vector2 v = rb.linearVelocity;
        v.y = jumpForce;
        rb.linearVelocity = v;
        animator.SetTrigger("jump");
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = spriteRenderer.flipX ? -1f : 1f;
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            dashDirection = Mathf.Sign(moveInput.x);
        }

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        StopCoroutine("DoSpin");
        isSpinning = false;
        transform.rotation = Quaternion.identity;
        animator.SetTrigger("dash");

        StartCoroutine(SpawnGhosts());

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator SpawnGhosts()
    {
        while (isDashing)
        {
            if (ghostPrefab != null)
            {
                GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);

                GhostTrail ghostScript = ghost.GetComponent<GhostTrail>();
                if (ghostScript != null)
                {
                    ghostScript.Setup(spriteRenderer.sprite, spriteRenderer.flipX, Color.white);
                }
            }

            yield return new WaitForSeconds(ghostSpawnDelay);
        }
    }

    private void Update()
    {
        if (isDashing) return;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        if (spriteRenderer != null && Mathf.Abs(moveInput.x) > 0.01f)
        {
            spriteRenderer.flipX = moveInput.x < 0f;
            animator.SetBool("running", true);
        }
        else
        {
            animator.SetBool("running", false);
        }

        animator.SetBool("isGrounded", isGrounded);

        if (!isSpinning)
        {
            HandleRunningTilt();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        Vector2 v = rb.linearVelocity;
        v.x = moveInput.x * moveSpeed;
        rb.linearVelocity = v;
    }

    private void HandleRunningTilt()
    {
        float targetZ = 0f;
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            targetZ = -moveInput.x * tiltAngle;
        }
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
    }

    private IEnumerator DoSpin()
    {
        isSpinning = true;
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;
        float direction = spriteRenderer.flipX ? 1f : -1f;
        float totalAngle = 360f * numberOfSpins * direction;

        while (elapsed < spinDuration)
        {
            if (isDashing) yield break;

            elapsed += Time.deltaTime;
            float percent = elapsed / spinDuration;
            float currentZ = Mathf.Lerp(0, totalAngle, percent);
            transform.rotation = startRotation * Quaternion.Euler(0, 0, currentZ);
            yield return null;
        }

        transform.rotation = Quaternion.identity;
        isSpinning = false;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}