using UnityEngine;

[RequireComponent( typeof( PlayerInputHandler ) )]
public class PlayerAimController: MonoBehaviour {
    public float maxTurnSpeed = 7.5f;
    public float deadZone = 0.18f;

    Vector3 aimDirection;
    PlayerInputHandler inputHandler;

    void Awake() {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void OnEnable() {
        inputHandler.OnMouseLook += OnMouseLook;
        inputHandler.OnStickLook += OnStickLook;
    }

    void OnDisable() {
        inputHandler.OnMouseLook -= OnMouseLook;
        inputHandler.OnStickLook -= OnStickLook;
    }

    void Update() {
        RotateTowardMouse(); // only for mouse movement
    }

    void OnMouseLook( Vector2 screenPos ) {
        Ray ray = Camera.main.ScreenPointToRay( screenPos );
        Plane groundPlane = new Plane( Vector3.up, transform.position.y );
        if( groundPlane.Raycast( ray, out float distance ) ) {
            Vector3 point = ray.GetPoint( distance );
            Vector3 direction = (point - transform.position);
            direction.y = 0f;
            //Debug.DrawLine( transform.position, transform.position + aimDirection * 1.8f, Color.red );
            aimDirection = direction.normalized;
        }
    }

    void OnStickLook( Vector3 velocity ) {
        Vector3 flatDir = new Vector3( velocity.x, 0f, velocity.z ).normalized;
        float inputStrength = Mathf.Max( velocity.magnitude - deadZone, 0f );
        float turnSpeed = Mathf.Min( inputStrength * maxTurnSpeed, maxTurnSpeed );

        transform.forward = Vector3.RotateTowards( transform.forward, flatDir, turnSpeed * Time.deltaTime, 0f );
    }

    void RotateTowardMouse() {
        Vector3 flatDir = new Vector3( aimDirection.x, 0f, aimDirection.z );
        if( flatDir != Vector3.zero ) {
            transform.forward = flatDir;
        }
    }
}

