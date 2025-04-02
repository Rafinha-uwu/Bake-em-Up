using System.Collections.Generic;
using System.Net.Sockets;
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
	private OvenDish _ovenDish;
	private RecipeData _shapedDoughRecipe;

	private void Awake()
	{
		_collider = GetComponent<Collider>();

		foreach(MultipleSocketsManager manager in _shapedDoughSocketManager)
		{
			_socketsManagerDict.Add(manager.GetSocketsCount(), manager);
			manager.OnValidateObject += ValidateRecipe;
			manager.OnGridEmpty += GridIsEmpty;
		}
	}

	private void Start()
	{
		_woodenBoard = GetComponentInParent<WoodenBoard>();
		_ovenDish = GetComponentInParent<OvenDish>();

		if (_woodenBoard || _ovenDish)
			_collider.enabled = false;
	}

	private void OnDestroy()
	{
		foreach (MultipleSocketsManager manager in _shapedDoughSocketManager)
		{
			manager.OnValidateObject -= ValidateRecipe;
			manager.OnGridEmpty -= GridIsEmpty;
		}
	}

	public void GridIsEmpty(MultipleSocketsManager manager)
	{
		manager.gameObject.SetActive(false);
		if (_woodenBoard)
		{
			_woodenBoard.ShapedDoughsGridIsEmpty();
		}
		else
		{
			_collider.enabled = true;
		}
	}

	public void ReceivedShapedDough()
	{
		_collider.enabled = true;
	}

	public void ReleaseAllDough()
	{
        MultipleSocketsManager manager = _socketsManagerDict[_shapedDoughRecipe.shapedDoughCount];
		manager.ReleaseAllDough();
    }

	private bool ValidateRecipe(GameObject objectToValidate)
	{
		RecipeData recipe = objectToValidate.GetComponentInParent<ShapedDough>().GetRecipe();
		return _shapedDoughRecipe == recipe;
	}

	private void OnTriggerEnter(Collider other)
	{
		RecipeData recipe = null;
		if(other.gameObject.CompareTag("Shaped Dough"))
		{
			recipe = other.gameObject.GetComponentInParent<ShapedDough>().GetRecipe();
		}
		else if (other.gameObject.CompareTag("Bread"))
		{
            recipe = other.gameObject.GetComponentInParent<Bread>().GetRecipe();
        }

		if(recipe != null)
		{
            XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();

            if (interactable.isSelected)
                return;

            MultipleSocketsManager manager = _socketsManagerDict[recipe.shapedDoughCount];
            manager.gameObject.SetActive(true);

            _shapedDoughRecipe = recipe;
            _collider.enabled = false;
        }
        
    }
}