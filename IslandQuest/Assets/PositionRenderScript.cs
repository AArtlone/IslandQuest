using Anima2D;
using UnityEngine;

public class PositionRenderScript : MonoBehaviour
{
    private int sortingOrderBase = 1000;
    [SerializeField]
    private int offset = 0;
    [SerializeField]
    private bool runOnlyOnce = false;
    [SerializeField]
    private bool _specialMesh = false;
    [SerializeField]
    private bool _particle = false;

    private float timer;
    private float timerMax = .1f;
    private Renderer myRenderer;
    private SpriteMeshInstance _meshInstance;
    private ParticleSystemRenderer _particleRendeder;
    

    // Start is called before the first frame update
    private void Awake()
    {
        if (_specialMesh)
            _meshInstance = gameObject.GetComponent<SpriteMeshInstance>();
        else if (_particle)
            _particleRendeder = gameObject.GetComponent<ParticleSystemRenderer>();
        else
            myRenderer = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = timerMax;
            if (_specialMesh)
                _meshInstance.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
            else if (_particle)
                _particleRendeder.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
            else
                myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
            if (runOnlyOnce)
            {
                Destroy(this);
            }
        }
    }
}
