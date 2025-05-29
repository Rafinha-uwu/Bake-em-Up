using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Transform roulote;
    public Transform targetZombies;
    public Transform playerStartPosition;

	[SerializeField]
	private Mixer _mixer;

    [SerializeField]
    private Bowl _bowl;

    [SerializeField]
    private OvenDish _dish1;

	[SerializeField]
	private OvenDish _dish2;

	[SerializeField]
	private FryerBasket _basket;

	private void Awake()
    {
		if (Instance != null && Instance != this)
		{
			Debug.LogWarning("Duplicate LevelManager detected. Destroying new instance.");
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

    public Bowl GetBowl()
    {
        if (_bowl.IsUnityNull())
        {
            _bowl = FindAnyObjectByType<Bowl>();
        }

        return _bowl;
    }

    public Mixer GetMixer()
    {
        if (_mixer.IsUnityNull())
        {
            _mixer = FindAnyObjectByType<Mixer>();
        }

        return _mixer;
    }

	public List<OvenDish> GetOvenDishes()
	{
        List<OvenDish> dishes = new();

		if (_dish1.IsUnityNull() || _dish2.IsUnityNull())
		{
            dishes = FindObjectsByType<OvenDish>(FindObjectsSortMode.InstanceID).ToList();
            Debug.Log(dishes.Count);
            _dish1 = dishes[0];
			_dish2 = dishes[1];
		}
        else
        {
            dishes.Add(_dish1);
            dishes.Add(_dish2);
        }

		return dishes;
	}

	public FryerBasket GetBasket()
	{
		if (_basket.IsUnityNull())
		{
			_basket = FindAnyObjectByType<FryerBasket>();
		}

		return _basket;
	}


	private void OnDestroy()
    {
        // Clear the static reference when this instance is destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
}
