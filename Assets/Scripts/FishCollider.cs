using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCollider : MonoBehaviour
{
    private FishBehaviour fishBehaviour;

    void Start()
    {
        fishBehaviour = GetComponentInParent<FishBehaviour>();
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.collider.CompareTag("Player") && fishBehaviour.myStats.canBeCaught)
        {
            //fishBehaviour.Catch();
        }
    }
}
