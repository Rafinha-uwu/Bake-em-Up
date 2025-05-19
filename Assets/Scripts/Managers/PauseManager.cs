using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public InputActionProperty pauseAction; // Drag your Pause action here in Inspector
    private bool isPaused = false;
    public Transform playerCamera;
    public GameObject darkBackgroundPanel;
    [SerializeField] private GameObject optionsMenu;

    void OnEnable()
    {
        pauseAction.action.Enable();
    }

    void OnDisable()
    {
        pauseAction.action.Disable();
    }

    void Update()
    {
        if (pauseAction.action.WasPressedThisFrame())
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenu.SetActive(isPaused);
        darkBackgroundPanel.SetActive(isPaused);

        if (isPaused)
        {
            pauseMenu.transform.position = playerCamera.position + playerCamera.forward * 1.5f;
            pauseMenu.transform.LookAt(playerCamera);
        }
    }

    public void ToggleOptionsMenu()
    {
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(!optionsMenu.activeSelf);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}