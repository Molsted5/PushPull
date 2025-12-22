using UnityEngine;

[RequireComponent( typeof( PlayerInputHandler ) )]
public class PlayerActionController: MonoBehaviour {
    public VacuumCleaner vacuumCleaner;
    public VacuumCleanerVFX vacuumVFX;

    public enum ActionState {
        None,
        Pushing,
        Pulling,
        Reloading
    }

    ActionState actionState = ActionState.None;
    ActionState previousActionState = ActionState.None;
    ActionState ActionStateInQuestion = ActionState.None;

    bool isPushingHeld = false;
    bool isPullingHeld = false;
    bool isReloadingHeld = false;

    PlayerInputHandler inputHandler;

    void Awake() {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void OnEnable() {
        inputHandler.OnPushStarted += OnPushStarted;
        inputHandler.OnPushCanceled += OnPushCanceled;

        inputHandler.OnPullStarted += OnPullStarted;
        inputHandler.OnPullCanceled += OnPullCanceled;

        inputHandler.OnReloadStarted += OnReloadStarted;
        inputHandler.OnReloadCanceled += OnReloadCanceled;
    }

    void OnDisable() {
        inputHandler.OnPushStarted -= OnPushStarted;
        inputHandler.OnPushCanceled -= OnPushCanceled;

        inputHandler.OnPullStarted -= OnPullStarted;
        inputHandler.OnPullCanceled -= OnPullCanceled;

        inputHandler.OnReloadStarted -= OnReloadStarted;
        inputHandler.OnReloadCanceled -= OnReloadCanceled;
    }

    public void OnPushStarted() {
        ActionStateInQuestion = ActionState.Pushing;
        DecideActionState();
    }

    public void OnPushCanceled() {
        ActionStateInQuestion = ActionState.None;
        DecideActionState();
    }

    public void OnPullStarted() {
        ActionStateInQuestion = ActionState.Pulling;
        DecideActionState();
    }

    public void OnPullCanceled() {
        ActionStateInQuestion = ActionState.None;
        DecideActionState();
    }

    public void OnReloadStarted() {
        ActionStateInQuestion = ActionState.Reloading;
        DecideActionState();
    }

    public void OnReloadCanceled() {
        ActionStateInQuestion = ActionState.None;
        DecideActionState();
    }

    void DecideActionState() {
        switch( ActionStateInQuestion ) {
            case ActionState.Pushing:
                if( actionState == ActionState.Reloading ) {
                    print( "Can't push.. is reloading" );
                    break;
                }
                // enter pushing state and prev and state value
                break;
            case ActionState.Pulling:
                break;
            case ActionState.Reloading:
                break;
            case ActionState.None:
                break;
        }
    }

    // play VFX/audio here
    void EnterActionState( ActionState newState ) {
        switch( newState ) {
            case ActionState.Pushing:
                Debug.Log( "Started pushing" );
                vacuumCleaner.StartPush( vacuumCleaner.forceOrigin );
                vacuumVFX.StartEffect( vacuumCleaner.vacuumLength, vacuumCleaner.vacuumRadius);
                break;
            case ActionState.Pulling:
                Debug.Log( "Started pulling" );
                vacuumCleaner.StartPull( vacuumCleaner.forceOrigin );
                vacuumVFX.StartEffect( vacuumCleaner.vacuumLength, vacuumCleaner.vacuumRadius );
                break;
            case ActionState.Reloading:
                Debug.Log( "Started reloading" );
                vacuumCleaner.StartReload();
                break;
            case ActionState.None:
                Debug.Log( "No action started" );
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
            case ActionState.Reloading:
                Debug.Log( "Stopped reloading" );
                vacuumCleaner.StopReload();
                break;
            case ActionState.None:
                Debug.Log( "No action stopped" );
                break;
        }
    }
}