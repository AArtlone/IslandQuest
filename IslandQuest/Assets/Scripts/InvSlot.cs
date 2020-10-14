using UnityEngine;

public class InvSlot : MonoBehaviour
{
    public bool IsOccupied;
    public InvSlotContent InvSlotContent;
    public GameObject Object;
    public void ResetInvSlot()
    {
        IsOccupied = false;
        InvSlotContent = null;
        Destroy(Object);
    }
}
