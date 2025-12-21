using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner: MonoBehaviour {
    public Enemy enemy;
    List<Transform> availableSpawns = new List<Transform>();
    float moveSpeed = 1.8f; float damage = 1f; float attackDistanceTreshold = 0.5f; float timeBetweenAttacks = 1f; float health = 5f; Color color = Color.white;
    LivingEntity playerEntity;
    Coroutine spawnCoroutine;

    void Awake() {
        playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<LivingEntity>();
        foreach( Transform child in transform ) {
            if( child.name.StartsWith( "Spawn Point" ) ) {
                availableSpawns.Add( child );
            }
        }
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

        yield return SpawnEnemiesCoroutine( 1 );

        yield return new WaitForSeconds( 2f );

        yield return SpawnEnemiesCoroutine( 2, 1.5f );

        yield return new WaitForSeconds( 1.5f );

        yield return SpawnEnemiesCoroutine( 3, 2f );

        yield return new WaitForSeconds( 2f );

        yield return SpawnEnemiesCoroutine( 5, 1.4f );

        yield return new WaitForSeconds( 2f );

        yield return SpawnEnemiesCoroutine( 10, 1.4f );

        yield return new WaitForSeconds( 4f );

        yield return SpawnEnemiesCoroutine( 20, 1.7f );

        spawnCoroutine = null;
    }

    IEnumerator SpawnEnemiesCoroutine( int amount, float timeBetweenSpawns = 1f, float spawnPointTimeout = 0.9f ) {
        for( int i = 0; i < amount; i++ ) {
            Transform spawnPoint = RandomSpawnPoint( spawnPointTimeout );
            Enemy spawnedEnemy = Instantiate( enemy, spawnPoint.position, spawnPoint.rotation ) as Enemy;
            spawnedEnemy.SetCharacteristics( moveSpeed, damage, attackDistanceTreshold, timeBetweenAttacks, health, color );
            // wait after each spawn except for the last one
            if( i < amount - 1 ) {
                yield return new WaitForSeconds( timeBetweenSpawns );
            }
        }
    }

    Transform RandomSpawnPoint( float spawnPointTimeout ) {
        int index = UnityEngine.Random.Range( 0, availableSpawns.Count );
        Transform spawnPoint = availableSpawns[index];
        availableSpawns.RemoveAt( index );
        StartCoroutine( ReturnSpawnAfterDelay( spawnPoint, spawnPointTimeout ) );
        return spawnPoint;
    }

    IEnumerator ReturnSpawnAfterDelay( Transform spawnPoint, float spawnPointTimeout ) {
        yield return new WaitForSeconds( spawnPointTimeout );
        availableSpawns.Add( spawnPoint );
    }

    void OnPlayerDeath( ) {
        if( spawnCoroutine != null ) {
            StopCoroutine( spawnCoroutine );
        }
    }

}
