using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler: MonoBehaviour {
    public event Action<Vector2> OnMovePerformed;
    public event Action OnMoveCanceled;

    public event Action OnPushStarted;
    public event Action OnPushCanceled;

    public event Action OnPullStarted;
    public event Action OnPullCanceled;

    InputSystem_Actions controls;

    bool leftHeld, rightHeld, upHeld, downHeld;
    bool pushButtonHeld, pullButtonHeld;
    float pushAnalogValue, pullAnalogValue;

    enum HorizontalDirection { None, Left, Right }
    enum VerticalDirection { None, Up, Down }
    enum InteractionType { None, Push, Pull }

    HorizontalDirection lastHorizontal = HorizontalDirection.None;
    VerticalDirection lastVertical = VerticalDirection.None;
    InteractionType lastInteraction = InteractionType.None;

    void Awake() => controls = new InputSystem_Actions();

    void OnEnable() {
        controls.Enable();

        controls.Player.Move.performed += ctx => ResolveMovement();
        controls.Player.Move.canceled += ctx => OnMoveCanceled?.Invoke();

        controls.Player.MoveLeft.performed += ctx => { leftHeld = true; lastHorizontal = HorizontalDirection.Left; ResolveMovement(); };
        controls.Player.MoveLeft.canceled += ctx => { leftHeld = false; ResolveMovement(); };

        controls.Player.MoveRight.performed += ctx => { rightHeld = true; lastHorizontal = HorizontalDirection.Right; ResolveMovement(); };
        controls.Player.MoveRight.canceled += ctx => { rightHeld = false; ResolveMovement(); };

        controls.Player.MoveUp.performed += ctx => { upHeld = true; lastVertical = VerticalDirection.Up; ResolveMovement(); };
        controls.Player.MoveUp.canceled += ctx => { upHeld = false; ResolveMovement(); };

        controls.Player.MoveDown.performed += ctx => { downHeld = true; lastVertical = VerticalDirection.Down; ResolveMovement(); };
        controls.Player.MoveDown.canceled += ctx => { downHeld = false; ResolveMovement(); };

        // Push/Pull Button
        controls.Player.Push.performed += ctx => { pushButtonHeld = true; lastInteraction = InteractionType.Push; ResolveInteraction(); };
        controls.Player.Push.canceled += ctx => { pushButtonHeld = false; ResolveInteraction(); };

        controls.Player.Pull.performed += ctx => { pullButtonHeld = true; lastInteraction = InteractionType.Pull; ResolveInteraction(); };
        controls.Player.Pull.canceled += ctx => { pullButtonHeld = false; ResolveInteraction(); };

        // Push/Pull Analog
        controls.Player.PushAnalog.performed += ctx => { pushAnalogValue = ctx.ReadValue<float>(); lastInteraction = InteractionType.Push; ResolveInteraction(); };
        controls.Player.PushAnalog.canceled += ctx => { pushAnalogValue = 0f; ResolveInteraction(); };

        controls.Player.PullAnalog.performed += ctx => { pullAnalogValue = ctx.ReadValue<float>(); lastInteraction = InteractionType.Pull; ResolveInteraction(); };
        controls.Player.PullAnalog.canceled += ctx => { pullAnalogValue = 0f; ResolveInteraction(); };
    }

    void OnDisable() {
        controls.Player.Move.performed -= ctx => ResolveMovement();
        controls.Player.Move.canceled -= ctx => OnMoveCanceled?.Invoke();

        controls.Player.MoveLeft.performed -= ctx => { leftHeld = true; lastHorizontal = HorizontalDirection.Left; ResolveMovement(); };
        controls.Player.MoveLeft.canceled -= ctx => { leftHeld = false; ResolveMovement(); };

        controls.Player.MoveRight.performed -= ctx => { rightHeld = true; lastHorizontal = HorizontalDirection.Right; ResolveMovement(); };
        controls.Player.MoveRight.canceled -= ctx => { rightHeld = false; ResolveMovement(); };

        controls.Player.MoveUp.performed -= ctx => { upHeld = true; lastVertical = VerticalDirection.Up; ResolveMovement(); };
        controls.Player.MoveUp.canceled -= ctx => { upHeld = false; ResolveMovement(); };

        controls.Player.MoveDown.performed -= ctx => { downHeld = true; lastVertical = VerticalDirection.Down; ResolveMovement(); };
        controls.Player.MoveDown.canceled -= ctx => { downHeld = false; ResolveMovement(); };

        controls.Player.Push.performed -= ctx => { pushButtonHeld = true; lastInteraction = InteractionType.Push; ResolveInteraction(); };
        controls.Player.Push.canceled -= ctx => { pushButtonHeld = false; ResolveInteraction(); };

        controls.Player.Pull.performed -= ctx => { pullButtonHeld = true; lastInteraction = InteractionType.Pull; ResolveInteraction(); };
        controls.Player.Pull.canceled -= ctx => { pullButtonHeld = false; ResolveInteraction(); };

        controls.Player.PushAnalog.performed -= ctx => { pushAnalogValue = ctx.ReadValue<float>(); lastInteraction = InteractionType.Push; ResolveInteraction(); };
        controls.Player.PushAnalog.canceled -= ctx => { pushAnalogValue = 0f; ResolveInteraction(); };

        controls.Player.PullAnalog.performed -= ctx => { pullAnalogValue = ctx.ReadValue<float>(); lastInteraction = InteractionType.Pull; ResolveInteraction(); };
        controls.Player.PullAnalog.canceled -= ctx => { pullAnalogValue = 0f; ResolveInteraction(); };

        controls.Disable();
    }

    void ResolveMovement() {
        Vector2 analog = controls.Player.Move.ReadValue<Vector2>();
        Vector2 resolved = Vector2.zero;

        resolved.x = leftHeld && !rightHeld ? -1 :
                     rightHeld && !leftHeld ? 1 :
                     leftHeld && rightHeld ? ( lastHorizontal == HorizontalDirection.Right ? 1 : -1) :
                     analog.x;

        resolved.y = upHeld && !downHeld ? 1 :
                     downHeld && !upHeld ? -1 :
                     upHeld && downHeld ? (lastVertical == VerticalDirection.Down ? -1 : 1) :
                     analog.y;

        OnMovePerformed?.Invoke( resolved );
    }

    void ResolveInteraction() {
        bool pushActive = pushButtonHeld || pushAnalogValue > 0.1f;
        bool pullActive = pullButtonHeld || pullAnalogValue > 0.1f;

        if( !pushActive && !pullActive ) {
            OnPushCanceled?.Invoke();
            OnPullCanceled?.Invoke();
            return;
        }

        if( !pullActive )
            OnPushStarted?.Invoke();
        else if( !pushActive )
            OnPullStarted?.Invoke();
        else
            (lastInteraction == InteractionType.Pull ? OnPullStarted : OnPushStarted)?.Invoke();
    }
}