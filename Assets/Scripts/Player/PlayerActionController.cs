using System;
using UnityEngine;
using UnityEngine.Windows;

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

    enum InputType { 
        None, 
        Push,
        Pull,
        Reload
    }

    enum InputPhase {
        None,
        Started, 
        Performed, 
        Canceled
    }

    ActionState actionState = ActionState.Idle;
    ActionState previousActionState = ActionState.Idle;
    InputType inputType;
    InputPhase inputPhase;

    float pushValue;
    float pullValue;
    float reloadValue;

    PlayerInputHandler inputHandler;

    void Awake() {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void OnEnable() {
        inputHandler.OnPushStarted += OnPushStarted;
        inputHandler.OnPushPerformed += OnPushPerformed;
        inputHandler.OnPushCanceled += OnPushCanceled;

        inputHandler.OnPullStarted += OnPullStarted;
        inputHandler.OnPullPerformed += OnPullPerformed;
        inputHandler.OnPullCanceled += OnPullCanceled;

        inputHandler.OnReloadStarted += OnReloadStarted;
        inputHandler.OnReloadCanceled += OnReloadCanceled;
    }

    void OnDisable() {
        inputHandler.OnPushStarted -= OnPushStarted;
        inputHandler.OnPushPerformed -= OnPushPerformed;
        inputHandler.OnPushCanceled -= OnPushCanceled;

        inputHandler.OnPullStarted -= OnPullStarted;
        inputHandler.OnPullPerformed -= OnPullPerformed;
        inputHandler.OnPullCanceled -= OnPullCanceled;

        inputHandler.OnReloadStarted -= OnReloadStarted;
        inputHandler.OnReloadCanceled -= OnReloadCanceled;
    }

    void Update() {
        DecideActionState();
    }

    // wrapper because of action<float> signature from input script
    void OnPushStarted( float value ) { HandlePushInput( InputPhase.Started, value ); }
    void OnPushPerformed( float value ) { HandlePushInput( InputPhase.Performed, value ); }
    void OnPushCanceled() { HandlePushInput( InputPhase.Canceled, 0 ); }

    void OnPullStarted(float value) { HandlePullInput( InputPhase.Started, value ); } 
    void OnPullPerformed(float value) { HandlePullInput( InputPhase.Performed, value ); }
    void OnPullCanceled() { HandlePullInput( InputPhase.Canceled, 0 ); }

    void OnReloadStarted() { HandleReloadInput( InputPhase.Started, 1f ); }
    void OnReloadCanceled() { HandleReloadInput( InputPhase.Canceled, 0 ); }

    void HandlePushInput( InputPhase phase, float value ) {
        inputType = InputType.Push;
        inputPhase = phase;
        pushValue = value;
    }

    void HandlePullInput( InputPhase phase, float value ) {
        inputType = InputType.Pull;
        inputPhase = phase;
        pullValue = value;
    }

    void HandleReloadInput( InputPhase phase, float value ) {
        inputType = InputType.Reload;
        inputPhase = phase;
        reloadValue = value;
    }

    void DecideActionState() {
        switch( actionState ) {
            case ActionState.Pushing:
                if( inputType == InputType.Push ) {
                    if( inputPhase == InputPhase.Canceled ) {
                        // stop pushing
                        // stop vfx
                    }
                    else if( inputPhase == InputPhase.Performed ) {
                        // update push
                        // perhaps update vfx
                    }
                }
                if( inputType == InputType.Pull ) {
                    // and so on
                }
                break;
            case ActionState.Pulling:

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