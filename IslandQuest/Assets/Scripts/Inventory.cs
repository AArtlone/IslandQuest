using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public Player Player;
    public int InventoryLimit = 8;
    [Tooltip("How much time before inventory turns off")]
    public int TogglingTimer = 5;
    public GameObject ItemsContainer;
    public GameObject HandEqSlot;
    public GameObject BodyEqSlot;
    public Image InvHint;
    [Header("Dodge Icon Related")]
    public GameObject DodgeIcon;
    public Sprite DodgeIconNormal;
    public Sprite DodgeIconDimmed;
    public GameObject DodgeButtonImage;
    public GameObject DodgeTextObject;
    [Header("Sprites")]
    public Sprite InvHintEquipDrop;
    public Sprite InvHintEquipDropConsume;
    [Space(2f)]
    public Transform IconsContainer;
    [Header("Birdge Pieces")]
    public Sprite BridgeLeft;
    public Sprite BridgeRight;
    public Sprite BridgeDown;
    public Sprite BridgeUp;
    [Header("Initial Items")]
    public Item PickAxe;
    public Item Axe;
    [Header("Prefabs")]
    public GameObject TorchPrefab;
    public GameObject BridgePrefab;
    public GameObject CampfirePrefab;
    public Item EmptyBucket;
    public Item FullBucket;
    public GameObject InventoryItemPrefab;
    public GameObject ResourceDropPrefab;
    public GameObject ItemDropPrefab;
    public GameObject ItemOnMapPreshowPrefab;
    public GameObject TrapPrefab;
    public GameObject BlockIconPrefab;
    public GameObject ThrowIconPrefab;
    private int _invSlotIndex; // ID to know which item is currently selected
    private List<InvSlot> _allInvSlots = new List<InvSlot>();
    private InvSlot _selectedInvSlot;
    private GameObject _blockIcon;
    private GameObject _throwIcon;
    [System.NonSerialized]
    public InvSlot HandEquipment;
    [System.NonSerialized]
    public InvSlot BodyEquipment;
    [System.NonSerialized]
    public GameObject ItemOnMapPreshow;
    private GameObject _shieldInHand;
    [System.NonSerialized]
    public bool IsPreshowingItemOnMap;
    //private int _itemIdCount; // ID that indicated each item's place in the Items List
    //private InvSlotContent _selectedInvSlotContent;
    //private int _wearablesIndex;
    //public List<InvSlotContent> InventoryList = new List<InvSlotContent>();
    //private List<GameObject> _allItems = new List<GameObject>();
    //private List<GameObject> _wearables = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < ItemsContainer.transform.childCount; i++)
        {
            ItemsContainer.transform.GetChild(i).gameObject.AddComponent<InvSlot>();
            _allInvSlots.Add(ItemsContainer.transform.GetChild(i).gameObject.GetComponent<InvSlot>());
        }
        HandEquipment = _allInvSlots[_allInvSlots.Count - 2];
        BodyEquipment = _allInvSlots[_allInvSlots.Count - 1];
        _selectedInvSlot = _allInvSlots[_invSlotIndex];
        AddItem(new InvSlotContent(Axe));
        AddItem(new InvSlotContent(PickAxe));
    }
    public void AddItem(InvSlotContent inventorySlotContent, List<KeyValuePair<Resource.ResourceType, int>> resourceList, KeyValuePair<Item.ItemType, int> _pair)
    {
        foreach (KeyValuePair<Resource.ResourceType, int> pair in resourceList)
        {
            /*int amountToUpdate = 0;
            foreach (Resource resource in Player.AllResources)
            {
                if (resource.Type == pair.Key)
                {
                    amountToUpdate = resource.Amount - pair.Value;
                }
            }*/
            UpdatePlayerResources(-pair.Value, pair.Key);
        }
        List<InvSlot> tempList = new List<InvSlot>();
        int matcheCounter = 0;
        foreach (InvSlot invSlot in _allInvSlots)
        {
            if (matcheCounter < _pair.Value)
            {
                if (invSlot.IsOccupied && invSlot.InvSlotContent.IsItem && invSlot.InvSlotContent.Item.Type == _pair.Key)
                {
                    matcheCounter++;
                    tempList.Add(invSlot);
                }
            }
        }
        foreach (InvSlot slot in tempList)
        {
            Player.AllItems.Remove(slot.InvSlotContent.Item);
            if (slot == HandEquipment)
                CancelItemOnMapPreshow();
            slot.ResetInvSlot();
        }
        foreach (InvSlot invSlot in _allInvSlots)
        {
            if (!invSlot.IsOccupied)
            {
                invSlot.InvSlotContent = inventorySlotContent;
                invSlot.IsOccupied = true;
                GameObject invSlotObject = Instantiate(InventoryItemPrefab, invSlot.gameObject.transform);
                invSlot.Object = invSlotObject;
                invSlotObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + inventorySlotContent.IconName);
                invSlotObject.transform.GetChild(1).gameObject.SetActive(false);
                return;
            }
        }
    }
    public bool IsInventoryFull()
    {
        int fullSlotsCount = 0;
        foreach (InvSlot invSlot in _allInvSlots)
        {
            if (invSlot.IsOccupied && invSlot != HandEquipment && invSlot != BodyEquipment)
            {
                fullSlotsCount++;
            }
        }
        if (fullSlotsCount >= InventoryLimit)
        {
            //TODO: Let player know there is no more space
            Debug.Log("Inventory is full");
            return true;
        } else
        {
            return false;
        }
    }
    public bool IsInventoryFull(ResourceDrop resourceDrop)
    {
        foreach (InvSlot slot in _allInvSlots)
        {
            if (slot.IsOccupied && slot.InvSlotContent.Resource && slot.InvSlotContent.ResourceDrop.Type == resourceDrop.Type)
            {
                return false;
            }
        }
        int fullSlotsCount = 0;
        foreach (InvSlot invSlot in _allInvSlots)
        {
            if (invSlot.IsOccupied && invSlot != HandEquipment && invSlot != BodyEquipment)
            {
                fullSlotsCount++;
            }
        }
        if (fullSlotsCount >= InventoryLimit)
        {
            //TODO: Let player know there is no more space
            Debug.Log("Inventory is full");
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AddItem(InvSlotContent inventorySlotContent)
    {
        bool isDuplicate = false;
        if (inventorySlotContent.Resource)
        {
            foreach (InvSlot invSlot in _allInvSlots)
            {
                if (invSlot.IsOccupied && invSlot.InvSlotContent.Resource)
                {
                    if (invSlot.InvSlotContent.ResourceDrop.Type == inventorySlotContent.ResourceDrop.Type)
                    {
                        isDuplicate = true;
                    }
                }
            }
            if (isDuplicate)
            {
                //int newAmount = 0;
                foreach (InvSlot invSlot in _allInvSlots)
                {
                    if (invSlot.IsOccupied && invSlot.InvSlotContent.Resource)
                    {
                        if (invSlot.InvSlotContent.ResourceDrop.Type == inventorySlotContent.ResourceDrop.Type)
                        {
                            //newAmount = invSlot.InvSlotContent.Amount + inventorySlotContent.Amount;
                            UpdatePlayerResources(inventorySlotContent.Amount, invSlot.InvSlotContent.ResourceDrop.Type);
                        }
                    }
                }
            }
            else
            {
                foreach (InvSlot invSlot in _allInvSlots)
                {
                    if (!invSlot.IsOccupied)
                    {
                        invSlot.InvSlotContent = inventorySlotContent;
                        invSlot.IsOccupied = true;
                        GameObject invSlotObject = Instantiate(InventoryItemPrefab, invSlot.gameObject.transform);
                        invSlot.Object = invSlotObject;
                        invSlotObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + inventorySlotContent.IconName);
                        UpdatePlayerResources(inventorySlotContent.Amount, inventorySlotContent.ResourceDrop.Type);
                        return;
                    }
                }
            }
        } else
        {
            foreach (InvSlot invSlot in _allInvSlots)
            {
                if (!invSlot.IsOccupied)
                {
                    invSlot.InvSlotContent = inventorySlotContent;
                    invSlot.IsOccupied = true;
                    GameObject invSlotObject = Instantiate(InventoryItemPrefab, invSlot.gameObject.transform);
                    invSlot.Object = invSlotObject;
                    invSlotObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + inventorySlotContent.IconName);
                    invSlotObject.transform.GetChild(1).gameObject.SetActive(false);
                    return;
                }
            }
        }
    }
    public void FillUpBucket()
    {
        HandEquipment.ResetInvSlot();
        InvSlotContent content = new InvSlotContent(FullBucket);
        AssigHandEquipment(content);
    }
    public void ClearBucket()
    {
        HandEquipment.ResetInvSlot();
        InvSlotContent content = new InvSlotContent(EmptyBucket);
        AssigHandEquipment(content);
    }
    public bool CanDrop()
    {
        if (_selectedInvSlot.IsOccupied)
            return true;
        return false;
    }
    public bool CanEquip()
    {
        if (_selectedInvSlot.IsOccupied)
            return true;
        return false;
    }
    public void DropItem()
    {
        if (_selectedInvSlot != null && _selectedInvSlot.IsOccupied)
        {
            if (_selectedInvSlot.InvSlotContent.Resource)
            {
                GameObject ResourceDrop = Instantiate(ResourceDropPrefab, Player.transform.position, Quaternion.identity);
                ResourceDrop.GetComponent<ResourceDrop>().Type = _selectedInvSlot.InvSlotContent.ResourceDrop.Type;
                ResourceDrop.GetComponent<ResourceDrop>().Amount = 1;
                ResourceDrop.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + _selectedInvSlot.InvSlotContent.SpriteName);
                UpdatePlayerResources(-1, _selectedInvSlot.InvSlotContent.ResourceDrop.Type);
            } else if (_selectedInvSlot.InvSlotContent.IsItem)
            {
                GameObject itemDrop = Instantiate(ItemDropPrefab, Player.transform.position, Quaternion.identity);
                itemDrop.GetComponent<ItemDrop>().Type = _selectedInvSlot.InvSlotContent.Item.Type;
                itemDrop.GetComponent<ItemDrop>().Item = _selectedInvSlot.InvSlotContent.Item;
                itemDrop.GetComponent<ItemDrop>().Name = _selectedInvSlot.InvSlotContent.Name;
                itemDrop.GetComponent<ItemDrop>().IconName = _selectedInvSlot.InvSlotContent.IconName;
                itemDrop.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + _selectedInvSlot.InvSlotContent.SpriteName);
                Player.AllItems.Remove(_selectedInvSlot.InvSlotContent.Item);
                if (_selectedInvSlot.InvSlotContent.Item.Type == Item.ItemType.Shield)
                {
                    ToggleBlockIcon(false);
                    CancelShieldOnMapPreshow();
                }
                else
                {
                    ToggleThrowIcon(false);
                    CancelItemOnMapPreshow();
                }
                ToggleInvHint(false);
                _selectedInvSlot.ResetInvSlot();
                Player.DamageValue = Player.BasicDamageValue;
            }
        }
    }
    public void ThrowItem()
    {
        if (HandEquipment != null)
        {
            if (HandEquipment.InvSlotContent.Resource)
            {
                GameObject resourceDrop = Instantiate(ResourceDropPrefab, Player.transform.position, Quaternion.identity);
                resourceDrop.GetComponent<ResourceDrop>().Type = HandEquipment.InvSlotContent.ResourceDrop.Type;
                resourceDrop.GetComponent<ResourceDrop>().Amount = HandEquipment.InvSlotContent.Amount;
                print(HandEquipment.InvSlotContent.SpriteName);
                resourceDrop.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + HandEquipment.InvSlotContent.SpriteName);
                UpdatePlayerResources(-1, HandEquipment.InvSlotContent.ResourceDrop.Type);

                //Throwing
                FlyingObject flyingObject = resourceDrop.GetComponent<FlyingObject>();
                if (Player.FacingRight)
                {
                    flyingObject.Throw(resourceDrop, resourceDrop.transform.right, Player);
                }
                else
                {
                    flyingObject.Throw(resourceDrop, -resourceDrop.transform.right, Player);
                }
            }
            else if (HandEquipment.InvSlotContent.IsItem)
            {
                ItemDrop itemDrop = Instantiate(ItemDropPrefab, Player.transform.position, Quaternion.identity).GetComponent<ItemDrop>();
                itemDrop.Type = HandEquipment.InvSlotContent.Item.Type;
                itemDrop.Item = HandEquipment.InvSlotContent.Item;
                itemDrop.Name = HandEquipment.InvSlotContent.Name;
                itemDrop.IconName = HandEquipment.InvSlotContent.IconName;
                itemDrop.DamageValue = HandEquipment.InvSlotContent.Item.DamageValue;
                itemDrop.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + HandEquipment.InvSlotContent.SpriteName);

                // Throwing
                FlyingObject flyingObject = itemDrop.GetComponent<FlyingObject>();
                if (Player.FacingRight)
                {
                    flyingObject.Throw(itemDrop.gameObject, itemDrop.transform.right, Player);
                }
                else
                {
                    flyingObject.Throw(itemDrop.gameObject, -itemDrop.transform.right, Player);
                }
                Player.AllItems.Remove(HandEquipment.InvSlotContent.Item);
                HandEquipment.ResetInvSlot();
                Player.DamageValue = Player.BasicDamageValue;
                ToggleThrowIcon(false);
                CancelItemOnMapPreshow();
            }
        }
    }
    public void ItemAction(Vector2 vector)
    {
        if (vector.y == -1)
        {
            DropItem();
            Player.HideInstructionsSprite();
            Player.ShowInstructionsSprite();
        }
        else if (vector.x == 1)
        {
            if (_selectedInvSlot.IsOccupied) 
            {
                if (_selectedInvSlot == HandEquipment)
                {
                    UnEquipHand();
                    Player.HideInstructionsSprite();
                    Player.ShowInstructionsSprite();
                }
                else if (_selectedInvSlot == BodyEquipment)
                {
                    UnEquipBody();
                    Player.HideInstructionsSprite();
                    Player.ShowInstructionsSprite();
                }
                else
                {
                    EquipItem();
                    Player.HideInstructionsSprite();
                    Player.ShowInstructionsSprite();
                }
            }
        }
        else if (vector.y == 1)
        {
            if (_selectedInvSlot.IsOccupied && _selectedInvSlot.InvSlotContent.Resource && _selectedInvSlot.InvSlotContent.ResourceDrop.Consubamle)
            {
                Consume(_selectedInvSlot.InvSlotContent.ResourceDrop);
                // Check to remove the thing from inventory
            }
        }
    }
    private void Consume(ResourceDrop resource)
    {
        if (resource.EffectOnPlayer == Resource.EffectOnPlayer.Healthy)
        {
            Player.Heal(Player.HealthyFoodHealAmount);
        }
        else if (resource.EffectOnPlayer == Resource.EffectOnPlayer.Poisonous)
        {
            Player.TakeDamage(Player.PoisonousFoodDamage);
        }
        UpdatePlayerResources(-1 , resource.Type);
    }
    public void CancelItemOnMapPreshow()
    {
        IsPreshowingItemOnMap = false;
        //EnvironementGenerator.Instance.NonAnimalObjects.Remove(ItemOnMapPreshow);
        Destroy(ItemOnMapPreshow);
    }
    public void CancelShieldOnMapPreshow()
    {
        Destroy(_shieldInHand);
    }
    public void SnapOntoRiver(Bridge bridge)
    {
        if (Player.RiverPieceToSnapTo == null)
            return;
        RiverGenerator.Side side = Player.RiverPieceToSnapTo.GetComponentInParent<RiverPiece>().Side;
        Sprite sprite = null;
        switch (side)
        {
            case RiverGenerator.Side.DownIsh:
                sprite = BridgeDown;
                break;
            case RiverGenerator.Side.UpIsh:
                sprite = BridgeUp;
                break;
            case RiverGenerator.Side.LeftIsh:
                sprite = BridgeLeft;
                break;
            case RiverGenerator.Side.RightIsh:
                sprite = BridgeRight;
                break;
        }
        bridge.GetComponent<Bridge>().PlaceBridge(BridgePrefab, sprite, HandEquipment.InvSlotContent.Item, HandEquipment.InvSlotContent.Name, HandEquipment.InvSlotContent.IconName, Player);
        //bridge.GetComponent<Bridge>().RiverPieceToSnapTo.transform.GetChild(0).gameObject.SetActive(false);
        CancelItemOnMapPreshow();
        HandEquipment.ResetInvSlot();
        ToggleThrowIcon(false);
        ToggleInvHint(false);
        Player.DamageValue = Player.BasicDamageValue;
    }
    public void PlaceObjectOnMap()
    {
        if (HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Bridge)
        {
            SnapOntoRiver(ItemOnMapPreshow.GetComponent<Bridge>());
            return;
        }
        else if (HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Campfire)
        {
            GameObject objectOnMap = Instantiate(CampfirePrefab, ItemOnMapPreshow.transform.position, Quaternion.identity);
            objectOnMap.GetComponent<Campfire>().LinkedPlayer = Player;
            ItemDrop itemDrop = objectOnMap.AddComponent<ItemDrop>();
            itemDrop.Type = HandEquipment.InvSlotContent.Item.Type;
            itemDrop.Item = HandEquipment.InvSlotContent.Item;
            itemDrop.Name = HandEquipment.InvSlotContent.Name;
            itemDrop.IconName = HandEquipment.InvSlotContent.IconName;
            itemDrop.DamageValue = HandEquipment.InvSlotContent.Item.DamageValue;
            ToggleThrowIcon(false);
        }
        else
        {
            GameObject objectOnMap = Instantiate(TrapPrefab, ItemOnMapPreshow.transform.position, Quaternion.identity);
            objectOnMap.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + HandEquipment.InvSlotContent.SpriteName);
            /*ItemDrop itemDrop = objectOnMap.AddComponent<ItemDrop>();
            itemDrop.Type = HandEquipment.InvSlotContent.Item.Type;
            itemDrop.Item = HandEquipment.InvSlotContent.Item;
            itemDrop.Name = HandEquipment.InvSlotContent.Name;
            itemDrop.IconName = HandEquipment.InvSlotContent.IconName;
            itemDrop.DamageValue = HandEquipment.InvSlotContent.Item.DamageValue;*/
            ToggleThrowIcon(false);
        }
        ToggleInvHint(false);
        IsPreshowingItemOnMap = false;
        CancelItemOnMapPreshow();
        HandEquipment.ResetInvSlot();
        Player.DamageValue = Player.BasicDamageValue;
    }
    private void EquipItem()
    {
        if (_selectedInvSlot.InvSlotContent.Resource)
        {
            if (HandEquipment.IsOccupied)
            {
                ToggleThrowIcon(false);
                ToggleInvHint(false);
                SwapItems(HandEquipment, "Hand");
            }
            else
            {
                AssigHandEquipment();
            }
            return;
        }
        if (_selectedInvSlot.InvSlotContent.Item.Type != Item.ItemType.Shield)
        {
            Item.ItemType type = _selectedInvSlot.InvSlotContent.Item.Type;
            /*if (type == Item.ItemType.Torch || type == Item.ItemType.Pickaxe || type == Item.ItemType.Axe)
                Player.GetClosestObject("Enter");*/
            if (HandEquipment.IsOccupied)
            {
                ToggleThrowIcon(false);
                ToggleInvHint(false);
                SwapItems(HandEquipment, "Hand");
            } else
            {
                AssigHandEquipment();
            }
        }
        else if (_selectedInvSlot.InvSlotContent.Item.Type == Item.ItemType.Shield)
        {
            if (BodyEquipment.IsOccupied)
            {
                ToggleBlockIcon(false);
                SwapItems(BodyEquipment, "Body");
            } else
            {
                AssignBodyEquipment();
            }
        }
    }
    private void InstantiateItemInHand()
    {
        if (ItemOnMapPreshow != null)
            Destroy(ItemOnMapPreshow);
        if (HandEquipment.InvSlotContent.Resource)
        {
            ItemOnMapPreshow = Instantiate(ItemOnMapPreshowPrefab, Player.HandPosition);
            ItemOnMapPreshow.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + HandEquipment.InvSlotContent.IconName);
            //EnvironementGenerator.Instance.NonAnimalObjects.Add(ItemOnMapPreshow);
            return;
        }
        Item.ItemType type = HandEquipment.InvSlotContent.Item.Type;
        if (type == Item.ItemType.Axe || type == Item.ItemType.Pickaxe || type == Item.ItemType.Sword)
        {
            ItemOnMapPreshow = Instantiate(ItemOnMapPreshowPrefab, Player.WeaponToolPosition);
        } else if (type == Item.ItemType.Torch)
        {
            ItemOnMapPreshow = Instantiate(TorchPrefab, Player.HandPosition);
        }
        else
        {
            ItemOnMapPreshow = Instantiate(ItemOnMapPreshowPrefab, Player.HandPosition);
        }
        if (type == Item.ItemType.Torch)
        {
            ItemOnMapPreshow.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + HandEquipment.InvSlotContent.IconName + " Unlit");
        }
        else
        {
            ItemOnMapPreshow.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + HandEquipment.InvSlotContent.IconName);
        }
        if (HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Campfire || HandEquipment.InvSlotContent.Item.Type == Item.ItemType.BearTrap)
        {
            IsPreshowingItemOnMap = true;
            ItemOnMapPreshow.transform.Find("Radius").gameObject.SetActive(true);
            ItemOnMapPreshow.transform.Find("Instructions Image").gameObject.SetActive(true);
            ItemOnMapPreshow.transform.Find("Radius").gameObject.AddComponent<ItemPreshow>();
        }
        else if (HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Bridge)
        {
            IsPreshowingItemOnMap = true;
            ItemOnMapPreshow.AddComponent<Bridge>();
            ItemOnMapPreshow.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    private void InstantiateShieldInHand()
    {
        if (_shieldInHand != null)
            Destroy(_shieldInHand);
        _shieldInHand = Instantiate(ItemOnMapPreshowPrefab, Player.ShieldInHandPosition);
        _shieldInHand.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + BodyEquipment.InvSlotContent.IconName);
    }
    private void AssigHandEquipment()
    {
        HandEquipment.InvSlotContent = _selectedInvSlot.InvSlotContent;
        HandEquipment.IsOccupied = true;
        GameObject handEquipmentObj = Instantiate(InventoryItemPrefab, HandEqSlot.transform);
        HandEquipment.Object = handEquipmentObj;
        handEquipmentObj.transform.GetChild(1).gameObject.SetActive(false);
        handEquipmentObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + HandEquipment.InvSlotContent.IconName);
        if (HandEquipment.InvSlotContent.Resource)
        {
            Player.DamageValue = Player.BasicDamageValue;
        }
        else
        {
            Player.DamageValue = HandEquipment.InvSlotContent.Item.DamageValue;
        }
        InstantiateItemInHand();
        _selectedInvSlot.ResetInvSlot();
        ToggleThrowIcon(true);
        ToggleInvHint(false);
    }
    private void AssigHandEquipment(InvSlotContent content)
    {
        HandEquipment.InvSlotContent = content;
        HandEquipment.IsOccupied = true;
        GameObject handEquipmentObj = Instantiate(InventoryItemPrefab, HandEqSlot.transform);
        HandEquipment.Object = handEquipmentObj;
        handEquipmentObj.transform.GetChild(1).gameObject.SetActive(false);
        handEquipmentObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + HandEquipment.InvSlotContent.IconName);
        if (HandEquipment.InvSlotContent.Resource)
        {
            Player.DamageValue = Player.BasicDamageValue;
        } else
        {
            Player.DamageValue = HandEquipment.InvSlotContent.Item.DamageValue;
        }
        InstantiateItemInHand();
    }
    private void AssignBodyEquipment()
    {
        BodyEquipment.InvSlotContent = _selectedInvSlot.InvSlotContent;
        _selectedInvSlot.ResetInvSlot();
        BodyEquipment.IsOccupied = true;
        GameObject bodyEquipmentObj = Instantiate(InventoryItemPrefab, BodyEqSlot.transform);
        BodyEquipment.Object = bodyEquipmentObj;
        bodyEquipmentObj.transform.GetChild(1).gameObject.SetActive(false);
        bodyEquipmentObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + BodyEquipment.InvSlotContent.IconName);
        InstantiateShieldInHand();
        ToggleBlockIcon(true);
    }
    private void SwapItems(InvSlot invSlot, string bodyPart)
    {
        InvSlotContent tempInvSlotContent = invSlot.InvSlotContent;
        invSlot.ResetInvSlot();
        if (bodyPart == "Body")
        {
            AssignBodyEquipment();
        } else if (bodyPart == "Hand")
        {
            AssigHandEquipment();
        }
        AddItem(tempInvSlotContent);
    }
    private void UnEquipHand()
    {
        if (HandEquipment.InvSlotContent.Item.Type == Item.ItemType.BearTrap  || HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Campfire)
        {
            CancelItemOnMapPreshow();
            ToggleThrowIcon(false);
            ToggleInvHint(false);
        }
        if (IsInventoryFull())
        {
            DropItem();
        }
        else
        {
            AddItem(HandEquipment.InvSlotContent);
            HandEquipment.ResetInvSlot();
            Player.DamageValue = Player.BasicDamageValue;
            CancelItemOnMapPreshow();
            ToggleThrowIcon(false);
            ToggleInvHint(false);
        }
    }
    private void UnEquipBody()
    {
        if (IsInventoryFull())
        {
            DropItem();
        } else
        {
            AddItem(BodyEquipment.InvSlotContent);
            BodyEquipment.ResetInvSlot();
            Player.DamageValue = Player.BasicDamageValue;
            ToggleBlockIcon(false);
            ToggleInvHint(false);
            CancelShieldOnMapPreshow();
        }
    }
    public void SelectingInvSlot(string _direction)
    {
        if (_direction == "Right")
        {
            if (_invSlotIndex >= _allInvSlots.Count - 1)
            {
                _invSlotIndex = -1;
            }
            _invSlotIndex++;
        }
        else if (_direction == "Left")
        {
            if (_invSlotIndex > 0)
            {
                _invSlotIndex--;
            } else
            {
                _invSlotIndex = _allInvSlots.Count - 1;
            }
        }
        SelectSlot();
    }
    private void SelectSlot()
    {
        DeselectAllInvSlots();
        _selectedInvSlot = _allInvSlots[_invSlotIndex];
        ToggleInvHint(true);
        ActivateSelectedInvSlotBoarder();
    }
    private void ToggleInvHint(bool on)
    {
        if (on)
        {
            if (_selectedInvSlot.IsOccupied)
            {
                InvHint.gameObject.SetActive(true);

                if (_selectedInvSlot.InvSlotContent.Resource && _selectedInvSlot.InvSlotContent.ResourceDrop.Consubamle)
                {
                    InvHint.sprite = InvHintEquipDropConsume;
                }
                else
                {
                    InvHint.sprite = InvHintEquipDrop;
                }
            }
            else
            {
                InvHint.gameObject.SetActive(false);
            }
        }
        else
        {
            InvHint.gameObject.SetActive(false);
        }
    }
    private void ToggleBlockIcon(bool on)
    {
        if (on)
        {
            _blockIcon = Instantiate(BlockIconPrefab, IconsContainer);
        }
        else
        {
            Destroy(_blockIcon);
        }
    }
    private void ToggleThrowIcon(bool on)
    {
        if (on)
        {
            _throwIcon = Instantiate(ThrowIconPrefab, IconsContainer);
        }
        else
        {
            Destroy(_throwIcon);
        }
    }
    private void ActivateSelectedInvSlotBoarder()
    {
        _selectedInvSlot.transform.GetChild(1).gameObject.SetActive(true);
    }
    private void DeselectAllInvSlots()
    {
        foreach (InvSlot obj in _allInvSlots)
        {
            obj.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    private void UpdatePlayerResources(int amount, Resource.ResourceType type)
    {
        foreach (Resource resource in Player.AllResources)
        {
            if (resource.Type == type)
            {
                resource.Amount += amount;
            }
        }
        Player.Shop.GetComponent<Shop>().UpdateShopResourcesAndItemsAmounts();
        //ChallengesManager.Instance.CheckForChallenge(type, amount, Player);
        UpdatePlayerResourcesUI();
    }
    private void UpdatePlayerResourcesUI()
    {
        foreach (Resource resource in Player.AllResources)
        {
            foreach (InvSlot invSlot in _allInvSlots)
            {
                if (invSlot.IsOccupied && invSlot.InvSlotContent.Resource)
                {
                    if (invSlot.InvSlotContent.ResourceDrop.Type == resource.Type)
                    {
                        invSlot.InvSlotContent.Amount = resource.Amount;
                        if (invSlot.InvSlotContent.Amount <= 0)
                        {
                            invSlot.ResetInvSlot();
                            CancelItemOnMapPreshow();
                            ToggleThrowIcon(false);
                            ToggleInvHint(false);
                            Player.DamageValue = Player.BasicDamageValue;
                        }
                        invSlot.Object.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = resource.Amount.ToString();
                    }
                }
            }
        }
    }
    public void ToggleDodgeIcon(bool activate)
    {
        if (activate)
        {
            DodgeIcon.GetComponent<Image>().sprite = DodgeIconNormal;
        }
        else
        {
            DodgeIcon.GetComponent<Image>().sprite = DodgeIconDimmed;
        }
        DodgeTextObject.SetActive(!activate);
        DodgeButtonImage.SetActive(activate);
    }
    private void OnEnable()
    {
        ActivateSelectedInvSlotBoarder();
        if (_selectedInvSlot != null && _selectedInvSlot.IsOccupied)
        {
            ToggleInvHint(true);
        }
    }
    private void OnDisable()
    {
        InvHint.gameObject.SetActive(false);
        DeselectAllInvSlots();
    }
}
