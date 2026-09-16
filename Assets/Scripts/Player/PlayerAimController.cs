using System.Drawing;
using UnityEngine;

[RequireComponent( typeof( PlayerInputHandler ) )]
public class PlayerAimController: MonoBehaviour {
    public float maxTurnSpeed = 7.5f;
    public float deadZone = 0.18f;

    Vector3 direction;
    Vector3 heightCorrectedPoint;
    bool usingStick;
    PlayerInputHandler input;

    void Awake() {
        input = GetComponent<PlayerInputHandler>();
    }

    void Update() {
        if( !usingStick ) {
            OnMouseLook();
            RotateTowardMouse();
        }
        usingStick = false;
    }

    void OnMouseLook() {
        Vector2 screenPos = input.MouseLookValue;
        Ray ray = Camera.main.ScreenPointToRay( screenPos );
        Plane groundPlane = new Plane( Vector3.up, transform.position );
        if( groundPlane.Raycast( ray, out float distance ) ) {
            Vector3 point = ray.GetPoint( distance );
            heightCorrectedPoint = new Vector3( point.x, transform.position.y, point.z );
        }
    }

    void RotateTowardMouse() {
        direction = heightCorrectedPoint - transform.position;
        direction.y = 0f;
        direction = direction.normalized;
        if( direction != Vector3.zero ) {
            transform.forward = direction;
        }
    }

    void OnStickLook( Vector3 velocity ) {
        usingStick = true;
        Vector3 flatDir = new Vector3( velocity.x, 0f, velocity.z ).normalized;
        float inputStrength = Mathf.Max( velocity.magnitude - deadZone, 0f );
        float turnSpeed = Mathf.Min( inputStrength * maxTurnSpeed, maxTurnSpeed );

        transform.forward = Vector3.RotateTowards( transform.forward, flatDir, turnSpeed * Time.deltaTime, 0f );
    }


}

