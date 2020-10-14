using UnityEngine;

public class SearchRadius : MonoBehaviour
{
    public Animal AnimalInterface;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.GetComponent<Player>() != null) Player.PlayerPool.Add(other.GetComponent<Player>());
        else if (other.GetComponent<Animal>() != null) Animal.Pool.Add(other.GetComponent<Animal>());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null) Player.PlayerPool.Remove(other.GetComponent<Player>());
        else if (other.GetComponent<Animal>() != null) Animal.Pool.Remove(other.GetComponent<Animal>());
    }

    /*Transform GetClosestObject(Transform[] obj)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in obj)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }*/
}
