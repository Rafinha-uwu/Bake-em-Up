using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip start_sound;

    [SerializeField] private GameObject blackout;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void StartGame()
    {
        PlayStartButtonSound();
        blackout.GetComponent<Animator>().Play("Dark");
        Invoke("GoToMain", 5);
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
        blackout.GetComponent<Animator>().Play("Dark");
        Invoke("GoToMain", 5);
    }

    private void GoToMain()
    {
        SceneManager.LoadScene("Main");
    }


    private void PlayStartButtonSound()
    {
        _audioSource.clip = start_sound;
        _audioSource.Play();
    }
}