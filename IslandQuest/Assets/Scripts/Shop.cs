using LitJson;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public Player Player;
    [System.NonSerialized]
    public int CategoryIndex = 0;
    public int ItemIndex = 0;
    public List<Category> AllCategories = new List<Category>();
    public List<GameObject> AllCategoryButtons = new List<GameObject>();
    public Transform RecipeContainer;
    public Transform BackgroundContainer;
    public Image BuyButton;
    public TextMeshProUGUI ItemDescription;

    public GameObject ItemPrefab; // A prefab to be instantiated in the Item Container
    public GameObject RecipeElementPrefab; // A prefab to be instantiated in the Recipe Container
    //public GameObject ItemDescriptionPrefab;

    [Header("Sprites to be used for toggling")]
    public Sprite CategoryButtonSelected;
    public Sprite CategoryButtonDeselected;
    public Sprite BuyButtonSelected;
    public Sprite BuyButtonDeSelected;

    [System.NonSerialized]
    public Item SelectedItem;
    [System.NonSerialized]
    public Category SelectedCategory;
    private List<KeyValuePair<Resource.ResourceType, int>> _tempResourceList = new List<KeyValuePair<Resource.ResourceType, int>>(); // List that hold data for resources needed for crating amount
    private List<Item> _tempItemList = new List<Item>(); // List that hold data for items needed for crating 
    private List<GameObject> _recipePrefabs = new List<GameObject>();
    private int _matchedItemsCount;
    private Item.ItemType _matchedItemType;

    private void Awake()
    {
        AllCategories[0].InstantiateItemPrefabsInTheContainer();
        SelectedCategory = AllCategories[0];
        SelectItem();
        gameObject.layer = 9;
    }

    // Comment out before pushing
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.B) && Player.IsShopOpen)
        {
            if (CanCraftItem())
                CraftItem();
        }
    }
    public void ToggleShop()
    {
        ItemIndex = 0;
        gameObject.SetActive(!gameObject.activeSelf);
        DeselectAllItems();
        AllCategoryButtons[CategoryIndex].GetComponentInChildren<Image>().sprite = CategoryButtonSelected;
        ToggleBuyButton();
        SelectItem();
    }
    public bool CanCraftItem()
    {
        _tempResourceList.Clear();
        TextAsset itemRecipes = Resources.Load<TextAsset>("Item Recipes");
        JsonData itemRecipesJson = JsonMapper.ToObject(itemRecipes.text);
        int counter = 0; // Counter that checks if the player has enough of each resource needed to craft an item
        for (int i = 0; i < itemRecipesJson["Recipes"].Count; i++)
        {
            if (itemRecipesJson["Recipes"][i]["Name"].ToString() == SelectedItem.Name)
            {
                for (int j = 0; j < itemRecipesJson["Recipes"][i]["RequieredResources"].Count; j++)
                {
                    JsonData ItemInfo = itemRecipesJson["Recipes"][i]["RequieredResources"][j];
                    foreach (Resource resource in Player.AllResources)
                    {
                        Resource.ResourceType resourceType = (Resource.ResourceType)System.Enum.Parse(typeof(Resource.ResourceType), ItemInfo["ResourceType"].ToString());
                        if (resource.Type == resourceType)
                        {
                            int amountNeeded = int.Parse(ItemInfo["Amount"].ToString());
                            if (resource.Amount >= amountNeeded)
                            {
                                counter++;
                                _tempResourceList.Add(new KeyValuePair<Resource.ResourceType, int>(resourceType, amountNeeded));
                                break;
                            }
                        }
                    }
                }
                _matchedItemsCount = 0;
                for (int k = 0; k < itemRecipesJson["Recipes"][i]["RequieredItems"].Count; k++)
                {
                    JsonData ItemInfo = itemRecipesJson["Recipes"][i]["RequieredItems"][k];
                    int amountNeeded = int.Parse(ItemInfo["Amount"].ToString());
                    Item.ItemType itemType = (Item.ItemType)System.Enum.Parse(typeof(Item.ItemType), ItemInfo["ItemType"].ToString());
                    _matchedItemType = itemType;
                    foreach (Item _item in Player.AllItems)
                    {
                        if (_item.Type == itemType)
                        {
                            if (_matchedItemsCount < amountNeeded)
                                _matchedItemsCount++;
                        }
                    }
                    if (_matchedItemsCount >= int.Parse(ItemInfo["Amount"].ToString()))
                    {
                        counter++;
                    }
                }


                if (counter == itemRecipesJson["Recipes"][i]["RequieredResources"].Count + itemRecipesJson["Recipes"][i]["RequieredItems"].Count)
                {
                    if (!Player.Inventory.GetComponent<Inventory>().IsInventoryFull())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void CraftItem()
    {
        UpdateShopResourcesAndItemsAmounts();
        InvSlotContent inventorySlotContent = new InvSlotContent(SelectedItem);
        Player.Inventory.GetComponent<Inventory>().AddItem(inventorySlotContent, _tempResourceList, new KeyValuePair<Item.ItemType, int>(_matchedItemType, _matchedItemsCount));
        Player.AllItems.Add(SelectedItem);
        //ChallengesManager.Instance.CheckForChallenge(SelectedItem.Type, Player);
    }
    public void UpdateShopResourcesAndItemsAmounts()
    {
        foreach (GameObject obj in _recipePrefabs)
        {
            RecipeElement recipeElement = obj.GetComponent<RecipeElement>();
            foreach (Resource resource in Player.AllResources)
            {
                if (recipeElement.IsResouce && recipeElement.ResourceType == resource.Type)
                {
                    obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = resource.Amount.ToString() + " / " + recipeElement.AmountNeeded.ToString();
                }
            }

            int itemsNeededNumber = 0;
            if (recipeElement.IsItem)
            {
                itemsNeededNumber = recipeElement.AmountNeeded;
            }
            for (int i = 0; i < itemsNeededNumber; i++)
            {
                int itemNeededInInventory = 0;
                foreach (Item item in Player.AllItems)
                {
                    if (recipeElement.IsItem && recipeElement.ItemType == item.Type)
                    {
                        itemNeededInInventory++;
                    }
                }
                if (itemNeededInInventory > 0)
                {
                    obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (itemNeededInInventory - itemsNeededNumber).ToString() + " / " + recipeElement.AmountNeeded.ToString();
                    //return;
                }
            }
        }
    }
    public void SelectingShopCategory(string _direction)
    {
        int _categoriesCount = AllCategoryButtons.Count;
        foreach (GameObject button in AllCategoryButtons)
        {
            button.GetComponentInChildren<Image>().sprite = CategoryButtonDeselected;
        }
        ClearItemContainer();
        if (_direction == "Down")
        {
            if (CategoryIndex < _categoriesCount - 1)
            {
                CategoryIndex++;
            }
            else if (CategoryIndex == _categoriesCount - 1)
            {
                CategoryIndex = 0;
            }
            MoveCategoryButtonsUp();
        } else if (_direction == "Up")
        {
            if (CategoryIndex == 0)
            {
                CategoryIndex = _categoriesCount - 1;
            } else
            {
                CategoryIndex--;
            }
            MoveCategoryButtonsDown();
        }
        SelectedCategory = AllCategories[CategoryIndex];
        AllCategoryButtons[CategoryIndex].GetComponentInChildren<Image>().sprite = CategoryButtonSelected;
        SelectedCategory.InstantiateItemPrefabsInTheContainer();
        ItemIndex = 0;
        SelectItem();
        if(CanCraftItem())
        {
            ToggleBuyButton();
        }
    }
    public void ToggleBuyButton()
    {
        // Check if selected item can be crafted and toggling BuyButtton
        /*if (CanCraftItem())
        {
            BuyButton =
        }*/
        BuyButton.sprite = CanCraftItem() ? BuyButtonSelected : BuyButtonDeSelected;
    }
    public void SelectingShopItem(string _direction)
    {
        int itemsCount = SelectedCategory.InstantiatedItems.Count;
        if (_direction == "Left")
        {
            if (ItemIndex == 0)
            {
                ItemIndex = itemsCount - 1;
            }
            else if (CategoryIndex <= itemsCount - 1)
            {
                ItemIndex--;
            }
            SelectItem();
            MoveItemsRight();
        }
        else if (_direction == "Right")
        {
            if (ItemIndex < itemsCount - 1)
            {
                ItemIndex++;
            }
            else
            {
                ItemIndex = 0;
            }
            SelectItem();
            MoveItemsLeft();
        }
        //AllCategoryButtons[CategoryIndex].GetComponent<Image>().sprite = CategoryButtonSelected;
        //AllCategories[CategoryIndex].InstantiateItemPrefabsInTheContainer();
        if (CanCraftItem())
        {
            ToggleBuyButton();
        }
    }
    private void SelectItem()
    {
        DeselectAllItems();
        SelectedItem = SelectedCategory.CategoryItems[ItemIndex];
        SelectedCategory.InstantiatedItems[ItemIndex].transform.Find("Item Selected Background").gameObject.SetActive(true);
        FillInCraftInformation();
    }
    private void DeselectAllItems()
    {
        foreach (GameObject obj in SelectedCategory.InstantiatedItems)
        {
            obj.transform.Find("Item Selected Background").gameObject.SetActive(false);
        }
    }
    private void FillInCraftInformation()
    {
        ClearRecipeContainer();
        FillInRecipeContainer();
        FillInItemDescription();
    }
    private void DeselectCategory()
    {
        foreach (GameObject obj in SelectedCategory.InstantiatedItems)
        {
            obj.transform.GetChild(0).gameObject.SetActive(false);
        }
        SelectedCategory = null;
    }
    private void FillInItemDescription()
    {
        ItemDescription.text = SelectedItem.Description;
    }
    public void FillInRecipeContainer()
    {
        TextAsset itemRecipes = Resources.Load<TextAsset>("Item Recipes");
        JsonData itemRecipesJson = JsonMapper.ToObject(itemRecipes.text);
        for (int i = 0; i < itemRecipesJson["Recipes"].Count; i++)
        {
            if (SelectedItem.Name == itemRecipesJson["Recipes"][i]["Name"].ToString())
            {
                for (int j = 0; j < itemRecipesJson["Recipes"][i]["RequieredResources"].Count; j++)
                {
                    JsonData ItemInfo = itemRecipesJson["Recipes"][i]["RequieredResources"][j];
                    RecipeElement recipeElement = Instantiate(RecipeElementPrefab, RecipeContainer).GetComponent<RecipeElement>();
                    _recipePrefabs.Add(recipeElement.gameObject);

                    recipeElement.IsResouce = true;
                    Resource.ResourceType resourceType = (Resource.ResourceType)System.Enum.Parse(typeof(Resource.ResourceType), ItemInfo["ResourceType"].ToString());
                    recipeElement.ResourceType = resourceType;

                    recipeElement.transform.Find("Recipe Element Icon").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + ItemInfo["ResourceIcon"].ToString());

                    foreach (Resource resource in Player.AllResources)
                    {
                        if (resource.Type == resourceType)
                        {
                            recipeElement.Amount = resource.Amount;
                            break; // Not sure
                        }
                    }
                    recipeElement.AmountNeeded = int.Parse(itemRecipesJson["Recipes"][i]["RequieredResources"][j]["Amount"].ToString());
                    recipeElement.transform.Find("Recipe Element Amount").GetComponent<TextMeshProUGUI>().text = recipeElement.Amount.ToString() + " / " + recipeElement.AmountNeeded;

                }
                for (int k = 0; k < itemRecipesJson["Recipes"][i]["RequieredItems"].Count; k++)
                {
                    JsonData ItemInfo = itemRecipesJson["Recipes"][i]["RequieredItems"][k];
                    RecipeElement recipeElement = Instantiate(RecipeElementPrefab, RecipeContainer).GetComponent<RecipeElement>();
                    _recipePrefabs.Add(recipeElement.gameObject);

                    recipeElement.GetComponent<RecipeElement>().IsItem = true;
                    Item.ItemType itemType = (Item.ItemType)System.Enum.Parse(typeof(Item.ItemType), ItemInfo["ItemType"].ToString());
                    recipeElement.GetComponent<RecipeElement>().ItemType = itemType;

                    recipeElement.transform.Find("Recipe Element Icon").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + ItemInfo["ItemIcon"].ToString());

                    int matchedItemsCount = 0;
                    foreach (Item item in Player.AllItems)
                    {
                        if (item.Type == itemType)
                            matchedItemsCount++;
                    }
                    recipeElement.AmountNeeded = int.Parse(itemRecipesJson["Recipes"][i]["RequieredItems"][k]["Amount"].ToString());
                    recipeElement.Amount = matchedItemsCount;

                    recipeElement.transform.Find("Recipe Element Amount").GetComponent<TextMeshProUGUI>().text = recipeElement.Amount.ToString() + " / " + recipeElement.AmountNeeded.ToString();
                    /*recipeElement.transform.Find("Recipe Element Amount Needed").GetComponent<TextMeshProUGUI>().text = itemRecipesJson["Recipes"][i]["RequieredItems"][k]["Amount"].ToString();*/
                }
            }
        }
    }
    private void MoveItemsLeft()
    {
        List<float> AllPositions = new List<float>();
        List<GameObject> categoryItems = SelectedCategory.InstantiatedItems;
        foreach (GameObject item in categoryItems)
        {
            AllPositions.Add(item.GetComponent<RectTransform>().localPosition.x);
        }
        for (int i = 0; i < categoryItems.Count; i++)
        {
            int positionIndex = i;
            if (i == 0)
            {
                positionIndex = categoryItems.Count;
            }
            Vector3 _newPosition = new Vector3(AllPositions[positionIndex - 1], categoryItems[i].transform.localPosition.y, categoryItems[i].transform.localPosition.z);
            categoryItems[i].transform.localPosition = _newPosition;
        }
    }
    private void MoveItemsRight()
    {
        List<float> AllPositions = new List<float>();
        List<GameObject> categoryItems = SelectedCategory.InstantiatedItems;
        foreach (GameObject item in categoryItems)
        {
            AllPositions.Add(item.GetComponent<RectTransform>().localPosition.x);
        }
        for (int i = 0; i < categoryItems.Count; i++)
        {
            int positionIndex = i;
            if (i == categoryItems.Count - 1)
            {
                positionIndex = -1;
            }
            Vector3 _newPosition = new Vector3(AllPositions[positionIndex + 1], categoryItems[i].transform.localPosition.y, categoryItems[i].transform.localPosition.z);
            categoryItems[i].transform.localPosition = _newPosition;
        }
    }
    public void MoveCategoryButtonsUp()
    {
        List<float> AllPositions = new List<float>();
        foreach (GameObject button in AllCategoryButtons)
        {
            AllPositions.Add(button.GetComponent<RectTransform>().localPosition.y);
        }
        for (int i = 0; i < AllCategoryButtons.Count; i++)
        {
            int positionIndex = i;
            if (i == 0)
            {
                positionIndex = AllCategoryButtons.Count;
            }
            Vector3 _newPosition = new Vector3(AllCategoryButtons[i].transform.localPosition.x, AllPositions[positionIndex - 1], AllCategoryButtons[i].transform.localPosition.z);
            AllCategoryButtons[i].transform.localPosition = _newPosition;
        }
    }
    public void MoveCategoryButtonsDown()
    {
        List<float> AllPositions = new List<float>();
        foreach (GameObject button in AllCategoryButtons)
        {
            AllPositions.Add(button.GetComponent<RectTransform>().localPosition.y);
        }
        for (int i = 0; i < AllCategoryButtons.Count; i++)
        {
            int positionIndex = i;
            if (i == AllCategoryButtons.Count - 1)
            {
                positionIndex = -1;
            }
            Vector3 _newPosition = new Vector3(AllCategoryButtons[i].transform.localPosition.x, AllPositions[positionIndex + 1], AllCategoryButtons[i].transform.localPosition.z);
            AllCategoryButtons[i].transform.localPosition = _newPosition;
        }
    }
    public void ClearItemContainer()
    {
        for (int i = 0; i < SelectedCategory.ItemsContainer.transform.childCount; i++)
        {
            Destroy(SelectedCategory.ItemsContainer.transform.GetChild(i).gameObject);
            ItemIndex = 0;
            SelectedCategory.InstantiatedItems.Clear();
        }
    }
    public void ClearRecipeContainer()
    {
        _recipePrefabs.Clear();
        for (int i = 0;  i < RecipeContainer.transform.childCount; i++)
        {
            Destroy(RecipeContainer.transform.GetChild(i).gameObject);
        }
    }
}
