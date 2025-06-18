using UnityEngine;

public class PauseMenuMusic : MonoBehaviour
{
    private AudioSource _audioSource;


    void OnEnable()
    {
        if( _audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
        if (_audioSource && !_audioSource.isPlaying)
        {
            _audioSource.GetComponent<AudioSource>().Play();
        }
    }

    void OnDisable()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
        if (_audioSource && _audioSource.isPlaying)
        {
            _audioSource.GetComponent<AudioSource>().Stop();
        }
    }
}