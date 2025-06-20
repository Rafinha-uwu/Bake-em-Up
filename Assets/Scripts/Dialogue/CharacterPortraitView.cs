using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class CharacterPortraitView : DialoguePresenterBase
{
	[SerializeField] SerializableDictionary<string, Sprite> _charactersPortrait = new();

	[SerializeField]
	private Image _portraitImage;

	public override YarnTask OnDialogueStartedAsync()
	{
		return YarnTask.CompletedTask;
	}

	public override YarnTask OnDialogueCompleteAsync()
	{
		return YarnTask.CompletedTask;
	}

	public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
	{
		if (string.IsNullOrEmpty(line.CharacterName) || !_charactersPortrait.TryGetValue(line.CharacterName, out Sprite portrait))
		{
			throw new NullReferenceException($"Can't set character portrait to empty charactername or no character found in the dict<string, Sprite>");
		}

		_portraitImage.sprite = portrait;

		return YarnTask.CompletedTask;
	}

	public override YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] dialogueOptions, CancellationToken cancellationToken)
	{
		return YarnTask.FromResult<DialogueOption?>(null);
	}
}
