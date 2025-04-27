using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable)), RequireComponent(typeof(Resettable))]
public class PastryBag : ToolContainer
{
	[SerializeField]
	private Transform _canvasTransformLeftHand; 
	private int _remainingCream = 0;
	private int _maxCream = 0;

	private RecipeData _recipeData;
	private XRGrabInteractable _interactable;
	private PastryBagCanvas _pastryBagCanvas;
	private Resettable _resettable;

	protected override void Awake()
	{
		base.Awake();
		_pastryBagCanvas = _toolCanvas as PastryBagCanvas;
		DisableCanvas();

		_interactable = GetComponent<XRGrabInteractable>();
		_interactable.selectEntered.AddListener(SelectEntered);
		_interactable.selectExited.AddListener(SelectExited);

		_resettable = GetComponent<Resettable>();
		_resettable.OnObjectCreateCopy += TransferObjectData;
	}

	private void OnDestroy()
	{
		_interactable.selectEntered.RemoveListener(SelectEntered);
		_interactable.selectExited.RemoveListener(SelectExited);
		_resettable.OnObjectCreateCopy -= TransferObjectData;
	}

	public void Shot()
	{
		if (_remainingCream == 0)
			return;

		_remainingCream -= 1;
		if (_remainingCream == 0)
		{
			_recipeData = null;
			_maxCream = 0;
		}
		_pastryBagCanvas.UpdateCounter(_remainingCream);
	}

	public void CopyData(int remainingCream, int maxCream, RecipeData recipe)
	{
		_remainingCream = remainingCream;
		_maxCream = maxCream;
		_recipeData = recipe;

		if (recipe != null)
			_pastryBagCanvas.UpdateCounter(_remainingCream);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Cream"))
		{
			XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
			if (interactable == null || interactable.isSelected)
				return;

			PastryCream cream = other.gameObject.GetComponentInParent<PastryCream>();
			_recipeData = cream.GetRecipe();
			AddCream();

			Destroy(cream.gameObject);
			return;
		}
	}

	private void SelectEntered(SelectEnterEventArgs args)
	{
		if (args.interactorObject.transform.CompareTag("Player"))
		{
			Debug.Log("Entrou");
			Transform canvasFollow = args.interactableObject.IsSelectedByLeft() ? _canvasTransformLeftHand : _transformForCanvasToFollow;
			_pastryBagCanvas.AddTransformToFollow(canvasFollow);
			EnableCanvas();
		}
	}

	private void SelectExited(SelectExitEventArgs args)
	{
		if (args.interactorObject.transform.CompareTag("Player"))
			DisableCanvas();
	}

	private void AddCream()
	{
		_remainingCream += _recipeData.shapedDoughCount;
		
		_maxCream = _recipeData.shapedDoughCount * 2;
		
		if(_remainingCream > _maxCream)
		{
			_remainingCream = _maxCream;
		}

		_pastryBagCanvas.UpdateCounter(_remainingCream);
	}

	private void TransferObjectData(GameObject copy)
	{
		PastryBag copyPastryBag = copy.GetComponent<PastryBag>();
		copyPastryBag.CopyData(_remainingCream, _maxCream, _recipeData);
	}
}
