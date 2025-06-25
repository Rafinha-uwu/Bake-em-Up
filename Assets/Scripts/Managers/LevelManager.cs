using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
	public GameObject roulote_object;
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

    public bool WaveStarted = false;

    public DialogueRunner DialogueRunner;

    public WaveSpawner WaveSpawner;

	public MilitarPhone MilitarPhone;

	public ScientistPhone ScientistPhone;

	public GameObject SceneNewRecipe;

	public GameObject SecondRecipe;

	public GameObject Barrels;
	public GameObject PasteBag;

	public bool SpawnedFirstRecipe = false;

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

	[YarnCommand("spawn_pastry_bag")]
	public static void SpawnPastryBag()
	{
        Instance.PasteBag.SetActive(true);
    }

	[YarnCommand("spawn_barrels")]
	public static void SpawnBarrels()
	{
        Instance.Barrels.SetActive(true);
    }

	[YarnCommand("start_ringing_phones")]
	public static void StartPhonesRinging()
	{
		LevelEvents.PhonesStartRinging();
	}

	[YarnCommand("start_wave")]
	public static void StartWaveAfterDialogue()
	{
        if (Instance.WaveSpawner.IsUnityNull())
            throw new NullReferenceException("LevelManager.Instance has no WaveSpawner to start the wave after the dialogue");

		Instance.WaveSpawner.StartAfterDialogue();
	}

	[YarnCommand("spawn_new_recipe")]
	public static void SpawnNewRecipe(string person)
	{
		if (person.Equals("military", StringComparison.OrdinalIgnoreCase))
		{
			if (!Instance.SpawnedFirstRecipe)
			{
				Instance.MilitarPhone.SpawnNewRecipe(Instance.SceneNewRecipe);
				Instance.SpawnedFirstRecipe = true;
			}
			else
				Instance.MilitarPhone.SpawnNewRecipe(Instance.SecondRecipe);
		}
		else if(person.Equals("scientist", StringComparison.OrdinalIgnoreCase))
		{
			if (!Instance.SpawnedFirstRecipe)
			{
				Instance.ScientistPhone.SpawnNewRecipe(Instance.SceneNewRecipe);
				Instance.SpawnedFirstRecipe = true;
			}
			else
				Instance.ScientistPhone.SpawnNewRecipe(Instance.SecondRecipe);
		}
	}

	[YarnCommand("hang_up")]
	public static void HangUpPhone(string person)
	{
		if (person.Equals("military", StringComparison.OrdinalIgnoreCase))
		{
			Instance.MilitarPhone.HangUpPhone();
		}
		else if (person.Equals("scientist", StringComparison.OrdinalIgnoreCase))
		{
			Instance.ScientistPhone.HangUpPhone();
		}
	}

	public void PhonePickedUp()
	{
		if(!Instance.MilitarPhone.IsUnityNull())
			Instance.MilitarPhone.StopRinging();
		
		if(!Instance.ScientistPhone.IsUnityNull())
			Instance.ScientistPhone.StopRinging();
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
