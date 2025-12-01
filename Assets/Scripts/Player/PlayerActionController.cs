using UnityEngine;

[RequireComponent( typeof( PlayerInputHandler ) )]
public class PlayerActionController: MonoBehaviour {
    public VacuumCleaner vacuumCleaner;
    public VacuumCleanerVFX vacuumVFX;

    public enum ActionState {
        None,
        Pushing,
        Pulling
    }

    ActionState actionState = ActionState.None;
    ActionState previousActionState = ActionState.None;
    ActionState lastActionState = ActionState.None;

    bool isPushingHeld = false;
    bool isPullingHeld = false;

    PlayerInputHandler inputHandler;

    void Awake() {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void OnEnable() {
        inputHandler.OnPushStarted += OnPushStarted;
        inputHandler.OnPushCanceled += OnPushCanceled;

        inputHandler.OnPullStarted += OnPullStarted;
        inputHandler.OnPullCanceled += OnPullCanceled;
    }

    void OnDisable() {
        inputHandler.OnPushStarted -= OnPushStarted;
        inputHandler.OnPushCanceled -= OnPushCanceled;

        inputHandler.OnPullStarted -= OnPullStarted;
        inputHandler.OnPullCanceled -= OnPullCanceled;
    }

    public void OnPushStarted() {
        isPushingHeld = true;
        lastActionState = ActionState.Pushing;
        DecideActionState();
    }

    public void OnPushCanceled() {
        isPushingHeld = false;
        DecideActionState();
    }

    public void OnPullStarted() {
        isPullingHeld = true;
        lastActionState = ActionState.Pulling;
        DecideActionState();
    }

    public void OnPullCanceled() {
        isPullingHeld = false;
        DecideActionState();
    }

    void DecideActionState() {
        ActionState newState;

        if( isPushingHeld && isPullingHeld ) {
            newState = lastActionState;
        }
        else if( isPushingHeld ) {
            newState = ActionState.Pushing;
        }
        else if( isPullingHeld ) {
            newState = ActionState.Pulling;
        }
        else {
            newState = ActionState.None;
        }

        if( newState == actionState ) return;

        ExitActionState( actionState );
        EnterActionState( newState );
        previousActionState = actionState;
        actionState = newState;
    }

    // play VFX/audio here
    void EnterActionState( ActionState newState ) {
        switch( newState ) {
            case ActionState.Pushing:
                Debug.Log( "Started pushing" );
                vacuumCleaner.StartPush( vacuumCleaner.forceOrigin );
                vacuumVFX.StartEffect( vacuumCleaner.forceStrength );
                break;
            case ActionState.Pulling:
                Debug.Log( "Started pulling" );
                vacuumCleaner.StartPull( vacuumCleaner.forceOrigin );
                vacuumVFX.StartEffect( vacuumCleaner.forceStrength );
                break;
            case ActionState.None:
                Debug.Log( "No action" );
                break;
        }
    }

    void ExitActionState( ActionState oldState ) {
        switch( oldState ) {
            case ActionState.Pushing:
                Debug.Log( "Stopped pushing" );
                vacuumCleaner.StopPush();
                vacuumVFX.StopEffect();
                break;
            case ActionState.Pulling:
                Debug.Log( "Stopped pulling" );
                vacuumCleaner.StopPull();
                vacuumVFX.StopEffect();
                break;
        }
    }
}