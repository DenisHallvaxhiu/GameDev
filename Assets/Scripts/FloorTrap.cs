using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FloorTrap : MonoBehaviour {
    //Detection
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private Vector2 detectionOffset = Vector2.zero;
    [SerializeField] private LayerMask playerMask;

    //Animator
    [SerializeField] private string wakeTrigger = "Wake";   // leave blank to skip
    [SerializeField] private string nearBool = "Triggered"; // leave blank to skip

    private Animator anim;
    private bool wasNear;

    void Awake() { anim = GetComponent<Animator>(); }

    void Update() {
        Vector2 center = (Vector2)transform.position + detectionOffset;
        bool near = Physics2D.OverlapCircle(center,detectionRadius,playerMask);
        Debug.Log(near);
        //if(near && !wasNear && !string.IsNullOrEmpty(wakeTrigger))
        //    anim.SetTrigger(wakeTrigger);

        //if(!string.IsNullOrEmpty(nearBool))
        //    anim.SetBool(nearBool,near);

        wasNear = near;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Vector2 center = (Vector2)transform.position + detectionOffset;
        Gizmos.DrawWireSphere(center,detectionRadius);
    }
}
