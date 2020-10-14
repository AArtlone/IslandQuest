using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    //Night Time
    public AudioClip nightTime;
    public AudioSource nightTimeSource;
    //Burning

    //Bear eats poison berries

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
        nightTimeSource = GetComponent<AudioSource>();
    }

}
