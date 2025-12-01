using System.Collections;
using UnityEngine;

public class VacuumCleanerVFX: MonoBehaviour {
    public GameObject effect;

    void Awake() {
        effect.SetActive( false );    
    }

    public void StartEffect( float length, float halfWidth ) {
        effect.transform.localScale = new Vector3( halfWidth / 5f, 1f, length / 10f );
        effect.transform.localPosition = new Vector3( 0, 0, length / 2f );
        effect.SetActive( true );
    }

    public void StopEffect() {
        effect.SetActive( false );
    }

}