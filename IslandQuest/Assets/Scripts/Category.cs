using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Category : MonoBehaviour
{
    public string CategoryName;
    public List<Item> CategoryItems = new List<Item>();
    [System.NonSerialized]
    public List<GameObject> InstantiatedItems = new List<GameObject>();
    public GameObject ItemsContainer;

    public void InstantiateItemPrefabsInTheContainer()
    {
        foreach (Item item in CategoryItems)
        {
            GameObject itemPrefab = Instantiate(FindObjectOfType<Shop>().ItemPrefab, ItemsContainer.transform);
            itemPrefab.transform.Find("Item Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + item.IconName);
            InstantiatedItems.Add(itemPrefab);
        }
    }
}
