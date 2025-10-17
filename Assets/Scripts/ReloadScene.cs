using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour {

    int reloadDelay = 500; //miliseconds



    private void ReloadScene_OnDeath() {
        Thread.Sleep(reloadDelay);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}
