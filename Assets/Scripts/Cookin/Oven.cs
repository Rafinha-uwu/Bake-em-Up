using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Oven : ToolCooker
{
    [SerializeField]
    private XRSocketToolInteractor _socketDish2;
	
    [SerializeField]
	private Transform _transformForCanvas2ToFollow;

	private OvenDish _dish1;
    private OvenDish _dish2;

	private MixerCanvas _dish1Canvas;
	private MixerCanvas _dish2Canvas;

	private OvenDoor _ovenDoor;

    private InteractionLayerMask _dishInteractionLayerMask;
	[SerializeField]
	private InteractionLayerMask _trackInteractionLayerMask;

    private RecipeData _recipeDataDish1;
    private RecipeData _recipeDataDish2;
    private bool _isHeating = false;
    private float _currentTimeDish1 = 0f;
    private float _currentTimeDish2 = 0f;
    private float _badTimerDish1 = 0f;
    private float _badTimerDish2 = 0f;
    private bool _heatingCompleteDish1 = false;
    private bool _heatingCompleteDish2 = false;
    private bool _burnedDish1 = false;
    private bool _burnedDish2 = false;
    private bool _showWarning = false;

	protected override void Awake()
	{
		base.Awake();
		_dish1Canvas = _toolCanvas as MixerCanvas;

		GameObject canvas = Instantiate(_canvasObject, transform.position, transform.rotation);
		_dish2Canvas = canvas.GetComponent<MixerCanvas>();
		_dish2Canvas.AddTransformToFollow(_transformForCanvas2ToFollow);
		_dish2Canvas.DisableCanvas();
	}

	protected override void Start()
    {
        base.Start();
        _ovenDoor = GetComponentInChildren<OvenDoor>();
        _ovenDoor.OnClose += TurnOn;
        _ovenDoor.OnOpen += TurnOff;
    }
    private void OnDestroy()
    {
        _ovenDoor.OnClose -= TurnOn;
        _ovenDoor.OnOpen -= TurnOff;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isHeating)
            return;

        _showWarning = false;
        if (_socket.Interactable != null)
        {
            HeatDish1();
        }
        if (_socketDish2.Interactable != null)
        {
            HeatDish2();
        }

        if (_showWarning)
            _warningHelper.Show();
    }

    public override void SocketSelectedEnter(XRSocketToolInteractor socket)
    {
        OvenDish ovendish = socket.Interactable.transform.gameObject.GetComponent<OvenDish>();
        ovendish.GetRecipe(out RecipeData recipe);

		if (!IsCorrectRecipe(recipe))
			return;

		if (socket == _socket)
        {
            _recipeDataDish1 = recipe;

            _badTimerDish1 = _recipeDataDish1.OvenTime * BadTimerMultiplier;

            if (ovendish.HasBurnedBread)
            {
                _currentTimeDish1 = _badTimerDish1;
                _burnedDish1 = true;
            }
            else if (ovendish.HasCompletedBread)
            {
                _currentTimeDish1 = _recipeDataDish1.OvenTime;
                _heatingCompleteDish1 = true;
            }

			_dish1Canvas.UpdateTimer(_currentTimeDish1, _recipeDataDish1.OvenTime, _badTimerDish1);
			_dish1Canvas.SetRecipe(_recipeDataDish1.recipeSprite);
			_dish1Canvas.EnableCanvas();

            _dish1 = ovendish;

        }
        else if (socket == _socketDish2)
        {
            _recipeDataDish2 = recipe;

			_badTimerDish2 = _recipeDataDish2.OvenTime * BadTimerMultiplier;

            if (ovendish.HasBurnedBread)
            {
                _currentTimeDish2 = _badTimerDish2;
                _burnedDish2 = true;
            }
            else if (ovendish.HasCompletedBread)
            {
                _currentTimeDish2 = _recipeDataDish2.OvenTime;
                _heatingCompleteDish1 = true;
            }

			_dish2Canvas.UpdateTimer(_currentTimeDish2, _recipeDataDish2.OvenTime, _badTimerDish2);
			_dish2Canvas.SetRecipe(_recipeDataDish2.recipeSprite);
			_dish2Canvas.EnableCanvas();

            _dish2 = ovendish;
        }
    }

    public override void SocketSelectedExit(XRSocketToolInteractor socket)
    {
        if (socket == _socket)
        {
            _recipeDataDish1 = null;
            _currentTimeDish1 = 0f;
            _badTimerDish1 = 0f;
            _burnedDish1 = false;
            _heatingCompleteDish1 = false;

			_dish1Canvas.ClearCanvas();
			_dish1Canvas.DisableCanvas();
		}
        else if (socket == _socketDish2)
        {
            _recipeDataDish2 = null;
            _currentTimeDish2 = 0f;
            _badTimerDish2 = 0f;
            _burnedDish2 = false;
            _heatingCompleteDish2 = false;

			_dish2Canvas.ClearCanvas();
			_dish2Canvas.DisableCanvas();
		}
    }

    protected override void TurnOff()
    {
        Debug.Log("Desligou");

        if (_socket.Interactable != null)
        {
            _socket.IsToolOn = false;

            XRBaseInteractable grabInteractable = _socket.Interactable.transform.gameObject.GetComponent<XRBaseInteractable>();
            grabInteractable.interactionLayers = _dishInteractionLayerMask;
            _isHeating = false;
        }

        if (_socketDish2.Interactable != null)
        {

            _socketDish2.IsToolOn = false;

            XRBaseInteractable grabInteractable = _socketDish2.Interactable.transform.gameObject.GetComponent<XRBaseInteractable>();
            grabInteractable.interactionLayers = _dishInteractionLayerMask;
            _isHeating = false;
        }

		_warningHelper.Hide();
	}

    protected override void TurnOn()
    {
        Debug.Log("Ligou");

        if (_socket.Interactable != null)
        {
            _socket.IsToolOn = true;

            XRBaseInteractable grabInteractable = _socket.Interactable.transform.gameObject.GetComponent<XRBaseInteractable>();
            _dishInteractionLayerMask = grabInteractable.interactionLayers;
            grabInteractable.interactionLayers = _trackInteractionLayerMask;

            if (_recipeDataDish1 != null)
            {
                _isHeating = true;
            }
        }

        if (_socketDish2.Interactable != null)
        {
            _socketDish2.IsToolOn = true;

            XRBaseInteractable grabInteractable = _socketDish2.Interactable.transform.gameObject.GetComponent<XRBaseInteractable>();
            _dishInteractionLayerMask = grabInteractable.interactionLayers;
            grabInteractable.interactionLayers = _trackInteractionLayerMask;

            if (_recipeDataDish2 != null)
            {
                _isHeating = true;
            }
        }
    }

    private bool IsCorrectRecipe(RecipeData recipeData)
    {
        if(recipeData == null)
            return false;

        if (recipeData.OvenTime == 0f)
            return false;

        return true;
    }

    private void HeatDish1()
    {
        _currentTimeDish1 += Time.deltaTime;

		_dish1Canvas.UpdateTimer(_currentTimeDish1, _recipeDataDish1.OvenTime, _badTimerDish1);

        if (_currentTimeDish1 >= _recipeDataDish1.OvenTime)
            _showWarning = true;

		if (!_burnedDish1 && _currentTimeDish1 >= _badTimerDish1)
        {
			BurnedBread(_socket);
        }
        else if (!_heatingCompleteDish1 && _currentTimeDish1 >= _recipeDataDish1.OvenTime)
        {
			MakeBread(_socket);
		}
    }
    private void HeatDish2()
    {
        _currentTimeDish2 += Time.deltaTime;

		_dish2Canvas.UpdateTimer(_currentTimeDish2, _recipeDataDish2.OvenTime, _badTimerDish2);

		if (_currentTimeDish2 >= _recipeDataDish2.OvenTime)
			_showWarning = true;

		if (!_burnedDish2 && _currentTimeDish2 >= _badTimerDish2)
        {
            BurnedBread(_socketDish2);
        }
        else if (!_heatingCompleteDish2 && _currentTimeDish2 >= _recipeDataDish2.OvenTime)
        {
            MakeBread(_socketDish2);
        }

    }

    private void MakeBread(XRSocketToolInteractor socket)
    {
        if (_socket == socket)
        {
            _heatingCompleteDish1 = true;
        }
        else if (_socketDish2 == socket)
        {
            _heatingCompleteDish2 = true; 
        }

        OvenDish ovendish = socket.Interactable.transform.gameObject.GetComponent<OvenDish>();
        ovendish.MakeBread();
    }

    private void BurnedBread(XRSocketToolInteractor socket)
    {

        if (_socket == socket)
        {
            _burnedDish1 = true;
        }
        else if (_socketDish2 == socket)
        {
            _burnedDish2 = true;
        }

        OvenDish ovendish = socket.Interactable.transform.gameObject.GetComponent<OvenDish>();
        ovendish.MakeBread(burned: true);
    }
}
