using UnityEngine;

public class River : MonoBehaviour
{
    public float SlowFactor;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.MovementSpeed *= SlowFactor;
            return;
        }           

        Animal animal = collision.GetComponent<Animal>();
        if (animal != null)
            animal.Speed *= SlowFactor;       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.MovementSpeed /= SlowFactor;           
            return;
        }           

        Animal animal = collision.GetComponent<Animal>();
        if (animal != null)
            animal.Speed /= SlowFactor;
            
    }
}
