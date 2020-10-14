using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    public Resource.ResourceType Type;
    public bool CanBeSetOnFire;
    public bool IsOnFire;
    public bool Consubamle;
    public Resource.EffectOnPlayer EffectOnPlayer;
    [System.NonSerialized]
    public int Amount = 1;
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
