using UnityEngine;
using System.Collections;

public class CrushingCeiling : MonoBehaviour {
    [SerializeField] private Transform transformFrom;
    [SerializeField] private Transform transformTo;

    [SerializeField] private float moveTime = 15f;
    private AnimationCurve ease = AnimationCurve.Linear(0,0,1,1);

    Rigidbody2D rb;
    float normalized = 0f;


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void FixedUpdate() {
        normalized += Time.fixedDeltaTime / moveTime;
        float eased = ease.Evaluate(Mathf.Clamp01(normalized));
        Vector2 target = Vector2.Lerp(transformFrom.position,transformTo.position,eased);
        rb.MovePosition(target);
    }

}
