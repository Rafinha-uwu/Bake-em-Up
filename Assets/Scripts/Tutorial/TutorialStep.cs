using UnityEngine;

public class TutorialStep: MonoBehaviour
{
    [SerializeField]
	protected Sprite _indicatorSprite;
	[SerializeField]
	protected Transform _targetTransform;
	protected bool _isCurrentStep = false;
	protected TutorialConditionStep _conditionStep;

	protected virtual void Start()
	{
		_conditionStep = GetComponentInParent<TutorialConditionStep>();
	}

	public virtual void ShowStep(WorldIndicatorHelper indicatorHelper)
	{
		indicatorHelper.SetTargetPosition(_targetTransform);
		indicatorHelper.SetIndicatorImage(_indicatorSprite);
		_isCurrentStep = true;
	}

	public void StepCompleted()
	{
		_isCurrentStep = false;
		_conditionStep.TutorialStepCompleted(this);
	}

	public void StepFailed()
	{
		_conditionStep.TutorialStepFailed(this);
	}
}
