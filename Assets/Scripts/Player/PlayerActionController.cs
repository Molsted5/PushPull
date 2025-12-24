using UnityEngine;

[RequireComponent( typeof( PlayerInputHandler ) )]
public class PlayerActionController: MonoBehaviour {
    public VacuumCleaner vacuumCleaner;
    public VacuumCleanerVFX vacuumVFX;

    public enum ActionState {
        Idle,
        Pushing,
        Pulling,
        Reloading
    }

    ActionState actionState = ActionState.Idle;
    ActionState previousActionState = ActionState.Idle;

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

    // can be extended with queues and logic if the order of inputs becomes relevant. Maybe i should go back to calling decideActionState method from here. Should input queue be handled in input class? I really dont know if update should run state or states should happen right away to events or if they should do both that and then only prepare for a state changed which happens in update.. 
    public void OnPushStarted() { isPushingHeld = true; }
    public void OnPushCanceled() { isPushingHeld = false; }

    public void OnPullStarted() { isPullingHeld = true; }
    public void OnPullCanceled() { isPullingHeld = false; }

    public void OnReloadStarted() { isReloadingHeld = true; }
    public void OnReloadCanceled() { isReloadingHeld = false; }

    void DecideActionState() {
        switch( actionState ) {
            case ActionState.Pushing:
                if( actionState == ActionState.Reloading ) {
                    print( "Can't push.. is reloading" );
                    break;
                }
                previousActionState = actionState;
                actionState = ActionState.Pushing;
                ExitActionState( previousActionState );
                EnterActionState( actionState );
                break;
            case ActionState.Pulling:
                if( actionState == ActionState.Reloading ) {
                    print( "Can't pull.. is reloading" );
                    break;
                }
                previousActionState = actionState;
                actionState = ActionState.Pulling;
                ExitActionState( previousActionState );
                EnterActionState( actionState );
                break;
            case ActionState.Reloading:
                if( actionState == ActionState.Reloading ) {
                    print( "Can't reload.. is already reloading" );
                    break;
                }
                previousActionState = actionState;
                actionState = ActionState.Pulling;
                ExitActionState( previousActionState );
                EnterActionState( actionState );
                break;
            case ActionState.Idle:
                if( actionState == ActionState.Reloading ) {
                    print( "Can't go Idle.. is reloading" );
                    break;
                }
                else if( previousActionState != ActionState.Idle && previousActionState != ActionState.Reloading ) {

                }

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
            case ActionState.Idle:
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
            case ActionState.Idle:
                Debug.Log( "No action stopped" );
                break;
        }
    }
}