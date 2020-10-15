using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int RoundCounter = 1;
    public int TimeBetweenDayAndNight = 60;
    public bool isNight = false;
    public SpriteRenderer NightSprite;

    [SerializeField] private Transform _camp1;

    [SerializeField] private Player player;

    public bool gameStarted;
    [System.NonSerialized]
    public int playersReady = 0;

    public AudioClip roundWin;
    public AudioSource roundWinSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        gameStarted = false;
        AudioManager.Instance.nightTimeSource.clip = AudioManager.Instance.nightTime;
        //spawn players after all players are confirmed in
        StartCoroutine(GameStartedSequenceCo());
    }

    public void EnablePlayers()
    {
        player.ControlsDisabled = false;
    }

    private IEnumerator GameStartedSequenceCo()
    {
        yield return new WaitForSeconds(1f);
        AnnounceRound();
        EnvironementGenerator.Instance.SpawnAnimals();
        yield return new WaitForSeconds(4f);
        ChallengesManager.Instance.AnnounceNewChallenges();
    }
    public void AnnounceRound()
    {
        GameObject roundAnnouncement = player.RoundAnnouncement;
        roundAnnouncement.GetComponentInChildren<TextMeshProUGUI>().text = "Round " + RoundCounter.ToString();
        roundAnnouncement.SetActive(true);
        roundAnnouncement.GetComponent<RoundAnnouncement>().enabled = true;
    }
    public void RoundFinished(Player player)
    {
        player.UnlockedBlueprints++;

        player.ControlsDisabled = true;

        roundWinSource.PlayOneShot(roundWin);
        
        BlueprintsManager.Instance.ReceiveBlueprint(player);
    }
    public bool GameFinished()
    {
        bool gameFinished = false;
        player.ControlsDisabled = true;

        if (player.UnlockedBlueprints >= BlueprintsManager.Instance.BlueprintsToCollect)
        {
            //TODO: End Game
            player.WinScreen.SetActive(true);
            Debug.Log("Game Finished");
            gameFinished = true;
        }
        else
        {
            player.LoseScreen.SetActive(true);
        }

        StartCoroutine(Restart());

        return gameFinished;
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }

    private IEnumerator DayNightCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeBetweenDayAndNight);
            if (isNight == true)
            {
                StartCoroutine(SwapDayNight());
                isNight = false;
                EnvironementGenerator.Instance.ChangeObjectToDay();
                AudioManager.Instance.nightTimeSource.Stop();
            }
            else
            {
                StartCoroutine(SwapNightDay());
                isNight = true;
                EnvironementGenerator.Instance.ChangeObjectsToNight();
                AudioManager.Instance.nightTimeSource.Play();
            }
        }       
    }

    private IEnumerator SwapDayNight()
    {
        var i = 0.0f;
        var rate = 1.0f / 3;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            NightSprite.color = Color.Lerp(new Color(1f, 1f, 1f, 0.6f), new Color(1f, 1f, 1f, 0f), i);
            yield return null;
        }
    }

    private IEnumerator SwapNightDay()
    {
        var i = 0.0f;
        var rate = 1.0f / 3;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            NightSprite.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 0.6f), i);
            yield return null;
        }
    }

    public IEnumerator FadeToBlack()
    {
        var i = 0.0f;
        var rate = 1.0f;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            NightSprite.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), i);
            yield return null;
        }
    }

    public IEnumerator FadeFromBlack()
    {
        var i = 0.0f;
        var rate = 1.0f;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            NightSprite.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0f), i);
            yield return null;
        }
    }
}
