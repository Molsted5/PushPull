using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumCleaner: MonoBehaviour {
    [Header( "Vacuum Settings" )]
    public float forceStrength = 10f;
    public float forceDuration = 0.5f;
    public float vacuumRadius = 5f;
    public float vacuumLength = 2f;
    public float coneAngle = 45f;
    public float effectCooldown = 0.1f;
    public LayerMask affectedLayers;

    Coroutine pushCoroutine;
    Coroutine pullCoroutine;

    public void StartPush( Transform origin ) {
        StopPull();
        StopPush();

        pushCoroutine = StartCoroutine( PushCoroutine( origin ) );
    }

    public void StopPush() {
        if( pushCoroutine != null ) {
            StopCoroutine( pushCoroutine );
            pushCoroutine = null;
        }

        // Stop push effects here
    }

    public void StartPull( Transform origin ) {
        StopPush();
        StopPull();

        pullCoroutine = StartCoroutine( PullCoroutine( origin ) );
    }

    public void StopPull() {
        if( pullCoroutine != null ) {
            StopCoroutine( pullCoroutine );
            pullCoroutine = null;
        }

        // Stop pull effects here
    }

    IEnumerator PushCoroutine( Transform origin ) {
        while( true ) {
            ApplyForce( origin.position, origin.forward );
            
            // Play push VFX/audio here
            
            yield return new WaitForSeconds( effectCooldown );
        }
    }

    IEnumerator PullCoroutine( Transform origin ) {
        while( true ) {
            ApplyForce( origin.position, -origin.forward );

            // Play pull VFX/audio here

            yield return new WaitForSeconds( effectCooldown );
        }
    }

    void ApplyForce( Vector3 origin, Vector3 direction ) {
        Vector3 forceDir = direction.normalized;
        RaycastHit[] hits = Physics.SphereCastAll( origin, vacuumRadius, forceDir, vacuumRadius * vacuumLength, affectedLayers );
        //Debug.DrawRay( origin, direction.normalized * vacuumRadius * vacuumLength, Color.red );

        foreach( RaycastHit hit in hits ) {
            Vector3 dirToTarget = hit.transform.position - origin;
            float angleToTarget = Vector3.Angle( forceDir, dirToTarget );

            if( angleToTarget <= coneAngle / 2f ) {
                Enemy pushableEnemy = hit.transform.GetComponent<Enemy>();

                if( pushableEnemy != null ) {
                    pushableEnemy.TakeForce( forceDir, forceStrength, forceDuration );
                }
            }
        }
    }

}