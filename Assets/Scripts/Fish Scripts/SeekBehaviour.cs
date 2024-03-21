using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    [SerializeField] private float targetReachedThreshold = 0.5f;
    [SerializeField] private bool showGizmos = true;
    
    bool reachedLastTarget = true;

    private Vector3 targetPositionCached;
    private float[] interestsTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIMovementData movementData)
    {
        if(reachedLastTarget)
        {
            if(movementData.targets == null || movementData.targets.Count <= 0)
            {
                movementData.currentTarget = null;
                return(danger, interest);
            }
            else
            {
                reachedLastTarget = false;
                movementData.currentTarget = movementData.targets.OrderBy(target => Vector3.Distance(target.position, transform.position)).FirstOrDefault();
            }
        }

        if(movementData.currentTarget != null && movementData.targets != null && movementData.targets.Contains(movementData.currentTarget))
        {
            targetPositionCached = movementData.currentTarget.position;
        }

        if(Vector3.Distance(transform.position, targetPositionCached) < targetReachedThreshold)
        {
            reachedLastTarget = true;
            movementData.currentTarget = null;
            return (danger, interest);
        }

        Vector3 directionToTarget = targetPositionCached - transform.position;
        for(int i = 0; i < interest.Length; i++)
        {
            float result = Vector3.Dot(directionToTarget.normalized, Directions.theDirections[i]);

            if(result > 0)
            {
                float valueToPutIn = result;
                if(valueToPutIn > interest[i])
                {
                    interest[i] = valueToPutIn;
                }
            }
        }
        interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos() 
    {
        if(!showGizmos)
        {
            return;
        }    

        Gizmos.DrawSphere(targetPositionCached, 0.2f);

        if(Application.isPlaying && interestsTemp != null)
        {
            Gizmos.color = Color.green;
            for(int i = 0; i < interestsTemp.Length; i++)
            {
                Gizmos.DrawRay(transform.position, Directions.theDirections[i] * interestsTemp[i]);
            }
            if(!reachedLastTarget)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(targetPositionCached, 0.1f);
            }
        }
    }
}
