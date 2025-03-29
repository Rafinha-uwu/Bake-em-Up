using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DoughSocketsManager : MonoBehaviour
{
	[SerializeField]
	private List<MultipleSocketsManager> _shapedDoughSocketManager;

	[SerializeField]
	private XRSocketInteractor _doughSocket;

	private Dictionary<int, MultipleSocketsManager> _socketsManagerDict = new();

	private Collider _collider;

	private bool _hasDoughSocket;
	private RecipeData _shapedDoughRecipe;

	private void Awake()
	{
		_collider = GetComponent<Collider>();
		_hasDoughSocket = _doughSocket != null;

		foreach(MultipleSocketsManager manager in _shapedDoughSocketManager)
		{
			_socketsManagerDict.Add(manager.GetSocketsCount(), manager);
		}
	}

	private void OnEnable()
	{
		if (_doughSocket)
			_doughSocket.selectExited.AddListener(DoughRemoved);
	}

	private void OnDisable()
	{
		if (_doughSocket)
			_doughSocket.selectExited.RemoveListener(DoughRemoved);
	}

	public bool IsSameRecipe(RecipeData recipe)
	{
		return _shapedDoughRecipe == recipe;
	}

	public void GridIsEmpty(MultipleSocketsManager manager)
	{
		manager.gameObject.SetActive(false);
		_shapedDoughRecipe = null;
		_collider.enabled = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();

		if (interactable.isSelected)
			return;

		if (_hasDoughSocket && _doughSocket.hasSelection)
			return;

		if (other.gameObject.CompareTag("Dough"))
		{
			_doughSocket.socketActive = true;
			_doughSocket.interactionManager.SelectEnter(_doughSocket as IXRSelectInteractor, interactable as IXRSelectInteractable);
		}
		else if(other.gameObject.CompareTag("Shaped Dough"))
		{
			RecipeData recipe = other.gameObject.GetComponentInParent<Dough>().GetRecipe();

			MultipleSocketsManager manager = _socketsManagerDict[recipe.shapedDoughCount];
			manager.gameObject.SetActive(true);

			_collider.enabled = false;
			_shapedDoughRecipe = recipe;
		}
	}

	private void DoughRemoved(SelectExitEventArgs args)
	{
		_doughSocket.socketActive = false;
	}
}