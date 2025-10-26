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
        yield return new WaitForSecondsRealtime(reloadDelay);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
        Time.timeScale = 1f;
    }

    public void Reload(float reloadDelay = 1f) {
        //GameInput.Instance.StopMovement();
        Time.timeScale = 0f;
        StartCoroutine(ReloadCoroutine(reloadDelay));
    }

}
