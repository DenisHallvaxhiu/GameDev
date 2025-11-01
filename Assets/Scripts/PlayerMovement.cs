using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    const string ANIMATOR_IS_RUNNING = "isRunning";

    [Header("Movement")]
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float accel = 50f;
    [SerializeField] float decel = 60f;

    [Header("Jump")]
    [SerializeField] float jumpVelocity = 12f;
    [SerializeField] float maxJumpHoldTime = 0.25f;
    [SerializeField] float jumpCutMultiplier = 0.4f;
    [SerializeField] float coyoteTime = 0.12f;
    [SerializeField] float jumpBuffer = 0.12f;

    [Header("Gravity")]
    [SerializeField] float fallMultiplier = 2.0f;
    [SerializeField] float apexHangMultiplier = 0.75f;
    [SerializeField] float apexHangVelThreshold = 1.5f;
    [SerializeField] float maxFallSpeed = -18f;

    [Header("Ground Check")]
    [SerializeField] LayerMask solidsMask;
    [SerializeField] Vector2 groundBoxSize = new Vector2(0.6f,0.1f);
    [SerializeField] Vector2 groundBoxOffset = new Vector2(0f,-0.55f);
    [SerializeField] bool autoSizeGroundFromCollider = true;

    [Header("Visuals")]
    [SerializeField] Animator animator;
    [SerializeField] Transform playerVisual;
    [SerializeField] float visualScaleX = 9f;

    Rigidbody2D rb;
    Collider2D col;
    float facing = 1f;

    float moveInput;
    bool jumpPressedThisFrame;
    bool jumpReleasedThisFrame;

    float lastGroundedTime;
    float lastJumpPressedTime;
    float jumpHoldCounter;
    bool isJumping;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if(autoSizeGroundFromCollider && col != null) {
            var b = col.bounds;
            // Make a thin box slightly wider than the collider, sitting just under the feet
            groundBoxSize = new Vector2(b.size.x * 0.9f,0.08f);
            // Offset to the bottom of the collider
            groundBoxOffset = new Vector2(0f,(b.min.y - transform.position.y) - 0.02f);
        }
    }

    void OnEnable() {
        if(GameInput.Instance != null) {
            GameInput.Instance.OnJumpStarted += OnJumpStarted;
            GameInput.Instance.OnJumpCanceled += OnJumpCanceled;
        }
    }

    void OnDisable() {
        if(GameInput.Instance != null) {
            GameInput.Instance.OnJumpStarted -= OnJumpStarted;
            GameInput.Instance.OnJumpCanceled -= OnJumpCanceled;
        }
    }

    void Update() {
        // --- INPUT ---
        moveInput = GameInput.Instance != null ? GameInput.Instance.GetMovementInput() : 0f;

        // Fallback input so you can test immediately if GameInput events aren’t wired
        if(Input.GetKeyDown(KeyCode.Space)) OnJumpStarted(null,EventArgs.Empty);
        if(Input.GetKeyUp(KeyCode.Space)) OnJumpCanceled(null,EventArgs.Empty);

        // Animator + facing
        animator?.SetBool(ANIMATOR_IS_RUNNING,Mathf.Abs(moveInput) > 0.01f);
        if(Mathf.Abs(moveInput) > 0.01f) {
            facing = moveInput > 0 ? 1f : -1f;
            if(playerVisual != null) {
                var s = playerVisual.localScale;
                s.x = visualScaleX * facing;
                playerVisual.localScale = s;
            }
        }

        // Ground / timers
        if(IsGrounded()) lastGroundedTime = coyoteTime;
        else lastGroundedTime -= Time.deltaTime;

        lastJumpPressedTime -= Time.deltaTime;

        if(isJumping) {
            jumpHoldCounter -= Time.deltaTime;
            if(jumpHoldCounter <= 0f) isJumping = false;
        }
    }

    void FixedUpdate() {
        // Horizontal smoothing
        float targetX = moveInput * moveSpeed;
        float accelRate = Mathf.Abs(targetX) > 0.01f ? accel : decel;
        Vector2 v = rb.linearVelocity;
        v.x = Mathf.MoveTowards(v.x,targetX,accelRate * Time.fixedDeltaTime);

        // Start jump if buffered + grounded (coyote)
        if(lastJumpPressedTime > 0f && lastGroundedTime > 0f) {
            v.y = jumpVelocity;
            isJumping = true;
            jumpHoldCounter = maxJumpHoldTime;
            lastJumpPressedTime = 0f;
            lastGroundedTime = 0f;
        }

        // Early release = cut jump
        if(jumpReleasedThisFrame && v.y > 0f) {
            v.y *= jumpCutMultiplier;
            isJumping = false;
        }

        // Gravity shaping
        float g = Physics2D.gravity.y; // negative
        float gravityScale = 1f;

        if(v.y < 0f) gravityScale = fallMultiplier;
        else if(Mathf.Abs(v.y) < apexHangVelThreshold) gravityScale = apexHangMultiplier;

        float extraG = (gravityScale - 1f) * g;
        v.y += extraG * Time.fixedDeltaTime;

        // Clamp terminal speed
        if(v.y < maxFallSpeed) v.y = maxFallSpeed;

        rb.linearVelocity = v;

        // consume one-frame flags
        jumpPressedThisFrame = false;
        jumpReleasedThisFrame = false;
    }

    void OnJumpStarted(object sender,EventArgs e) {
        lastJumpPressedTime = jumpBuffer; // buffer it
        jumpPressedThisFrame = true;
    }

    void OnJumpCanceled(object sender,EventArgs e) {
        jumpReleasedThisFrame = true;
    }

    bool IsGrounded() {
        Vector2 origin = (Vector2)transform.position + groundBoxOffset;
        RaycastHit2D hit = Physics2D.BoxCast(origin,groundBoxSize,0f,Vector2.down,0f,solidsMask);
        return hit.collider != null;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Vector2 origin = (Vector2)transform.position + groundBoxOffset;
        Gizmos.DrawWireCube(origin,groundBoxSize);
    }
}
