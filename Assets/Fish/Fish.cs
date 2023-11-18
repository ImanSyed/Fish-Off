using UnityEngine;

[CreateAssetMenu(fileName = "Fish")]
public class Fish : ScriptableObject
{
    public string fishType;
    public Sprite frontSprite, backSprite, sideSprite;
    public string onDetectBehaviour, onAttackedBehaviour;
    public float wanderSpeed, fleeSpeed, chaseSpeed, turnRate;
    public float fleeDistance, chaseDistance, chaseTime;
    public Fish childFish;

}
