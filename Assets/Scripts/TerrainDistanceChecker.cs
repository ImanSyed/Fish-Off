using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDistanceChecker : MonoBehaviour
{
    TerrainCollider terrainCollider;

    void Start()
    {
        terrainCollider = GetComponentInChildren<TerrainCollider>();
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && !terrainCollider.enabled)
        {
            terrainCollider.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player") && terrainCollider.enabled)
        {
            terrainCollider.enabled = false;
        }
    }
}
