using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject blackout;

    public void StartGame()
    {
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
        int savedWave = GameManager.Instance.LoadSavedWave();
        GameManager.Instance.lastWaveIndex = savedWave;
        blackout.GetComponent<Animator>().Play("Dark");
        Invoke("GoToMain", 5);
    }

    private void GoToMain()
    {
        SceneManager.LoadScene("1");
    }
}