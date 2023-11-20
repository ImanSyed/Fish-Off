using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    public Fish myStats;
    [SerializeField] private Collider wanderBounds;
    [SerializeField] private bool sharpTurns;
    [SerializeField] private SpriteRenderer frontSprite, sideSprite, backSprite;
    private Transform renderPoint;

    public enum BehaviourState
    {
        waiting,
        wandering, 
        chasing, 
        fleeing
    } 
    public BehaviourState currentBehaviourState;

    private Vector3 wayPoint, origin, previousDirection;
    private float wanderTime, chaseTime, fleeTime;
    private Transform preyTarget, predatorTarget;
    private FPSController playerController;


    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        wayPoint = origin;
        renderPoint = sideSprite.transform;

        frontSprite.sprite = myStats.frontSprite;
        sideSprite.sprite = myStats.sideSprite;
        backSprite.sprite = myStats.backSprite;


        playerController = FindObjectOfType<FPSController>();

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

        Color lineColor = Color.red;
        float lineLength = Mathf.Clamp(Vector3.Distance(transform.position, wayPoint), 1, wanderBounds.bounds.size.z);

        lineColor.r = Mathf.Clamp(Vector3.Distance(transform.position, wayPoint), 0, 25) / 25;
        lineColor.g = 1 - lineColor.r;
        
        int layermask = 1 << 2;
        layermask = ~layermask;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, newDirection, out hit, 50, layermask))
        {
            lineColor = Color.red;
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

        Debug.DrawRay(transform.position, newDirection * lineLength, lineColor);
        transform.rotation = Quaternion.LookRotation(newDirection);

        transform.position += transform.forward * myStats.wanderSpeed * Time.deltaTime;

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
        transform.position += transform.forward * myStats.chaseSpeed * Time.deltaTime;

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
        transform.position += transform.forward * myStats.fleeSpeed * Time.deltaTime;

        fleeTime += Time.deltaTime;

        if(fleeTime > myStats.chaseDuration)
        {
            predatorTarget = null;
            fleeTime = 0;
            SetBehaviourState(BehaviourState.wandering);
        }
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
        Vector3 playerDirection = playerController.transform.position - transform.position;
        float angleToPlayer = Vector2.Angle(transform.forward.normalized, playerDirection.normalized);
        Debug.Log(angleToPlayer);

        if(angleToPlayer < 45)
        {
            Debug.Log("Front");
            frontSprite.enabled = true;
            sideSprite.enabled = false;
            backSprite.enabled = false;
        }
        else if(angleToPlayer > 135)
        {
            Debug.Log("Back");
            frontSprite.enabled = false;
            sideSprite.enabled = false;
            backSprite.enabled = true;
        }
        else
        {
            Debug.Log("Side");
            frontSprite.enabled = false;
            sideSprite.enabled = true;
            backSprite.enabled = false;
        }

    }
}
