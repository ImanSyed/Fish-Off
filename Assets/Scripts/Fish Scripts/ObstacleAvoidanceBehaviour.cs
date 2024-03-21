using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private float agentColliderSize = 0.5f;

    [SerializeField] private bool showGizmos = true;
    float[] dangersResultTemp = null;

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

