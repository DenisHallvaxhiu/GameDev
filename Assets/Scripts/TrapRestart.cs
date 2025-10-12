using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapRestart : MonoBehaviour {

    [SerializeField] float reloadDelay = 0.5f; // Gotta add some kind of reload screen or sfx maybe idk
    bool reloading;


    private void OnCollisionEnter2D(Collision2D collision) {
        if(!reloading && collision.gameObject.CompareTag("Player"))
            StartCoroutine((Reload()));
    }

    IEnumerator Reload() {
        reloading = true;
        yield return new WaitForSeconds(reloadDelay);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}
