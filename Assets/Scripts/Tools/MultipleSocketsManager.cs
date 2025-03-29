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
	private string _tagToCompare;

	private Dictionary<XRSocketInteractor, bool> _socketAvailable = new();
	private Dictionary<XRBaseInteractable, XRSocketInteractor> _interactablesAttach = new();

	private DoughSocketsManager _doughSocketsManager;
	private int _usedSockets = 0;

	private void Awake()
	{
		foreach (XRSocketInteractor socket in _socketsInteractors)
		{
			_socketAvailable.Add(socket, true);
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

	private void Start()
	{
		_doughSocketsManager = GetComponentInParent<DoughSocketsManager>();
	}

	public int GetSocketsCount()
	{
		return _socketsInteractors.Count;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_compareTag && !other.gameObject.CompareTag(_tagToCompare))
			return;

		XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
		if (interactable.isSelected)
			return;

		if (_interactablesAttach.ContainsKey(interactable))
			return;
		
		XRSocketInteractor socket = _socketAvailable.FirstOrDefault(kv => kv.Value).Key;
		if (socket == null)
			return;

		if (!_doughSocketsManager.IsSameRecipe(other.gameObject.GetComponentInParent<Dough>().GetRecipe()))
			return;

		socket.socketActive = true;
		socket.interactionManager.SelectEnter(socket as IXRSelectInteractor, interactable as IXRSelectInteractable);
		_usedSockets += 1;

		_socketAvailable[socket] = false;
		_interactablesAttach.Add(interactable, socket);
	}

	private void InteractableRemoved(SelectExitEventArgs args)
	{
		XRBaseInteractable interactable = args.interactableObject as XRBaseInteractable;
		XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;

		socket.socketActive = false;
		_socketAvailable[socket] = true;
		_interactablesAttach.Remove(interactable);
		_usedSockets -= 1;
		if(_usedSockets == 0)
		{
			_doughSocketsManager.GridIsEmpty(this);
		}
	}
}

