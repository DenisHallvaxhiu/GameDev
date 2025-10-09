using UnityEngine;

public class PlayerMovement : MonoBehaviour {


    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D rb;

    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x,inputVector.y * 2,0f);
        transform.position += moveDir * moveSpeed * Time.deltaTime;



        Debug.Log(transform.position);
    }

}
