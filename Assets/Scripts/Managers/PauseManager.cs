using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public InputActionProperty pauseAction; // Drag your Pause action here in Inspector
    private bool isPaused = false;
    public Transform playerCamera;
    //public GameObject darkBackgroundPanel;
    [SerializeField] private GameObject optionsMenu;

    [Header("Near-Far Interactors")]
    public NearFarInteractor[] interactors; // Group that wraps direct + ray
    private InteractionLayerMask savedInteractorLayers;

    [SerializeField]
    private InteractionLayerMask nothingLayer;
    [SerializeField]
    private GameObject tv;

    void OnEnable()
    {
        pauseAction.action.Enable();
    }

    private void Start()
    {
        savedInteractorLayers = interactors[0].interactionLayers;
        Debug.Log(nothingLayer.value);
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
        //darkBackgroundPanel.SetActive(isPaused);

        if (isPaused)
        {
            foreach (var interactor in interactors)
            {
                interactor.interactionLayers = nothingLayer;
            }
            DisableAllButtonsInTV();
        }
        else
        {
            foreach (var interactor in interactors)
            {
                interactor.interactionLayers = savedInteractorLayers;
            }
            EnableAllButtonsInTV();
        }
    }
    public void ToggleOptionsMenu()
    {
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(!optionsMenu.activeSelf);
            pauseMenu.GetComponent<Renderer>().enabled = !optionsMenu.activeSelf;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    void DisableAllButtonsInTV()
    {
        Button[] buttons = tv.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }

    void EnableAllButtonsInTV()
    {
        Button[] buttons = tv.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            button.interactable = true;
        }
    }
}