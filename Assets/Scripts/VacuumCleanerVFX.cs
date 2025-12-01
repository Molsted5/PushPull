using System.Collections;
using UnityEngine;

public class VacuumCleanerVFX: MonoBehaviour {
    public GameObject effect;

    void Awake() {
        effect.SetActive( false );    
    }

    public void StartEffect( float strength ) {
        effect.SetActive( true );
    }

    public void StopEffect() {
        effect.SetActive( false );
    }

}