using UnityEngine;

[CreateAssetMenu(fileName = "Fish")]
public class Fish : ScriptableObject
{
    public string fishType;
    public string onSightBehaviour, onAttackBehaviour;
    public float wanderSpeed, fleeSpeed, chaseSpeed;
    public float fleeDistance, chaseDistance, chaseTime;
    public Fish smallFish;

}
