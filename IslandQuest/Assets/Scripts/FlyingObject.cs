using UnityEngine;

public class FlyingObject : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool _isFlying;
    public void Throw(GameObject obj, Vector3 side, Player player)
    {
        _isFlying = true;
        rb = obj.GetComponent<Rigidbody2D>();
        rb.velocity = side * 20f;
        Invoke("StopObject", 2f);
    }
    private void StopObject()
    {
        _isFlying = false;
        rb.velocity = Vector2.zero;
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        print(col.gameObject.name);
        if (_isFlying)
        {
            if (col.gameObject.GetComponent<Player>() != null)
            {
                col.gameObject.GetComponent<Player>().TakeDamage(10);
                
            }
            else if (col.gameObject.GetComponent<Animal>() != null)
            {
                col.gameObject.GetComponent<Animal>().TakeDamage(10);
            }
            StopObject();
        }
    }
}
