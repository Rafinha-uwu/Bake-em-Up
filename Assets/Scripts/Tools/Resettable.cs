using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Resettable : MonoBehaviour
{
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;
	private XRGrabInteractable _interactable;

	public delegate void ObjectResetHandler();
	public event ObjectResetHandler OnObjectReset;

	private bool _lateReset = false;

	private void Awake()
	{
		_initialPosition = transform.position;
		_initialRotation = transform.rotation;
		_interactable = GetComponent<XRGrabInteractable>();
	}

	private void Update()
	{
		if(_lateReset && !_interactable.isSelected)
		{
			_lateReset = false;
			StartCoroutine(ProcessReset(1.5f));
		}
	}

	public void ResetObject(bool window = false)
	{
		if (!window)
		{
			StartCoroutine(ProcessReset(0f));
		}
		else
		{
			_lateReset = true;
		}
	}

	public void CancelReset()
	{
		_lateReset = false;
	}

	private IEnumerator ProcessReset(float time)
	{
		yield return new WaitForSeconds(time);

		OnObjectReset?.Invoke();

		if (_interactable.isSelected)
			_interactable.interactionManager.SelectExit(_interactable.firstInteractorSelecting, _interactable);

		transform.SetPositionAndRotation(_initialPosition, _initialRotation);
	}
}
