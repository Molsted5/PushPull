using System;
using Unity.VisualScripting;
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
    //ActionState previousActionState = ActionState.Idle;
    InputType inputType;
    InputPhase inputPhase;

    bool pushInputStarted;
    bool pullInputStarted;

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
        pushInputStarted = false;
        pullInputStarted = false;
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
        if( pushInputStarted && phase == InputPhase.Performed ) {
            return;
        }
        if( phase == InputPhase.Started ) {
            pushInputStarted = true;
        }
        inputType = InputType.Push;
        inputPhase = phase;
        pushValue = value;
    }

    void HandlePullInput( InputPhase phase, float value ) {
        if( pullInputStarted && phase == InputPhase.Performed ) {
            return;
        }
        if( phase == InputPhase.Started ) {
            pullInputStarted = true;
        }
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
                // add here if something should override the rest like a shoot for the next 10 seconds powerup

                if( inputType == InputType.Push && inputPhase == InputPhase.Canceled ) {
                    ExitActionState( actionState );
                    actionState = ActionState.Idle;
                    if( pullValue != 0 ) {
                        // maybe add ammo check
                        actionState = ActionState.Pulling;
                    }
                    else if( reloadValue != 0 ) {
                        // maybe add ammo check
                        actionState = ActionState.Reloading;
                    }
                    EnterActionState( actionState );
                }
                else if( inputType == InputType.Pull && inputPhase == InputPhase.Started ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pulling;
                    EnterActionState( actionState );
                }
                else if( inputType == InputType.Reload && inputPhase == InputPhase.Started ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Reloading;
                    EnterActionState( actionState );
                }
                break;
            case ActionState.Pulling:
                // add here if something should override the rest like a shoot for the next 10 seconds powerup

                if( inputType == InputType.Pull && inputPhase == InputPhase.Canceled ) {
                    ExitActionState( actionState );
                    actionState = ActionState.Idle;
                    if( pushValue != 0 ) {
                        // maybe add ammo check
                        actionState = ActionState.Pushing;
                    }
                    else if( reloadValue != 0 ) {
                        // maybe add ammo check
                        actionState = ActionState.Reloading;
                    }
                    EnterActionState( actionState );
                }
                else if( inputType == InputType.Push && inputPhase == InputPhase.Started ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pushing;
                    EnterActionState( actionState );
                }
                else if( inputType == InputType.Reload && inputPhase == InputPhase.Started ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Reloading;
                    EnterActionState( actionState );
                }
                break;
            case ActionState.Reloading:
                // add here if something should override the rest like a shoot for the next 10 seconds powerup

                // subscribe to reload over event from the gun and set isReloading value in this script
                // if isReloading == false then exit action state and check other inputValues for new state

                // this is temporary because above should be enough when implemented
                if( inputType == InputType.Push && inputPhase == InputPhase.Started ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pushing;
                    EnterActionState( actionState );
                }
                else if( inputType == InputType.Pull && inputPhase == InputPhase.Started ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pulling;
                    EnterActionState( actionState );
                }
                break;
            case ActionState.Idle:
                // add here if something should override the rest like a shoot for the next 10 seconds powerup

                if( inputType == InputType.Push && inputPhase == InputPhase.Started ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pushing;
                    EnterActionState( actionState );
                }
                else if( inputType == InputType.Pull && inputPhase == InputPhase.Started ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pulling;
                    EnterActionState( actionState );
                }
                else if( inputType == InputType.Reload && inputPhase == InputPhase.Started ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Reloading;
                    EnterActionState( actionState );
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
                //vacuumCleaner.StartReload();
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
                //vacuumCleaner.StopReload();
                break;
            case ActionState.Idle:
                Debug.Log( "No action stopped" );
                break;
        }
    }
}