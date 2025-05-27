using UnityEngine;

public class PauseMenuMusic : MonoBehaviour
{
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (_audioSource && !_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }

    void OnDisable()
    {
        if (_audioSource && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
}