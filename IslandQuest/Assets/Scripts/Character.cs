using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Player Player;
    private Animator _anim;

    private void Awake()
    {
        _anim = Player._anim;
    }

    private void EndAttack()
    {
        Player.isAttacking = false;
    }

    private void EndDodge()
    {
        foreach (Player Player in Player.PlayerPool)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Player.GetComponent<Collider2D>(), false);
        }
        foreach (Animal Animal in Animal.Pool)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Animal.GetComponent<Collider2D>(), false);
        }
        Player.isDodging = false;
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.1f);
        Player.Die();
    }
}
