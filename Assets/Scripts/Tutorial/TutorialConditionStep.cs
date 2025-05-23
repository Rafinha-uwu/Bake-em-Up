using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialConditionStep : MonoBehaviour
{
    [SerializeField]
    private TutorialCondition _condition;
	[SerializeField]
	private List<TutorialStep> _steps = new();

    private bool _conditionAchieved = false;
	private bool _isCurrentCondition = false;
	private WorldIndicatorHelper _helper;
	
	private int _currentTutorialStep = 0;

	[Serializable]
    public class TutorialCondition
    {
        public ConditionToolName tool;

        public BowlConditions bowl;

        public MixerConditions mixer;

		public WoodBoardConditions woodBoard;

        public OvenDishConditions ovenDish;

        public OvenConditions oven;

        public BalconyConditions balcony;
	}

    public enum ConditionToolName { Bowl, Mixer, WoodBoard, OvenDish, Oven, Balcony }

    public enum BowlConditions { RecipeReady }

    public enum MixerConditions { MixerTunedOn, MixingComplete, MixerTurnedOff}

    public enum WoodBoardConditions { DoughOnBoard, DoughKneaded }

    public enum OvenDishConditions { DoughsOnDish }

    public enum OvenConditions { OvenOpenDoor, OvenTurnedOn, HeatingComplete }

    public enum BalconyConditions { BreadOnBalcony }


	private void Start()
	{
        if (_condition.tool == ConditionToolName.Bowl)
        {
            Bowl bowl = TutorialManager.Instance.Bowl;
            if(_condition.bowl == BowlConditions.RecipeReady)
            {
                bowl.OnRecipeReady += ConditionAchieved;
				bowl.OnRecipeNotReady += ConditionUnattained;
				return;
			}
        }
        
		if(_condition.tool == ConditionToolName.Mixer)
        {
			Mixer mixer = TutorialManager.Instance.Mixer;
			if(_condition.mixer == MixerConditions.MixerTunedOn)
			{
				mixer.OnMixerTurnedOn += ConditionAchieved;
				mixer.OnMixerTurnedOff += ConditionUnattained;
				return;
			}
			
			if(_condition.mixer == MixerConditions.MixingComplete)
			{
				mixer.OnMixingComplete += ConditionAchieved;
				mixer.OnMixingFailed += ConditionUnattained;
				return;
			}
			
			if(_condition.mixer == MixerConditions.MixerTurnedOff)
			{
				mixer.OnMixerTurnedOff += ConditionAchieved;
				mixer.OnMixerTurnedOn += ConditionUnattained;
				return;
			}
        }

		if (_condition.tool == ConditionToolName.WoodBoard)
		{
			WoodenBoard woodenBoard = TutorialManager.Instance.WoodBoard;
			if (_condition.woodBoard == WoodBoardConditions.DoughOnBoard)
			{
				woodenBoard.OnDoughOnBoard += ConditionAchieved;
				woodenBoard.OnDoughRemovedFromBoard += ConditionUnattained;
				return;
			}

			if(_condition.woodBoard == WoodBoardConditions.DoughKneaded)
			{
				woodenBoard.OnDoughKneaded += ConditionAchieved;
				return;
			}
		}

		if (_condition.tool == ConditionToolName.OvenDish)
		{
			OvenDish dish1 = TutorialManager.Instance.OvenDish1;
			OvenDish dish2 = TutorialManager.Instance.OvenDish2;

			if(_condition.ovenDish == OvenDishConditions.DoughsOnDish)
			{
				dish1.OnOvenDishHasDough += ConditionAchieved;
				dish1.OnOvenDishEmpty += ConditionUnattained;

				dish2.OnOvenDishHasDough += ConditionAchieved;
				dish2.OnOvenDishEmpty += ConditionUnattained;
				return;
			}
		}
		
		if (_condition.tool == ConditionToolName.Oven)
		{
			Oven oven = TutorialManager.Instance.Oven;
			if(_condition.oven == OvenConditions.OvenOpenDoor)
			{
				oven.OnOvenTurnOff += ConditionAchieved;
				oven.OnOvenTurnOn += ConditionUnattained;
				return;
			}

			if(_condition.oven == OvenConditions.OvenTurnedOn)
			{
				oven.OnOvenTurnOn += ConditionAchieved;
				oven.OnOvenTurnOff += ConditionUnattained;
				return;
			}

			if (_condition.oven == OvenConditions.HeatingComplete)
			{
				oven.OnHeatingComplete += ConditionAchieved;
				oven.OnHeatingFailed += ConditionUnattained;
				return;
			}
		}
		
		if (_condition.tool == ConditionToolName.Balcony)
		{

		}
	}

	public virtual void StartCondition(WorldIndicatorHelper indicatorHelper)
	{
		if (_conditionAchieved)
		{
			TutorialManager.Instance.TutorialConditionCompleted(this);
			return;
		}

		//_currentTutorialStep = 0;
		_helper = indicatorHelper;
		_isCurrentCondition = true;

		if(_steps.Count > 0)
			_steps[_currentTutorialStep].ShowStep(_helper);
	}

	public void TutorialStepCompleted(TutorialStep step)
	{
		if (!_isCurrentCondition)
			return;

		if (_steps[_currentTutorialStep].GetInstanceID() == step.GetInstanceID())
		{
			_currentTutorialStep = (_currentTutorialStep == _steps.Count - 1) ? 0 : _currentTutorialStep + 1;
			_steps[_currentTutorialStep].ShowStep(_helper);
		}
	}

	public void TutorialStepFailed(TutorialStep step)
	{
		if (!_isCurrentCondition || _currentTutorialStep == 0)
			return;

		if (_steps[_currentTutorialStep - 1].GetInstanceID() == step.GetInstanceID())
		{
			_currentTutorialStep--;
			_steps[_currentTutorialStep].ShowStep(_helper);
		}
	}

	private void ConditionAchieved()
	{
		_isCurrentCondition = false;
		_conditionAchieved = true;
		TutorialManager.Instance.TutorialConditionCompleted(this);
	}

	private void ConditionUnattained()
	{
		_conditionAchieved = false;
		TutorialManager.Instance.TutorialConditionFailed(this);
	}
}
