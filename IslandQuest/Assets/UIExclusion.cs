using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIExclusion : MonoBehaviour
{
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = gameObject.GetComponent<Camera>();

        if (PlayerInput.GetPlayerByIndex(0).transform == gameObject.GetComponentInParent<Transform>())
        {
            Debug.Log("code");
            _camera.cullingMask ^= 1 << LayerMask.NameToLayer("P1 UI");
            _camera.cullingMask ^= 1 << LayerMask.NameToLayer("P2 UI");
            _camera.cullingMask ^= 1 << LayerMask.NameToLayer("P1 Cam");
            _camera.cullingMask ^= 1 << LayerMask.NameToLayer("P2 Cam");
            gameObject.layer = 9;
        }
    }
}
