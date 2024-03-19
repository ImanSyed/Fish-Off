using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AITargetDetector : Detector
{
    [SerializeField] private float detectionRange = 5;
    [SerializeField] private LayerMask detectableLayerMask, playerLayerMask;
    [SerializeField] private bool showGizmos = false;
    
    private List<Transform> colliders;

    public override void Detect(AIMovementData movementData)
    {

        Collider[] detectedColliders = Physics.OverlapSphere(transform.position, detectionRange, playerLayerMask);
        if(detectedColliders.Length > 0)
        {
            Collider playerCollider = detectedColliders[0];
            Vector3 targetDirection = (playerCollider.transform.position - transform.position).normalized;
            RaycastHit hit;
            if(Physics.Raycast(transform.position, targetDirection, out hit, detectionRange, detectableLayerMask))
            {
                if(hit.collider.gameObject.layer == 8)
                {
                    Debug.Log(gameObject.name + " detecting player!");

                    Debug.DrawRay(transform.position, targetDirection * detectionRange, Color.magenta);
                    colliders = new List<Transform>(){playerCollider.transform};
                }
                else
                {
                    colliders = null;
                }   
            }
        }
        else
        {
            colliders = null;
        }
        movementData.targets = colliders;
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
