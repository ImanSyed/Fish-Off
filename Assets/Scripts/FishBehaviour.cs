using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    [SerializeField] private Fish myStats;
    [SerializeField] private Collider collider;


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



    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        wayPoint = origin;

        SetBehaviourState(BehaviourState.wandering);
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentBehaviourState)
        {
            case BehaviourState.wandering: Wander();
            return;
            
            case BehaviourState.chasing: 
            return;
            
            case BehaviourState.fleeing: 
            return;
            
        }
    }

    private void Wander()
    {
        if(Vector3.Distance(transform.position, wayPoint) < 1f)
        {
            GenerateWayPoint();
        }

        Vector3 targetDirection = wayPoint - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 3 * Time.deltaTime, 0);

        Color lineColor = Color.red;
        float lineLength = Mathf.Clamp(Vector3.Distance(transform.position, wayPoint), 1, 10);

        lineColor.r = Mathf.Clamp(Vector3.Distance(transform.position, wayPoint), 0, 25) / 25;
        lineColor.g = 1 - lineColor.r;
        
        int layermask = 1 << 2;
        layermask = ~layermask;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, newDirection, out hit, 15, layermask))
        {
            lineColor = Color.red;

            if(Physics.Raycast(transform.position, previousDirection, out hit, 15, layermask))
            {
                newDirection = Vector3.RotateTowards(transform.forward, transform.right, 6 * Time.deltaTime, 0);
            }
            else
            {
                newDirection = Vector3.RotateTowards(transform.forward, previousDirection, 6 * Time.deltaTime, 0);
            }
        }

        Debug.DrawRay(transform.position, newDirection * lineLength, lineColor);
        transform.rotation = Quaternion.LookRotation(newDirection);

        transform.position += transform.forward * myStats.wanderSpeed * Time.deltaTime;

        wanderTime += Time.deltaTime;

        if(wanderTime > 60)
        {
            GenerateWayPoint();
        }
        
        if(previousDirection != newDirection)
        {
            previousDirection = newDirection;

        }
    }

    private void SetBehaviourState(BehaviourState behaviourState)
    {
        currentBehaviourState = behaviourState;
    }

    private void GenerateWayPoint()
    {
        wanderTime = 0;

        Vector3 newPoint = origin;
        newPoint.x += Random.Range(-collider.bounds.extents.x, collider.bounds.extents.x);
        newPoint.y += Random.Range(-collider.bounds.extents.y, collider.bounds.extents.y);
        newPoint.z += Random.Range(-collider.bounds.extents.z, collider.bounds.extents.z);
        
        wayPoint = newPoint;
    }
}
