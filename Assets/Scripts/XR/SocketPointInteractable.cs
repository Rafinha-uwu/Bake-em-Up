using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SocketPointInteractable : MonoBehaviour
{
	[SerializeField]
	private Transform _socketPointTransform;

	private Transform _defaultAttachTransform;
	private XRGrabInteractable _interactable;

	private void Awake()
	{
		_interactable = GetComponent<XRGrabInteractable>();
		_defaultAttachTransform = _interactable.attachTransform;
		_interactable.selectEntered.AddListener(SelectEnter);
		_interactable.selectExited.AddListener(SelectExit);
	}

	private void OnDestroy()
	{
		_interactable.selectEntered.RemoveListener(SelectEnter);
		_interactable.selectExited.RemoveListener(SelectExit);
	}

	private void SelectEnter(SelectEnterEventArgs args)
	{
		if (_socketPointTransform != null && args.interactorObject is XRSocketInteractor)
		{
			_interactable.attachTransform = _socketPointTransform;
		}
	}

	private void SelectExit(SelectExitEventArgs args)
	{
		if (_socketPointTransform != null && args.interactorObject is XRSocketInteractor)
		{
			_interactable.attachTransform = _defaultAttachTransform;
		}
	}
}
