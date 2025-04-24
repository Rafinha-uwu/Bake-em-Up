using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ResizeInContainer : MonoBehaviour
{
    [SerializeField]
    private float _resizeValue = 0.8f;

	private void Awake()
	{
		if (_resizeValue == 0f)
		{
			_resizeValue = 0.8f;
			throw new ArgumentOutOfRangeException($"Resize Value is 0. Needs to be different than 0!");
		}
	}

	public void EnteredContainer(XRBaseInteractable interactable)
    {
		interactable.transform.localScale *= _resizeValue;
	}

    public void ExitedContainer(XRBaseInteractable interactable)
    {
		interactable.transform.localScale = Vector3.one;
	}
}
