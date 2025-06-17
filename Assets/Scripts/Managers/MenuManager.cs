using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip start_sound;


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void StartGame()
    {
        PlayStartButtonSound();
        SceneManager.LoadScene("Main");
    }


    public void ToggleOptionsMenu()
    {
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(!optionsMenu.activeSelf);
        }
    }

    public void QuitGame()
    {
        Application.Quit();

    }

    public void Continue()
    {
        PlayStartButtonSound();
        int savedWave = GameManager.Instance.LoadSavedWave();
        GameManager.Instance.lastWaveIndex = savedWave;

        SceneManager.LoadScene("Main");
    }


    private void PlayStartButtonSound()
    {
        _audioSource.clip = start_sound;
        _audioSource.Play();
    }
}