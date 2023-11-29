using UnityEngine;

[CreateAssetMenu(fileName = "Bounty")]
public class Bounty : ScriptableObject
{
    public string fishType;
    public int reward, rewardRandomiser;
    public Sprite sprite;
}
