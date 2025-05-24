using UnityEngine;

public class UISoundPlayer : MonoBehaviour
{
    public static UISoundPlayer Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("UI Audio Clips")]
    public AudioClip ForwardClickSound;
    public AudioClip BackwardClickSound;

    [Header("Game Audio Clips")]
    public AudioClip VictorySound;
    public AudioClip DefeatSound;
    public AudioClip AttackClickSound;
    public AudioClip PauseSound;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayForwardClickSound()
    {
        audioSource.PlayOneShot(ForwardClickSound);
    }

    public void PlayBackwardClickSound()
    {
        audioSource.PlayOneShot(BackwardClickSound);
    }

    public void PlayVictorySound()
    {
        audioSource.PlayOneShot(VictorySound);
    }

    public void PlayDefeatSound()
    {
        audioSource.PlayOneShot(DefeatSound);
    }

    public void PlayAttackClickSound()
    {
        audioSource.PlayOneShot(AttackClickSound);
    }

    public void PlayPauseSound()
    {
        audioSource.PlayOneShot(PauseSound);
    }

    public void PlayCashSound()
    {
        audioSource.PlayOneShot(VictorySound);
    }
}