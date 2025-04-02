using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Oven : ToolCooker
{
    [SerializeField]
    private XRSocketToolInteractor _socketDish2;

    private OvenDish _dish1;
    private OvenDish _dish2;

    private OvenDoor _ovenDoor;

    private InteractionLayerMask _dishInteractionLayerMask;
    private InteractionLayerMask _nothingInteractionLayerMask;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        if (_socket.Interactable != null)
        {
            HeatDish1();
        }
        if (_socketDish2.Interactable != null)
        {
            HeatDish2();
        }
    }

    public override void SocketSelectedEnter(XRSocketToolInteractor socket)
    {
        OvenDish ovendish = socket.Interactable.transform.gameObject.GetComponent<OvenDish>();

        if (socket == _socket)
        {
            if (ovendish.GetRecipe(out _recipeDataDish1))
            {
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

                ovendish.UpdateCanvasTimer(_currentTimeDish1, _recipeDataDish1.MixerTime, _badTimerDish1);
                ovendish.SetCanvasRecipe(_recipeDataDish1.recipeSprite);
            }
            ovendish.EnableCanvas();
            _dish1 = ovendish;

        }
        else if (socket == _socketDish2)
        {
            if (ovendish.GetRecipe(out _recipeDataDish2))
            {
                _badTimerDish2 = _recipeDataDish2.OvenTime * BadTimerMultiplier;

                if (ovendish.HasBurnedBread)
                {
                    _currentTimeDish2 = _badTimerDish2;
                    _burnedDish2 = true;
                }
                else if (ovendish.HasCompletedBread)
                {
                    _currentTimeDish2 = _recipeDataDish1.OvenTime;
                    _heatingCompleteDish1 = true;
                }

                ovendish.UpdateCanvasTimer(_currentTimeDish2, _recipeDataDish2.MixerTime, _badTimerDish2);
                ovendish.SetCanvasRecipe(_recipeDataDish2.recipeSprite);
            }
            ovendish.EnableCanvas();
            _dish2 = ovendish;
        }
    }

    public override void SocketSelectedExit(XRSocketToolInteractor socket)
    {
        OvenDish ovendish = socket.Interactable.transform.gameObject.GetComponent<OvenDish>();
        ovendish.ClearCanvas();
        ovendish.DisableCanvas();

        if (socket == _socket)
        {
            _recipeDataDish1 = null;
            _currentTimeDish1 = 0f;
            _badTimerDish1 = 0f;
            _burnedDish1 = false;
            _heatingCompleteDish1 = false;
        }
        else if (socket == _socketDish2)
        {
            _recipeDataDish2 = null;
            _currentTimeDish2 = 0f;
            _badTimerDish2 = 0f;
            _burnedDish2 = false;

            _heatingCompleteDish2 = false;
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
    }

    protected override void TurnOn()
    {
        Debug.Log("Ligou");
        if(_socket)

        if (_socket.Interactable != null)
        {
            _socket.IsToolOn = true;

            XRBaseInteractable grabInteractable = _socket.Interactable.transform.gameObject.GetComponent<XRBaseInteractable>();
            _dishInteractionLayerMask = grabInteractable.interactionLayers;
            grabInteractable.interactionLayers = _nothingInteractionLayerMask;

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
            grabInteractable.interactionLayers = _nothingInteractionLayerMask;

            if (_recipeDataDish2 != null)
            {
                _isHeating = true;
            }
        }
    }

    private void HeatDish1()
    {
        _currentTimeDish1 += Time.deltaTime;

        _dish1.UpdateCanvasTimer(_currentTimeDish1, _recipeDataDish1.OvenTime, _badTimerDish1);

        if (!_burnedDish1 && _currentTimeDish1 >= _badTimerDish1)
        {
            Debug.Log("Estragou a massa!");
            MakeBread(_socket);

        }
        else if (!_heatingCompleteDish1 && _currentTimeDish1 >= _recipeDataDish1.OvenTime)
        {
            Debug.Log("Terminou de Misturar");
            BurnedBread(_socket);
        }
    }
    private void HeatDish2()
    {
        _currentTimeDish2 += Time.deltaTime;

        _dish2.UpdateCanvasTimer(_currentTimeDish2, _recipeDataDish2.OvenTime, _badTimerDish2);

        if (!_burnedDish2 && _currentTimeDish2 >= _badTimerDish2)
        {
            Debug.Log("Estragou a massa!");
            MakeBread(_socketDish2);

        }
        else if (!_heatingCompleteDish2 && _currentTimeDish2 >= _recipeDataDish2.OvenTime)
        {
            Debug.Log("Terminou de Misturar");
            BurnedBread(_socketDish2);
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
        ovendish.BurnBread();

    }
}
