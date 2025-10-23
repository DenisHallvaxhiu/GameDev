using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour {
    [SerializeField] private float fallWait = .5f;
    private float destroyWait = 1f;

    private bool isFalling;
    Rigidbody2D rb;
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if(!isFalling && collision.gameObject.CompareTag("Player")) {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall() {
        isFalling = true;
        yield return new WaitForSeconds(fallWait);
        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject,destroyWait);
    }
}
