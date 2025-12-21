using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof( NavMeshAgent ) )]
public class Enemy: MonoBehaviour {
    public LayerMask mask;
    //public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;
    public LivingEntity myLivingEntity;

    [HideInInspector] public float attackDistanceTreshold = 0.5f;
    [HideInInspector] public float timebetweenAttacks = 1f;
    [HideInInspector] public float damage = 1f;
    
    public enum State { Idle, Chasing, Attacking };
    State currentState;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    //Material skinMaterial;
    //Color originalColor;
    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;
    bool hasTarget;
    List<Vector3> forces = new List<Vector3>();

    void Awake() {
        pathfinder = GetComponent<NavMeshAgent>();

        if( GameObject.FindGameObjectWithTag( "Player" ) != null ) {
            target = GameObject.FindGameObjectWithTag( "Player" ).transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetCollisionRadius = target.GetComponent<PlayerMovementController>().collisionRadius;
            hasTarget = true;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        }
    }

    void OnEnable() {
        targetEntity.OnDeath += OnTargetDeath;
    }

    void OnDisable() {
        targetEntity.OnDeath -= OnTargetDeath;
    }

    void Start() {
        if( hasTarget ) {
            currentState = State.Chasing;
            StartCoroutine( UpdatePath() );
        }
    }

    void Update() {
        // Attack
        if( hasTarget ) {
            if( Time.time > nextAttackTime ) {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if( sqrDstToTarget < Mathf.Pow( attackDistanceTreshold + myCollisionRadius + targetCollisionRadius, 2 ) ) {
                    Attack();
                    nextAttackTime = Time.time + timebetweenAttacks;
                    //AudioManager.Instance.PlaySound( "Enemy Attack", transform.position );
                }
            }
        }

    }

    public void SetCharacteristics( float moveSpeed, float damage, float attackDistanceTreshold, float timebetweenAttacks, float enemyHealth, Color skinColor ) {
        pathfinder.speed = moveSpeed;
        this.damage = damage;
        this.attackDistanceTreshold = attackDistanceTreshold;
        this.timebetweenAttacks = timebetweenAttacks;
        myLivingEntity.startingHealth = enemyHealth;

        //skinMaterial = GetComponent<Renderer>().material;
        //skinMaterial.color = skinColor;
        //originalColor = skinMaterial.color;
    }

    public void TakeHit( float damage, Vector3 hitPoint, Vector3 hitDirection ) {
        //AudioManager.Instance.PlaySound( "Impact", transform.position );
        if( damage >= myLivingEntity.health ) {
            if( OnDeathStatic != null ) {
                OnDeathStatic();
            }

            //AudioManager.Instance.PlaySound( "Enemy Death", transform.position );

            //GameObject effectInstance = Instantiate( deathEffect.gameObject, hitPoint, Quaternion.FromToRotation( Vector3.forward, hitDirection ) );

            //ParticleSystemRenderer renderer = effectInstance.GetComponent<ParticleSystemRenderer>();
            //renderer.material = new Material( renderer.material ); // Clone to avoid modifying the shared material
            //renderer.material.color = originalColor;

            //ParticleSystem particleSystem = effectInstance.GetComponent<ParticleSystem>();
            //Destroy( effectInstance, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax );
        }

        myLivingEntity.TakeHit( damage, hitPoint, hitDirection );
    }

    void OnTargetDeath() {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Attack() {
        targetEntity.TakeDamage( damage );
        print( "Attacked" );
    }

    IEnumerator UpdatePath() {
        while( hasTarget ) {
            if( currentState == State.Chasing && !myLivingEntity.dead ) {
                Vector3 position;

                Vector3 wishDirection = ( target.position - transform.position ).normalized;
                Vector3 forceOffset = Vector3.zero;

                for( int i = 0; i < forces.Count; i++ ) {
                    forceOffset += forces[i];
                }
                forces.Clear();

                if( forceOffset != Vector3.zero ) {
                    position = transform.position + forceOffset;
                } 
                else {
                    position = target.position;
                }

                pathfinder.SetDestination( position );
            }

            yield return null;
        }
    }

    public void TakeForce( Vector3 force ) {
        forces.Add( force );
    }

}