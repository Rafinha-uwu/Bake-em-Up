using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu; // Assign your options panel here in the inspector

    // Called when Start Button is pressed
    public void StartGame()
    {
        SceneManager.LoadScene("GameLoop");
    }

    // Called when Options Button is pressed
    public void ToggleOptionsMenu()
    {
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(!optionsMenu.activeSelf);
        }
    }

    // Called when Quit Button is pressed
    public void QuitGame()
    {
        Application.Quit();

    }
}