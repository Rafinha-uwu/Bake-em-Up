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

	private Collider _collider;
	private Dough _doughOnBoard;

	private void Awake()
	{
		_collider = GetComponent<Collider>();
	}

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
		_collider.enabled = true;
	}

	private void DoughRemoved(SelectExitEventArgs args)
	{
		_doughSocket.socketActive = false;
		_doughOnBoard = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Dough Roller"))
		{
			if (!_doughOnBoard)
				return;

			_doughOnBoard.KneadDough(gameObject);

			return;
		}

		XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
		if (interactable.isSelected)
			return;

		if (other.gameObject.CompareTag("Dough"))
		{
			if (_doughSocket.hasSelection)
				return;

			_doughSocket.socketActive = true;
			_doughSocket.interactionManager.SelectEnter(_doughSocket as IXRSelectInteractor, interactable as IXRSelectInteractable);

			_doughOnBoard = other.gameObject.GetComponentInParent<Dough>();
			_doughOnBoard.transform.SetParent(null, true);

			SetLayerAllChildren(_doughOnBoard.transform, "Grabbable");

			return;
		}
		else if (other.gameObject.CompareTag("Shaped Dough"))
		{
			_shapedDoughsSocketsManager.ReceivedShapedDough();
			_collider.enabled = false;

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
}
