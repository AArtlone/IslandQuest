using System.Collections;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public bool IsOnFire;
    [System.NonSerialized]
    public Player LinkedPlayer;
    private Player _player;
    [System.NonSerialized]
    public Sprite InstructionSprite;
    public AudioClip fireCrackle;
    public AudioSource fireCrackleSource;

    bool audioPlaying = false;

    private void Awake()
    {
        InstructionSprite = Resources.Load<Sprite>("Sprites/Interact Icon");
        transform.Find("Instructions Image").GetComponent<SpriteRenderer>().sprite = InstructionSprite;
        fireCrackleSource = GetComponent<AudioSource>();
        fireCrackleSource.clip = fireCrackle;
    }

    private void Update()
    {
        if (IsOnFire && !audioPlaying)
        {
            fireCrackleSource.Play();
            audioPlaying = true;
        }
        else if (!IsOnFire && audioPlaying)
        {
            fireCrackleSource.Stop();
            audioPlaying = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _player = other.GetComponent<Player>();
        if (_player != null && _player == LinkedPlayer)
            StartCoroutine(Heal());

        /*    _player = other.GetComponent<Player>();
        if (_player == null) return;
        if (_player.PlayerNumber == 1)
            StartCoroutine(Heal());
        //_player.InBase = true;*/
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _player = other.GetComponent<Player>();
        if (_player != null && _player == LinkedPlayer)
        {
            StopAllCoroutines();
        }
        /*    StopCoroutine(Heal());*/
        /*_player = other.GetComponent<Player>();
        if (_player.GetComponent<Player>() != null) return;
        if (_player.PlayerNumber == 1)
            StopCoroutine(Heal());*/
        //_player.InBase = false;
    }

    private IEnumerator Heal()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if (IsOnFire)
                LinkedPlayer.Heal(2);
        }
    }
}
