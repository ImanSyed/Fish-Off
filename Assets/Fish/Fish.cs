using UnityEngine;

[CreateAssetMenu(fileName = "Fish")]
public class Fish : ScriptableObject
{
    public string fishType;
    public int predatorHierarchy;
    public Sprite frontSprite, backSprite, sideSprite;
    public string onDetectBehaviour, onAttackedBehaviour;
    public float wanderSpeed, fleeSpeed, chaseSpeed, turnRate;
    public float fleeDuration, chaseDuration;
    public bool wanderSharpTurns, fleeSharpTurns, chaseSharpTurns;
    public float minimumShrinkSize;
    public Fish childFish;

}