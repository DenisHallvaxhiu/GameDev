using UnityEngine;

public class TrapRestart : MonoBehaviour {

    bool reloading;


    private void OnCollisionEnter2D(Collision2D collision) {
        if(!reloading && collision.gameObject.CompareTag("Player"))
            reloading = true;
        StartCoroutine((Reload()));
    }


}
