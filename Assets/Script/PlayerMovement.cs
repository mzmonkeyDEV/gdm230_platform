using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    //private Mobile m_Mobile;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float swipeDuration = 1f;
    private float swiping = 0f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ghost Trail Settings")]
    public GameObject ghostPrefab;
    public float ghostSpawnDelay = 0.05f;

    [Header("Visual Effects")]
    public ParticleSystem footSmoke;
    public float tiltAngle = 10f;
    public float tiltSpeed = 15f;
    public float spinDuration = 0.5f;
    [Range(1, 10)]
    public int numberOfSpins = 2;

    [Header("Audio")]
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip dashSound;

    [Header("Combat")]
    public Transform attackPoint;
    public GameObject attacksprite;// NEW: Drag your empty Attack Point object here

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    private AudioSource walkSource;
    private AudioSource sfxSource;

    private bool isGrounded;
    private bool canDoubleJump;
    private bool isSpinning;
    private bool isDashing;
    private bool canDash = true;

    private void Awake()
    {
        //m_Mobile = new Mobile();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        walkSource = gameObject.AddComponent<AudioSource>();
        walkSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    public void SwipeMoveLeft() { moveInput = Vector2.left; swiping = swipeDuration; }
    public void SwipeMoveRight() {  moveInput = Vector2.right; swiping = swipeDuration; }
    public void SwipeMoveStop() { moveInput = Vector2.zero; }
    public void SwipeMoveJump() 
    {
        if (isDashing) return;
        if (isGrounded)
        {
            PerformJump();
            canDoubleJump = true;
            CreateDust();
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

    //private void OnEnable()
    //{
    //    m_Mobile.Enable();
    //    m_Mobile.Player.Jump.performed += OnJumpPerformed;
    //    m_Mobile.Player.Dash.performed += OnDashPerformed;
    //}
    //private void OnDisable()
    //{
    //    m_Mobile.Disable();
    //    m_Mobile.Player.Jump.performed -= OnJumpPerformed;
    //    m_Mobile.Player.Dash.performed -= OnDashPerformed;
    //}

    //private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    //{
    //    if (isDashing) return;



    //        if (isGrounded)
    //        {
    //            PerformJump();
    //            canDoubleJump = true;
    //            CreateDust();
    //        }
    //        else if (canDoubleJump)
    //        {
    //            PerformJump();
    //            animator.SetTrigger("jump");

    //            StopCoroutine("DoSpin");
    //            StartCoroutine("DoSpin");

    //            canDoubleJump = false;
    //        }

    //}

    //private void OnMove(InputValue value)
    //{
    //    moveInput = value.Get<Vector2>();
    //}

    //private void OnJump(InputValue value)
    //{
    //    if (isDashing) return;

    //    if (value.isPressed)
    //    {
    //        if (isGrounded)
    //        {
    //            PerformJump();
    //            canDoubleJump = true;
    //            CreateDust();
    //        }
    //        else if (canDoubleJump)
    //        {
    //            PerformJump();
    //            animator.SetTrigger("jump");

    //            StopCoroutine("DoSpin");
    //            StartCoroutine("DoSpin");

    //            canDoubleJump = false;
    //        }
    //    }
    //}

    private void OnDashPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (canDash)
        {
            StartCoroutine(PerformDash());
        }
    }
    //private void OnDash(InputValue value)
    //{
    //    if (value.isPressed && canDash)
    //    {
    //        StartCoroutine(PerformDash());
    //    }
    //}

    private void PerformJump()
    {
        Vector2 v = rb.linearVelocity;
        v.y = jumpForce;
        rb.linearVelocity = v;
        animator.SetTrigger("jump");

        if (jumpSound != null) sfxSource.PlayOneShot(jumpSound);
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        if (dashSound != null) sfxSource.PlayOneShot(dashSound);

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

        CreateDust();

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
        //moveInput = m_Mobile.Player.Move.ReadValue<Vector2>();
        swiping -= Time.deltaTime;
        if (swiping <= 0)
        {
            SwipeMoveStop();
        }
        if (isDashing)
        {
            if (walkSource.isPlaying) walkSource.Stop();
            return;
        }

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        bool isMoving = Mathf.Abs(moveInput.x) > 0.01f;

        if (footSmoke != null)
        {
            if (isMoving && isGrounded)
            {
                if (!footSmoke.isPlaying) footSmoke.Play();
            }
            else
            {
                if (footSmoke.isPlaying) footSmoke.Stop();
            }
        }

        if (isMoving && isGrounded)
        {
            if (walkSound != null && !walkSource.isPlaying)
            {
                walkSource.clip = walkSound;
                walkSource.Play();
            }
        }
        else
        {
            if (walkSource.isPlaying) walkSource.Stop();
        }

        if (spriteRenderer != null && isMoving)
        {
            // --- NEW: Flip Logic ---
            bool isFacingLeft = moveInput.x < 0f;
            spriteRenderer.flipX = isFacingLeft;

            if (attackPoint != null)
            {
                // 1. Move the position horizontally
                Vector3 localPos = attackPoint.localPosition;
                localPos.x = Mathf.Abs(localPos.x) * (isFacingLeft ? -1f : 1f);
                attackPoint.localPosition = localPos;
                attacksprite.transform.localPosition = localPos;

                // 2. Flip the attack point's sprite (if it has one)
                SpriteRenderer attackSprite = attacksprite.GetComponent<SpriteRenderer>();
                if (attackSprite != null)
                {
                    attackSprite.flipX = isFacingLeft;
                }
            }
            // -----------------------

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

    private void CreateDust()
    {
        if (footSmoke != null)
        {
            footSmoke.Play();
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

        // Optional: Draw a little gizmo for the attack point so you can see it easily in the editor
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, 0.3f);
        }
    }
}