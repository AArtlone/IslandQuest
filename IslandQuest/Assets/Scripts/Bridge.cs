using UnityEngine;

public class Bridge : MonoBehaviour
{
    public void PlaceBridge(GameObject prefab, Sprite sprite, Item _item, string _name, string _iconName, Player player)
    {
        GameObject bridgeOnTheRiver = Instantiate(prefab, player.RiverPieceToSnapTo.transform.position, Quaternion.identity);
        bridgeOnTheRiver.GetComponent<SpriteRenderer>().sprite = sprite;
        player.RiverPieceToSnapTo.transform.GetChild(0).gameObject.SetActive(false);
        ItemDrop _itemDrop = bridgeOnTheRiver.AddComponent<ItemDrop>();
        _itemDrop.Type = _item.Type;
        _itemDrop.GetComponent<ItemDrop>().Item = _item;
        _itemDrop.GetComponent<ItemDrop>().Name = _name;
        _itemDrop.GetComponent<ItemDrop>().IconName = _iconName;

    }
    /*private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "River Piece")
        {
            RiverPieceToSnapTo = col.transform.parent.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "River Piece")
        {
            RiverPieceToSnapTo = null;
        }
    }*/
}
