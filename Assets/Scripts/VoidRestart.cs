using UnityEngine;

public class VoidRestart : MonoBehaviour {


    void Update() {
        if(transform.position.y < -6f) {
            Debug.Log("Void");
        }
    }
}
