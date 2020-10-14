using System.Collections;
using UnityEngine;

public class BlueprintsManager : MonoBehaviour
{
    public static BlueprintsManager Instance;
    
    public readonly int BlueprintsToCollect = 5;

    [SerializeField] private GameObject[] _boatPieces = default;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void ReceiveBlueprint(Player player)
    {
        StartCoroutine(ReceiveBlueprintCo(player));
    }

    private void UpdateShopBlueprints(Player player)
    {
        player.ShopBlueprintsText.text = player.UnlockedBlueprints.ToString() + "/" + BlueprintsToCollect.ToString(); 
    
    }
    private IEnumerator ReceiveBlueprintCo(Player player)
    {
        player.BlueprintsContainer.SetActive(true);
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < player.UnlockedBlueprints; i++)
        {
            GameObject blueprint = _boatPieces[i];
            if (!blueprint.activeSelf)
            {
                blueprint.SetActive(true);
                yield return new WaitForSeconds(2f);
            }
        }
        player.BlueprintsContainer.SetActive(false);
        UpdateShopBlueprints(player);
        if (!GameManager.Instance.GameFinished())
        {
            ChallengesManager.Instance.StartNewRound();
        }
    }
}