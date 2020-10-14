using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChallengesManager : MonoBehaviour
{
    public static ChallengesManager Instance;

    [SerializeField] private Player player;

    [SerializeField] private List<Challenge> AllChallanges = new List<Challenge>();
    public List<Challenge> ThisRoundChallenges = new List<Challenge>();
    public readonly int ChallengesPerRound = 2;

    private int Challenge1ResourceCollected; // Counter to keep track of how much resources was picked up for the first challenge
    private int Challenge2ResourceCollected; // Counter to keep track of how much resources was picked up for the second challenge


    [System.NonSerialized]
    public string FirstChallengeText;
    [System.NonSerialized]
    public string SecondChallengeText;
    [System.NonSerialized]
    public string FirstChallengeAmountText;
    [System.NonSerialized]
    public string SecondChallengeAmountText;

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
    }
    private void Start()
    {
        SelectRoundChallenges();
    }
    public void AnnounceNewChallenges()
    {
        GameObject challengesAnnouncement = player.ChallengesAnnouncement;
        GameObject challengesInTheShop = player.ChallengesInTheShop;
        FirstChallengeText = ThisRoundChallenges[0].TextToAnnounce;
        SecondChallengeText = ThisRoundChallenges[1].TextToAnnounce;
        FirstChallengeAmountText = ThisRoundChallenges[0].AmountCollectedKilledTrapped.ToString() + " / " + ThisRoundChallenges[0].AmountToCollectKillTrap.ToString();
        SecondChallengeAmountText = ThisRoundChallenges[1].AmountCollectedKilledTrapped.ToString() + " / " + ThisRoundChallenges[1].AmountToCollectKillTrap.ToString();
        Transform firstChallengeAnnouncement = challengesAnnouncement.transform.Find("First Challenge").transform;
        Transform secondChallengeAnnouncement = challengesAnnouncement.transform.Find("Second Challenge").transform;
        Transform firstShopChallenge = challengesInTheShop.transform.Find("First Challenge").transform;
        Transform secondShopChallenge = challengesInTheShop.transform.Find("Second Challenge").transform;

        firstChallengeAnnouncement.GetChild(1).GetComponent<TextMeshProUGUI>().text = FirstChallengeText;
        secondChallengeAnnouncement.GetChild(1).GetComponent<TextMeshProUGUI>().text = SecondChallengeText;

        firstShopChallenge.Find("Challenge Task").GetComponent<TextMeshProUGUI>().text = FirstChallengeText;
        secondShopChallenge.Find("Challenge Task").GetComponent<TextMeshProUGUI>().text = SecondChallengeText;

        firstShopChallenge.Find("Amount Text").GetComponent<TextMeshProUGUI>().text = FirstChallengeAmountText;
        secondShopChallenge.Find("Amount Text").GetComponent<TextMeshProUGUI>().text = SecondChallengeAmountText;

        StartCoroutine(ChallengesAnnouncementCo(challengesAnnouncement));
    }

    //private void UpdateShopChallenges(Player player)
    //{
    //    foreach (Player _player in GameManager.Instance.AllPlayers)
    //    {
    //        if (_player == player)
    //        {
    //            GameObject challengesInTheShop = _player.ChallengesInTheShop;
    //            Transform firstShopChallenge = challengesInTheShop.transform.Find("First Challenge").transform;
    //            Transform secondShopChallenge = challengesInTheShop.transform.Find("Second Challenge").transform;
    //            firstShopChallenge.Find("Amount Text").GetComponent<TextMeshProUGUI>().text = _player.PlayerChallenges[0].AmountCollectedKilledTrapped.ToString() + " / " + _player.PlayerChallenges[0].AmountToCollectKillTrap.ToString();
    //            secondShopChallenge.Find("Amount Text").GetComponent<TextMeshProUGUI>().text = _player.PlayerChallenges[1].AmountCollectedKilledTrapped.ToString() + " / " + _player.PlayerChallenges[1].AmountToCollectKillTrap.ToString();
    //        }
    //    }
    //    if (CheckIfChallengeIsComplete())
    //    {
    //        GameManager.Instance.RoundFinished(player);
    //    }
    //}

    private IEnumerator ChallengesAnnouncementCo(GameObject challengesAnnouncement)
    {
        player.RoundAnnouncement.SetActive(true);

        Animator animator = challengesAnnouncement.GetComponent<Animator>();
        animator.SetBool("Announce", true);
        yield return new WaitForSeconds(3f);
        animator.SetBool("Announce", false);
        player.ControlsDisabled = false;
        player.RoundAnnouncement.SetActive(false);
    }
    public void SelectRoundChallenges()
    {
        foreach (Challenge challenge in AllChallanges)
        {
            challenge.AmountCollectedKilledTrapped = 0;
        }
        ThisRoundChallenges.Clear();
        List<Challenge> TempList = new List<Challenge>();
        for (int i = 0; i < AllChallanges.Count; i++)
        {
            TempList.Add(AllChallanges[i]);
        }
        for (int i = 0; i < ChallengesPerRound; i++)
        {
            int randomNumber = Random.Range(0, AllChallanges.Count);
            if (AllChallanges.Count > 1)
            {
                while (ThisRoundChallenges.Contains(AllChallanges[randomNumber]))
                {
                    randomNumber = Random.Range(0, AllChallanges.Count);
                }
            }
            else
            {
                randomNumber = 0;
            }
            ThisRoundChallenges.Add(AllChallanges[randomNumber]);
            AllChallanges.Remove(AllChallanges[randomNumber]);
        }
        AllChallanges.Clear();
        AllChallanges = TempList;

        player.PlayerChallenges.Clear();
        foreach (Challenge challenge in ThisRoundChallenges)
        {
            player.PlayerChallenges.Add(challenge);
        }
    }
    //public void CheckForChallenge(Resource.ResourceType type, int amount, Player player)
    //{
    //    bool challengeMatched = false;
    //    foreach (Player _player in GameManager.Instance.AllPlayers)
    //    {
    //        if (_player == player)
    //        {
    //            foreach (Challenge challenge in _player.PlayerChallenges)
    //            {
    //                if (challenge.TypeToCollect == type)
    //                {
    //                    challenge.AmountCollectedKilledTrapped += amount;
    //                    challengeMatched = true;
    //                    break;
    //                }
    //            }
    //            break; // Not sure
    //        }
    //    }
    //    if (challengeMatched)
    //    {
    //        UpdateShopChallenges(player);
    //    }
    //}
    //public void CheckForChallenge(Animal.AnimalType animalType, Player player)
    //{
    //    bool challengeMatched = false;
    //    foreach (Player _player in GameManager.Instance.AllPlayers)
    //    {
    //        if (_player == player)
    //        {
    //            foreach (Challenge challenge in _player.PlayerChallenges)
    //            {
    //                if (challenge.AnimalToKillOrTrap == animalType)
    //                {
    //                    challenge.AmountCollectedKilledTrapped++;
    //                    challengeMatched = true;
    //                    break;
    //                }
    //            }
    //            break; // Not sure
    //        }
    //    }
    //    if (challengeMatched)
    //    {
    //        UpdateShopChallenges(player);
    //    }
    //}
    //public void CheckForChallenge(Item.ItemType itemType, Player player)
    //{
    //    bool challengeMatched = false;
    //    foreach (Player _player in GameManager.Instance.AllPlayers)
    //    {
    //        if (_player == player)
    //        {
    //            foreach (Challenge challenge in _player.PlayerChallenges)
    //            {
    //                if (challenge.ItemToCraft == itemType)
    //                {
    //                    challenge.AmountCollectedKilledTrapped++;
    //                    challengeMatched = true;
    //                    break;
    //                }
    //            }
    //            break; // Not sure 
    //        }
    //    }
    //    if (challengeMatched)
    //    {
    //        UpdateShopChallenges(player);
    //    }
    //}
    //public void CheckForChallenge(Player player)
    //{
    //    bool challengeMatched = false;
    //    foreach (Player _player in GameManager.Instance.AllPlayers)
    //    {
    //        if (_player == player)
    //        {
    //            foreach (Challenge challenge in _player.PlayerChallenges)
    //            {
    //                if (challenge.Type == Challenge.ChallengeType.KillingPlayer)
    //                {
    //                    challenge.AmountCollectedKilledTrapped++;
    //                    challengeMatched = true;
    //                    break;
    //                }
    //            }
    //            break; // Not sure 
    //        }
    //    }
    //    if (challengeMatched)
    //    {
    //        UpdateShopChallenges(player);
    //    }
    //}

    public bool CheckIfChallengeIsComplete()
    {
        bool challengeCompleted = false;
        foreach (Challenge challenge in ThisRoundChallenges)
        {
            if (challenge.AmountCollectedKilledTrapped >= challenge.AmountToCollectKillTrap)
            {
                challengeCompleted = true;
                break;
            }
        }
        return challengeCompleted;
    }
    public void StartNewRound()
    {
        SelectRoundChallenges();
        StartCoroutine(NewRoundAnim());
    }

    public IEnumerator NewRoundAnim()
    {
        GameManager.Instance.FadeToBlack();
        yield return new WaitForSeconds(1f);
        EnvironementGenerator.Instance.DestroyEnvironment();
        EnvironementGenerator.Instance.SpawnAnimals();
        //reload environment
        GameManager.Instance.FadeFromBlack();
        GameManager.Instance.AnnounceRound();
        yield return new WaitForSeconds(4f);
        GameManager.Instance.EnablePlayers();
        AnnounceNewChallenges();
    }
}
