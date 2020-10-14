using UnityEngine;

public class ResourceMine : MonoBehaviour
{
    public static readonly int DefaultAmount = 1;
    public Resource.ResourceType Type;
    public Resource.ResourceType Type2;
    public Item.ItemType NeededItem;
    public bool NeedsItemToInteract;
    public bool WillBeDestroyed;
    public bool WillChangeSprite;
    public bool CanBeSetOnFire;
    public bool ConsumableDrop;
    public bool BigMine;
    [System.NonSerialized]
    public int Amount;
    public Resource.EffectOnPlayer EffectOnPlayer;
    [System.NonSerialized]
    public bool IsOnFire;
    [System.NonSerialized]
    public bool CanBeCollected = true;
    [Header("Assing if the mine needs to change sprite")]
    public Sprite SpriteToChangeTo;
    [System.NonSerialized]
    public Sprite InstructionSprite;

    private void Awake()
    {
        InstructionSprite = Resources.Load<Sprite>("Sprites/Interact Icon");
        transform.Find("Instructions Image").GetComponent<SpriteRenderer>().sprite = InstructionSprite;
        Amount = (BigMine) ? Amount = DefaultAmount : Amount = DefaultAmount * 2;
    }
}
