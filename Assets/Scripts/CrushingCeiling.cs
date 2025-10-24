using UnityEngine;
using System.Collections;

public class CrushingCeiling : MonoBehaviour {
    [SerializeField] private Transform transformFrom;
    [SerializeField] private Transform transformTo;

    [SerializeField] private float moveTime = 50f;
    private const float TO_SECONDS = 100f;
    private AnimationCurve ease = AnimationCurve.Constant(0,1,1);

    Rigidbody2D rb;
    float normalized = 0f;


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void FixedUpdate() {
        if(rb.transform.position.y < transformTo.transform.position.y + 0.1) {
            ReloadScene.Instance.Reload();
        }

        normalized += Time.deltaTime / moveTime;
        float eased = ease.Evaluate(Mathf.Clamp01(normalized)) / TO_SECONDS;
        Vector2 target = Vector2.Lerp(transformFrom.position,transformTo.position,eased);
        rb.MovePosition(target);


    }

}
