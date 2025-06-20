using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager Instance { get; private set; }

	[SerializeField]
    private WorldIndicatorHelper _indicatorHelper;

    [SerializeField]
    private List<TutorialConditionStep> _tutorialConditionSteps;

	[Header("Dialogue")]
	[SerializeField]
	private SerializableDictionary<int, string> _tutorialAndDialogueConditions = new();

	[SerializeField]
	private BirdTutorial _bird;

	[Header("Scene Objects")]
	[SerializeField]
	private Bowl _bowl;
	public Bowl Bowl => _bowl;

	[SerializeField]
	private Mixer _mixer;
	public Mixer Mixer => _mixer;

	[SerializeField]
	private WoodenBoard _woodBoard;
	public WoodenBoard WoodBoard => _woodBoard;

	[SerializeField]
	private OvenDish _ovenDish1;
	public OvenDish OvenDish1 => _ovenDish1;

	[SerializeField]
	private OvenDish _ovenDish2;
	public OvenDish OvenDish2 => _ovenDish2;

	[SerializeField]
	private Oven _oven;
	public Oven Oven => _oven;

	[SerializeField]
	private List<IngredientContainerManager> _ingredients;
	public List<IngredientContainerManager> Ingredients => _ingredients;

	[SerializeField]
	private Tool _rollingPin;
	public Tool RollingPin => _rollingPin;

	[SerializeField]
	private GameObject _overDoor;
	public GameObject OverDoor => _overDoor;

	[SerializeField]
	private Balcony _balcony;
	public Balcony Balcony => _balcony;

	private int _currentTutorialCondition = 0;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		_tutorialConditionSteps[_currentTutorialCondition].StartCondition(_indicatorHelper);
	}

	public void TutorialConditionCompleted(TutorialConditionStep condition)
	{
		if(_tutorialConditionSteps[_currentTutorialCondition].GetInstanceID() == condition.GetInstanceID())
		{
			string dialogueCondition = _tutorialAndDialogueConditions[_currentTutorialCondition];
			LevelManager.Instance.DialogueRunner.VariableStorage.SetValue(dialogueCondition, true);

			if (_currentTutorialCondition == _tutorialConditionSteps.Count - 1)
			{
				_bird.FinishTutorial();
				Destroy(gameObject);
				_indicatorHelper.Hide();
				return;
			}

			_bird.ResetDialogue();
			_currentTutorialCondition++;
			_tutorialConditionSteps[_currentTutorialCondition].StartCondition(_indicatorHelper);
		}
	}

	public void TutorialConditionFailed(TutorialConditionStep condition)
	{
		if (_currentTutorialCondition == 0 || _currentTutorialCondition == _tutorialConditionSteps.Count - 1)
			return;

		if (_tutorialConditionSteps[_currentTutorialCondition-1].GetInstanceID() == condition.GetInstanceID())
		{
			_currentTutorialCondition--;
			_tutorialConditionSteps[_currentTutorialCondition].StartCondition(_indicatorHelper);

			string dialogueCondition = _tutorialAndDialogueConditions[_currentTutorialCondition];
			LevelManager.Instance.DialogueRunner.VariableStorage.SetValue(dialogueCondition, false);

			_bird.ResetDialogue();
		}
	}
}
