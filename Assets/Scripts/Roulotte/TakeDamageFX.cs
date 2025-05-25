using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class TakeDamageFX : MonoBehaviour
{
	[SerializeField]
	private Animator _canvasAnimator;

	[SerializeField]
	[Range(0.1f, 1f)]
	private float _hapticAmplitude = 0.5f;
	[SerializeField]
	[Range(0.1f, 1f)]
	private float _hapticDuration = 0.2f;

	private InputDevice _leftHand;
    private InputDevice _rightHand;

	private bool _isPlaying;

	void Start()
    {
		InitializeDevices();
	}

    private void InitializeDevices()
    {
		var left = new List<InputDevice>();
		var right = new List<InputDevice>();

		InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, left);
		InputDevices.GetDevicesAtXRNode(XRNode.RightHand, right);

		if (left.Count > 0) _leftHand = left[0];
		if (right.Count > 0) _rightHand = right[0];
	}

	public void PlayFX()
	{
		if(!_isPlaying)
			StartCoroutine(StartFXCoroutine());
	}

	private IEnumerator StartFXCoroutine()
	{
		_isPlaying = true;
		TriggerHaptics(_hapticAmplitude, _hapticDuration);
		_canvasAnimator.SetTrigger("Hit");
		yield return new WaitForSeconds(1f);

		_isPlaying = false;
	}

	private void TriggerHaptics(float amplitude, float duration)
	{
		if (!_leftHand.isValid || !_rightHand.isValid)
			InitializeDevices();

		if (_leftHand.isValid)
		{
			_leftHand.TryGetHapticCapabilities(out var capabilities);
			Debug.Log(capabilities.ToString());
			if(capabilities.supportsImpulse)
				_leftHand.SendHapticImpulse(0, amplitude, duration);
		}

		if (_rightHand.isValid)
		{
			_rightHand.TryGetHapticCapabilities(out var capabilities);
			Debug.Log(capabilities.ToString());
			if (capabilities.supportsImpulse)
				_rightHand.SendHapticImpulse(0, amplitude, duration);
		}
	}
}
