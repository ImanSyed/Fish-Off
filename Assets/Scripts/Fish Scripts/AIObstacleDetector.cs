using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIObstacleDetector : Detector
{
    [SerializeField] private float detectionRadius = 3;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool showGizmos = true;

    Collider[] colliders;

    public override void Detect(AIMovementData movementData)
    {
        colliders = Physics.OverlapSphere(transform.position, detectionRadius, layerMask);
        movementData.obstacles = colliders;
    }

    private void OnDrawGizmos() 
    {
        if(!showGizmos)
        {
            return;
        }

        if(Application.isPlaying && colliders != null)
        {
            Gizmos.color = Color.red;
            foreach(Collider obstacleCollider in colliders)
            {
                Gizmos.DrawSphere(obstacleCollider.transform.position, 0.2f);
            }
        }
    }
}
