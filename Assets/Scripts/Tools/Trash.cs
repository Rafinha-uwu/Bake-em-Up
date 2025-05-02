using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Trash : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			return;

		Resettable resettable = other.GetComponentInParent<Resettable>();
		if (resettable != null)
		{
			resettable.ResetObject();
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
