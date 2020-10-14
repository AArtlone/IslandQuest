using System.Collections;
using UnityEngine;

public class ObjectOnFire : MonoBehaviour
{
    [System.NonSerialized]
    public Player PlayerLinked;
    public bool IsOnFire = false;
    private float _timeOnFire;
    private float _maximumTimeOnFire = 15f;
    private Player _nearbyPLayer;
    private ResourceDrop _resourceDrop;
    private ItemDrop _nearbyItemDrop;
    private void Update()
    {
        _timeOnFire += Time.deltaTime;
        if (_timeOnFire >= _maximumTimeOnFire)
        {
            GameObject firePrefab = transform.Find("Fire Prefab").gameObject;
            firePrefab.SetActive(false);
            if (GetComponent<Campfire>() == null)
            {
                if (GetComponent<ResourceMine>() != null)
                {
                    ResourceMine resourceMine = GetComponent<ResourceMine>();
                    resourceMine.CanBeCollected = false;
                    resourceMine.CanBeSetOnFire = false;
                    //Destroy(resourceMine);
                }
                else if (GetComponent<ResourceDrop>() != null)
                {
                    //Destroy(GetComponent<ResourceDrop>());
                }
                GameObject burntSprite = transform.Find("Burnt Sprite").gameObject;
                GetComponent<SpriteRenderer>().enabled = false;
                burntSprite.SetActive(true);
                GetComponent<BoxCollider2D>().enabled = false;
            }
            Invoke("RemoveAsh", 5f);
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        _nearbyPLayer = col.GetComponent<Player>();
        if (_nearbyPLayer != null)
        {
            StartCoroutine(Damage());
            return;
        }
        
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        _nearbyPLayer = col.GetComponent<Player>();
        if (_nearbyPLayer != null)
        {
            StopAllCoroutines();
        }
    }
    private IEnumerator Damage()
    {
        while (true)
        {
            if (_nearbyPLayer != null)
                _nearbyPLayer.TakeDamage(5);
            yield return new WaitForSeconds(2f);
        }
    }
    private void RemoveAsh()
    {
        PlayerLinked.ResetInstructionIcon();
        Destroy(this.gameObject);
    }
}
