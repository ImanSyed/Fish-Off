using UnityEngine;

[CreateAssetMenu(fileName = "Fish")]
public class Fish : ScriptableObject
{
    public string fishType;
    public int predatorHierarchy;
    public string onDetectBehaviour, onAttackedBehaviour;
    public float wanderSpeed, fleeSpeed, chaseSpeed, turnRate;
    public float fleeDuration, chaseDuration;
    public bool wanderSharpTurns, fleeSharpTurns, chaseSharpTurns;
    public float minimumShrinkSize, attackDamage;
    public Fish childFish;
    public bool canBeCaught;
    public AudioClip swimSound, attackSound;
    public AnimatorOverrideController animatorController;

}
