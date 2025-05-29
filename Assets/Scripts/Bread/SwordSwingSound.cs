using UnityEngine;

public class SwordSwingSound : MonoBehaviour
{
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySwingSound()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }
}