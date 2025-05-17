using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WoodenBoard : MonoBehaviour
{
	[SerializeField]
	private XRSocketInteractor _doughSocket;
	[SerializeField]
	private ShapedDoughsSocketsManager _shapedDoughsSocketsManager;

	private Dough _doughOnBoard;
	private bool _hasShapedDough;

	public delegate void WoodenBoardHandler();
	public event WoodenBoardHandler OnDoughOnBoard;
	public event WoodenBoardHandler OnDoughRemovedFromBoard;
	public event WoodenBoardHandler OnDoughKneaded;

	private void Start()
	{
		_shapedDoughsSocketsManager = GetComponentInChildren<ShapedDoughsSocketsManager>();
	}

	private void OnEnable()
	{
		_doughSocket.selectExited.AddListener(DoughRemoved);
	}

	private void OnDisable()
	{
		_doughSocket.selectExited.RemoveListener(DoughRemoved);
	}

	public void ShapedDoughsGridIsEmpty()
	{
		_hasShapedDough = false;
	}

	public void ReleaseAllDough()
	{
        _shapedDoughsSocketsManager.ReleaseAllDough();
    }

	private void DoughRemoved(SelectExitEventArgs args)
	{
		_doughSocket.socketActive = false;
		_doughOnBoard = null;

		StartCoroutine(DetectIfLeftSocketByPlayerHand(args.interactableObject));
	}


	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Dough Roller"))
		{
			if (!_doughOnBoard)
				return;

			if (_doughOnBoard.KneadDough())
				OnDoughKneaded?.Invoke();

			return;
		}

		if (_hasShapedDough)
		{
			_shapedDoughsSocketsManager.OnContainerTriggerEnter(other.gameObject);
			return;
		}

		XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
		if (interactable.isSelected)
			return;

		if (other.gameObject.CompareTag("Dough"))
		{
			if (_doughSocket.hasSelection)
				return;

			Bowl bowl = interactable.GetComponentInParent<Bowl>();
			if (!bowl.IsUnityNull())
				bowl.DoughRemoved();

			OnDoughOnBoard?.Invoke();

			_doughSocket.socketActive = true;
			_doughSocket.interactionManager.SelectEnter(_doughSocket as IXRSelectInteractor, interactable as IXRSelectInteractable);

			_doughOnBoard = other.gameObject.GetComponentInParent<Dough>();
			_doughOnBoard.transform.SetParent(null, true);

			SetLayerAllChildren(_doughOnBoard.transform, "Grabbable");

			return;
		}
		else if (other.gameObject.CompareTag("Shaped Dough"))
		{
			_hasShapedDough = true;
			RecipeData recipe = other.gameObject.GetComponentInParent<ShapedDough>().GetRecipe();
			_shapedDoughsSocketsManager.ReceivedItem(recipe, other.gameObject);

			return;
		}
	}

	private void SetLayerAllChildren(Transform root, string layerName)
	{
		var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (var child in children)
		{
			child.gameObject.layer = LayerMask.NameToLayer(layerName);
		}
	}

	private IEnumerator DetectIfLeftSocketByPlayerHand(IXRSelectInteractable interactable)
	{
		yield return new WaitForEndOfFrame();

		if (!interactable.isSelected)
		{
			OnDoughRemovedFromBoard?.Invoke();
		}
	}
}
