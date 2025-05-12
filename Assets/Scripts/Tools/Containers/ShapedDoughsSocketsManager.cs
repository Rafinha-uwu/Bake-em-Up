using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ShapedDoughsSocketsManager : MonoBehaviour
{
	[SerializeField]
	private List<MultipleSocketsManager> _shapedDoughSocketManager;

	private Dictionary<int, MultipleSocketsManager> _socketsManagerDict = new();

	private WoodenBoard _woodenBoard;
	private ToolContainer _toolContainer;
	private RecipeData _shapedDoughRecipe;
	private GameObject _firstItemEntered;
	private MultipleSocketsManager _currentActiveManager;

	private void Awake()
	{
		foreach(MultipleSocketsManager manager in _shapedDoughSocketManager)
		{
			_socketsManagerDict.Add(manager.GetSocketsCount(), manager);
			manager.OnValidateObject += ValidateRecipe;
			manager.OnValidateObject += ValidatePriority;
			manager.OnGridEmpty += GridIsEmpty;
		}
	}

	private void Start()
	{
		_woodenBoard = GetComponentInParent<WoodenBoard>();
		_toolContainer = GetComponentInParent<ToolContainer>();
	}

	private void OnDestroy()
	{
		foreach (MultipleSocketsManager manager in _shapedDoughSocketManager)
		{
			manager.OnValidateObject -= ValidateRecipe;
			manager.OnValidateObject -= ValidatePriority;
			manager.OnGridEmpty -= GridIsEmpty;
		}
	}

	public void GridIsEmpty(MultipleSocketsManager manager)
	{
		Debug.Log($"Ficou vazio {transform.root.name}");
		manager.gameObject.SetActive(false);
		_shapedDoughRecipe = null;
		_currentActiveManager = null;
		_firstItemEntered = null;
		MessageContainerThatIsEmpty();
	}

	public void ReceivedItem(RecipeData recipe, GameObject item)
	{
		if (!recipe.IsUnityNull())
		{
			if (!ValidatePriority(item))
			{
				MessageContainerThatIsEmpty();
				return;
			}

			MultipleSocketsManager manager = _socketsManagerDict[recipe.shapedDoughCount];
			manager.gameObject.SetActive(true);

			_firstItemEntered = item;
			_currentActiveManager = manager;
			_shapedDoughRecipe = recipe;
			
			OnContainerTriggerEnter(item);
		}
		else
		{
			throw new NullReferenceException($"Recipe is null on ShapedDoughManager for the container {transform.root.name}!");
		}
	}

	public void ReleaseAllDough()
	{
        MultipleSocketsManager manager = _socketsManagerDict[_shapedDoughRecipe.shapedDoughCount];
		manager.ReleaseAllItems();
    }

	public void DestroyAllDough()
	{
		if (!_shapedDoughRecipe.IsUnityNull())
		{
			MultipleSocketsManager manager = _socketsManagerDict[_shapedDoughRecipe.shapedDoughCount];
			manager.DestroyAllItems();
		}
	}

	public void OnContainerTriggerEnter(GameObject item)
	{
		if (!_shapedDoughRecipe.IsUnityNull())
		{
			_currentActiveManager.OnContainerTriggerEnter(item);
		}
		else
		{
			throw new NullReferenceException($"Recipe is null on ShapedDoughManager for the container {transform.root.name}!");
		}
	}

	public int GetSocketsInUse()
	{
		return _currentActiveManager.GetSocketsInUse();
	}

	private void MessageContainerThatIsEmpty()
	{
		if (_woodenBoard)
		{
			_woodenBoard.ShapedDoughsGridIsEmpty();
		}
		else
		{
			_toolContainer.ContainerIsEmpty();
		}
	}

	private bool ValidateRecipe(GameObject objectToValidate)
	{
		RecipeData recipe = null;
		if (objectToValidate.CompareTag("Shaped Dough"))
		{
			recipe = objectToValidate.GetComponentInParent<ShapedDough>().GetRecipe();
		}
		else if (objectToValidate.CompareTag("Bread"))
		{
			Bread firstBread = _firstItemEntered.GetComponentInParent<Bread>();
			Bread validateBread = objectToValidate.GetComponentInParent<Bread>();
			if (firstBread.IsBurned() != validateBread.IsBurned())
				return false;

			recipe = validateBread.GetRecipe();
		}

		return _shapedDoughRecipe == recipe;
	}

	private bool ValidatePriority(GameObject objectToValidate)
	{
		XRBaseInteractable interactable = objectToValidate.GetComponentInParent<XRBaseInteractable>();
		if (!interactable.isSelected)
			return true;

		XRBaseInteractor interactor = interactable.firstInteractorSelecting as XRBaseInteractor;
		//Provavelmente aqui da para fazer para mostrar o mesh do objeto que ta sendo segurado pelo player e colocar no meio mesmo
		if (interactable.IsSelectedByLeft() || interactable.IsSelectedByRight())
			return false;

		if (_toolContainer == null)
			return false;

		return _toolContainer.HasPriorityOver(interactor.gameObject);
	}
}