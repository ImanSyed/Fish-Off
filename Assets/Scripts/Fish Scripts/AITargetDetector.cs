using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AITargetDetector : Detector
{
    [SerializeField] private float detectionRange = 5;
    [SerializeField] private LayerMask detectableLayerMask, playerLayerMask;
    [SerializeField] private bool showGizmos = false;
    [SerializeField] private Transform targetTransform;
    private Vector3 currentTarget = Vector3.zero;

    
    private List<Transform> colliders;

    public void SetTarget(Vector3 newTarget)
    {
        currentTarget = newTarget;
        Debug.Log("Current Target: " + currentTarget);
    }

    public void ResetTarget()
    {
        currentTarget = Vector3.zero;
    }

    public override void Detect(AIMovementData movementData)
    {
        Collider[] detectedColliders = Physics.OverlapSphere(transform.position, detectionRange, playerLayerMask);
        if(detectedColliders.Length > 0)
        {
            Collider playerCollider = detectedColliders[0];
            Vector3 targetDirection = (playerCollider.transform.position - transform.position).normalized;

            if(currentTarget != Vector3.zero)
            {
                targetDirection = (currentTarget - transform.position).normalized;
                targetTransform.position = currentTarget;
            }
            else
            {
                targetTransform = null;
            }

            Debug.Log("Direction: " + targetDirection);
            Debug.Log("Current Target: " + currentTarget);

            RaycastHit hit;
            if(Physics.Raycast(transform.position, targetDirection, out hit, detectionRange, detectableLayerMask))
            {
                if(hit.collider.gameObject.layer == 8)
                {
                    Debug.Log(0);
                    Debug.DrawRay(transform.position, targetDirection * detectionRange, Color.magenta);
                    movementData.targets = new List<Transform>(){playerCollider.transform};
                }
                else
                {
                    Debug.Log(1);
                    movementData.targets = new List<Transform>(){targetTransform};
                }   
            }
            else
            {
                movementData.targets = new List<Transform>(){targetTransform};

            }
        }
        else
        {
            Debug.Log(2);
            movementData.targets = null;
        }

        if(movementData.targets.Count > 0)
        {
            Debug.Log("Target: " + movementData.targets[0]);

        }
        
    }

    private void OnDrawGizmos() 
    {
        if(!showGizmos)
        {
            return;
        }

        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if(Application.isPlaying && colliders != null)
        {
            Gizmos.color = Color.magenta;
            foreach(var item in colliders)
            {
                Gizmos.DrawSphere(item.transform.position, 0.3f);
            }
        }
    }


}
