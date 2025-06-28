using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip start_sound;

    [SerializeField] private GameObject blackout;
    [SerializeField] private GameObject continue_button;

    [SerializeField] private Sprite blocked_continue;
    [SerializeField] private Sprite available_continue;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if(GameManager.Instance.lastWaveIndex > 0)
        {
            continue_button.GetComponent<Image>().sprite = available_continue;
            continue_button.GetComponent<Button>().interactable = true;
        }
        else
        {
            continue_button.GetComponent<Image>().sprite = blocked_continue;
            continue_button.GetComponent<Button>().interactable = false;
        }
    }
    public void StartGame()
    {
        PlayerPrefs.SetInt("IsEndlessMode", 0);
        PlayerPrefs.Save();
        PlayStartButtonSound();
        blackout.GetComponent<Animator>().Play("Dark");
		StartCoroutine(GoToMain());
	}

    public void StartEndlessMode()
    {
        PlayerPrefs.SetInt("IsEndlessMode", 1);
        PlayerPrefs.Save();
        PlayStartButtonSound();
        blackout.GetComponent<Animator>().Play("Dark");
		StartCoroutine(GoToMain());
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
        GameManager.Instance.LoadSavedWave();
        blackout.GetComponent<Animator>().Play("Dark");
        StartCoroutine(GoToMain());
    }

    private IEnumerator GoToMain()
    {

        yield return new WaitForSeconds(4f);
        //SceneManager.LoadScene("mathews");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("mathews");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
	}

    private void PlayStartButtonSound()
    {
        _audioSource.clip = start_sound;
        _audioSource.Play();
    }
}