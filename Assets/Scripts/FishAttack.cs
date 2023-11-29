using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAttack : MonoBehaviour
{
    private FishBehaviour fishBehaviour;

    void Start()
    {
        fishBehaviour = transform.parent.GetComponent<FishBehaviour>();    
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(fishBehaviour.currentBehaviourState == FishBehaviour.BehaviourState.chasing && other.CompareTag("Player"))
        {
            fishBehaviour.Attack();
        }    
    }
}
