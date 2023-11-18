using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishDetector : MonoBehaviour
{
    [SerializeField] private FishBehaviour fishBehaviour;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            fishBehaviour.Detect(1, other.transform);
        }    
    }
}
