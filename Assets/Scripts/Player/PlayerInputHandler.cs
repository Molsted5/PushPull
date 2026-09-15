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

        controls.Player.MoveLeft.performed += HandleMoveDirectionInput;
        controls.Player.MoveLeft.canceled += HandleMoveDirectionInput;

        controls.Player.MoveRight.performed += HandleMoveDirectionInput;
        controls.Player.MoveRight.canceled += HandleMoveDirectionInput;

        controls.Player.MoveUp.performed += HandleMoveDirectionInput;
        controls.Player.MoveUp.canceled += HandleMoveDirectionInput;

        controls.Player.MoveDown.performed += HandleMoveDirectionInput;
        controls.Player.MoveDown.canceled += HandleMoveDirectionInput;

        controls.Player.Look.performed += HandleLookInput;
        controls.Player.Look.canceled += HandleLookInput;

        controls.Player.Push.started += PushStartedInput;
        controls.Player.Push.performed += PushPerformedInput;
        controls.Player.Push.canceled += PushCanceledInput; 

        controls.Player.Pull.started += PullStartedInput;
        controls.Player.Pull.performed += PullPerformedInput;
        controls.Player.Pull.canceled += PullCanceledInput;

        controls.Player.Reload.started += ReloadStartedInput;
        controls.Player.Reload.canceled += ReloadCanceledInput;
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

        controls.Player.Look.performed -= HandleLookInput;
        controls.Player.Look.canceled -= HandleLookInput;

        controls.Player.Push.started -= PushStartedInput;
        controls.Player.Push.performed -= PushPerformedInput;
        controls.Player.Push.canceled -= PushCanceledInput;

        controls.Player.Pull.started -= PullStartedInput;
        controls.Player.Pull.performed -= PullPerformedInput;
        controls.Player.Pull.canceled -= PullCanceledInput;

        controls.Player.Reload.started -= ReloadStartedInput;
        controls.Player.Reload.canceled -= ReloadCanceledInput;

        controls.Disable();
    }

    private void Update() {
        // Script execution order skal sættes til -2 for at denne update udføres før de andre, hvilket er nødvendigt for at de er garranteret at få den korrekte værdi.
        UpdateMovementInput();
    }

    // movement Handlers
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

    //void HandleMoveRightInput( InputAction.CallbackContext ctx ) {
    //    lastHorizontal = HorizontalDirection.Left;
    //}

    //void HandleMoveRightInput( InputAction.CallbackContext ctx ) {
    //    lastHorizontal = HorizontalDirection.Right;
    //}

    //void HandleMoveUpInput( InputAction.CallbackContext ctx ) {
    //    lastVertical = VerticalDirection.Up;
    //}

    //void HandleMoveDownInput( InputAction.CallbackContext ctx ) {   
    //    lastVertical = VerticalDirection.Down;
    //}

    // look handler
    void HandleLookInput( InputAction.CallbackContext ctx ) {
        ResolveLook();
    } 

    // push handlers
    void PushStartedInput( InputAction.CallbackContext ctx ) { 
        OnPushStarted?.Invoke( ctx.ReadValue<float>() ); 
    }

    void PushPerformedInput( InputAction.CallbackContext ctx ) {
        OnPushPerformed?.Invoke( ctx.ReadValue<float>() );
    }

    void PushCanceledInput( InputAction.CallbackContext ctx ) {
        OnPushCanceled?.Invoke();
    }

    // pull handlers
    void PullStartedInput( InputAction.CallbackContext ctx ) {
        OnPullStarted?.Invoke( ctx.ReadValue<float>() );
    }

    void PullPerformedInput( InputAction.CallbackContext ctx ) {
        OnPullPerformed?.Invoke( ctx.ReadValue<float>() );
    }

    void PullCanceledInput( InputAction.CallbackContext ctx ) {
        OnPullCanceled?.Invoke();
    }

    // reload handlers
    void ReloadStartedInput( InputAction.CallbackContext ctx ) {
        OnReloadStarted?.Invoke();
    }

    void ReloadCanceledInput( InputAction.CallbackContext ctx ) {
        OnReloadCanceled?.Invoke();
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