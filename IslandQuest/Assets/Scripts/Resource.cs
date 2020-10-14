using System;
using UnityEngine;

[Serializable]
public class Resource
{
    public int Amount;
    public enum ResourceType { Wood, BearSkin, GoldOre, IronOre, Berry, PoisonBerry, RawMeat, CookedMeat, Leaf, RawCrabMeat, None};
    public ResourceType Type;
    [Header("Fill in the resource can be consumed")]
    public bool Consumable;
    public enum EffectOnPlayer { Healthy, Poisonous, None};
    public EffectOnPlayer Effect;

    public void IncreaseResource(int _amount)
    {
        Amount += _amount;
    }

    public void DecreaseResource(int _amount)
    {
        Amount -= _amount;
    }
}
