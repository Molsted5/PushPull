using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof( NavMeshAgent ) )]
public class Enemy: MonoBehaviour {

    public enum State { Idle, Chasing, Attacking };
    State currentState;

    //public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;
    
    public LivingEntity livingEntity;
    
    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    
    //Material skinMaterial;

    //Color originalColor;

    float attackDistanceTreshold = 0.5f;
    float timebetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    List<Vector3> forces = new List<Vector3>();

    void Awake() {
        pathfinder = GetComponent<NavMeshAgent>();

        if( GameObject.FindGameObjectWithTag( "Player" ) != null ) {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag( "Player" ).transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<PlayerMovementController>().collisionRadius;
        }
    }

    void Start() {
        if( hasTarget ) {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine( UpdatePath() );
        }
    }

    void Update() {
        // Attack
        if( hasTarget ) {
            if( Time.time > nextAttackTime ) {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; // c^2 instead of c (pythagoras) because its cheaper than sqrroot and its the same in a relative camparison
                if( sqrDstToTarget < Mathf.Pow( attackDistanceTreshold + myCollisionRadius + targetCollisionRadius, 2 ) ) {
                    nextAttackTime = Time.time + timebetweenAttacks;
                    //AudioManager.Instance.PlaySound( "Enemy Attack", transform.position );
                    StartCoroutine( Attack() );
                }
            }
        }

    }

    public void SetCharacteristics( float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor ) {
        pathfinder.speed = moveSpeed;

        if( hasTarget ) {
            damage = Mathf.Ceil( targetEntity.startingHealth / hitsToKillPlayer );
        }
        livingEntity.startingHealth = enemyHealth;

        //skinMaterial = GetComponent<Renderer>().material;
        //skinMaterial.color = skinColor;
        //originalColor = skinMaterial.color;
    }

    public void TakeHit( float damage, Vector3 hitPoint, Vector3 hitDirection ) {
        //AudioManager.Instance.PlaySound( "Impact", transform.position );
        if( damage >= livingEntity.health ) {
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

        livingEntity.TakeHit( damage, hitPoint, hitDirection );
    }

    void OnTargetDeath() {
        hasTarget = false;
        currentState = State.Idle;
    }

    IEnumerator Attack() {
        currentState = State.Attacking;
        pathfinder.enabled = false;
        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * myCollisionRadius;

        float attackSpeed = 3;
        
        //skinMaterial.color = Color.red;

        bool hasAppliedDamage = false;
        float fraction = 0;
        while( fraction <= 1 ) {

            if( fraction >= 0.5 && !hasAppliedDamage ) {
                hasAppliedDamage = true;
                targetEntity.TakeDamage( damage );
            }

            fraction += Time.deltaTime * attackSpeed;
            float interpolation = ( -fraction * fraction + fraction ) * 4;
            transform.position = Vector3.Lerp( originalPosition, attackPosition, interpolation );

            yield return null;
        }

        //skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath() {
        while( hasTarget ) {
            if( currentState == State.Chasing && !livingEntity.dead ) {
                Vector3 dirToTarget = ( target.position - transform.position ).normalized;
                Vector3 velocityOffset = Vector3.zero;

                for( int i = 0; i < forces.Count; i++ ) {
                    velocityOffset += forces[i];
                }
                forces.Clear();

                Vector3 velocity = dirToTarget + velocityOffset;
                Vector3 direction = velocity.normalized;
                velocity = direction * Mathf.Min( velocity.magnitude, 20f );

                Vector3 position = transform.position + velocity;
                //position -= (target.position - position).normalized * (myCollisionRadius + targetCollisionRadius + attackDistanceTreshold / 2);
                pathfinder.SetDestination( position );
            }

            yield return null;
        }
    }

    public void TakeForce( Vector3 force ) {
        forces.Add( force );
    }

}