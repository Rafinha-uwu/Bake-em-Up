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
	private float _badTimer = 0f;
	private bool _mixingComplete = false;
	private bool _mixingRuined = false;

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
		_mixerCanvas.UpdateTimer(_currentTime, _recipeData.MixerTime, _badTimer);

		if (!_mixingRuined && _currentTime >= _badTimer)
		{
			Debug.Log("Estragou a massa!");
			MakeBadDough();
			
		}
		else if(!_mixingComplete && _currentTime >= _recipeData.MixerTime)
		{
			Debug.Log("Terminou de Misturar");
			MakeDough();
		}
	}

	public override void SocketSelectedEnter(XRSocketToolInteractor socket)
	{
		Bowl bowl = _socket.Interactable.transform.gameObject.GetComponent<Bowl>();
		if (bowl.GetRecipe(out _recipeData))
		{
			_badTimer = _recipeData.MixerTime * BadTimerMultiplier;

			if (bowl.HasBadDough)
			{
				_currentTime = _badTimer;
				_mixingRuined = true;
			}
			else if (bowl.HasCompletedDough)
			{
				_currentTime = _recipeData.MixerTime;
				_mixingComplete = true;
			}

			_mixerCanvas.SetRecipe(_recipeData.recipeSprite);
			_mixerCanvas.UpdateTimer(_currentTime, _recipeData.MixerTime, _badTimer);
		}
		_mixerCanvas.EnableCanvas();
	}

	public override void SocketSelectedExit(XRSocketToolInteractor socket)
	{
		_mixerCanvas.ClearCanvas();
		_mixerCanvas.DisableCanvas();
		
		_recipeData = null;
		_currentTime = 0f;
		_badTimer = 0f;
		_mixingRuined = false;
		_mixingComplete = false;
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

		_mixingComplete = true;

		Bowl bowl = _socket.Interactable.transform.gameObject.GetComponent<Bowl>();
		bowl.MakeDough();
	}

	private void MakeBadDough()
	{
		if (_socket.Interactable == null)
			return;

		_mixingRuined = true;

		Bowl bowl = _socket.Interactable.transform.gameObject.GetComponent<Bowl>();
		bowl.MakeBadDough();
	}
}
