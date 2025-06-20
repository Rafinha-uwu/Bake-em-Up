using System;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

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

    public bool WaveStarted = false;

    public DialogueRunner DialogueRunner;

    public WaveSpawner WaveSpawner;

	public MilitarPhone MilitarPhone;

	public ScientistPhone ScientistPhone;

	public GameObject SceneNewRecipe;

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
			Instance.MilitarPhone.SpawnNewRecipe(Instance.SceneNewRecipe);
		}
		else if(person.Equals("scientist", StringComparison.OrdinalIgnoreCase))
		{
			Instance.ScientistPhone.SpawnNewRecipe(Instance.SceneNewRecipe);
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
