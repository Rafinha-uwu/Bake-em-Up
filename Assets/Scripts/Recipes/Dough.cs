using System.ComponentModel.Design;
using UnityEngine;

public class Dough : MonoBehaviour
{
	[SerializeField]
	private RecipeData _recipeData;

	[SerializeField]
	private bool _isBadDough = false;

	[SerializeField]
	private bool _isShaped = false;

	private int _kneadsToShape = 5;
	public int _kneadCount = 0;

	public RecipeData GetRecipe()
	{
		return _recipeData;
	}

	public void KneadDough(GameObject parent)
	{
		if (_isBadDough)
			return;

		_kneadCount += 1;

		if(_kneadCount == _kneadsToShape)
		{
			Debug.Log(parent.name);
			for(int i = 0; i < _recipeData.shapedDoughCount; i++)
			{
				Vector3 positionToSpawn = new Vector3(transform.position.x + Random.Range(-0.05f, 0.05f),
					transform.position.y, transform.position.z + Random.Range(-0.05f, 0.05f));
				GameObject shapedDough = Instantiate(_recipeData.shapedDoughPrefab, positionToSpawn, Quaternion.identity);
			}

			Destroy(gameObject);
		}
	}
}
