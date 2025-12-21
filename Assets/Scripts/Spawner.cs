using System;
using System.Collections;
using UnityEngine;

public class Spawner: MonoBehaviour {
    public Enemy enemy;

    float moveSpeed = 1.8f; float damage = 1f; float attackDistanceTreshold = 0.5f; float timeBetweenAttacks = 1f; float health = 5f; Color color = Color.white;
    LivingEntity playerEntity;
    Coroutine spawnCoroutine;

    void Awake() {
        //new Wave(true, -1, 1.8f, 2.2f, 5, 1f, Color.white)
        playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<LivingEntity>();
    }

    void OnEnable() {
        playerEntity.OnDeath += OnPlayerDeath;  
    }

    void OnDisable() {
        playerEntity.OnDeath -= OnPlayerDeath;
    }

    void Start() {
        if( spawnCoroutine != null ) {
            StopCoroutine( spawnCoroutine );
        }
        spawnCoroutine = StartCoroutine( SpawnCoroutine() );
    }

    IEnumerator SpawnCoroutine() {
        yield return new WaitForSeconds( 1f );

        yield return SpawnEnemyCoroutine( 10, 1f );

        yield return new WaitForSeconds( 1f );

        spawnCoroutine = null;
    }

    IEnumerator SpawnEnemyCoroutine( int amount, float timeBetweenSpawns ) {
        for( int i = 0; i < amount; i++ ) {
            Enemy spawnedEnemy = Instantiate( enemy, new Vector3( 0, 0.5f, 0 ), Quaternion.identity ) as Enemy;
            spawnedEnemy.SetCharacteristics( moveSpeed, damage, attackDistanceTreshold, timeBetweenAttacks, health, color );
            // wait except after last spawn
            if( i < amount - 1 ) {
                yield return new WaitForSeconds( timeBetweenSpawns );
            }
        }
    }

    void OnPlayerDeath( ) {
        if( spawnCoroutine != null ) {
            StopCoroutine( spawnCoroutine );
        }
    }

}
