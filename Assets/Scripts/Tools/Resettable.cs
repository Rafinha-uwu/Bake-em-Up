using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Resettable : MonoBehaviour
{
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;
	private XRGrabInteractable _interactable;

	public delegate void ObjectResetHandler();
	public event ObjectResetHandler OnObjectReset;

	private void Awake()
	{
		_initialPosition = transform.position;
		_initialRotation = transform.rotation;
		_interactable = GetComponent<XRGrabInteractable>();
	}

	public void ResetObject()
	{
		OnObjectReset?.Invoke();

		if(_interactable.isSelected)
			_interactable.interactionManager.SelectExit(_interactable.firstInteractorSelecting, _interactable);

		transform.SetPositionAndRotation(_initialPosition, _initialRotation);
		//_rb.linearVelocity = Vector3.zero;
		//_rb.angularVelocity = Vector3.zero;
	}
}
