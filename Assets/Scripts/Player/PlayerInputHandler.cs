using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler: MonoBehaviour {
    public event Action<Vector2> OnMovePerformed;

    public event Action<Vector2> OnMouseLook;
    public event Action<Vector3> OnStickLook;

    public event Action<float> OnPushStarted;
    public event Action<float> OnPushPerformed;
    public event Action OnPushCanceled;

    public event Action<float> OnPullStarted;
    public event Action<float> OnPullPerformed;
    public event Action OnPullCanceled;

    public event Action OnReloadStarted;
    public event Action OnReloadCanceled;

    InputSystem_Actions controls;
    PlayerInput playerInput;

    enum HorizontalDirection { None, Left, Right }
    enum VerticalDirection { None, Up, Down }

    HorizontalDirection lastHorizontal = HorizontalDirection.None;
    VerticalDirection lastVertical = VerticalDirection.None;

    void Awake() {
        controls = new InputSystem_Actions();
        playerInput = GetComponent<PlayerInput>();
    } 

    void OnEnable() {
        controls.Enable();

        controls.Player.MoveLeft.performed += HandleMoveLeftInput;
        controls.Player.MoveLeft.canceled += HandleMoveLeftInput;

        controls.Player.MoveRight.performed += HandleMoveRightInput;
        controls.Player.MoveRight.canceled += HandleMoveRightInput;

        controls.Player.MoveUp.performed += HandleMoveUpInput;
        controls.Player.MoveUp.canceled += HandleMoveUpInput;

        controls.Player.MoveDown.performed += HandleMoveDownInput;
        controls.Player.MoveDown.canceled += HandleMoveDownInput;

        controls.Player.Look.performed += HandleLookInput;
        controls.Player.Look.canceled += HandleLookInput;

        controls.Player.Push.started += HandlePushInput;
        controls.Player.Push.performed += HandlePushInput;
        controls.Player.Push.canceled += HandlePushInput;

        controls.Player.Pull.started += HandlePullInput;
        controls.Player.Pull.performed += HandlePullInput;
        controls.Player.Pull.canceled += HandlePullInput;

        controls.Player.Reload.started += HandleReloadInput;
        controls.Player.Reload.performed += HandleReloadInput;
        controls.Player.Reload.canceled += HandleReloadInput;
    }

    void OnDisable() {
        controls.Player.MoveLeft.performed -= HandleMoveLeftInput;
        controls.Player.MoveLeft.canceled -= HandleMoveLeftInput;

        controls.Player.MoveRight.performed -= HandleMoveRightInput;
        controls.Player.MoveRight.canceled -= HandleMoveRightInput;

        controls.Player.MoveUp.performed -= HandleMoveUpInput;
        controls.Player.MoveUp.canceled -= HandleMoveUpInput;

        controls.Player.MoveDown.performed -= HandleMoveDownInput;
        controls.Player.MoveDown.canceled -= HandleMoveDownInput;

        controls.Player.Look.performed -= HandleLookInput;
        controls.Player.Look.canceled -= HandleLookInput;

        controls.Player.Push.started -= PushStartedInput;
        controls.Player.Push.performed -= PushPerformedInput;
        controls.Player.Push.canceled -= PushCancelledInput;

        controls.Player.Pull.started -= PullStartedInput;
        controls.Player.Pull.performed -= PullPerformedInput;
        controls.Player.Pull.canceled -= PullCancelledInput;

        controls.Player.Reload.started -= HandleReloadInput;
        controls.Player.Reload.performed -= HandleReloadInput;
        controls.Player.Reload.canceled -= HandleReloadInput;

        controls.Disable();
    }

    // movement Handlers
    void HandleMoveLeftInput( InputAction.CallbackContext ctx ) {
        lastHorizontal = HorizontalDirection.Left;
        ResolveMovement();
    }

    void HandleMoveRightInput( InputAction.CallbackContext ctx ) {
        lastHorizontal = HorizontalDirection.Right;
        ResolveMovement();
    }

    void HandleMoveUpInput( InputAction.CallbackContext ctx ) {
        lastVertical = VerticalDirection.Up;
        ResolveMovement();
    }

    void HandleMoveDownInput( InputAction.CallbackContext ctx ) {   
        lastVertical = VerticalDirection.Down;
        ResolveMovement();
    }

    // look handler
    void HandleLookInput( InputAction.CallbackContext ctx ) {
        ResolveLook();
    } 

    // push/pull handlers
    void PushStartedInput( InputAction.CallbackContext ctx ) { 
        OnPushPerformed?.Invoke( ctx.ReadValue<float>() ); 
    }

    void PushPerformedInput( InputAction.CallbackContext ctx ) {
        OnPushPerformed?.Invoke( ctx.ReadValue<float>() );
    }

    void PushCancelledInput( InputAction.CallbackContext ctx ) {
        OnPushPerformed?.Invoke( ctx.ReadValue<float>() );
    }

    void HandlePullInput( InputAction.CallbackContext ctx ) { 
        OnPullPerformed?.Invoke( ctx.ReadValue<float>() ); 
    }

    // reload handler
    void HandleReloadInput( InputAction.CallbackContext ctx ) { OnReloadPerformed?.Invoke(); }

    void ResolveMovement() {
        float left = controls.Player.MoveLeft.ReadValue<float>();
        float right = controls.Player.MoveRight.ReadValue<float>();
        float up = controls.Player.MoveUp.ReadValue<float>();
        float down = controls.Player.MoveDown.ReadValue<float>();

        Vector2 resolved = Vector2.zero;

        resolved.x = right > left ? right - left :
                     left > right ? -( left - right ) :
                     lastHorizontal == HorizontalDirection.Right ? right : -left;

        resolved.y = up > down ? up - down :
                     down > up ? -( down - up ) :
                     lastVertical == VerticalDirection.Down ? -down : up;

        OnMovePerformed?.Invoke( resolved );
    }

    void ResolveLook() {
        Vector2 lookValue = controls.Player.Look.ReadValue<Vector2>();
        string scheme = playerInput.currentControlScheme;

        if( scheme == "Keyboard&Mouse" ) {
            // lookValue is already screen position
            OnMouseLook?.Invoke( lookValue );
        }
        else {
            // lookValue is stick velocity
            Vector3 velocity = new Vector3( lookValue.x, 0f, lookValue.y );
            OnStickLook?.Invoke( velocity );
        }
    }
}