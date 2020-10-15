using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class Player : MonoBehaviour
{
    public static readonly int MaxHealth = 100;
    public int Health = 100;
    public static readonly int BasicDamageValue = 5;
    public int DamageValue = 5;
    public int Defense = 10;
    public float MovementSpeed = 10;
    public bool IsShopOpen = false;
    public bool IsInvToggled = false;
    public bool isAttacking = false;
    public bool isDefending = false;
    public float CurrentStunTime;

    [Header("Adjustable Variables")]
    public float UITogglingSensitivity;
    public float UITogglingDelay;
    public int PoisonousFoodDamage;
    public int HealthyFoodHealAmount;

    private GameObject ClosestObject;
    private ResourceMine NearbyResourceMine;
    private ResourceDrop NearbyResourceDrop;
    private Campfire NearbyCampfire;
    public ItemDrop NearbyItemDrop;
    public int UnlockedBlueprints;

    [Header("Needs reference")]
    public GameObject Shop;
    public GameObject Inventory;
    public GameObject ResourceDropPrefab;
    public TextMeshProUGUI ShopBlueprintsText;
    public Slider HealthBar;
    public List<Image> Blueprints = new List<Image>();
    public Animator AtkRef;
    public Animator DefRef;
    [Header("Reference to characters prefabs")]
    public GameObject Character1;

    public static Player Instance;
    PlayerInputs input;
    private Shop _shop;
    private Inventory _inventory;
    
    [NonSerialized] public Transform CharacterTransform; //The transform of the character object to which movement should be applied
    [NonSerialized] public Transform HandPosition; //The transorm of the hand position of the character
    [NonSerialized] public Transform WeaponToolPosition; //The transorm of the moving hand position of the character
    [NonSerialized] public Transform ShieldInHandPosition; //The transorm of the moving hand position of the character
    [NonSerialized] public GameObject ChallengesAnnouncement;
    [NonSerialized] public GameObject ChallengesInTheShop;
    [NonSerialized] public GameObject RoundAnnouncement;
    [NonSerialized] public GameObject BlueprintsContainer;
    [NonSerialized] public Transform BlueprintsToActivateContainer;
    
    [Space(25f)] 
    public List<Resource> AllResources = new List<Resource>();
    [NonSerialized] public List<Item> AllItems = new List<Item>();
    [NonSerialized] public List<Challenge> PlayerChallenges = new List<Challenge>();
    [NonSerialized] public Animator _anim;
    private Rigidbody2D rb;
    [NonSerialized] public List<GameObject> AllInstructionsObjectsColliders = new List<GameObject>();
    private Vector2 mv;
    private Vector2 _cs; // Variable that stores the value of the left stick during category selection in the shop
    private Vector2 _is; // Variable that store the value of the left stick during item selection in the shop
    //private Vector2 rv;
    private Vector2 _iss; // Variable that store the value of the right stick during inventory slot selection in the inventory
    private Vector2 _ia; // Variable that stores the value of the left dpad; used for determining which action should be applied to the item
    private Vector2 s; // Varible that stores the value for dodging
    private float _categorySwitchingTimer;
    private float _itemSwitchingTimer;
    private float _invSlotSwitchingTimer;
    private float _invTogglingTimer; // Keep tracks of how the hasn't been using inventory
    [SerializeField]
    private List<GameObject> _instructionsToggled = new List<GameObject>();
    public bool FacingRight = true;
    public int PlayerNumber = 0;
    public bool InBase;
    private bool _canDodge = true;
    private GameObject _nearbyWaterSource;
    private bool _nearWaterSource; // TODO: add a check if the player is near water and change this variable accordingly
    public GameObject RiverPieceToSnapTo;
    private bool _firstInstruction = true; // Bolean that help to keep track of objects that to have the intsruction image toggled. It is used to know whether the _instructionsShown is not null, which cannot be done with != null in this specific case
    private GameObject _instructionsShown;
    private float _timeSinceDodge = 5f;
    float dashTime = 0.3f;
    bool canTakeDamage = true;

    [NonSerialized] public bool isDodging = false;

    public readonly static HashSet<Player> PlayerPool = new HashSet<Player>();

    public GameObject Player1UIPackage;
    public Slider Player1HP;
    public Text RespawnTimer1;
    public Transform Camp1;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    [NonSerialized] public Transform Canvas2;

    AudioSource audioSource;
    public AudioClip chop;
    public AudioClip swordAttack;
    public AudioClip dead;
    public AudioClip pickUp;
    public AudioClip button;
    public AudioClip craft;
    public AudioClip footstepsP1;
    public AudioClip ExtinguishSound;

    [NonSerialized] public bool ControlsDisabled = false;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        input = new PlayerInputs();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Player1UIPackage.SetActive(true);
        _shop = Shop.GetComponent<Shop>();
        _inventory = Inventory.GetComponent<Inventory>();
        
        AssignPlayerVariables();
      
        RespawnTimer1.enabled = false;
    }

    void Update()
    {
        ShowInstructions();
        if (Input.GetKeyUp(KeyCode.H))
        {
            PickUp();
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            BuyItem();
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            ToggleShop();
        }

        if (!ControlsDisabled)
        {
            if (!IsShopOpen)
            {
                PlayerMovement();
                InvSlotSelectionControls();
            }
            else
            {
                CategorySelectionControls();
                ItemSelectionControls();
            }
        }

        if (isDodging)
        {
            dashTime -= Time.deltaTime;

            if (dashTime <= 0)
            {
                dashTime = 0.3f;
                //MovementSpeed /= 5f;

                isDodging = false;
                _timeSinceDodge = 5;
            }
        }
        if (!_canDodge)
        {
            _timeSinceDodge -= Time.deltaTime;
            _inventory.DodgeTextObject.GetComponent<TextMeshProUGUI>().text = _timeSinceDodge.ToString("0");
            if (_timeSinceDodge <= 0)
            {
                _inventory.ToggleDodgeIcon(true);
                _canDodge = true;
                _timeSinceDodge = 5f;
            }
        }

        ShowInstructions();
    }
    private void AssignPlayerVariables()
    {
        Transform canvas = transform.Find("Canvas");

        Character1.SetActive(true);
        CharacterTransform = Character1.transform;
        GameManager.Instance.playersReady++;
        PlayerNumber = 1;
        _anim = Character1.GetComponent<Animator>();
        HealthBar = Player1HP;
        AtkRef = transform.Find("Character 1/Bones/HipBone/Torso/ArmR/Weapon Tool Position/Attack").GetComponent<Animator>();
        DefRef = transform.Find("Character 1/Bones/HipBone/Torso/ArmR/Weapon Tool Position/Shield").GetComponent<Animator>();


        _anim = CharacterTransform.GetComponent<Animator>();
        
        WeaponToolPosition = CharacterTransform.Find("Bones").Find("HipBone").Find("Torso").Find("ArmR").Find("Weapon Tool Position");
        HandPosition = CharacterTransform.Find("Right Arm").Find("Hand Position");
        ShieldInHandPosition = CharacterTransform.Find("Bones").Find("HipBone").Find("Torso").Find("ArmL");
        Canvas2 = transform.Find("Canvas 2");
        ChallengesAnnouncement = Canvas2.Find("Challenges Announcement").gameObject;
        ChallengesInTheShop = Shop.transform.Find("Challenges").gameObject;
        RoundAnnouncement = Canvas2.Find("Round Announcement").gameObject;
        BlueprintsContainer = Canvas2.Find("Boat Blueprints").gameObject;
        BlueprintsToActivateContainer = BlueprintsContainer.transform.Find("Boat Pieces");
    }
    private void OnLeftStick(InputValue value)
    {
        if (!IsShopOpen)
        {
            mv = value.Get<Vector2>();
        }
        else
        {
            _cs = value.Get<Vector2>();
            _is = value.Get<Vector2>();
        }
        if (_is == Vector2.zero)
        {
            _itemSwitchingTimer = 0f;
        }
        if (_cs == Vector2.zero)
        {
            _categorySwitchingTimer = 0f;
            //_itemSwitchingTimer = 0f;
        }
    }
    private void OnRightStick(InputValue value)
    {
        if (!IsShopOpen)
        {
            _iss = value.Get<Vector2>();
        }
        if (_iss == Vector2.zero)
        {
            _invSlotSwitchingTimer = 0f;
        }
    }
    public void OnDPad(InputValue value)
    {
        _inventory.ItemAction(value.Get<Vector2>());
    }
    public void OnButtonNorth()
    {
        if (CanThrow())
        {
            _inventory.ThrowItem();
        }
    }
    public void OnButtonSouth()
    {
        if (IsShopOpen)
        {
            BuyItem();
        }
        else
        {
            if (CanPickUp())
                PickUp();
        }
    }
    public void OnButtonEast()
    {
        if (IsShopOpen)
        {
            ToggleShop();
            return;
        }
        Attack();
    }
    // TODO: Show player what button can be pressed
    public void OnButtonWest()
    {
        if (CanSetOnFire())
        {
            SetOnFire();
            return;
        }
        else if (CanInteractWithMine())
        {
            InteractWithMine(NearbyResourceMine);
            return;
        }
        else if (CanFillUpBucket())
        {
            FillUpBucket();
            return;
        }
        else if (CanExtinguish())
        {
            Extinguish();
            return;
        }
        else if (CanPlaceObjectOnMap())
        {
            PlaceObjectOnMap();
            return;
        }
    }
    public void OnLeftTrigger()
    {
        if (_canDodge)
            Dodge();
    }
    public void OnRightShoulder()
    {
        ToggleShop();
    }
    public void OnRightTrigger()
    {
        Guard();
    }
    /*public void OnInvItemInteraction(InputValue value)
    {
        _ia = value.Get<Vector2>().normalized;
        _inventory.ItemAction(_ia);
    }
    private void OnInventorySlotSelection(InputValue value)
    {
        _iss = value.Get<Vector2>();
        if (_iss == Vector2.zero)
        {
            _invSlotSwitchingTimer = 0f;
        }
    }*/
    private bool CanThrow()
    {
        if (_inventory.HandEquipment.IsOccupied)
            return true;
        else
            return false;
    }
    private bool CanPickUp()
    {
        if (_inventory.HandEquipment.IsOccupied && _inventory.HandEquipment.InvSlotContent.IsItem && (_inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Torch || _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Bridge || _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.BearTrap || _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Campfire))
            return false;
        if (ClosestObject == null)
            return false;
        if (ClosestObject.GetComponent<ResourceDrop>() != null)
        {
            NearbyResourceDrop = ClosestObject.GetComponent<ResourceDrop>();
            if (NearbyResourceDrop.IsOnFire)
                return false;
            if (!_inventory.IsInventoryFull(NearbyResourceDrop))
            {
                return true;
            }
        }
        else if (ClosestObject.GetComponent<ItemDrop>() != null)
        {
            ItemDrop itemDrop = ClosestObject.GetComponent<ItemDrop>();
            if (itemDrop.Type == Item.ItemType.Campfire)
            {
                if (!itemDrop.GetComponent<Campfire>().IsOnFire)
                {
                    if (!_inventory.IsInventoryFull())
                    {
                        NearbyItemDrop = itemDrop;
                        return true;
                    }
                }
            }
            else
            {
                if (!_inventory.IsInventoryFull())
                {
                    NearbyItemDrop = itemDrop;
                    return true;
                }
            }
        }
        return false;
    }
    public void PickUp()
    {
        audioSource.PlayOneShot(pickUp);
        if (NearbyResourceDrop != null)
        {
            if (NearbyResourceDrop.IsOnFire)
                return;
            if (!_inventory.IsInventoryFull(NearbyResourceDrop))
            {
                InvSlotContent inventorySlotContent = new InvSlotContent(NearbyResourceDrop, NearbyResourceDrop.Amount);
                Inventory.GetComponent<Inventory>().AddItem(inventorySlotContent); _instructionsToggled.Remove(NearbyResourceDrop.transform.Find("Instructions Image").gameObject);
                _instructionsToggled.Remove(NearbyResourceDrop.transform.Find("Instructions Image").gameObject);
                _firstInstruction = true;
                Destroy(NearbyResourceDrop.gameObject);
            }
        }
        else if (NearbyItemDrop != null)
        {
            if (!_inventory.IsInventoryFull())
            {
                if (NearbyItemDrop.Type == Item.ItemType.Bridge)
                {
                    RiverPieceToSnapTo.transform.GetChild(0).gameObject.SetActive(true);
                }
                InvSlotContent inventorySlotContent = new InvSlotContent(NearbyItemDrop.Item);
                Inventory.GetComponent<Inventory>().AddItem(inventorySlotContent); _instructionsToggled.Remove(NearbyItemDrop.transform.Find("Instructions Image").gameObject);
                _instructionsToggled.Remove(NearbyItemDrop.transform.Find("Instructions Image").gameObject);
                _firstInstruction = true;
                Destroy(NearbyItemDrop.gameObject);
            }
        }
    }
    private bool CanSetOnFire()
    {
        // Checking if the player has a fire source equiped. If not then exits the function
        if (_inventory.HandEquipment.IsOccupied)
        {
            if (_inventory.HandEquipment.InvSlotContent.Resource)
            {
                return false;
            }
            else if (_inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Torch)
            {
                if (ClosestObject == null)
                    return false;
                if (ClosestObject.GetComponent<Campfire>() != null && !ClosestObject.GetComponent<Campfire>().IsOnFire)
                {
                    return true;
                }
                else if (ClosestObject.GetComponent<ResourceMine>() != null)
                {
                    ResourceMine resourceMine = ClosestObject.GetComponent<ResourceMine>();
                    if (resourceMine.CanBeSetOnFire && !resourceMine.IsOnFire)
                    {
                        return true;
                    }
                } else if (ClosestObject.GetComponent<ResourceDrop>() != null)
                {
                    ResourceDrop resourceDrop = ClosestObject.GetComponent<ResourceDrop>();
                    if (resourceDrop.CanBeSetOnFire && !resourceDrop.IsOnFire)
                        return true;
                }
            }
        }
        return false;
    }
    // Check whether the nearby object can be set on fire
    private void SetOnFire()
    {
        if (ClosestObject == null)
            return;
        if (ClosestObject.GetComponent<ResourceMine>() != null)
            NearbyResourceMine = ClosestObject.GetComponent<ResourceMine>();
        else if (ClosestObject.GetComponent<ResourceDrop>() != null)
            NearbyResourceDrop = ClosestObject.GetComponent<ResourceDrop>();
        else if (ClosestObject.GetComponent<Campfire>() != null)
            NearbyCampfire = ClosestObject.GetComponent<Campfire>();

        if (NearbyResourceMine != null && NearbyResourceMine.CanBeSetOnFire)
        {
            //SFX: fire
            ActivateFirePrefab(NearbyResourceMine.gameObject, false);
            NearbyResourceMine.IsOnFire = true;
            NearbyResourceMine.CanBeCollected = false;
            HideInstructionsSprite();
        }
        else if (NearbyResourceDrop != null && NearbyResourceDrop.CanBeSetOnFire)
        {
            //SFX: fire
            ActivateFirePrefab(NearbyResourceDrop.gameObject, false);
            NearbyResourceDrop.IsOnFire = true;
            HideInstructionsSprite();
        }
        else if (NearbyCampfire != null && NearbyCampfire)
        {
            //SFX: fire
            ActivateFirePrefab(NearbyCampfire.gameObject, true);
            NearbyCampfire.IsOnFire = true;
            HideInstructionsSprite();
        }
    }
    private void ActivateFirePrefab(GameObject obj, bool campfire)
    {
        GameObject objToSetOnFire = obj.transform.Find("Fire Prefab").gameObject;
        if (objToSetOnFire != null)
        {
            objToSetOnFire.SetActive(true);
            if (!campfire)
            {
                objToSetOnFire.transform.parent.gameObject.AddComponent<ObjectOnFire>().PlayerLinked = this;
            }
        }
    }
    private bool CanInteractWithMine()
    {
        if (_inventory.HandEquipment.IsOccupied && _inventory.HandEquipment.InvSlotContent.IsItem && (_inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Torch || _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Bridge || _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.BearTrap || _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Campfire))
            return false;
        if (ClosestObject != null && ClosestObject.GetComponent<ResourceMine>() != null)
            NearbyResourceMine = ClosestObject.GetComponent<ResourceMine>();
        if (NearbyResourceMine == null)
            return false;
        if (!NearbyResourceMine.CanBeCollected)
            return false;
        if (NearbyResourceMine.tag == "CanBeSetOnFire")
        {
            return false;
        }
        if (NearbyResourceMine.NeedsItemToInteract)
        {
            if (NearbyResourceMine.CanBeCollected && _inventory.HandEquipment.IsOccupied && _inventory.HandEquipment.InvSlotContent.Item &&_inventory.HandEquipment.InvSlotContent.Item.Type == NearbyResourceMine.NeededItem)
            {
                return true;
            }
        }
        else
        {
            return true;
        }
        return false;
    }
    private void InteractWithMine(ResourceMine mine)
    {
        // Needed piece of code to hide the Instrcution image after interacting with smth
        HideInstructionsSprite();
        _firstInstruction = true;
        NearbyResourceMine = null;
        audioSource.PlayOneShot(chop);
        _anim.SetTrigger("isChop");
        int amountmountOfDrop = mine.Amount;
        if (mine.BigMine)
            amountmountOfDrop *= 2;
        for (int i = 0; i < amountmountOfDrop; i++)
        {
            float randomNumber = UnityEngine.Random.Range(-2, -2);
            /*if (randomNumber >= -1 && randomNumber <= 1)
            {
                randomNumber *= 2;
            }*/
            Vector3 positionToSpawn;
            if (mine.Type == Resource.ResourceType.Wood)
            {
                positionToSpawn = new Vector3(mine.transform.position.x + randomNumber, mine.transform.position.y, mine.transform.position.z);
            }
            else
            {
                positionToSpawn = new Vector3(mine.transform.position.x + randomNumber, mine.transform.position.y + randomNumber, mine.transform.position.z);
            }
            ResourceDrop ResourceDrop = Instantiate(ResourceDropPrefab, positionToSpawn, Quaternion.identity).GetComponent<ResourceDrop>();

            ResourceDrop.Type = mine.Type;
            if (ResourceDrop.Type == Resource.ResourceType.Wood || ResourceDrop.Type == Resource.ResourceType.Leaf)
            {
                ResourceDrop.CanBeSetOnFire = true;
            }
            ResourceDrop.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + mine.Type.ToString());
            ResourceDrop.Consubamle = mine.ConsumableDrop;
            if (ResourceDrop.Consubamle)
            {
                ResourceDrop.EffectOnPlayer = mine.EffectOnPlayer;
            }
        }
        if (mine.WillBeDestroyed)
        {
            Destroy(mine.gameObject);
        }
        else if (mine.WillChangeSprite)
        {
            mine.GetComponent<SpriteRenderer>().sprite = mine.SpriteToChangeTo;
            mine.CanBeCollected = false;
        }
        if (mine.Type2 != Resource.ResourceType.None)
        {
            for (int i = 0; i < amountmountOfDrop; i++)
            {
                int randomNumber = UnityEngine.Random.Range(-1, 2);
                Vector3 positionToSpawn = new Vector3(mine.transform.position.x + randomNumber, mine.transform.position.y + randomNumber, mine.transform.position.z);
                ResourceDrop ResourceDrop = Instantiate(ResourceDropPrefab, positionToSpawn, Quaternion.identity).GetComponent<ResourceDrop>();

                ResourceDrop.Type = mine.Type2;
                if (ResourceDrop.Type == Resource.ResourceType.Wood || ResourceDrop.Type == Resource.ResourceType.Leaf)
                {
                    ResourceDrop.CanBeSetOnFire = true;
                }
                ResourceDrop.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + mine.Type.ToString());
                ResourceDrop.Consubamle = mine.ConsumableDrop;
                if (ResourceDrop.Consubamle)
                {
                    ResourceDrop.EffectOnPlayer = mine.EffectOnPlayer;
                }
            }
            if (mine.WillBeDestroyed)
            {
                Destroy(mine.gameObject);
            }
            else if (mine.WillChangeSprite)
            {
                mine.GetComponent<SpriteRenderer>().sprite = mine.SpriteToChangeTo;
                mine.CanBeCollected = false;
            }
        }

    }
    private bool CanFillUpBucket()
    {
        // Checking if the player is near water source and if player has Empty Bucket equiped
        if (_inventory.HandEquipment.IsOccupied && _inventory.HandEquipment.InvSlotContent.IsItem &&_inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.EmptyBucket)
        {
            if (ClosestObject != null && ClosestObject.tag == "River Piece")
                return true;
        }
        return false;
    }
    private void FillUpBucket()
    {
        //SFX: water fill sound
        _inventory.FillUpBucket();
        HideInstructionsSprite();
        ShowInstructionsSprite();
    }
    private bool CanExtinguish()
    {
        // Checking if the player has Full Bucket Equiped
        if (_inventory.HandEquipment.IsOccupied && _inventory.HandEquipment.InvSlotContent.IsItem && _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.FullBucket)
        {
            if (ClosestObject == null)
                return false;
            if (ClosestObject.GetComponent<Campfire>() != null && ClosestObject.GetComponent<Campfire>().IsOnFire)
            {
                return true;
            }
            else if (ClosestObject.GetComponent<ResourceMine>() != null)
            {
                ResourceMine resourceMine = ClosestObject.GetComponent<ResourceMine>();
                if (resourceMine.CanBeSetOnFire && resourceMine.IsOnFire)
                {
                    return true;
                }
            }
            else if (ClosestObject.GetComponent<ResourceDrop>() != null)
            {
                ResourceDrop resourceDrop = ClosestObject.GetComponent<ResourceDrop>();
                if (resourceDrop.CanBeSetOnFire && resourceDrop.IsOnFire)
                    return true;
            }
        }
        return false;
    }
    private void Extinguish()
    {
        if (ClosestObject == null)
            return;
        if (ClosestObject.GetComponent<ResourceMine>() != null)
            NearbyResourceMine = ClosestObject.GetComponent<ResourceMine>();
        else if (ClosestObject.GetComponent<ResourceDrop>() != null)
            NearbyResourceDrop = ClosestObject.GetComponent<ResourceDrop>();
        else if (ClosestObject.GetComponent<Campfire>() != null)
            NearbyCampfire = ClosestObject.GetComponent<Campfire>();

        //SFX: extinguish sound
        if (NearbyResourceMine != null && NearbyResourceMine.IsOnFire)
        {
            DisableFirePrefab(NearbyResourceMine.gameObject);
            NearbyResourceMine.IsOnFire = false;
            Destroy(NearbyResourceMine.GetComponent<ObjectOnFire>());
        }
        else if (NearbyResourceDrop != null && NearbyResourceDrop.IsOnFire)
        {
            DisableFirePrefab(NearbyResourceDrop.gameObject);
            NearbyResourceDrop.IsOnFire = false;
            Destroy(NearbyResourceDrop.GetComponent<ObjectOnFire>());
        }
        else if (NearbyCampfire != null && NearbyCampfire.IsOnFire)
        {
            DisableFirePrefab(NearbyCampfire.gameObject);
            NearbyCampfire.IsOnFire = false;
            Destroy(NearbyCampfire.GetComponent<ObjectOnFire>());
        }
        HideInstructionsSprite();
        audioSource.PlayOneShot(ExtinguishSound);
        _inventory.ClearBucket();
    }
    private void DisableFirePrefab(GameObject obj)
    {
        GameObject objToExtinguish = obj.transform.Find("Fire Prefab").gameObject;
        if (objToExtinguish != null && objToExtinguish.activeSelf)
        {
            objToExtinguish.SetActive(false);
            /*if (
            objToExtinguish.transform.parent.gameObject.GetComponent<ObjectOnFire>() != null)
            {
                objToExtinguish.transform.parent.gameObject.GetComponent<ObjectOnFire>().enabled = false;
            }*/
        }
    }
    private bool CanPlaceObjectOnMap()
    {
        if (_inventory.IsPreshowingItemOnMap)
        {
            if (_inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Bridge)
            {
                if (ClosestObject != null && ClosestObject.tag == "River Piece")
                {
                    RiverPieceToSnapTo = ClosestObject.transform.parent.gameObject;
                    return true;
                }
            }
            else if (_inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Campfire || _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.BearTrap)
                return true;
        }
        return false;
    }
    private void PlaceObjectOnMap()
    {
        //SFX: Place Trap/Place Item
        _inventory.PlaceObjectOnMap();
    }
    public void TakeDamage(int damage)
    {
        print("Damaging function works");
        //if (isDodging) return;
        if (!canTakeDamage) return;
        Health -= damage;
        HealthBar.value = Health;
        if (Health <= 0)
        {
            audioSource.PlayOneShot(dead);
            // TODO: Play death animation
            // TODO: Open game over screen
            _anim.SetTrigger("isDead");
            StartCoroutine(RespawnTimer());
        }
        else
        {
            StartCoroutine(IFrames());
            //SFX: Hurt Sound
        }
    }
    public void Heal(int healAmount)
    {
        print("Healing function works");
        if (Health >= MaxHealth)
            return;
        if (Health + healAmount >= MaxHealth)
        {
            Health = MaxHealth;
        }
        else
        {
            Health += healAmount;
            HealthBar.value = Health;
            print("Healing");
        }
    }
    public void Stun(float stunValue)
    {
        CurrentStunTime = stunValue;
        _anim.SetBool("isStunned", true);
    }
    private void PlayerMovement()
    {
        if (!ControlsDisabled)
        {
            if (!isDodging)
            {
                GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x + mv.x * MovementSpeed * Time.deltaTime, transform.position.y + mv.y * MovementSpeed * Time.deltaTime));
                _anim.SetBool("isMoving", mv != Vector2.zero);
            }
            else
            {
                transform.Translate(s * Time.deltaTime, Space.World);
                _anim.SetBool("isMoving", false);
            }
        }       
        /*Vector2 m = new Vector2(mv.x, mv.y) * MovementSpeed * Time.deltaTime;
        if (!isDodging)
        {
            transform.Translate(m, Space.World);
            _anim.SetBool("isMoving", m != Vector2.zero);
        }
        else
        {
            transform.Translate(s * Time.deltaTime, Space.World);
            _anim.SetBool("isMoving", false);
        }*/


        /*Vector2 r = new Vector2(-rv.x, -rv.y) * 100f * Time.deltaTime;
        transform.Rotate(new Vector3(0, 0, r.x), Space.World);*/

        if (mv.x < 0 && FacingRight)
        {
            FlipCharacter("Left");
        }
        else if (mv.x > 0 && !FacingRight)
        {
            FlipCharacter("Right");
        }
    }
    private void FlipCharacter(string side)
    {
        bool hasObjectInHand = false;
        if (_inventory.HandEquipment.IsOccupied && _inventory.HandEquipment.InvSlotContent.IsItem && (_inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Campfire || _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.BearTrap))
            hasObjectInHand = true;
        if (side == "Left")
        {
            FacingRight = false;
            CharacterTransform.localScale = new Vector3(-CharacterTransform.localScale.x, CharacterTransform.localScale.y, CharacterTransform.localScale.z);
            if (hasObjectInHand)
            {
                Transform itemOnMapPreshow = _inventory.ItemOnMapPreshow.transform.Find("Instructions Image");
                itemOnMapPreshow.localScale = new Vector3(-itemOnMapPreshow.localScale.x, itemOnMapPreshow.localScale.y, itemOnMapPreshow.localScale.z);
            }
        }
        else if (side == "Right")
        {
            FacingRight = true;
            CharacterTransform.localScale = new Vector3(Mathf.Abs(CharacterTransform.localScale.x), CharacterTransform.localScale.y, CharacterTransform.localScale.z);
            if (hasObjectInHand)
            {
                Transform itemOnMapPreshow = _inventory.ItemOnMapPreshow.transform.Find("Instructions Image");
                itemOnMapPreshow.localScale = new Vector3(Mathf.Abs(itemOnMapPreshow.localScale.x), itemOnMapPreshow.localScale.y, itemOnMapPreshow.localScale.z);
            }
        }
    }
    private void CategorySelectionControls()
    {
        string _direction = string.Empty;
        if (_cs != Vector2.zero)
        {
            if (_cs.y > UITogglingSensitivity)
            {
                _direction = "Up";
            }
            else if (_cs.y < -UITogglingSensitivity && _cs.x < .3f && _cs.x > -.3f)
            {
                _direction = "Down";
            }
        }
        if (_direction != string.Empty)
        {
            if (_categorySwitchingTimer == 0f)
            {
                _shop.SelectingShopCategory(_direction);
                audioSource.PlayOneShot(button);
            }
            _categorySwitchingTimer += Time.deltaTime;
            if (_categorySwitchingTimer > UITogglingDelay)
            {
                _categorySwitchingTimer = 0f;
            }
        }
    }
    private void ItemSelectionControls()
    {
        string _direction = string.Empty;
        if (_is != Vector2.zero)
        {
            if (_is.x > UITogglingSensitivity && _cs.y < .3f && _cs.y > -.3f)
            {
                _direction = "Right";
            }
            else if (_is.x < -UITogglingSensitivity && _cs.y < .3f && _cs.y > -.3f)
            {
                _direction = "Left";
            }
        }
        if (_direction != string.Empty)
        {
            if (_itemSwitchingTimer == 0f)
            {
                _shop.SelectingShopItem(_direction);
                audioSource.PlayOneShot(button);
            }
            _itemSwitchingTimer += Time.deltaTime;
            if (_itemSwitchingTimer > UITogglingDelay)
            {
                _itemSwitchingTimer = 0f;
            }
        }
    }
    private void InvSlotSelectionControls()
    {
        _invTogglingTimer += Time.deltaTime;
        string _direction = string.Empty;
        if (_iss != Vector2.zero)
        {
            if (_iss.x > 0)
            {
                _direction = "Right";
            }
            else if (_iss.x < 0)
            {
                _direction = "Left";
            }
        }
        if (_direction != string.Empty)
        {
            if (_invSlotSwitchingTimer == 0f)
            {
                if (!_inventory.enabled)
                {
                    _inventory.enabled = true;
                }
                else
                {
                    _inventory.SelectingInvSlot(_direction);
                    audioSource.PlayOneShot(button);
                }
                _invTogglingTimer = 0;
            }
            _invSlotSwitchingTimer += Time.deltaTime;
            if (_invSlotSwitchingTimer > UITogglingDelay)
            {
                _invSlotSwitchingTimer = 0f;
            }
        }
        if (_invTogglingTimer > _inventory.TogglingTimer)
        {
            _inventory.enabled = false;
        }
    }
    private void ToggleShop()
    {
        //if (!InBase) return;
        audioSource.PlayOneShot(button);
        _shop.ToggleShop();
        IsShopOpen = !IsShopOpen;
    }
    private void Attack()
    {
        ///Debug.Log("hit hit hit");
        if (!isDefending && !isAttacking && !isDodging && !ControlsDisabled)
        {
            audioSource.PlayOneShot(swordAttack);
            AtkRef.SetTrigger("Attack");
            _anim.SetTrigger("isAttacking");
            isAttacking = true;
        }
    }
    private void Guard()
    {
        if (_inventory.BodyEquipment.IsOccupied)
        {
            if (!isAttacking && !isDefending && !isDodging && !ControlsDisabled)
            {
                //SFX: block sound
                DefRef.SetTrigger("Defend");
                isDefending = true;
            }
        }
    }
    private void Dodge()
    {
        if (!isAttacking && !isDefending && !isDodging && !ControlsDisabled)
        {
            _canDodge = false;
            foreach (Player Player in PlayerPool)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Player.GetComponent<Collider2D>(), true);
            }
            foreach (Animal Animal in Animal.Pool)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Animal.GetComponent<Collider2D>(), true);
            }
            _inventory.ToggleDodgeIcon(false);
            isDodging = true;
            _anim.SetTrigger("isDodge");
            //SFX: dodge sound
            if (mv != Vector2.zero)
            {
                s = new Vector2(mv.x, mv.y) * MovementSpeed;
            }
            else
            {
                if (FacingRight)
                {
                    s = new Vector2(1, 0) * MovementSpeed;
                }
                else
                {
                    s = new Vector2(-1, 0) * MovementSpeed;
                }
            }
        
        }
    }
    private void BuyItem()
    {
        if (_shop.CanCraftItem())
        {
            audioSource.PlayOneShot(craft);
            _shop.CraftItem();
        }
           
    }
    private void OnEnable()
    {
        PlayerPool.Add(this);
        input.Player.Enable();
    }
    private void OnDisable()
    {
        PlayerPool.Remove(this);
        input.Player.Disable();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!AllInstructionsObjectsColliders.Contains(col.gameObject))
        {
            if (col.GetComponent<ResourceMine>() != null || col.GetComponent<ResourceDrop>() != null || col.GetComponent<ItemDrop>() != null || col.GetComponent<Campfire>() != null || col.tag == "River Piece")
            {
                AllInstructionsObjectsColliders.Add(col.gameObject);
            }
        }
    }
    public void ShowInstructionsSprite()
    {
        if (_inventory.HandEquipment.IsOccupied && _inventory.HandEquipment.InvSlotContent.IsItem && (_inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.Campfire | _inventory.HandEquipment.InvSlotContent.Item.Type == Item.ItemType.BearTrap))
            return;
        if (ClosestObject == null)
            _firstInstruction = true;
        if (ClosestObject != null)
        {
            if (_firstInstruction)
            {
                if (ClosestObject != null)
                {
                    if (ClosestObject.GetComponent<ResourceMine>() != null)
                    {
                        if (CanInteractWithMine() || CanSetOnFire())
                        {
                            if (ClosestObject.transform.Find("Instructions Image") != null)
                            {
                                ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                _firstInstruction = false;
                            }
                        }
                        else if (CanExtinguish())
                        {
                            if (ClosestObject.transform.Find("Instructions Image") != null)
                            {
                                ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                _firstInstruction = false;
                            }
                        }
                    }
                    else if (ClosestObject.GetComponent<ResourceDrop>() != null)
                    {
                        if (CanPickUp())
                        {
                            ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                            _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                            _firstInstruction = false;
                        } else if (CanSetOnFire())
                        {
                            ClosestObject.transform.Find("Instructions Image 2").gameObject.SetActive(true);
                            _instructionsShown = ClosestObject.transform.Find("Instructions Image 2").gameObject;
                            _firstInstruction = false;
                        }
                        else if (CanExtinguish())
                        {
                            if (ClosestObject.transform.Find("Instructions Image") != null)
                            {
                                ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                _firstInstruction = false;
                            }
                        }
                    }
                    else if (ClosestObject.GetComponent<Campfire>() != null)
                    {
                        if (CanSetOnFire())
                        {
                            if (ClosestObject.transform.Find("Instructions Image 2") != null)
                            {
                                ClosestObject.transform.Find("Instructions Image 2").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image 2").gameObject;
                                _firstInstruction = false;
                            }
                        }
                        else if (CanExtinguish())
                        {
                            if (ClosestObject.transform.Find("Instructions Image 2") != null)
                            {
                                ClosestObject.transform.Find("Instructions Image 2").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image 2").gameObject;
                                _firstInstruction = false;
                            }
                        }
                        else if (CanPickUp())
                        {
                            if (ClosestObject.transform.Find("Instructions Image") != null)
                            {
                                ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                _firstInstruction = false;
                            }
                        }
                    }
                    else if (ClosestObject.tag == "River Piece")
                    {
                        if (CanFillUpBucket())
                        {
                            if (ClosestObject.transform.Find("Instructions Image") != null)
                            {
                                ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                _firstInstruction = false;
                            }
                        }
                        else if (CanPlaceObjectOnMap())
                        {
                            if (ClosestObject.transform.Find("Instructions Image") != null)
                            {
                                ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                _firstInstruction = false;
                            }
                        }
                    }
                    else
                    {
                        if (ClosestObject.transform.Find("Instructions Image") != null)
                        {
                            ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                            _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                            _firstInstruction = false;
                        }
                    }
                }
            }
            else
            {
                if (!_instructionsShown.activeSelf)
                {
                    if (ClosestObject != null)
                    {
                        if (ClosestObject.GetComponent<ResourceMine>() != null)
                        {
                            if (CanInteractWithMine() || CanSetOnFire())
                            {
                                if (ClosestObject.transform.Find("Instructions Image") != null)
                                {
                                    ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                    _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                }
                            }
                            else if (CanExtinguish())
                            {
                                if (ClosestObject.transform.Find("Instructions Image") != null)
                                {
                                    ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                    _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                }
                            }
                        }
                        else if (ClosestObject.GetComponent<ResourceDrop>() != null)
                        {
                            if (CanPickUp())
                            {
                                ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                            }
                            else if (CanSetOnFire())
                            {
                                ClosestObject.transform.Find("Instructions Image 2").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image 2").gameObject;
                            }
                            else if (CanExtinguish())
                            {
                                if (ClosestObject.transform.Find("Instructions Image") != null)
                                {
                                    ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                    _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                }
                            }
                        }
                        else if (ClosestObject.GetComponent<Campfire>() != null)
                        {
                            if (CanSetOnFire())
                            {
                                if (ClosestObject.transform.Find("Instructions Image 2") != null)
                                {
                                    ClosestObject.transform.Find("Instructions Image 2").gameObject.SetActive(true);
                                    _instructionsShown = ClosestObject.transform.Find("Instructions Image 2").gameObject;
                                }
                            }
                            else if (CanExtinguish())
                            {
                                if (ClosestObject.transform.Find("Instructions Image 2") != null)
                                {
                                    ClosestObject.transform.Find("Instructions Image 2").gameObject.SetActive(true);
                                    _instructionsShown = ClosestObject.transform.Find("Instructions Image 2").gameObject;
                                }
                            }
                            else if (CanPickUp())
                            {
                                if (ClosestObject.transform.Find("Instructions Image") != null)
                                {
                                    ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                    _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                }
                            }
                        }
                        else if (ClosestObject.tag == "River Piece")
                        {
                            if (CanFillUpBucket())
                            {
                                if (ClosestObject.transform.Find("Instructions Image") != null)
                                {
                                    ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                    _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                }
                            }
                            else if (CanPlaceObjectOnMap())
                            {
                                if (ClosestObject.transform.Find("Instructions Image") != null)
                                {
                                    ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                    _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                                }
                            }
                        }
                        else
                        {
                            if (ClosestObject.transform.Find("Instructions Image") != null)
                            {
                                ClosestObject.transform.Find("Instructions Image").gameObject.SetActive(true);
                                _instructionsShown = ClosestObject.transform.Find("Instructions Image").gameObject;
                            }
                        }
                    }
                }
            }
        }
    }
    public void ResetInstructionIcon()
    {
        _firstInstruction = true;
    }
    public void HideInstructionsSprite()
    {
        if (_instructionsShown != null)
            _instructionsShown.SetActive(false);
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (AllInstructionsObjectsColliders.Contains(col.gameObject))
        {
            AllInstructionsObjectsColliders.Remove(col.gameObject);
            if (ClosestObject != null && ClosestObject == col.gameObject)
            {
                HideInstructionsSprite();
                if (ClosestObject.GetComponent<ResourceMine>() != null)
                {
                    NearbyResourceMine = null;
                }
                else if(ClosestObject.GetComponent<ResourceDrop>() != null)
                {
                    NearbyResourceDrop = null;
                }
                else if (ClosestObject.GetComponent<ItemDrop>() != null)
                {
                    NearbyItemDrop = null;
                }
                else if (ClosestObject.GetComponent<Campfire>() != null)
                {
                    NearbyCampfire = null;
                }
                ClosestObject = null;
            }
        }
    }
    private void ShowInstructions()
    {
        float smallestDistance = 100f;
        foreach (GameObject obj in AllInstructionsObjectsColliders)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                if (ClosestObject != obj)
                {
                    HideInstructionsSprite();
                }
                ClosestObject = obj;
            }
        }
        ShowInstructionsSprite();
    }
    IEnumerator IFrames()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(1f);
        canTakeDamage = true;
    }

    public void Die()
    {
        _anim.SetBool("isMoving", true);
        ControlsDisabled = true;
        canTakeDamage = false;
        _anim.SetBool("isMoving", false);

        Character1.SetActive(false);
        HealthBar.enabled = false;
    }

    public IEnumerator RespawnTimer()
    {
        RespawnTimer1.enabled = true;
        RespawnTimer1.text = "3";
        yield return new WaitForSeconds(1f);
        RespawnTimer1.text = "2";
        yield return new WaitForSeconds(1f);
        RespawnTimer1.text = "1";
        yield return new WaitForSeconds(1f);
        RespawnTimer1.enabled = false;

        Respawn();
    }

    public void Respawn()
    {
        ControlsDisabled = false;
        canTakeDamage = true;
        Health = MaxHealth;
        HealthBar.value = MaxHealth;

        transform.position = Camp1.position;

        Character1.SetActive(true);
        HealthBar.enabled = true;
    }

    IEnumerator Footsteps()
    {
        while (true)
        {
            if (mv != Vector2.zero)
            {
                audioSource.PlayOneShot(footstepsP1);
                yield return new WaitForSeconds(2f);
            }
        }
    }
}
