using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using Yarn.Unity;

#nullable enable

public class DialoguePositionView : DialoguePresenterBase
{
	[SerializeField] SerializableDictionary<string, Transform> _characterPosition = new();

	[SerializeField]
	private DialogueUI _dialogueUI;

	[SerializeField]
	private DialogueRunner _dialogueRunner;

	public override YarnTask OnDialogueStartedAsync()
	{
		return YarnTask.CompletedTask;
	}

	public override YarnTask OnDialogueCompleteAsync()
	{
		_dialogueUI.RemoveTarget();
		return YarnTask.CompletedTask;
	}

	public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
	{
		Transform targetTransform = null;

		if (string.IsNullOrEmpty(line.CharacterName) || !_characterPosition.TryGetValue(line.CharacterName, out targetTransform))
		{
			Debug.LogError($"Can't set dialogue world position due to empty charactername or no character found in the dict<string, Transform>", this);
		}

		_dialogueUI.SetTargetPosition(targetTransform);
		_dialogueRunner.gameObject.transform.position = targetTransform.position;

		return YarnTask.CompletedTask;
	}

	public override YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] dialogueOptions, CancellationToken cancellationToken)
	{
		return YarnTask.FromResult<DialogueOption?>(null);
	}
}
