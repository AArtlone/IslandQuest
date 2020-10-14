using System.Collections.Generic;
using Anima2D;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironementGenerator : MonoBehaviour
{
    public static EnvironementGenerator Instance;
    public GameManager GM;
    public GameObject Crab;
    public GameObject BigIron;
    public GameObject SmallIron;
    public GameObject LargeGold;
    public GameObject SmallGold;
    public GameObject Tree;
    public GameObject LargeBush;
    public GameObject SmallBush;
    public GameObject Bear;
    public GameObject LargeBerries;
    public GameObject SmallBerries;
    public GameObject LargePoison;
    public GameObject SmallPoison;

    [SerializeField] private Player player;


    int BearsToSpawn;
    int CrabsToSpawn;
    int BigIronsToSpawn;
    int SmallIronsToSpawn;
    int LargeGoldsToSpawn;
    int SmallGoldsToSpawn;
    int TreesToSpawn;
    int LargeBushesToSpawn;
    int SmallBushesToSpawn;
    int LargeBerriesToSpawn;
    int SmallBerriesToSpawn;
    int LargePoisonToSpawn;
    int SmallPoisonToSpawn;

    public Collider2D[] Colliders;
    public float Radius;
    public Collider2D MapCollider;
    public Collider2D DirtCollider;
    public Collider2D GrassCollider;
    public static readonly HashSet<GameObject> EnviroPool = new HashSet<GameObject>();

    [Header("Night Related")]
    public Material MaterialToChangeTo;
    public Material DefaultMaterial;
    public GameObject Dirt;
    public GameObject Grass;
    public GameObject Water;
    public List<GameObject> NeedEditorReference = new List<GameObject>();
    [System.NonSerialized]
    public List<GameObject> PlacedRiverPieces = new List<GameObject>();
    [System.NonSerialized]
    public List<GameObject> NonAnimalObjects = new List<GameObject>();
    [System.NonSerialized]
    public List<GameObject> CrabObjects = new List<GameObject>();
    [System.NonSerialized]
    public List<GameObject> BearObjects = new List<GameObject>();

    private int _multiChance;
    private bool isSpawned = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        BearsToSpawn = Random.Range(3, 7);
        CrabsToSpawn = Random.Range(5, 15);
        BigIronsToSpawn = Random.Range(3, 7);
        SmallIronsToSpawn = Random.Range(5, 15);
        LargeGoldsToSpawn = Random.Range(3, 7);
        SmallGoldsToSpawn = Random.Range(5, 15);
        TreesToSpawn = Random.Range(10, 20);
        LargeBushesToSpawn = Random.Range(5, 10);
        SmallBushesToSpawn = Random.Range(10, 20);
        LargeBerriesToSpawn = Random.Range(5, 10);
        SmallBerriesToSpawn = Random.Range(10, 20);
        LargePoisonToSpawn = Random.Range(5, 10);
        SmallPoisonToSpawn = Random.Range(10, 20);
    }

    public void ChangeObjectsToNight()
    {
        Dirt.GetComponent<TilemapRenderer>().material = MaterialToChangeTo;
        Grass.GetComponent<TilemapRenderer>().material = MaterialToChangeTo;
        Water.GetComponent<TilemapRenderer>().material = MaterialToChangeTo;
        foreach (GameObject obj in PlacedRiverPieces)
        {
            obj.GetComponent<SpriteRenderer>().material = MaterialToChangeTo;
        }
        foreach (GameObject obj in CrabObjects)
        {
            for (int i = 0; i < 4; i++)
            {
                obj.transform.GetChild(i).GetComponent<SpriteMeshInstance>().sharedMaterial = MaterialToChangeTo;
            }
        }
        foreach (GameObject obj in BearObjects)
        {
            for (int i = 0; i < 6; i++)
            {
                obj.transform.GetChild(i).GetComponent<SpriteMeshInstance>().sharedMaterial = MaterialToChangeTo;
            }
        }
        foreach (GameObject obj in NonAnimalObjects)
        {
            obj.GetComponent<SpriteRenderer>().material = MaterialToChangeTo;
        }
        foreach (GameObject obj in NeedEditorReference)
        {
            obj.GetComponent<SpriteRenderer>().material = MaterialToChangeTo;
        }
        for (int i = 0; i < 6; i++)
        {
            player.CharacterTransform.GetChild(i).GetComponent<SpriteMeshInstance>().sharedMaterial = MaterialToChangeTo;
        }
    }
    public void ChangeObjectToDay()
    {
        Dirt.GetComponent<TilemapRenderer>().material = DefaultMaterial;
        Grass.GetComponent<TilemapRenderer>().material = DefaultMaterial;
        Water.GetComponent<TilemapRenderer>().material = DefaultMaterial;
        foreach (GameObject obj in PlacedRiverPieces)
        {
            obj.GetComponent<SpriteRenderer>().material = DefaultMaterial;
        }
        foreach (GameObject obj in CrabObjects)
        {
            for (int i = 0; i < 4; i++)
            {
                obj.transform.GetChild(i).GetComponent<SpriteMeshInstance>().sharedMaterial = DefaultMaterial;
            }
        }
        foreach (GameObject obj in BearObjects)
        {
            for (int i = 0; i < 6; i++)
            {
                obj.transform.GetChild(i).GetComponent<SpriteMeshInstance>().sharedMaterial = DefaultMaterial;
            }
        }
        foreach (GameObject obj in NonAnimalObjects)
        {
            obj.GetComponent<SpriteRenderer>().material = DefaultMaterial;
        }
        foreach (GameObject obj in NeedEditorReference)
        {
            obj.GetComponent<SpriteRenderer>().material = DefaultMaterial;
        }
        for (int i = 0; i < 6; i++)
        {
            player.CharacterTransform.GetChild(i).GetComponent<SpriteMeshInstance>().sharedMaterial = DefaultMaterial;
        }
    }
    private void Update()
    {
        if (GM.playersReady == 2 && GM.gameStarted == true && !isSpawned)
        {
            SpawnAnimals();
            isSpawned = true;
        }
    }

    public void SpawnAnimals()
    {
        SpawnObjects(Bear, BearsToSpawn, 1, 1);
        SpawnObjects(Crab, CrabsToSpawn, 1, 1);
    }

    public void SpawnObjects(GameObject itemToSpawn, int numberToSpawn, float minScale, float maxScale)
    {
        bool canMulti = true;

        Vector3 centerPoint = MapCollider.bounds.center;
        float width = MapCollider.bounds.extents.x;
        float height = MapCollider.bounds.extents.y;

        float leftExtent = centerPoint.x - width;
        float rightExtent = centerPoint.x + width;
        float lowerExtent = centerPoint.y - height;
        float upperExtent = centerPoint.y + height;

        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector3 spawnPos = new Vector3(0, 0, 0);
            bool canSpawnHere = false;
            int safetyNet = 0;
            GameObject newSpawn;
            float randomSeed;

            while (!canSpawnHere)
            {
                float spawnPosX = Random.Range(leftExtent, rightExtent);
                float spawnPosY = Random.Range(lowerExtent, upperExtent);

                spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
                canSpawnHere = PreventSpawnOverlap(spawnPos);

                if (canSpawnHere)
                {
                    break;
                }

                safetyNet++;
                if (safetyNet > 150)
                {
                    //Debug.Log("Too many attempts.");
                    break;
                }
            }
            _multiChance = Random.Range(1, 100);
            if (itemToSpawn == Bear)
            {
                newSpawn = Instantiate(itemToSpawn, spawnPos, Quaternion.identity);
                BearObjects.Add(newSpawn);
            }
            else if (itemToSpawn == Crab)
            {
                newSpawn = Instantiate(itemToSpawn, spawnPos, Quaternion.identity);
                CrabObjects.Add(newSpawn);
            }
            else
            {
                if (_multiChance <= 10 && canMulti)
                {
                    var multiAmount = Random.Range(2, 5);
                    for (int m = 0; m < multiAmount; m++)
                    {
                        newSpawn = Instantiate(itemToSpawn, spawnPos, Quaternion.identity);
                        NonAnimalObjects.Add(newSpawn);
                        randomSeed = Random.Range(minScale, maxScale);
                        newSpawn.transform.localScale = Vector2.one * randomSeed;
                        newSpawn.transform.position = Random.insideUnitCircle * 12;
                    }
                    canMulti = false;
                }
                else
                {
                    newSpawn = Instantiate(itemToSpawn, spawnPos, Quaternion.identity);
                    NonAnimalObjects.Add(newSpawn);
                }
            }
        }
    }

    bool PreventSpawnOverlap(Vector3 spawnPos)
    {
        Colliders = Physics2D.OverlapCircleAll(new Vector2(0, 0), Mathf.Infinity);

        foreach (var t in Colliders)
        {
            if (!t.isTrigger)
            {
                Vector3 centerPoint = t.bounds.center;
                float width = t.bounds.extents.x;
                float height = t.bounds.extents.y;

                float leftExtent = centerPoint.x - width;
                float rightExtent = centerPoint.x + width;
                float lowerExtent = centerPoint.y - height;
                float upperExtent = centerPoint.y + height;

                if (!(spawnPos.x >= leftExtent) || !(spawnPos.x <= rightExtent)) continue;
                if (spawnPos.y >= lowerExtent && spawnPos.y <= upperExtent)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void GenerateEnvironment()
    {
        SpawnObjects(BigIron, BigIronsToSpawn, 0.7f, 1);
        SpawnObjects(SmallIron, SmallIronsToSpawn, 0.7f, 1);
        SpawnObjects(LargeGold, LargeGoldsToSpawn, 0.7f, 1);
        SpawnObjects(SmallGold, SmallGoldsToSpawn, 0.7f, 1);
        SpawnObjects(Tree, TreesToSpawn, 4, 6);
        SpawnObjects(LargeBush, LargeBushesToSpawn, 1, 2);
        SpawnObjects(SmallBush, SmallBushesToSpawn, 1, 2);
        SpawnObjects(LargeBerries, LargeBerriesToSpawn, 1, 2);
        SpawnObjects(SmallBerries, SmallBerriesToSpawn, 1, 2);
        SpawnObjects(LargePoison, LargePoisonToSpawn, 1, 2);
        SpawnObjects(SmallPoison, SmallPoisonToSpawn, 1, 2);
    }

    public void DestroyEnvironment()
    {
        foreach (GameObject obj in EnviroPool)
        {
            EnviroPool.Remove(obj);
            Destroy(obj);
        }
        GenerateEnvironment();
    }
}
