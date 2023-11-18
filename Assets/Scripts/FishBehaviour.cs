using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    [SerializeField] private Fish myStats;
    [SerializeField] private Collider wanderBounds;
    [SerializeField] private bool sharpTurns;

    public enum BehaviourState
    {
        waiting,
        wandering, 
        chasing, 
        fleeing
    } 
    public BehaviourState currentBehaviourState;

    private Vector3 wayPoint, origin, previousDirection;
    private float wanderTime;
    private Transform preyTarget, predatorTarget;


    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        wayPoint = origin;

        SetBehaviourState(BehaviourState.wandering);
    }

    /// <summary>
    /// When the fish detects the player via its FishDetector
    /// </summary>
    public void Detect(int detectionType, Transform detectedBody)
    {
        // 0 prey, 1 player, 2 predator
        switch(detectionType)
        {
            case 0: preyTarget = detectedBody;
            break;
           
            case 1: 
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
            break;
           
            case 2: 
            break;
        }
        
    }

    /// <summary>
    /// Assign a new behaviour state for the fish
    /// </summary>
    /// <param name="behaviourState"></param>
    private void SetBehaviourState(BehaviourState behaviourState)
    {
        currentBehaviourState = behaviourState;
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentBehaviourState)
        {
            case BehaviourState.wandering: Wander();
            return;
            
            case BehaviourState.chasing: Chase();
            return;
            
            case BehaviourState.fleeing: 
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

        float turnRate = 3 * Time.deltaTime;

        if(sharpTurns)
        {
            turnRate = 100;
        }

        Vector3 targetDirection = wayPoint - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, turnRate, 0);

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

            if(Physics.Raycast(transform.position, previousDirection, out hit, 50, layermask))
            {
                bool b = Random.value > 0.5f;
                Vector3 leftRight = b ? transform.right : -transform.right; 
                Debug.Log(b + " " + leftRight);
                newDirection = Vector3.RotateTowards(transform.forward, leftRight + transform.forward, turnRate * 5, 0);
            }
            else
            {
                newDirection = Vector3.RotateTowards(transform.forward, previousDirection, turnRate * 5, 0);
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
}
