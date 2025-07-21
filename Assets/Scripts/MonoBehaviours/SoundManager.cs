using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]private AudioSource background;
    [SerializeField]private AudioSource effect;

    public AudioClip Music;
    public AudioClip effectSound;

    public static SoundManager Smanager;
    private void Awake()
    {
        if (Smanager == null)
        {
            Smanager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public void ReproduceEffect(AudioClip effectSound)
    {
        effect.PlayOneShot(effectSound);
    }

    public void StartMusic(AudioClip Music)
    {
        background.clip = Music;
        background.Play();
        background.loop = true;
    }
}
