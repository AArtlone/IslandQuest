using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public readonly static HashSet<Trap> TrapPool = new HashSet<Trap>();

    private void OnEnable()
    {
        TrapPool.Add(this);
    }
    private void OnDisable()
    {
        TrapPool.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.GetComponent<Animal>() != null)
        {
            coll.GetComponent<Animal>().Stun(30f);
            coll.GetComponent<Animal>().bearAttackSource.PlayOneShot(coll.GetComponent<Animal>().bearSick);
        }
    }
}
