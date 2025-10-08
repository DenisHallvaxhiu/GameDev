using UnityEngine;

public class PlayerMovement : MonoBehaviour {


    [SerializeField] private float moveSpeed = 3f;

    void Update() {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x,inputVector.y,0f);
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

}
