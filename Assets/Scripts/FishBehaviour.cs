using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    public Fish myStats;
    [SerializeField] private Collider wanderBounds;
    [SerializeField] private bool sharpTurns;
    [SerializeField] private SpriteRenderer frontSprite, sideSprite, backSprite;
    [SerializeField] private Transform myRenderersTransform, myColliderTransform;
    private Transform renderPoint;
    private Animator animator;
    private AudioSource audioSource;

    public enum BehaviourState
    {
        waiting,
        wandering, 
        chasing, 
        fleeing,
        succing,
        attacking
    } 
    public BehaviourState currentBehaviourState;

    private Vector3 wayPoint, origin, previousDirection;
    private float wanderTime, chaseTime, fleeTime;
    private Transform preyTarget, predatorTarget;
    private FPSController playerController;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        wayPoint = origin;
        renderPoint = sideSprite.transform;
        
        rb = GetComponent<Rigidbody>();

        animator = GetComponentInChildren<Animator>();

        frontSprite.enabled = false;
        sideSprite.enabled = false;
        backSprite.enabled = false;

        myRenderersTransform = frontSprite.transform.parent;

        playerController = FindObjectOfType<FPSController>();

        audioSource = GetComponent<AudioSource>();

        SetBehaviourState(BehaviourState.wandering);
    }

    /// <summary>
    /// When the fish detects the player via its FishDetector
    /// </summary>
    public void Detect(int detectedHierarchy, Transform detectedBody)
    {
        if(detectedHierarchy == -1)
        {
            switch (myStats.onDetectBehaviour)
            {
                case "Chase": 
                preyTarget = detectedBody;
                SetBehaviourState(BehaviourState.chasing);
                break;

                case "Flee": SetBehaviourState(BehaviourState.fleeing);
                break;

                default: Debug.Log("Incorrect detection behaviour");
                break;
            }
        }
        else
        {
            if(detectedHierarchy > myStats.predatorHierarchy)
            {
                predatorTarget = detectedBody;
                SetBehaviourState(BehaviourState.fleeing);
            }
            else if(detectedHierarchy < myStats.predatorHierarchy)
            {
                preyTarget = detectedBody;
                SetBehaviourState(BehaviourState.chasing);
            }
        }
    }

    /// <summary>
    /// Assign a new behaviour state for the fish
    /// </summary>
    /// <param name="behaviourState"></param>
    private void SetBehaviourState(BehaviourState behaviourState)
    {
        if(currentBehaviourState != behaviourState)
        {
            currentBehaviourState = behaviourState;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RenderFish();

        switch(currentBehaviourState)
        {
            case BehaviourState.wandering:
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                {
                    animator.Play("Move", 0);
                }
                if(myStats.wanderSharpTurns)
                {
                    sharpTurns = true;
                }
                else
                {
                    sharpTurns = false;
                }
                Wander();
            return;
            
            case BehaviourState.chasing: 
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                {
                    animator.Play("Move", 0);
                }
                if(myStats.chaseSharpTurns)
                {
                    sharpTurns = true;
                }
                else
                {
                    sharpTurns = false;
                }
                Chase();
            return;
            
            case BehaviourState.fleeing: 
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                {
                    animator.Play("Move", 0);
                }
                if(myStats.fleeSharpTurns)
                {
                    sharpTurns = true;
                }
                else
                {
                    sharpTurns = false;
                }
                Flee();            
            return;

            case BehaviourState.waiting:
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    animator.Play("Idle", 0);
                }
            return;

            case BehaviourState.attacking:
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    animator.Play("Attack", 0);
                }
            return;
            
        }
    }

    /// <summary>
    /// Make the fish wander within its wander bounds
    /// </summary>
    private void Wander()
    {
        if(Vector3.Distance(transform.position, wayPoint) < 1f)
        {
            GenerateWayPoint();
        }

        float turnRateModifier = Time.deltaTime;

        if(sharpTurns)
        {
            turnRateModifier = 100;
        }

        Vector3 targetDirection = wayPoint - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, myStats.turnRate * turnRateModifier, 0);

        
        int layermask = 1 << 2;
        layermask = ~layermask;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, newDirection, out hit, 50, layermask))
        {
            turnRateModifier *= 5;
            if(Physics.Raycast(transform.position, previousDirection, out hit, 50, layermask))
            {
                bool b = Random.value > 0.5f;
                Vector3 leftRight = b ? transform.right : -transform.right; 
                newDirection = Vector3.RotateTowards(transform.forward, leftRight + transform.forward, myStats.turnRate * turnRateModifier, 0);
            }
            else
            {
                newDirection = Vector3.RotateTowards(transform.forward, previousDirection, myStats.turnRate * turnRateModifier, 0);
            }
        }

        transform.rotation = Quaternion.LookRotation(newDirection);

        rb.velocity = transform.forward * myStats.wanderSpeed * Time.deltaTime;

        wanderTime += Time.deltaTime;

        if(wanderTime > 15)
        {
            GenerateWayPoint();
        }
        
        if(previousDirection != newDirection)
        {
            previousDirection = newDirection;
        }
    }

    private void Chase()
    {
        float turnRateModifier = Time.deltaTime;

        if(sharpTurns)
        {
            turnRateModifier = 100;
        }

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, preyTarget.position - transform.position, myStats.turnRate * turnRateModifier, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
        rb.velocity = transform.forward * myStats.chaseSpeed * Time.deltaTime;

        chaseTime += Time.deltaTime;

        if(chaseTime > myStats.chaseDuration)
        {
            chaseTime = 0;
            preyTarget = null;
            SetBehaviourState(BehaviourState.wandering);
        }
    }

     private void Flee()
    {
        float turnRateModifier = Time.deltaTime;

        if(sharpTurns)
        {
            turnRateModifier = 100;
        }

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, transform.position - predatorTarget.position, myStats.turnRate * turnRateModifier, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
        rb.velocity = transform.forward * myStats.fleeSpeed * Time.deltaTime;

        fleeTime += Time.deltaTime;

        if(fleeTime > myStats.chaseDuration)
        {
            predatorTarget = null;
            fleeTime = 0;
            SetBehaviourState(BehaviourState.wandering);
        }
    }

    public void Attack()
    {
        SetBehaviourState(BehaviourState.attacking);
        playerController.TakeDamage(myStats.attackDamage);
        audioSource.PlayOneShot(myStats.attackSound);
    }

    public void GenerateWayPoint()
    {
        wanderTime = 0;

        Vector3 newPoint = origin;
        newPoint.x += Random.Range(-wanderBounds.bounds.extents.x, wanderBounds.bounds.extents.x);
        newPoint.y += Random.Range(-wanderBounds.bounds.extents.y, wanderBounds.bounds.extents.y);
        newPoint.z += Random.Range(-wanderBounds.bounds.extents.z, wanderBounds.bounds.extents.z);
        
        wayPoint = newPoint;
    }

    private void RenderFish()
    {
        Vector3 playerDirection = playerController.transform.position - renderPoint.position;
        playerDirection.y = 0;

        float angleToPlayer = Vector3.Angle(transform.forward.normalized, playerDirection.normalized);

        if(angleToPlayer < 45)
        {
            frontSprite.enabled = true;
            sideSprite.enabled = false;
            backSprite.enabled = false;
        }
        else if(angleToPlayer > 135 )
        {

            frontSprite.enabled = false;
            sideSprite.enabled = false;
            backSprite.enabled = true;
        }
        else
        {
            frontSprite.enabled = false;
            sideSprite.enabled = true;
            backSprite.enabled = false;
            
            if(AngleDir(transform.forward.normalized, playerDirection, Vector3.up))
            {
                sideSprite.flipX = false;
            }
            else
            {
                sideSprite.flipX = true;
            }
        }
    }

    private bool AngleDir(Vector3 forward, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(forward, targetDir);
        float dir = Vector3.Dot(perp, up);
        
        if(dir > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void ShrinkMe(float magnitude)
    {
        if(myRenderersTransform.localScale.x > myStats.minimumShrinkSize)
        {
            myRenderersTransform.localScale -= Vector3.one * magnitude * Time.deltaTime;
            myColliderTransform.localScale = myRenderersTransform.localScale;
        }
        else if(!myStats.canBeCaught)
        {
            myStats = myStats.childFish;
            animator.runtimeAnimatorController = myStats.animatorController;
            myRenderersTransform.localScale = Vector3.one;
            myColliderTransform.localScale = Vector3.one;
        }
    }

    public void SuckMe(float magnitude)
    {
        if(!myStats.canBeCaught)
        {
            return;
        }

        if(currentBehaviourState != BehaviourState.succing)
        {
            currentBehaviourState = BehaviourState.succing;
        }
        Vector3 succPos = Vector3.MoveTowards(transform.position, playerController.transform.position, magnitude * Time.deltaTime);
        transform.position = succPos;
    }

    public void Catch()
    {
        playerController.fishCollection[myStats.fishType] = playerController.fishCollection[myStats.fishType]++;

    }
}
