using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Mixer : ToolCooker
{
	private MixerCanvas _mixerCanvas;

	private InteractionLayerMask _bowlInteractionLayerMask;
	private InteractionLayerMask _nothingInteractionLayerMask;

	private RecipeData _recipeData;
	private bool _isMixing = false;
	private float _currentTime = 0f;

	protected override void Awake()
	{
		base.Awake();
		_mixerCanvas = _toolCanvas as MixerCanvas;
	}

	protected override void Start()
	{
		base.Start();
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
		_mixerCanvas.UpdateTimer(_currentTime, _recipeData.MixerTime);

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

	public override void SocketSelectedEnter(XRSocketToolInteractor socket)
	{
		Bowl bowl = _socket.Interactable.transform.gameObject.GetComponent<Bowl>();
		if (bowl.GetRecipe(out _recipeData))
		{
			_mixerCanvas.SetRecipe(_recipeData.recipeSprite);
		}
		_mixerCanvas.EnableCanvas();
	}

	public override void SocketSelectedExit(XRSocketToolInteractor socket)
	{
		_mixerCanvas.DisableCanvas();
		_mixerCanvas.ClearCanvas();
		_recipeData = null;
	}

	protected override void TurnOn()
	{
		Debug.Log("Ligou");
		_socket.IsToolOn = true;

		if (_socket.Interactable != null)
		{
			XRBaseInteractable grabInteractable = _socket.Interactable.transform.gameObject.GetComponent<XRBaseInteractable>();
			_bowlInteractionLayerMask = grabInteractable.interactionLayers;
			grabInteractable.interactionLayers = _nothingInteractionLayerMask;

			if (_recipeData != null)
			{
				_isMixing = true;
			}
		}
	}

	protected override void TurnOff()
	{
		Debug.Log("Desligou");
		_socket.IsToolOn = false;

		if (_socket.Interactable != null)
		{
			XRBaseInteractable grabInteractable = _socket.Interactable.transform.gameObject.GetComponent<XRBaseInteractable>();
			grabInteractable.interactionLayers = _bowlInteractionLayerMask;
			_isMixing = false;
		}
	}

	private void MakeDough()
	{
		if (_socket.Interactable == null)
			return;

		Bowl bowl = _socket.Interactable.transform.gameObject.GetComponent<Bowl>();
		bowl.MakeDough();
	}
}
