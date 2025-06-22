using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Yarn.Unity;

#nullable enable

[CreateAssetMenu(fileName = "VoiceOverLine", menuName = "Scriptable Objects/VoiceOverLine")]
public class VoiceOverLine : ScriptableObject, IAssetProvider
{
	public AudioClip? audioClip;

	public bool TryGetAsset<T>([NotNullWhen(true)] out T? result) where T : UnityEngine.Object
	{
		if (typeof(T).IsAssignableFrom(typeof(VoiceOverLine)))
		{
			result = (T)(object)this;
			return true;
		}

		if (typeof(T).IsAssignableFrom(typeof(AudioClip)))
		{
			if (audioClip != null)
			{
				result = (T)(object)audioClip;
				return true;
			}
		}

		result = null;
		return false;
	}

	public IEnumerable<T> GetAssetsOfType<T>() where T : UnityEngine.Object
	{
		if (TryGetAsset(out T? result))
		{
			return new[] { result };
		}
		else
		{
			return Array.Empty<T>();
		}
	}
}
