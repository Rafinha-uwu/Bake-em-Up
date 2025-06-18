using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Trash : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			return;

		var interactable = other.gameObject.GetComponentInParent<XRGrabInteractable>();

		if (!interactable.IsUnityNull() && interactable.isSelected)
			return;

		Resettable resettable = other.GetComponentInParent<Resettable>();
		if (resettable != null)
		{
			resettable.ResetObject();
		}
		else
		{
			if (!interactable.IsUnityNull())
				Destroy(interactable.gameObject);
		}

		GetComponent<Animator>().Play("Shake_Trash");
	}
}
