using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum LatestGunInputType { None, Push, Pull, Reload }
public enum LatestGunInputPhaze { None, Performed, Canceled }

public class PlayerInputHandler: MonoBehaviour {
    InputSystem_Actions controls;
    PlayerInput playerInput;

    // Move inputs
    enum HorizontalDirection { None, Left, Right }
    enum VerticalDirection { None, Up, Down }
    
    HorizontalDirection lastHorizontal = HorizontalDirection.None;
    VerticalDirection lastVertical = VerticalDirection.None;

    public Vector2 MoveValue { get; private set; }

    // Look inputs
    public Vector2 MouseLookValue { get; private set; }
    public Vector3 StickLookValue { get; private set; }

    // Gun inputs
    public float pullValue { get; private set; }
    public float pushValue { get; private set; }
    public float reloadValue { get; private set; }
    
    public LatestGunInputType LatestGunInputTypeState { get; private set; }
    public LatestGunInputPhaze LatestGunInputPhazeState { get; private set; }

    void Awake() {
        controls = new InputSystem_Actions();
        playerInput = GetComponent<PlayerInput>();
    } 

    void OnEnable() {
        controls.Enable();

        controls.Player.MoveLeft.performed += HandleMoveDirectionInput;
        controls.Player.MoveLeft.canceled += HandleMoveDirectionInput;

        controls.Player.MoveRight.performed += HandleMoveDirectionInput;
        controls.Player.MoveRight.canceled += HandleMoveDirectionInput;

        controls.Player.MoveUp.performed += HandleMoveDirectionInput;
        controls.Player.MoveUp.canceled += HandleMoveDirectionInput;

        controls.Player.MoveDown.performed += HandleMoveDirectionInput;
        controls.Player.MoveDown.canceled += HandleMoveDirectionInput;

        controls.Player.Push.performed += HandlePushInput;
        controls.Player.Push.canceled += HandlePushInput; 

        controls.Player.Pull.performed += HandlePullInput;
        controls.Player.Pull.canceled += HandlePullInput;

        controls.Player.Reload.performed += HandleReloadInput;
        controls.Player.Reload.canceled += HandleReloadInput;
    }

    private void Update() {
        // Script execution order skal sættes til et negativt tal for at denne update udføres før update i andre scripts, hvilket er nødvendigt for at de er garranteret at få den korrekte værdi.
        UpdateMovementInput();
        UpdateLookInput();
        UpdateGunInputs();
    }

    // Handle inputs
    void HandleMoveDirectionInput( InputAction.CallbackContext ctx ) {
        if( ctx.action == controls.Player.MoveLeft ) {
            lastHorizontal = HorizontalDirection.Left;
        }
        else if( ctx.action == controls.Player.MoveRight ) {
            lastHorizontal = HorizontalDirection.Right;
        }
        else if( ctx.action == controls.Player.MoveUp ) {
            lastVertical = VerticalDirection.Up;
        }
        else if( ctx.action == controls.Player.MoveDown ) {
            lastVertical = VerticalDirection.Down;
        }
    }

    void HandleGunInput( InputAction.CallbackContext ctx, LatestGunInputType type ) {
        LatestGunInputTypeState = type;
        LatestGunInputPhazeState = ctx.performed ? LatestGunInputPhaze.Performed :
                                   ctx.canceled ? LatestGunInputPhaze.Canceled : LatestGunInputPhazeState;
    }

    void HandlePushInput( InputAction.CallbackContext ctx ) { 
        HandleGunInput( ctx, LatestGunInputType.Push );
    }

    void HandlePullInput( InputAction.CallbackContext ctx ) {
        HandleGunInput( ctx, LatestGunInputType.Pull );
    }

    void HandleReloadInput( InputAction.CallbackContext ctx ) {
        HandleGunInput( ctx, LatestGunInputType.Reload );
    }

    // Update input values
    void UpdateGunInputs() {
        pushValue = controls.Player.Push.ReadValue<float>();
        pullValue = controls.Player.Pull.ReadValue<float>();
        reloadValue = controls.Player.Reload.ReadValue<float>();
    }

    void UpdateMovementInput() {
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

        MoveValue = resolved;
    }

    void UpdateLookInput() {
        Vector2 lookValue = controls.Player.Look.ReadValue<Vector2>();
        string scheme = playerInput.currentControlScheme;

        if( scheme == "Keyboard&Mouse" ) {
            // lookValue is already screen position
            MouseLookValue = lookValue;
        }
        else {
            // lookValue is stick velocity
            StickLookValue = new Vector3( lookValue.x, 0f, lookValue.y );
        }
    }

    void OnDisable() {
        controls.Player.MoveLeft.performed -= HandleMoveDirectionInput;
        controls.Player.MoveLeft.canceled -= HandleMoveDirectionInput;

        controls.Player.MoveRight.performed -= HandleMoveDirectionInput;
        controls.Player.MoveRight.canceled -= HandleMoveDirectionInput;

        controls.Player.MoveUp.performed -= HandleMoveDirectionInput;
        controls.Player.MoveUp.canceled -= HandleMoveDirectionInput;

        controls.Player.MoveDown.performed -= HandleMoveDirectionInput;
        controls.Player.MoveDown.canceled -= HandleMoveDirectionInput;

        controls.Player.Push.performed -= HandlePushInput;
        controls.Player.Push.canceled -= HandlePushInput;

        controls.Player.Pull.performed -= HandlePullInput;
        controls.Player.Pull.canceled -= HandlePullInput;

        controls.Player.Reload.performed -= HandleReloadInput;
        controls.Player.Reload.canceled -= HandleReloadInput;

        controls.Disable();
    }
}