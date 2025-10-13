using UnityEngine;

public class VacuumCleanerVFX: MonoBehaviour {
    [SerializeField] LineRenderer coneRenderer;

    Transform originTransform;
    float length;
    bool isActive = false;

    public void ShowLine( Transform origin, float len ) {
        originTransform = origin;
        length = len;
        isActive = true;

        coneRenderer.enabled = true;
        coneRenderer.positionCount = 2;
    }

    public void HideLine() {
        isActive = false;
        coneRenderer.enabled = false;
    }

    void Update() {
        if( !isActive || originTransform == null ) return;

        Vector3 origin = originTransform.position;
        Vector3 direction = originTransform.forward; 
        coneRenderer.SetPosition( 0, origin );
        coneRenderer.SetPosition( 1, origin + direction * length );
    }
}