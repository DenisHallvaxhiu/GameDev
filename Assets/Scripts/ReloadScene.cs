using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour {

    int reloadDelay = 500; //miliseconds
    public static ReloadScene Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }
    private void Reload() {
        Thread.Sleep(reloadDelay);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}
