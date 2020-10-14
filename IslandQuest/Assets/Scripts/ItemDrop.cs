using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public Item.ItemType Type;
    public Item Item;
    public string Name;
    public string IconName;
    public int DamageValue;
    public string SpriteName;
    [System.NonSerialized]
    public Sprite InstructionSprite;
    [System.NonSerialized]
    public Sprite InstructionSprite2;

    private void Awake()
    {
        InstructionSprite = Resources.Load<Sprite>("Sprites/Pick Up Icon");
        transform.Find("Instructions Image").GetComponent<SpriteRenderer>().sprite = InstructionSprite;
        InstructionSprite2 = Resources.Load<Sprite>("Sprites/Interact Icon");
        transform.Find("Instructions Image 2").GetComponent<SpriteRenderer>().sprite = InstructionSprite2;
    }
}
