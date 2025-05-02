using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabInteractablesOnSecondHand : MonoBehaviour
{
	[SerializeField]
	private int ignoreFirstHandGrabLayerIndex;
	
	[SerializeField]
	private int defaultInteractableLayerIndex;

	private XRGrabInteractable _interactable;
    private List<GameObject> _interactablesOnSockets = new();

	private bool _isGrabedByHand = false;

	private void Awake()
	{
		_interactable = GetComponent<XRGrabInteractable>();
		_interactable.selectEntered.AddListener(SocketGrabed);
		_interactable.selectExited.AddListener(SocketReleased);
	}

	private void OnDestroy()
	{
		_interactable.selectEntered.RemoveListener(SocketGrabed);
		_interactable.selectExited.RemoveListener(SocketReleased);
	}

	public void InteractableEnteredSocket(XRBaseInteractable interactable)
	{
		GameObject interactableGO = interactable.gameObject;
		if (_interactablesOnSockets.Contains(interactableGO))
			return;

		_interactablesOnSockets.Add(interactableGO);

		if (!_isGrabedByHand)
			SetLayerAllChildren(interactableGO.transform, ignoreFirstHandGrabLayerIndex);
	}

	public void InteractableExitedSocket(XRBaseInteractable interactable)
	{
		GameObject interactableGO = interactable.gameObject;

		if (!_interactablesOnSockets.Contains(interactableGO))
			return;

		_interactablesOnSockets.Remove(interactableGO);
		SetLayerAllChildren(interactableGO.transform, defaultInteractableLayerIndex);
	}

	private void SocketGrabed(SelectEnterEventArgs args)
	{
		if (args.interactorObject.transform.CompareTag("Player"))
		{
			foreach(GameObject obj in _interactablesOnSockets)
			{
				SetLayerAllChildren(obj.transform, defaultInteractableLayerIndex);
			}
		}
	}

	private void SocketReleased(SelectExitEventArgs args)
	{
		if (args.interactorObject.transform.CompareTag("Player"))
		{
			foreach (GameObject obj in _interactablesOnSockets)
			{
				SetLayerAllChildren(obj.transform, ignoreFirstHandGrabLayerIndex);
			}
		}
	}

	private void SetLayerAllChildren(Transform root, int layerIndex)
	{
		var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (var child in children)
		{
			child.gameObject.layer = layerIndex;
		}
	}
}
