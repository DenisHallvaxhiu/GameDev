using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {

    public static GameInput Instance { get; private set; }

    public event EventHandler OnJumpStarted;
    public event EventHandler OnJumpCanceled;

    private PlayerInputActions playerInputActions;


    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable() {
        if(playerInputActions == null) playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.started += Jump_started;
        playerInputActions.Player.Jump.canceled += Jump_canceled;
    }

    private void OnDisable() {
        if(playerInputActions != null) {
            playerInputActions.Player.Jump.started -= Jump_started;
            playerInputActions.Player.Jump.canceled -= Jump_canceled;
            playerInputActions.Player.Disable();
        }
    }

    private void OnDestroy() {
        if(Instance == this) Instance = null;
    }
    private void Jump_started(InputAction.CallbackContext context) {
        OnJumpStarted?.Invoke(this,EventArgs.Empty);
    }

    private void Jump_canceled(InputAction.CallbackContext context) {
        OnJumpCanceled?.Invoke(this,EventArgs.Empty);
    }

    public float GetMovementInput() {
        float inputVector = playerInputActions.Player.Move.ReadValue<float>();
        return inputVector;
    }

    public void StopMovement() {
        playerInputActions.Player.Disable();
    }
}
