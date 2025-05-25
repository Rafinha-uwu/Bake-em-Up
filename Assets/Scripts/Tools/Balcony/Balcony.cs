using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Balcony : MonoBehaviour
{
	[SerializeField]
	private List<RecipeContainer> _breadContainers;

	private RecipeContainer _containerAux;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Bread"))
		{
			XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
			if (interactable.IsSelectedByLeft() || interactable.IsSelectedByRight())
				return;

			Bread bread = other.gameObject.GetComponentInParent<Bread>();

			RecipeData recipe = bread.GetRecipe();

			Debug.Log(recipe.name);

			if(!_containerAux.IsUnityNull() && _containerAux.GetRecipe() == recipe)
			{
				AddBreadInContainer(_containerAux, bread.gameObject);
				return;
			}

			foreach(RecipeContainer container in _breadContainers)
			{
				if(container.GetRecipe() == recipe)
				{
					_containerAux = container;
					AddBreadInContainer(_containerAux, bread.gameObject);
					return;
				}
			}
		}
	}

	private void AddBreadInContainer(RecipeContainer container, GameObject bread)
	{
		container.AddRecipe();
		Destroy(bread);
	}
}
