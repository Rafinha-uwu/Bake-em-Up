using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.UI;
using System.Collections.Generic;

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

    public List<AudioSource> gameplayAudioSources;
    private List<AudioSource> pausedAudioSources = new List<AudioSource>();

    private CanvasGroup canvasGroup;

    void OnEnable()
    {
        pauseAction.action.Enable();
    }

    private void Start()
    {
        savedInteractorLayers = interactors[0].interactionLayers;
        Debug.Log(nothingLayer.value);
        canvasGroup = pauseMenu.GetComponent<CanvasGroup>();
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
        if (isPaused)
        {
            foreach (AudioSource audio in gameplayAudioSources)
            {
                if (audio.isPlaying)
                {
                    audio.Pause();
                    pausedAudioSources.Add(audio);
                }
            }
        }
        else
        {
            foreach (AudioSource audio in pausedAudioSources)
            {
                if (audio != null)
                {
                    audio.UnPause();
                }
            }

            pausedAudioSources.Clear();
        }
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
            if (optionsMenu.activeSelf)
            {
                canvasGroup.alpha = 0f;          // Invisível
                canvasGroup.interactable = false; // Não responde a eventos
                canvasGroup.blocksRaycasts = false; // Não bloqueia clique
            }
            if (!optionsMenu.activeSelf)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
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

    public void WaveRestart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}