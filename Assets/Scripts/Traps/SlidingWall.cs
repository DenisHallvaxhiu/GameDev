using UnityEngine;
using System.Collections;

public class SlidingWall : MonoBehaviour {

    [SerializeField] private Transform transformFrom;
    [SerializeField] private Transform transformTo;

    private float moveTime = 1.5f;
    private float awaitAtEnds = .2f;
    private bool backAndForth = false;
    private AnimationCurve ease = AnimationCurve.EaseInOut(0,0,1,1);

    private bool startOnPlayerProximity = true;
    [SerializeField] private float startDistance = 4f;
    [SerializeField] LayerMask playerMast;

    Rigidbody2D rb;
    float normalized = 0f;
    int direction = 1;
    bool moving = false;


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void FixedUpdate() {
        if(!moving) {
            if(startOnPlayerProximity && !PlayerClose()) return;
            moving = true;
        }

        normalized += (Time.fixedDeltaTime / moveTime) * direction;
        float eased = ease.Evaluate(Mathf.Clamp01(normalized));
        Vector2 target = Vector2.Lerp(transformFrom.position,transformTo.position,eased);
        rb.MovePosition(target);

        if((direction > 0 && normalized >= 1f) || (direction < 0 && normalized <= 0f)) {
            StartCoroutine(WaitAndFlip());
        }

    }

    IEnumerator WaitAndFlip() {
        moving = false;
        yield return new WaitForSeconds(awaitAtEnds);
        if(backAndForth) {
            direction *= -1;
        }
        else {
            enabled = false;
        }
    }

    bool PlayerClose() {
        return Physics2D.OverlapCircle(transform.position,startDistance,playerMast);
    }

    private void OnDrawGizmosSelected() {
        if(transformFrom && transformTo) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transformFrom.position,transformTo.position);
        }
        Gizmos.DrawWireSphere(transform.position,startDistance);
    }
}
