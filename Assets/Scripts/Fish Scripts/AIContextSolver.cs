using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIContextSolver : MonoBehaviour
{
    [SerializeField] private bool showGizmos = true;

    float[] interestGizmo = new float[26];
    Vector3 resultDirection = Vector3.zero;
    private float rayLength = 1;

    public Vector3 GetDirectionToMove(List<SteeringBehaviour> behaviours, AIMovementData movementData)
    {
        float[] danger = new float[26];
        float[] interest = new float[26];

        foreach(SteeringBehaviour behaviour in behaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest, movementData);
        }

        for(int i = 0; i < interest.Length; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        interestGizmo = interest;

        Vector3 outputDirection = Vector3.zero;
        for(int i = 0; i < interest.Length; i++)
        {
            outputDirection += Directions.theDirections[i] * interest[i];
        }
        outputDirection.Normalize();

        resultDirection = outputDirection;

        return resultDirection;
    }

    private void OnDrawGizmos() 
    {
        if(!showGizmos || !Application.isPlaying)
        {
            return;
        }    

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, resultDirection * rayLength);
    }
}
