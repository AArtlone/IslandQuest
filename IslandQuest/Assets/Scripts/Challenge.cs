using UnityEngine;

[CreateAssetMenu(fileName = "New Challenge", menuName = "Challenges/Challenge")]
public class Challenge : ScriptableObject
{
    public enum ChallengeType { ResourceCollection, KillingOrTrapping, Craft, KillingPlayer};
    public ChallengeType Type;
    [Tooltip("Text that will be shown in the Challenge Announcement")]
    public string TextToAnnounce;
    [System.NonSerialized]
    public bool Complete;
    [Tooltip("Write down the amount that the player needs to collect or kill")]
    public int AmountToCollectKillTrap;
    [Space(25f)]
    [Header("Fill in if the challenge requires the player to collect certain amount of a particular resource")]
    //public bool Collect;
    public Resource.ResourceType TypeToCollect;
    [Space(25f)]
    [Header("Fill in if the challenge requires the player to kill or trap someone")]
    public Animal.AnimalType AnimalToKillOrTrap;
    [Space(25f)]
    [Header("Fill in if the challenge requires the player to craft something")]
    //public int AmountToKill;
    public Item.ItemType ItemToCraft;

    [System.NonSerialized]
    public int AmountCollectedKilledTrapped;
}
