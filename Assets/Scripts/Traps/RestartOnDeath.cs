using UnityEngine;
using System;

public class TrapRestart : MonoBehaviour {

    bool reloading;
    public event EventHandler OnDeath;


    private void OnCollisionEnter2D(Collision2D collision) {
        if(!reloading && collision.gameObject.CompareTag("Player")) {
            reloading = true;
            OnDeath?.Invoke(this,EventArgs.Empty);
        }
    }

}
