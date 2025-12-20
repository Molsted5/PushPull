using System;
using System.Collections;
using UnityEngine;

public class Spawner: MonoBehaviour {
    public bool devMode;

    public class Wave {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;

        public Wave(bool infinite, int enemyCount, float timeBetweenSpawns, float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor) {
            this.infinite = infinite;
            this.enemyCount = enemyCount;
            this.timeBetweenSpawns = timeBetweenSpawns;

            this.moveSpeed = moveSpeed;
            this.hitsToKillPlayer = hitsToKillPlayer;
            this.enemyHealth = enemyHealth;
            this.skinColor = skinColor;
        }
    }

    public Wave[] waves;

    public Enemy enemy;

    public event Action<int> OnNewWave;

    public PlayerData PlayerData;
    
    LivingEntity playerEntity;
    Transform playerTransform;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    //MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campTresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;
    bool isDisabled;

    Coroutine spawnEnemyCoroutine;

    void Awake() {
        waves = new Wave[] {
            new Wave(true, -1, 1.8f, 2.2f, 5, 1f, Color.white)
        };
    }

    void Start() {
        playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<LivingEntity>();
        playerTransform = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerTransform.position;
        playerEntity.OnDeath += OnPlayerDeath;

        //map = FindAnyObjectByType<MapGenerator>();
        NextWave();
    }

    void Update() {
        if( !isDisabled ) {
            if( Time.time > nextCampCheckTime ) {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = Vector3.Distance( playerTransform.position, campPositionOld ) < campTresholdDistance;
                campPositionOld = playerTransform.position;
            }

            if( (enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime ) {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                spawnEnemyCoroutine = StartCoroutine( SpawnEnemy() );
            }
        }

        if( devMode ) {
            if( Input.GetKeyDown( KeyCode.Return ) ) {
                if( spawnEnemyCoroutine != null ) {
                    StopCoroutine( spawnEnemyCoroutine );
                    spawnEnemyCoroutine = null;
                }

                foreach( Enemy enemy in FindObjectsByType<Enemy>( FindObjectsSortMode.None ) ) {
                    GameObject.Destroy( enemy.gameObject );
                }
                NextWave();
            }

        }
    }

    IEnumerator SpawnEnemy() {
        float spawnDelay = 1;
        //float tileFlashSpeed = 4;

        //Transform spawnTile = map.GetRandomOpenTile();
        //if( isCamping ) {
        //    spawnTile = map.GetTileFromPosition( playerTransform.position );
        //}
        //Material tileMat = spawnTile.GetComponent<Renderer>().material;
        //Color initialColor = map.tilePrefab.GetComponent<Renderer>().sharedMaterial.color;
        //Color flashColor = Color.red;
        float spawnTimer = 0;

        while( spawnTimer < spawnDelay ) {

            //float t = Mathf.Sin( 2 * Mathf.PI * tileFlashSpeed * spawnTimer - 0.5f * Mathf.PI ) * 0.5f + 0.5f;
            //tileMat.color = Color.Lerp( initialColor, flashColor, t );

            spawnTimer += Time.deltaTime;
            yield return null;
        }
        Vector3 playerPos = PlayerData.position;
        Vector2 randomCircleOffset = UnityEngine.Random.insideUnitCircle * 5f;
        Vector3 randomPos = new Vector3( playerPos.x + randomCircleOffset.x, playerPos.y, playerPos.z + randomCircleOffset.y );

        Enemy spawnedEnemy = Instantiate( enemy, randomPos, Quaternion.identity ) as Enemy;
        spawnedEnemy.myLivingEntity.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics( currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor );

        spawnEnemyCoroutine = null;
    }

    void OnPlayerDeath( ) {
        isDisabled = true;
    }

    void OnEnemyDeath( ) {
        enemiesRemainingAlive--;

        if( enemiesRemainingAlive == 0 ) {
            NextWave();
        }
    }

    void NextWave() {
        //if( currentWaveNumber > 0 ) {
        //    AudioManager.Instance.PlaySound2D( "Level Complete" );
        //}

        currentWaveNumber++;

        if( currentWaveNumber - 1 < waves.Length ) {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if( OnNewWave != null ) {
                OnNewWave( currentWaveNumber );
            }
        }
    }
}
