using UnityEngine;

[RequireComponent( typeof( PlayerInputHandler ) )]
public class PlayerMovementController: MonoBehaviour {
    public float speed = 5f;
    public float collisionRadius = 1f;

    public PlayerData PlayerData;

    enum MovementMode {
        Idle,
        Walking
    }

    MovementMode movementState = MovementMode.Idle;
    MovementMode previousMovementState = MovementMode.Idle;

    Vector3 velocity;
    Vector3 wishDir;
    PlayerInputHandler inputHandler;
    CharacterController characterController;

    void Awake() {
        inputHandler = GetComponent<PlayerInputHandler>();
        characterController = GetComponent<CharacterController>();
    }

    void OnEnable() {
        inputHandler.OnMovePerformed += InputMovementDirection;
    }

    void OnDisable() {
        inputHandler.OnMovePerformed -= InputMovementDirection;
    }

    void Update() {
        DecideMovementState();

        PlayerData.position = transform.position; // references position before moving
        if( movementState == MovementMode.Walking ) {
            // call move a method from movement class later if movement gets complicated
            velocity = wishDir * speed;
            characterController.Move( velocity * Time.deltaTime );
        }
    }

    public void InputMovementDirection( Vector2 input ) {
        wishDir = Vector3.ClampMagnitude( new Vector3( input.x, 0f, input.y ), 1f );
    }

    void DecideMovementState() {
        MovementMode newState;

        if( wishDir != Vector3.zero ) {
            newState = MovementMode.Walking;
        }
        else {
            newState = MovementMode.Idle;
        }

        if( newState == movementState ) return;

        ExitMovementState( movementState );
        EnterMovementState( newState );
        previousMovementState = movementState;
        movementState = newState;
    }

    // start/stop effect coroutines or methods
    void EnterMovementState( MovementMode newState ) {
        switch( newState ) {
            case MovementMode.Walking:
                //debug.log( "Started moving" );
                break;
            case MovementMode.Idle:
                //debug.log( "Idle" );
                break;
        }
    }

    void ExitMovementState( MovementMode oldState ) {
        if( oldState == MovementMode.Walking ) {
            //debug.log( "Stopped moving" );
        }
    }
}