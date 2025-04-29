using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Fryer : ToolCooker
{
	[SerializeField]
	private XRSocketToolInteractor _socketFryerOil;

	private FryerBasket _basket;

	//private InteractionLayerMask _basketInteractionLayerMask;
	//[SerializeField]
	//private InteractionLayerMask _trackInteractionLayerMask;

	private RecipeData _recipeData;
	private bool _isHeating = false;
	private float _currentTimeBasket = 0f;
	private float _badTimerBasket = 0f;
	private bool _heatingCompleteBasket = false;
	private bool _burnedBasket = false;

	// Update is called once per frame
	void Update()
	{
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

		basket.UpdateCanvasTimer(_currentTimeBasket, _recipeData.FryingTime, _badTimerBasket);
		basket.SetCanvasRecipe(_recipeData.recipeSprite);
		basket.EnableCanvas();

		_basket = basket;

		if (socket == _socketFryerOil)
			TurnOn();
	}

	public override void SocketSelectedExit(XRSocketToolInteractor socket)
	{
		if (socket == _socketFryerOil)
			TurnOff();

		FryerBasket basket = socket.Interactable.transform.gameObject.GetComponent<FryerBasket>();
		basket.ClearCanvas();
		basket.DisableCanvas();

		_recipeData = null;
		_currentTimeBasket = 0f;
		_badTimerBasket = 0f;
		_burnedBasket = false;
		_heatingCompleteBasket = false;
	}

	protected override void TurnOn()
	{
		Debug.Log("Ligou Fritadeira");

		if (_recipeData != null)
		{
			_isHeating = true;
		}
	}

	protected override void TurnOff()
	{
		Debug.Log("Desligou Fritadeira");
		_isHeating = false;
		_basket = null;
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

		_basket.UpdateCanvasTimer(_currentTimeBasket, _recipeData.FryingTime, _badTimerBasket);

		if (!_burnedBasket && _currentTimeBasket >= _badTimerBasket)
		{
			Debug.Log("Estragou a massa!");
			BurnedBread();
		}
		else if (!_heatingCompleteBasket && _currentTimeBasket >= _recipeData.FryingTime)
		{
			Debug.Log("Terminou de Misturar");
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

		_basket.BurnBread();
	}
}
