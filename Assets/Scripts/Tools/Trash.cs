using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Trash : MonoBehaviour
{
	[SerializeField]
	private Collider _collider;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			return;


		if (other.transform.root.TryGetComponent<Resettable>(out var objToReset))
		{
			objToReset.ResetObject();
		}
		else
		{
			var interactable = other.gameObject.GetComponentInParent<XRGrabInteractable>();
			if (interactable.isSelected)
			{
				interactable.interactionManager.SelectExit(interactable.firstInteractorSelecting, interactable);
			}

			Destroy(interactable.gameObject);
		}
	}
}
