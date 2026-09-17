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
    [HideInInspector] public float moveSpeed = 1.8f;
    [HideInInspector] public float acceleration = 20f;
    public float angularSpeed = 720f;
    [HideInInspector] public float angularAcceleration = 20f;
    float currentAngularSpeed;
    Vector3 rotationTargetLookAtPosition;

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

    public float rotationTargetCooldown = 0.2f;
    float timeSinceNewRotationTarget;

    void Awake() {
        pathfinder = GetComponent<NavMeshAgent>();
        pathfinder.updateRotation = false;

        if( GameObject.FindGameObjectWithTag( "Player" ) != null ) {
            target = GameObject.FindGameObjectWithTag( "Player" ).transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetCollisionRadius = target.GetComponent<PlayerMovementController>().collisionRadius;
            hasTarget = true;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        }

        rotationTargetLookAtPosition = transform.position + transform.forward;
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

    public void SetCharacteristics( float moveSpeed, float angularSpeed, float acceleration, float damage, float attackDistanceTreshold, float timebetweenAttacks, float enemyHealth, Color skinColor ) {
        this.moveSpeed = moveSpeed;
        pathfinder.speed = moveSpeed;
        this.angularSpeed = angularSpeed;
        this.acceleration = acceleration;
        pathfinder.acceleration = acceleration;
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

                // y should maybe just be transform and adjust for slope? not sure if nav mesh agents do that or need to
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

                pathfinder.speed = moveSpeed * Mathf.Max( 1f, forceOffset.magnitude );

                float angularSpeed;
                if( position != target.position ) {
                    //angularSpeed = 55f;
                    angularSpeed = this.angularSpeed;
                    pathfinder.acceleration = acceleration * 2f;
                }
                else {
                    angularSpeed = this.angularSpeed;
                    pathfinder.acceleration = acceleration;
                }

                pathfinder.SetDestination( position );

                Vector3 wishRotationDirection;
                timeSinceNewRotationTarget += Time.deltaTime;
                if( timeSinceNewRotationTarget >= rotationTargetCooldown ) {
                    rotationTargetLookAtPosition = target.position;
                    wishRotationDirection = wishDirection;
                    timeSinceNewRotationTarget = 0f;
                }
                else {
                    wishRotationDirection = ( rotationTargetLookAtPosition - transform.position ).normalized;
                    //currentAngularSpeed += angularAcceleration * Time.deltaTime;
                }
                currentAngularSpeed = this.angularSpeed;
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation( wishRotationDirection, Vector3.up ),
                    currentAngularSpeed * Time.deltaTime
                );

            }

            yield return null;
        }
    }

    public void TakeForce( Vector3 force ) {
        forces.Add( force );
    }

}