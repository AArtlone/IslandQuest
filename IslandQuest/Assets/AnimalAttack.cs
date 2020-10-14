using UnityEngine;

public class AnimalAttack : MonoBehaviour
{
    public Animal AnimalInterface;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (AnimalInterface.Target != null && AnimalInterface.Target.GetComponent<Player>() != null)
        {
            Player player = AnimalInterface.Target.GetComponent<Player>();
            if (collision == player.CharacterTransform.GetComponent<BoxCollider2D>())
            {
                AnimalInterface.Target.GetComponent<Player>().TakeDamage(AnimalInterface.Damage);
            }
        }
        Animal animal = GetComponent<Animal>();
        if (animal != null)
            animal.TakeDamage(AnimalInterface.Damage);
    }
}
