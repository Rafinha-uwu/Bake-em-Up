using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class BowlController : MonoBehaviour
{
	[SerializeField]
	private GameObject _container;

	private Dictionary<IngredientName, int> _ingredientsInside = new();

	private void OnTriggerEnter(Collider other)
	{
		GameObject obj = other.gameObject;

		if (obj.TryGetComponent<XRGrabInteractable>(out var interactable))
		{
			if (!interactable.isSelected) return;
			ReleaseItem(interactable);
			InsertItem(obj);
		}
	}

	private void InsertItem(GameObject obj)
	{
		obj.transform.SetParent(_container.transform, false);
		obj.transform.localPosition = Vector3.zero;
		int LayerInsideBowl = LayerMask.NameToLayer("Inside Bowl");
		obj.layer = LayerInsideBowl;

		if(obj.TryGetComponent<IngredientController>(out var ingredient))
		{
			AddIngredient(ingredient);
		}
	}

	private void AddIngredient(IngredientController ingredient)
	{
		IngredientName name = ingredient.IngredientName;
		if(_ingredientsInside.TryGetValue(name, out int value))
		{
			_ingredientsInside[name] += 1;
		}
		else
		{
			_ingredientsInside.Add(name, 1);
		}
	}

	private void ReleaseItem(XRGrabInteractable interactable)
	{
		XRInteractionManager interactionManager = interactable.interactionManager;

		interactionManager.SelectExit(interactable.firstInteractorSelecting, interactable);
	}
}
