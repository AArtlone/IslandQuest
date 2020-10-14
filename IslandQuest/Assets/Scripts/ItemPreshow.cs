using UnityEngine;

public class ItemPreshow : MonoBehaviour
{
    private float _roatationSpeed = 25;
    private void Update()
    {
        //transform.Rotate(Vector3.up * (_roatationSpeed * Time.deltaTime));
        transform.Rotate(0f, 0f, 1f * (_roatationSpeed * Time.deltaTime));
    }
}
