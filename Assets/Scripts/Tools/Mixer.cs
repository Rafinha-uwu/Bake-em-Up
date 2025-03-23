using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Mixer : MonoBehaviour
{
    private XRSocketToolInteractor _bowlSocket;
    private ToolButton _toolButton;

	private InteractionLayerMask _bowlInteractionLayerMask;
	private InteractionLayerMask _nothingInteractionLayerMask;

	private RecipeData _recipeData;
	private bool _isMixing = false;
	private float _currentTime = 0f;

	private void Awake()
	{
		_bowlSocket = GetComponentInChildren<XRSocketToolInteractor>();
		_toolButton = GetComponentInChildren<ToolButton>();
	}

	private void OnEnable()
	{
		_toolButton.OnTurnOn += TurnOn;
		_toolButton.OnTurnOff += TurnOff;
	}

	private void OnDisable()
	{
		_toolButton.OnTurnOn -= TurnOn;
		_toolButton.OnTurnOff -= TurnOff;
	}

	private void Update()
	{
		if (!_isMixing)
			return;

		_currentTime += Time.deltaTime;
		Debug.Log(_currentTime % 60);

		if (_currentTime >= _recipeData.MixerTime * 1.5f)
		{
			Debug.Log("Estragou a massa!");
			
		}
		else if(_currentTime >= _recipeData.MixerTime)
		{
			Debug.Log("Terminou de Misturar");
			_isMixing = false;
			MakeDough();
			_currentTime = 0f;
		}
	}

	private void TurnOn()
	{
		Debug.Log("Ligou");
		_bowlSocket.IsToolOn = true;

		if (_bowlSocket.Interactable != null)
		{
			XRBaseInteractable grabInteractable = _bowlSocket.Interactable.transform.gameObject.GetComponent<XRBaseInteractable>();
			_bowlInteractionLayerMask = grabInteractable.interactionLayers;
			grabInteractable.interactionLayers = _nothingInteractionLayerMask;

			Bowl bowl = _bowlSocket.Interactable.transform.gameObject.GetComponent<Bowl>();
			if (bowl.GetRecipe(out _recipeData))
			{
				_isMixing = true;
			}
		}
	}

	private void TurnOff()
	{
		Debug.Log("Desligou");
		_bowlSocket.IsToolOn = false;

		if (_bowlSocket.Interactable != null)
		{
			XRBaseInteractable grabInteractable = _bowlSocket.Interactable.transform.gameObject.GetComponent<XRBaseInteractable>();
			grabInteractable.interactionLayers = _bowlInteractionLayerMask;
			_isMixing = false;
		}
	}

	private void MakeDough()
	{
		if (_bowlSocket.Interactable == null)
			return;

		Bowl bowl = _bowlSocket.Interactable.transform.gameObject.GetComponent<Bowl>();
		bowl.MakeDough();
	}
}
