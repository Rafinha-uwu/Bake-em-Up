using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Linq;


public class MultipleSocketsManager : MonoBehaviour
{
	[SerializeField]
	private List<XRSocketInteractor> _socketsInteractors = new();

	[SerializeField]
	private bool _compareTag = false;
	[SerializeField]
	private List<string> _tagsToCompare;

	private Dictionary<XRSocketInteractor, bool> _socketsAvailables = new();
	private Dictionary<XRBaseInteractable, XRSocketInteractor> _interactablesAttach = new();

	public delegate void GridEmptyHandler(MultipleSocketsManager manager);
	public event GridEmptyHandler OnGridEmpty;

	public delegate bool ValidateObjectHandler(GameObject objectToValidate);
	public event ValidateObjectHandler OnValidateObject;

	private int _usedSockets = 0;

	private void Awake()
	{
		foreach (XRSocketInteractor socket in _socketsInteractors)
		{
			_socketsAvailables.Add(socket, true);
			socket.selectExited.AddListener(InteractableRemoved);
		}
	}
	private void OnDestroy()
	{
		foreach (XRSocketInteractor socket in _socketsInteractors)
		{
			socket.selectExited.RemoveListener(InteractableRemoved);
		}
	}

	public int GetSocketsCount()
	{
		return _socketsInteractors.Count;
	}

	public void ReleaseAllDough()
	{
		List<XRBaseInteractable> interactables = _interactablesAttach.Keys.ToList();

		foreach(var inter in interactables)
		{
            inter.interactionManager.SelectExit(inter.firstInteractorSelecting as IXRSelectInteractor, inter as IXRSelectInteractable);
        }
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_compareTag && !CompareTags(other.tag))
			return;

		XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
		if (interactable.isSelected)
			return;

		if (_interactablesAttach.ContainsKey(interactable))
			return;
		
		XRSocketInteractor socket = _socketsAvailables.FirstOrDefault(kv => kv.Value).Key;
		if (socket == null)
			return;

		if (!CheckValidations(other.gameObject))
			return;

		socket.socketActive = true;
		socket.interactionManager.SelectEnter(socket as IXRSelectInteractor, interactable as IXRSelectInteractable);
		_usedSockets += 1;

		_socketsAvailables[socket] = false;
		_interactablesAttach.Add(interactable, socket);
	}

	private bool CompareTags(string objectTag)
	{
		bool result = false;
		foreach(string tag in _tagsToCompare)
		{
			if (tag.Equals(objectTag))
				result = true;
		}
		return result;
	}

	private void InteractableRemoved(SelectExitEventArgs args)
	{
		XRBaseInteractable interactable = args.interactableObject as XRBaseInteractable;
		XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;

		socket.socketActive = false;
		_socketsAvailables[socket] = true;
		_interactablesAttach.Remove(interactable);
		_usedSockets -= 1;
		if(_usedSockets == 0)
		{
			OnGridEmpty?.Invoke(this);
		}
	}

	private bool CheckValidations(GameObject objectToValidate)
	{
		if(OnValidateObject == null) return true;

		return OnValidateObject.GetInvocationList()
		.Cast<ValidateObjectHandler>()
		.All(ev => ev.Invoke(objectToValidate));
	}
}

