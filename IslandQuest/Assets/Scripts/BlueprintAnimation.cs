using UnityEngine;

public class BlueprintAnimation : MonoBehaviour
{
    void Update()
    {
        if (transform.localScale.x < 1)
        {
            transform.localScale = new Vector3(transform.localScale.x + .1f, transform.localScale.y + .1f, transform.localScale.z + .1f);
        }
        else
        {
            Invoke("RemoveAnimationScript", 3f);
        }
    }

    private void RemoveAnimationScript()
    {
        enabled = false;
        //gameObject.SetActive(false);
        //transform.localScale = new Vector3(0f, 0f, 0f);
    }
}
