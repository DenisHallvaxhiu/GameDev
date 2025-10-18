using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour {

    public static ReloadScene Instance { get; private set; }

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerator ReloadCoroutine(float reloadDelay) {
        yield return new WaitForSeconds(reloadDelay);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void Reload(float reloadDelay = 0.5f) {
        StartCoroutine(ReloadCoroutine(reloadDelay));
    }

}
