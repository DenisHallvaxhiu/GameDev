using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingFloor : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Vector2 detectionOffset = Vector2.zero;
    [SerializeField] private float detectionRadius = 1f;
    [SerializeField] private LayerMask playerMask;

    [Header("Falling Settings")]
    [SerializeField] private float fallDelay = 0.5f;     // Time before fall
    [SerializeField] private float destroyDelay = 5f;    // Destroy after falling (optional)
    [SerializeField] private bool respawn = false;       // Reset after fall?
    [SerializeField] private float respawnTime = 3f;

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private bool triggered = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        startPosition = transform.position;
    }

    void Update()
    {
        if (!triggered && PlayerOnTop())
        {
            triggered = true;
            StartCoroutine(FallAfterDelay());
        }
    }

    bool PlayerOnTop()
    {
        Vector2 center = (Vector2)transform.position + detectionOffset;
        return Physics2D.OverlapCircle(center, detectionRadius, playerMask);
    }

    IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(fallDelay);
        Debug.Log("Changing body type to Dynamic!");
        rb.bodyType = RigidbodyType2D.Dynamic; // starts falling

        if (respawn)
        {
            yield return new WaitForSeconds(respawnTime);
            ResetFloor();
        }
        else if (destroyDelay > 0)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    void ResetFloor()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.position = startPosition;
        triggered = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 center = (Vector2)transform.position + detectionOffset;
        Gizmos.DrawWireSphere(center, detectionRadius);
    }
}
