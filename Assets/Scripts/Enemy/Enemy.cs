using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float moveSpeed = 1.8f;
    public float acceleration = 20f;
    public float angularSpeed = 60f;
    public float maxAngularSpeed = 80f;
    public float angularAcceleration = 20f;
    public float maxPredictionLength = 2f; 
    
    float currentAngularSpeed;
    float deltaAngleTarget;

    Vector3 oldTargetPosition;

    public enum State { Idle, Chasing, Attacking };
    State currentState;

    NavMeshAgent pathfinder;
    Transform target;
    CharacterController playerCC;
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
        pathfinder.updateRotation = false;

        if( GameObject.FindGameObjectWithTag( "Player" ) != null ) {
            target = GameObject.FindGameObjectWithTag( "Player" ).transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetCollisionRadius = target.GetComponent<PlayerMovementController>().collisionRadius;
            playerCC = target.GetComponent<CharacterController>();
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
                Vector3 forceOffset = Vector3.zero;

                // y should maybe just be transform and adjust for slope? not sure if nav mesh agents do that or need to
                Vector3 dirToTarget = ( target.position - transform.position ).normalized;
                Vector3 playerDir = playerCC.velocity.normalized;

                for( int i = 0; i < forces.Count; i++ ) {
                    forceOffset += forces[i];
                }
                forces.Clear();

                if( forceOffset != Vector3.zero ) {
                    pathfinder.acceleration = acceleration * 2f;
                    position = transform.position + forceOffset;
                }
                else {
                    pathfinder.acceleration = acceleration;
                    position = target.position;
                    if( oldTargetPosition != target.position ) {
                        float t = 1f - Mathf.Abs( Vector3.Dot( dirToTarget, playerDir ) );
                        Vector3 predictionDistance = playerDir * maxPredictionLength * t;
                        position += predictionDistance;
                    }
                    oldTargetPosition = target.position;
                }
                
                pathfinder.speed = moveSpeed * Mathf.Max( 1f, forceOffset.magnitude );

                pathfinder.SetDestination( position );

                // Rotation
                Quaternion targetRotation = Quaternion.LookRotation( dirToTarget, Vector3.up );
                float deltaAngleTarget = Mathf.DeltaAngle( transform.rotation.eulerAngles.y, targetRotation.eulerAngles.y );
                if( deltaAngleTarget > 0 ) {
                    if( this.deltaAngleTarget >= 0 ) {
                        currentAngularSpeed += angularAcceleration * Time.deltaTime;
                    }
                    else if( this.deltaAngleTarget < 0 ) {
                        currentAngularSpeed = this.angularSpeed;
                    }
                }
                else if( deltaAngleTarget < 0 ) {
                    if( this.deltaAngleTarget <= 0 ) {
                        currentAngularSpeed += angularAcceleration * Time.deltaTime;
                    }
                    else if( this.deltaAngleTarget > 0 ) {
                        currentAngularSpeed = this.angularSpeed;
                    }
                }
                else {
                    currentAngularSpeed = this.angularSpeed;
                }
                currentAngularSpeed = Mathf.Min( maxAngularSpeed, currentAngularSpeed );
                this.deltaAngleTarget = deltaAngleTarget;

                transform.rotation = Quaternion.RotateTowards( transform.rotation, targetRotation, currentAngularSpeed * Time.deltaTime );
            }

            yield return null;
        }
    }

    public void TakeForce( Vector3 force ) {
        forces.Add( force );
    }

}