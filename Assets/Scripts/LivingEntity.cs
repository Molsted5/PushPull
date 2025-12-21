using System;
using UnityEngine;

public class LivingEntity: MonoBehaviour {
    public float startingHealth;
    public float health { get; set; }
    public bool dead;

    public event Action OnDeath;

    public PlayerData PlayerData;

    void Start() {
        health = startingHealth;
    }

    public void TakeHit( float damage, Vector3 hitPoint, Vector3 hitDirection ) {
        // do some stuff here with hit variable
        TakeDamage( damage );
    }

    public void TakeDamage( float damage ) {
        health -= damage;
        print( "damage " + damage );
        print( health );

        if( health <= 0 && !dead ) {
            Die();
        }
    }

    [ContextMenu( "Self Destruct" )]
    public void Die() {
        //PlayerData.position = transform.position;
        dead = true;
        OnDeath?.Invoke();
        GameObject.Destroy( gameObject );
    }
}