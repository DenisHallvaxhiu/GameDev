using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FloorTrap : MonoBehaviour {
    //Detection
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private Vector2 detectionOffset = Vector2.zero;
    [SerializeField] private LayerMask playerMask;

    //Animator
    private const string TRIGGER = "Triggered"; // leave blank to skip

    private Animator anim;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        Vector2 center = (Vector2)transform.position + detectionOffset;
        bool near = Physics2D.OverlapCircle(center,detectionRadius,playerMask);
        Debug.Log(anim.GetBool(TRIGGER));

        anim.SetBool(TRIGGER,near);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Vector2 center = (Vector2)transform.position + detectionOffset;
        Gizmos.DrawWireSphere(center,detectionRadius);
    }
}
