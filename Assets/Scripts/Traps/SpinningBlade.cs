using UnityEngine;

public class SpinningBlade : MonoBehaviour {

    [SerializeField] private float rotationSpeed = 5f;

    void FixedUpdate() {
        this.transform.Rotate(new Vector3(0,0,rotationSpeed));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        ReloadScene.Instance.Reload();
    }
}
