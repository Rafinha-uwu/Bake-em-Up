using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu; 


    public void StartGame()
    {
        SceneManager.LoadScene("GameLoop");
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

        SceneManager.LoadScene("GameLoop");
    }
}