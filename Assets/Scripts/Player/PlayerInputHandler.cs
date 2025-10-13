using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler: MonoBehaviour {
    public event Action<Vector2> OnMovePerformed;

    public event Action<Vector2> OnMouseLook;
    public event Action<Vector3> OnStickLook;

    public event Action OnPushStarted;
    public event Action OnPushCanceled;

    public event Action OnPullStarted;
    public event Action OnPullCanceled;

    InputSystem_Actions controls;
    PlayerInput playerInput;

    enum HorizontalDirection { None, Left, Right }
    enum VerticalDirection { None, Up, Down }
    enum InteractionType { None, Push, Pull }

    HorizontalDirection lastHorizontal = HorizontalDirection.None;
    VerticalDirection lastVertical = VerticalDirection.None;
    InteractionType lastInteraction = InteractionType.None;

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

        controls.Player.Push.performed += HandlePushInput;
        controls.Player.Push.canceled += HandlePushInput;

        controls.Player.Pull.performed += HandlePullInput;
        controls.Player.Pull.canceled += HandlePullInput;
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

        controls.Player.Push.performed -= HandlePushInput;
        controls.Player.Push.canceled -= HandlePushInput;

        controls.Player.Pull.performed -= HandlePullInput;
        controls.Player.Pull.canceled -= HandlePullInput;

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
    void HandlePushInput( InputAction.CallbackContext ctx ) {
        lastInteraction = InteractionType.Push;
        ResolveInteraction();
    }

    void HandlePullInput( InputAction.CallbackContext ctx ) {
        lastInteraction = InteractionType.Pull;
        ResolveInteraction();
    }

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

    void ResolveInteraction() {
        float pushValue = controls.Player.Push.ReadValue<float>();
        float pullValue = controls.Player.Pull.ReadValue<float>();

        if( pushValue <= 0.1f && pullValue <= 0.1f ) {
            OnPushCanceled?.Invoke();
            OnPullCanceled?.Invoke();
            return;
        }

        if( pullValue <= 0.1f )
            OnPushStarted?.Invoke();
        else if( pushValue <= 0.1f )
            OnPullStarted?.Invoke();
        else
            (lastInteraction == InteractionType.Pull ? OnPullStarted : OnPushStarted)?.Invoke();
    }
}