using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : SteeringBehaviour
{
    public Fish myStats;
    [SerializeField] private Collider wanderBounds;
    [SerializeField] private bool sharpTurns;
    [SerializeField] private SpriteRenderer frontSprite, sideSprite, backSprite;
    [SerializeField] private Transform myRenderersTransform, myColliderTransform;
    [SerializeField] private Collider frontCollider, sideCollider;
    private Transform renderPoint;
    private Animator animator;
    private float shrinkStep, originalShrinkStep;

    [Header("AI Detection")]
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private AIMovementData movementData;
    [SerializeField] private float detectionDelay = 0.1f;
    [SerializeField] private float radius = 2f;
    [SerializeField] private float agentColliderSize = 0.5f;

    [SerializeField] private bool showGizmos = true;
    float[] dangersResultTemp = null;

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
    ShopUI shopUI;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        wayPoint = origin;

        playerController = FindObjectOfType<FPSController>();
        shopUI = FindAnyObjectByType<ShopUI>();

        rb = GetComponent<Rigidbody>();

        SetBehaviourState(BehaviourState.wandering);

        if(myStats.fishType == "Dumbass Fish")
        {
            return;
        }

        renderPoint = sideSprite.transform;
        
        animator = GetComponentInChildren<Animator>();

        frontSprite.enabled = false;
        sideSprite.enabled = false;
        backSprite.enabled = false;

        myRenderersTransform = frontSprite.transform.parent;

        shrinkStep = 0.5f;
        originalShrinkStep = shrinkStep;

        StartCoroutine(PerformDetection());

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

                case "Flee": 
                SetBehaviourState(BehaviourState.fleeing);
                break;

                default: 
                Debug.Log("Incorrect detection behaviour");
                break;
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

    /*
    void Update()
    {
        if(shopUI.pauseGame)
        {
            return;
        }

        if(myStats.fishType == "Dumbass Fish")
        {
            Wander();
            return;
        }

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
    */

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
        if(Physics.Raycast(transform.position, newDirection, out hit, 5, layermask) && Vector3.Distance(hit.point, transform.position) < Vector3.Distance(wayPoint, transform.position))
        {
            Debug.Log("Obstacle detected.");
            newDirection = Vector3.RotateTowards(transform.forward, (targetDirection + transform.right + (transform.up/2)).normalized, myStats.turnRate * turnRateModifier, 0);
            
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

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIMovementData movementData)
    {
        foreach(Collider obstacleCollider in movementData.obstacles)
        {
            Vector3 directionToObstacle = obstacleCollider.ClosestPoint(transform.position) - transform.position;
            directionToObstacle = directionToObstacle.normalized;

            float distanceToObstacle = directionToObstacle.magnitude;

            float weight = distanceToObstacle <= agentColliderSize ? 1 : (radius - distanceToObstacle) / radius;

            for(int i  = 0; i < Directions.theDirections.Count; i++)
            {
                float result = Vector3.Dot(directionToObstacle, Directions.theDirections[i]);

                float valueToPutIn = result * weight;

                if(valueToPutIn > danger[i])
                {
                    danger[i] = valueToPutIn;
                }
            }
        }
        dangersResultTemp = danger;
        return (danger, interest);
    }

    void OnDrawGizmos() 
    {
        if(showGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wayPoint, 0.5f);

            if(dangersResultTemp != null)
            {
                Gizmos.color = Color.red;
                for(int i = 0; i < dangersResultTemp.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.theDirections[i] * dangersResultTemp[i]);
                }
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
    }

    IEnumerator PerformDetection()
    {
        yield return new WaitForSeconds(detectionDelay);
        foreach(Detector detector in detectors)
        {
            detector.Detect(movementData);
        }
        StartCoroutine(PerformDetection());
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
            if(sideCollider != null && sideCollider != frontCollider && !frontCollider.enabled)
            {
                sideCollider.enabled = false;
                frontCollider.enabled = true;
            }
        }
        else if(angleToPlayer > 135 )
        {
            frontSprite.enabled = false;
            sideSprite.enabled = false;
            backSprite.enabled = true;
            if(sideCollider != null && sideCollider != frontCollider && !frontCollider.enabled)
            {
                sideCollider.enabled = false;
                frontCollider.enabled = true;
            }
        }
        else
        {
            frontSprite.enabled = false;
            sideSprite.enabled = true;
            backSprite.enabled = false;
            if(sideCollider != null && sideCollider != frontCollider && frontCollider.enabled)
            {
                frontCollider.enabled = false;
                sideCollider.enabled = true;
            }
            
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
        
        return dir > 0;
        
        /*
        if(dir > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        */

    }

    public void ShrinkMe(float magnitude)
    {
        if(myStats.onAttackedBehaviour == "Chase" && currentBehaviourState != BehaviourState.chasing)
        {
            preyTarget = playerController.transform;
            SetBehaviourState(BehaviourState.chasing);
        }
        if(myStats.onAttackedBehaviour == "Flee" && currentBehaviourState != BehaviourState.fleeing)
        {
            predatorTarget = playerController.transform;
            SetBehaviourState(BehaviourState.fleeing);
        }
        if(transform.parent.localScale.x > myStats.minimumShrinkSize)
        {
            if(shrinkStep <= 0)
            {
                shrinkStep = originalShrinkStep;
                myStats.wanderSpeed -= magnitude;
                myStats.fleeSpeed -= magnitude;
                myStats.chaseSpeed -= magnitude;
                //myRenderersTransform.localScale -= Vector3.one * magnitude * Time.deltaTime;
                //myColliderTransform.localScale = myRenderersTransform.localScale;
                transform.parent.localScale -= Vector3.one * magnitude;
            }
            else
            {
                if(Mathf.Round(shrinkStep * 10) % 0.5f == 0)
                {
                    transform.RotateAround(transform.position, transform.parent.position - playerController.transform.position, Random.Range(-15, 15));
                    transform.position = new Vector3(transform.position.x + Random.Range(-transform.parent.localScale.x, transform.parent.localScale.x), transform.position.y + Random.Range(-transform.parent.localScale.x, transform.parent.localScale.x), transform.position.z + Random.Range(-transform.parent.localScale.x, transform.parent.localScale.x));
                }
                shrinkStep -= Time.deltaTime;
            }
           
        }
        else if(!myStats.canBeCaught)
        {
            string fishName = myStats.fishType;
            myStats = myStats.childFish;
            myStats.fishType = fishName;
            frontSprite.sprite = myStats.sprite;
            sideSprite.sprite = myStats.sprite;
            backSprite.sprite = myStats.sprite;
            animator.enabled = false;
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

}

public static class Directions
{
    public static List<Vector3> theDirections = new List<Vector3>
    {
        new Vector3(0, 0, 1).normalized,
        new Vector3(0, 0, -1).normalized,
        new Vector3(0, 1, 0).normalized,
        new Vector3(0, -1, 0).normalized,
        new Vector3(1, 0, 0).normalized,
        new Vector3(-1, 0, 0).normalized,
        new Vector3(1, 0, 1).normalized,
        new Vector3(1, 0, -1).normalized,
        new Vector3(-1, 0, 1).normalized,
        new Vector3(-1, 0, -1).normalized,
        new Vector3(0, 1, 1).normalized,
        new Vector3(0, 1, -1).normalized,
        new Vector3(0, -1, 1).normalized,
        new Vector3(0, -1, -1).normalized,
        new Vector3(1, 1, 0).normalized,
        new Vector3(-1, 1, 0).normalized,
        new Vector3(1, -1, 0).normalized,
        new Vector3(-1, -1, 0).normalized,
        new Vector3(1, 1, 1).normalized,
        new Vector3(1, 1, -1).normalized,
        new Vector3(1, -1, 1).normalized,
        new Vector3(1, -1, -1).normalized,
        new Vector3(-1, 1, 1).normalized,
        new Vector3(-1, 1, -1).normalized,
        new Vector3(-1, -1, 1).normalized,
        new Vector3(-1, -1, -1).normalized,
    };
}