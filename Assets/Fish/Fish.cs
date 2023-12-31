using UnityEngine;

[CreateAssetMenu(fileName = "Fish")]
public class Fish : ScriptableObject
{
    public string fishType;
    public string onDetectBehaviour, onAttackedBehaviour;
    public float wanderSpeed, fleeSpeed, chaseSpeed, turnRate;
    public float fleeDuration, chaseDuration;
    public bool wanderSharpTurns, fleeSharpTurns, chaseSharpTurns;
    public float minimumShrinkSize, attackDamage;
    public Fish childFish;
    public bool canBeCaught;
    public Sprite sprite;

}
