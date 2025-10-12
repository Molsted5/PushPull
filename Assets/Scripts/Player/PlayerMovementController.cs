using UnityEngine;
using static PlayerActionController;

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

    void Awake() {
        inputHandler = GetComponent<PlayerInputHandler>();
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
            velocity = wishDir * speed;
            transform.position += velocity * Time.deltaTime;
        }
    }

    public void InputMovementDirection( Vector2 input ) {
        wishDir = Vector3.ClampMagnitude( new Vector3( input.x, 0f, input.y ), 1f );
    }

    void DecideMovementState() {
        MovementMode newState = wishDir != Vector3.zero ? MovementMode.Walking : MovementMode.Idle;
        
        if( newState == movementState ) return;

        ExitMovementState( movementState );
        EnterMovementState( newState );
        previousMovementState = movementState;
        movementState = newState;
    }

    // is this helpfull?
    void EnterMovementState( MovementMode newState ) {
        switch( newState ) {
            case MovementMode.Walking:
                Debug.Log( "Started moving" );
                break;
            case MovementMode.Idle:
                Debug.Log( "Idle" );
                break;
        }
    }

    void ExitMovementState( MovementMode oldState ) {
        if( oldState == MovementMode.Walking ) {
            Debug.Log( "Stopped moving" );
        }
    }
}