using UnityEngine;

public class RiverPiece : MonoBehaviour
{
    public RiverGenerator.Side Side;

    private void OnTriggerStay2D(Collider2D col)
    {
        print("tick");
        if (col.gameObject.GetComponent<ResourceMine>() != null)
        {
            Destroy(col.gameObject);
        }
    }
}
