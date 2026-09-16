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

    ActionState actionState = ActionState.Idle;
    //ActionState previousActionState = ActionState.Idle;

    PlayerInputHandler input;

    void Awake() {
        input = GetComponent<PlayerInputHandler>();
    }

    void Update() {
        DecideActionState();
    }

    void DecideActionState() {
        switch( actionState ) {
            case ActionState.Pushing:
                // add here if something should override the rest like a shoot for the next 10 seconds powerup
                if( input.LatestGunInputTypeState == LatestGunInputType.Push && input.LatestGunInputPhazeState == LatestGunInputPhaze.Canceled ) {
                    ExitActionState( actionState );
                    actionState = ActionState.Idle;
                    if( input.pullValue != 0 ) {
                        // maybe add ammo check
                        actionState = ActionState.Pulling;
                    }
                    else if( input.reloadValue != 0 ) {
                        // maybe add ammo check
                        actionState = ActionState.Reloading;
                    }
                    EnterActionState( actionState );
                }
                else if( input.LatestGunInputTypeState == LatestGunInputType.Pull && input.LatestGunInputPhazeState == LatestGunInputPhaze.Performed ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pulling;
                    EnterActionState( actionState );
                }
                else if( input.LatestGunInputTypeState == LatestGunInputType.Reload && input.LatestGunInputPhazeState == LatestGunInputPhaze.Performed ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Reloading;
                    EnterActionState( actionState );
                }
                break;
            case ActionState.Pulling:
                // add here if something should override the rest like a shoot for the next 10 seconds powerup

                if( input.LatestGunInputTypeState == LatestGunInputType.Pull && input.LatestGunInputPhazeState == LatestGunInputPhaze.Canceled ) {
                    ExitActionState( actionState );
                    actionState = ActionState.Idle;
                    if( input.pushValue != 0 ) {
                        // maybe add ammo check
                        actionState = ActionState.Pushing;
                    }
                    else if( input.reloadValue != 0 ) {
                        // maybe add ammo check
                        actionState = ActionState.Reloading;
                    }
                    EnterActionState( actionState );
                }
                else if( input.LatestGunInputTypeState == LatestGunInputType.Push && input.LatestGunInputPhazeState == LatestGunInputPhaze.Performed ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pushing;
                    EnterActionState( actionState );
                }
                else if( input.LatestGunInputTypeState == LatestGunInputType.Reload && input.LatestGunInputPhazeState == LatestGunInputPhaze.Performed ) {
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
                if( input.LatestGunInputTypeState == LatestGunInputType.Push && input.LatestGunInputPhazeState == LatestGunInputPhaze.Performed ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pushing;
                    EnterActionState( actionState );
                }
                else if( input.LatestGunInputTypeState == LatestGunInputType.Pull && input.LatestGunInputPhazeState == LatestGunInputPhaze.Performed) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pulling;
                    EnterActionState( actionState );
                }
                break;
            case ActionState.Idle:
                // add here if something should override the rest like a shoot for the next 10 seconds powerup

                if( input.LatestGunInputTypeState == LatestGunInputType.Push && input.LatestGunInputPhazeState == LatestGunInputPhaze.Performed ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pushing;
                    EnterActionState( actionState );
                }
                else if( input.LatestGunInputTypeState == LatestGunInputType.Pull && input.LatestGunInputPhazeState == LatestGunInputPhaze.Performed ) {
                    ExitActionState( actionState );
                    // maybe add ammo check
                    actionState = ActionState.Pulling;
                    EnterActionState( actionState );
                }
                else if( input.LatestGunInputTypeState == LatestGunInputType.Reload && input.LatestGunInputPhazeState == LatestGunInputPhaze.Performed ) {
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