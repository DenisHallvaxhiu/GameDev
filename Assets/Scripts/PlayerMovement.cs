using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    const float GROUND_CHECK_DISTANCE = 0.2f;

    [SerializeField] private LayerMask solidsMask;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float fallMultiplier = 1.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float maxJumpTime = .35f;



    private Rigidbody2D rb;
    private bool canJump;
    private float jumpTimeCounter;



    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Start() {
        //Calls Events 
        GameInput.Instance.OnJumpStarted += OnJumpStarted;
        GameInput.Instance.OnJumpCanceled += OnJumpCanceled;
    }

    private void OnJumpStarted(object sender,EventArgs e) {
        //Checks if its grounded and allows it to jump
        if(IsGrounded()) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpForce);
            canJump = true;
            jumpTimeCounter = maxJumpTime;
        }
    }

    private void OnJumpCanceled(object sender,EventArgs e) {
        //When u stop holding the jump button
        canJump = false;
    }


    private void Update() {
        // Continue jumping as long as key held and time not exceeded
        if(canJump && jumpTimeCounter > 0f) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpForce);
            jumpTimeCounter -= Time.deltaTime;
        }
        else {
            canJump = false;
        }

        // Better gravity control
        if(rb.linearVelocity.y < 0) {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }
        else if(rb.linearVelocity.y > 0 && !canJump) {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime;
        }
    }

    void FixedUpdate() {
        float moveInput = GameInput.Instance.GetMovementInput();
        rb.linearVelocity = new Vector2(moveInput * moveSpeed,rb.linearVelocity.y);
    }
    public bool IsGrounded() {
        Vector2 position = transform.position;
        return Physics2D.OverlapCircle(position + Vector2.down * 0.5f,GROUND_CHECK_DISTANCE,solidsMask);
    }
}
