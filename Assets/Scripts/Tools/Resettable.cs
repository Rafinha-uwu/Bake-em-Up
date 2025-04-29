using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Resettable : MonoBehaviour
{
	[SerializeField]
	private GameObject prefab;

	private Vector3 _initialPosition;
	private Quaternion _initialRotation;
	private XRGrabInteractable _interactable;

	public delegate void ObjectResetHandler();
	public event ObjectResetHandler OnObjectReset;

	public delegate void ObjectCreateCopyHandler(GameObject copy);
	public event ObjectCreateCopyHandler OnObjectCreateCopy;

	private bool createCopy = false;
	private int createOne = 1;

	private void Awake()
	{
		_initialPosition = transform.position;
		_initialRotation = transform.rotation;
		_interactable = GetComponent<XRGrabInteractable>();
	}

	private void Update()
	{
		if(createCopy && !_interactable.isSelected && createOne == 1)
		{
			GameObject copy = Instantiate(prefab, _initialPosition, _initialRotation);
			OnObjectCreateCopy?.Invoke(copy);
			createCopy = false;
			createOne = 0;
		}
	}

	public void ResetObject(bool resetNewInstance = false)
	{
		if (!resetNewInstance)
		{
			OnObjectReset?.Invoke();

			if (_interactable.isSelected)
				_interactable.interactionManager.SelectExit(_interactable.firstInteractorSelecting, _interactable);
			
			transform.SetPositionAndRotation(_initialPosition, _initialRotation);
		}
		else
		{
			createCopy = true;
		}
	}

	public void CancelReset()
	{
		createCopy = false;
	}
}
