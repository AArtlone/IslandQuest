using UnityEngine;

[SerializeField]
public class InvSlotContent
{
    public string Name;
    public string IconName;
    public string SpriteName;
    public int Amount;
    public int DamageValue;
    public bool Resource;
    public bool IsItem;
    public Item Item;
    public ResourceDrop ResourceDrop;
    public Sprite InvHint;

    public InvSlotContent(Item item)
    {
        Item = item;
        IsItem = true;
        Name = item.Name;
        IconName = item.IconName;
        DamageValue = item.DamageValue;
        SpriteName = item.SpriteName;
    }
    public InvSlotContent(ResourceDrop resourceDrop, int amount)
    {
        ResourceDrop = resourceDrop;
        SpriteName = resourceDrop.Type.ToString();
        IconName = resourceDrop.Type.ToString() + " Icon";
        Amount = amount;
        Resource = true;
    }
}
