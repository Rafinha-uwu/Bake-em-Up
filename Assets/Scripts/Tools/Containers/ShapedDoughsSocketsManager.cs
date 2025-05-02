using System.Collections.Generic;
using System.Net.Sockets;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ShapedDoughsSocketsManager : MonoBehaviour
{
	[SerializeField]
	private List<MultipleSocketsManager> _shapedDoughSocketManager;

	private Dictionary<int, MultipleSocketsManager> _socketsManagerDict = new();

	private Collider _collider;
	private WoodenBoard _woodenBoard;
	private ToolContainer _toolContainer;
	private RecipeData _shapedDoughRecipe;

	private void Awake()
	{
		_collider = GetComponent<Collider>();

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
		_collider.enabled = false;
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
		manager.gameObject.SetActive(false);
		_shapedDoughRecipe = null;
		_collider.enabled = false;
		MessageContainerThatIsEmpty();
	}

	public void ReceivedItem()
	{
		_collider.enabled = true;
	}

	public void ReleaseAllDough()
	{
        MultipleSocketsManager manager = _socketsManagerDict[_shapedDoughRecipe.shapedDoughCount];
		manager.ReleaseAllItems();
    }

	public void DestroyAllDough()
	{
		if (_shapedDoughRecipe)
		{
			MultipleSocketsManager manager = _socketsManagerDict[_shapedDoughRecipe.shapedDoughCount];
			manager.DestroyAllItems();
		}
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
			recipe = objectToValidate.GetComponentInParent<Bread>().GetRecipe();
		}

		return _shapedDoughRecipe == recipe;
	}

	private bool ValidatePriority(GameObject objectToValidate)
	{
		XRBaseInteractable interactable = objectToValidate.GetComponentInParent<XRBaseInteractable>();
		if (!interactable.isSelected)
			return true;

		XRBaseInteractor interactor = interactable.firstInteractorSelecting as XRBaseInteractor;
		if (interactor.transform.CompareTag("Player"))
			return false;

		if (_toolContainer == null)
			return false;

		return _toolContainer.HasPriorityOver(interactor.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_collider.enabled)
			return;

		RecipeData recipe = null;
		if(other.gameObject.CompareTag("Shaped Dough"))
		{
			Debug.Log("rs");
			recipe = other.gameObject.GetComponentInParent<ShapedDough>().GetRecipe();
		}
		else if (other.gameObject.CompareTag("Bread"))
		{
            recipe = other.gameObject.GetComponentInParent<Bread>().GetRecipe();
        }

		if(recipe != null)
		{
            XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();

            if (!ValidatePriority(interactable.gameObject))
			{
                return;
			}

            MultipleSocketsManager manager = _socketsManagerDict[recipe.shapedDoughCount];
            manager.gameObject.SetActive(true);

            _shapedDoughRecipe = recipe;
            _collider.enabled = false;
        }
        
    }
}