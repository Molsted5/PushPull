using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumCleaner: MonoBehaviour {
    [Header( "Vacuum Settings" )]
    public float forceMagnitude = 10f;
    public float forceDuration = 0.5f;
    public float vacuumRadius = 5f;
    public float vacuumLength = 2f;
    public float coneAngle = 45f;
    public float effectCooldown = 0.1f;
    public LayerMask affectedLayers;

    public Transform forceOrigin;

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
    }

    IEnumerator PushCoroutine( Transform origin ) {
        while( true ) {
            ApplyForce( origin.position, origin.forward );
            //yield return new WaitForSeconds( effectCooldown );
            yield return null;
        }
    }

    IEnumerator PullCoroutine( Transform origin ) {
        while( true ) {
            ApplyForce( origin.position, -origin.forward );
            //yield return new WaitForSeconds( effectCooldown );
            yield return null;
        }
    }

    void ApplyForce( Vector3 origin, Vector3 direction ) {
        Vector3 forceDir = direction.normalized;
        Vector3 halfExtents = new Vector3( vacuumRadius, vacuumRadius, 0.2f );
        RaycastHit[] hits = Physics.BoxCastAll( origin, halfExtents, forceDir, transform.rotation, vacuumLength, affectedLayers );
        //Debug.DrawRay( origin, forceDir * vacuumLength, Color.red );

        foreach( RaycastHit hit in hits ) {
            Vector3 dirToTarget = ( hit.transform.position - origin ).normalized;
            float angleToTarget = Vector3.Angle( forceDir, dirToTarget );

            Enemy enemy = hit.transform.GetComponent<Enemy>();

            if( enemy != null ) {
                enemy.TakeForce( forceDir * forceMagnitude );
            }

            //if( angleToTarget <= coneAngle / 2f ) {
            //    Enemy enemy = hit.transform.GetComponent<Enemy>();

            //    if( enemy != null ) {
            //        enemy.TakeForce( forceDir, forceStrength, forceDuration );
            //    }
            //}
        }
    }

}