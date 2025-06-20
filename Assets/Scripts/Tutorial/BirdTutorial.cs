using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class BirdTutorial : MonoBehaviour
{
    private bool _startTutorial = false;
	private bool _isOnCooldown = false;
    private bool _finishedDialogue = false;
    private bool _isResetting = false;

    private void Update()
    {
        if (_isResetting)
            return;

        if (LevelManager.Instance.DialogueRunner.IsDialogueRunning) return;

        if (_finishedDialogue)
        {
            _finishedDialogue = false;
			StartCoroutine(DialogueCoolDown());
		}

        if (_startTutorial && !_isOnCooldown)
        {
            LevelManager.Instance.DialogueRunner.StartDialogue("BirdTutorial");

            _isOnCooldown = true;
        }
    }

    [YarnCommand("start_tutorial")]
    public void StartTutorial()
    {
        _startTutorial = true;
	}

    public void FinishTutorial()
    {
        _startTutorial = false;
		Destroy(this);
	}

    public void ResetDialogue()
    {
		if (_isResetting) return;

		_isResetting = true;
		StopAllCoroutines();
		LevelManager.Instance.DialogueRunner.Stop();

        _finishedDialogue = false;
        _isOnCooldown = false;

		StartCoroutine(CompleteReset());
	}

	private IEnumerator CompleteReset()
	{
		yield return new WaitForSeconds(0.1f);
		_isResetting = false; 
	}

	[YarnCommand("finished_dialogue")]
	public void FinishedDialogue()
    {
        _finishedDialogue = true;
	}

    private IEnumerator DialogueCoolDown()
    {
		yield return new WaitForSeconds(5);
        _isOnCooldown = false;
    }
}
