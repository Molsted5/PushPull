using UnityEngine;

[RequireComponent( typeof( PlayerInputHandler ) )]
public class PlayerMovementController: MonoBehaviour {
    public float moveSpeed = 5f;
    public float collisionRadius = 1f;

    public PlayerData PlayerData;

    public enum MovementState {
        Idle,
        Moving
    }

    MovementState movementState = MovementState.Idle;
    MovementState previousMovementState = MovementState.Idle;

    Vector3 velocity;
    PlayerInputHandler inputHandler;

    void Awake() {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void OnEnable() {
        inputHandler.OnMovePerformed += HandleMoveInput;
        inputHandler.OnMoveCanceled += StopMovement;
    }

    void OnDisable() {
        inputHandler.OnMovePerformed -= HandleMoveInput;
        inputHandler.OnMoveCanceled -= StopMovement;
    }

    void Update() {
        PlayerData.position = transform.position; // references position before moving
        if( movementState == MovementState.Moving ) {
            transform.position += velocity * Time.deltaTime;
        }
    }

    public void HandleMoveInput( Vector2 input ) {
        Vector3 velocity = new Vector3( input.x, 0f, input.y ) * moveSpeed;
        SetVelocity( velocity );
    }

    public void StartMovement(Vector3 newVelocity) {
        velocity = Vector3.zero;
        DecideMovementState();
    }

    public void SetVelocity( Vector3 newVelocity ) {
        velocity = newVelocity;
        DecideMovementState();
    }

    public void StopMovement() {
        velocity = Vector3.zero;
        DecideMovementState();
    }

    void DecideMovementState() {
        MovementState newState = velocity.sqrMagnitude > 0.01f ? MovementState.Moving : MovementState.Idle;
        if( newState == movementState ) return;

        ExitMovementState( movementState );
        EnterMovementState( newState );
        previousMovementState = movementState;
        movementState = newState;
    }

    // is this helpfull?
    void EnterMovementState( MovementState newState ) {
        switch( newState ) {
            case MovementState.Moving:
                Debug.Log( "Started moving" );
                break;
            case MovementState.Idle:
                Debug.Log( "Idle" );
                break;
        }
    }

    void ExitMovementState( MovementState oldState ) {
        if( oldState == MovementState.Moving ) {
            Debug.Log( "Stopped moving" );
        }
    }
}