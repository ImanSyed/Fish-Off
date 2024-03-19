using UnityEngine;

public class FishBounds : MonoBehaviour
{
    [SerializeField] Collider col;
    [SerializeField] FishBehaviour fishBehaviour;
    
    private void OnTriggerExit(Collider other) 
    {
        if(other == col && fishBehaviour.currentBehaviourState == FishBehaviour.BehaviourState.wandering)
        {
            fishBehaviour.GenerateWayPoint();
        }    
    }
}
