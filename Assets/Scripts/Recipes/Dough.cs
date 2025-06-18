using System.ComponentModel.Design;
using UnityEngine;

public class Dough : MonoBehaviour
{
	[SerializeField]
	private RecipeData _recipeData;

	private AudioSource _audioSource;

	[SerializeField]
	private bool _isBadDough = false;

	private int _kneadsToShape = 5;
	private int _kneadCount = 0;


    private void Start()
    {
		_audioSource = GetComponent<AudioSource>();
	}
    public RecipeData GetRecipe()
	{
		return _recipeData;
	}

	public bool KneadDough()
	{
		if (_isBadDough)
			return false;
		_audioSource.Play();
		_kneadCount += 1;

		if(_kneadCount == _kneadsToShape)
		{
			Destroy(gameObject);

			for (int i = 0; i < _recipeData.shapedDoughCount; i++)
			{
				Vector3 positionToSpawn = new Vector3(transform.position.x + Random.Range(-0.05f, 0.05f),
					transform.position.y, transform.position.z + Random.Range(-0.05f, 0.05f));
				Instantiate(_recipeData.shapedDoughPrefab, positionToSpawn, Quaternion.identity);
			}

			return true;
		}

		return false;
	}
}
