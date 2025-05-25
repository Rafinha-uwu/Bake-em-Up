using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Fryer : ToolCooker
{
	[SerializeField]
	private XRSocketToolInteractor _socketFryerOil;

	[SerializeField]
	private CookerCanvas _fryerCanvas;
	private FryerBasket _basket;

	private RecipeData _recipeData;
	private bool _isHeating = false;
	private float _currentTimeBasket = 0f;
	private float _badTimerBasket = 0f;
	private bool _heatingCompleteBasket = false;
	private bool _burnedBasket = false;

	[SerializeField]
	private Material _baketHelperMaterial;
	private MeshFilter _basketMeshFilter;
	private Matrix4x4 _basketMatrix;
	private bool _showBasketOnFryer = false;

	protected override void Awake()
	{
		base.Awake();
		_toolCanvas = _fryerCanvas;
		_fryerCanvas.DisableCanvas();
	}

	protected override void Start()
	{
		base.Start();

		_socketFryerOil.hoverEntered.AddListener(HoverEntered);
		_socketFryerOil.hoverExited.AddListener(HoverExited);

		_basket = LevelManager.Instance.GetBasket();
		_basket.OnBasketHasDough += ShowBasketMeshOnSocket;
		_basket.OnBasketEmpty += HideBasketMeshOnSocket;

		_basketMeshFilter = _basket.GetComponentInChildren<MeshFilter>();
		_basketMatrix = UtilsClass.GetHoverMeshMatrix(_basket.GetComponent<XRGrabInteractable>(), _basketMeshFilter, 1f, _socketFryerOil);
	}

	private void OnDestroy()
	{
		_socketFryerOil.hoverEntered.RemoveListener(HoverEntered);
		_socketFryerOil.hoverExited.RemoveListener(HoverExited);
	}

	// Update is called once per frame
	void Update()
	{
		if(_showBasketOnFryer)
			DrawHelperMesh();

		if (!_isHeating)
			return;

		if (_socketFryerOil.Interactable != null)
		{
			HeatBasket();
		}
	}

	public override void SocketSelectedEnter(XRSocketToolInteractor socket)
	{
		FryerBasket basket = socket.Interactable.transform.gameObject.GetComponent<FryerBasket>();
		basket.GetRecipe(out RecipeData recipe);

		if (!IsCorrectRecipe(recipe))
			return;

		_recipeData = recipe;

		_badTimerBasket = _recipeData.FryingTime * BadTimerMultiplier;

		if (basket.HasBurnedBread)
		{
			_currentTimeBasket = _badTimerBasket;
			_burnedBasket = true;
		}
		else if (basket.HasCompletedBread)
		{
			_currentTimeBasket = _recipeData.FryingTime;
			_heatingCompleteBasket = true;
		}

		_fryerCanvas.SetRecipe(_recipeData.recipeSprite);
		_fryerCanvas.UpdateTimer(_currentTimeBasket, _recipeData.FryingTime, _badTimerBasket);
		_fryerCanvas.EnableCanvas();

		_basket = basket;

		if (socket == _socketFryerOil)
			TurnOn();
	}

	public override void SocketSelectedExit(XRSocketToolInteractor socket)
	{
		if (socket == _socketFryerOil)
			TurnOff();

		if (!_fryerCanvas.IsUnityNull())
		{
			_fryerCanvas.ClearCanvas();
			_fryerCanvas.DisableCanvas();
		}

		_recipeData = null;
		_currentTimeBasket = 0f;
		_badTimerBasket = 0f;
		_burnedBasket = false;
		_heatingCompleteBasket = false;
		_basket = null;
	}

	protected override void TurnOn()
	{
		HideBasketMeshOnSocket();

		if (_recipeData != null)
		{
			_isHeating = true;
		}
	}

	protected override void TurnOff()
	{
		_isHeating = false;
		_basket = null;
		_warningHelper.Hide();
	}

	private bool IsCorrectRecipe(RecipeData recipeData)
	{
		if (recipeData == null)
			return false;

		if (recipeData.FryingTime == 0f)
			return false;

		return true;
	}

	private void HeatBasket()
	{
		_currentTimeBasket += Time.deltaTime;

		_fryerCanvas.UpdateTimer(_currentTimeBasket, _recipeData.FryingTime, _badTimerBasket);

		if (_currentTimeBasket > _recipeData.FryingTime)
			_warningHelper.Show();
		else 
			_warningHelper.Hide();

		if (!_burnedBasket && _currentTimeBasket >= _badTimerBasket)
		{
			BurnedBread();
		}
		else if (!_heatingCompleteBasket && _currentTimeBasket >= _recipeData.FryingTime)
		{
			MakeBread();
		}
	}

	private void MakeBread()
	{
		_heatingCompleteBasket = true;

		_basket.MakeBread();
	}

	private void BurnedBread()
	{
		_burnedBasket = true;

		_basket.MakeBread(burned: true);
	}

	private void ShowBasketMeshOnSocket()
	{
		_showBasketOnFryer = true;
	}

	private void HideBasketMeshOnSocket()
	{
		_showBasketOnFryer = false;
	}

	private void HoverEntered(HoverEnterEventArgs args)
	{
		if (args.interactorObject as XRSocketToolInteractor == _socketFryerOil)
			HideBasketMeshOnSocket();
	}

	private void HoverExited(HoverExitEventArgs args)
	{
		FryerBasket basket = args.interactableObject.transform.GetComponent<FryerBasket>();
		if (args.interactorObject as XRSocketToolInteractor == _socketFryerOil && basket.HasDough)
		{
			ShowBasketMeshOnSocket();
			Debug.Log(args.interactorObject.transform.name);
		}
	}

	private void DrawHelperMesh()
	{
		Graphics.DrawMesh(
				_basketMeshFilter.sharedMesh,
				_basketMatrix,
				_baketHelperMaterial,
				gameObject.layer
		);
	}
}
