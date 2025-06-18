using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Yarn.Unity;

public class PhoneController : MonoBehaviour
{
	[SerializeField]
	private Transform _transformForIndicatorHelper;

	[Header("Audio")]
	[SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _ringingClip;
	[SerializeField]
	private AudioClip _pickUpClip;
	[SerializeField]
	private AudioClip _hangUpClip;

	private string _nodeName;
    private bool _isRinging;

    private XRBaseInteractable _interactable;
	private WorldIndicatorHelper _warningHelper;

	private void Awake()
	{
		_interactable = GetComponent<XRBaseInteractable>();
        _interactable.selectEntered.AddListener(StartDialogue);

        LevelEvents.OnPhoneStartRinging += StartPhoneRinging;
	}

	private void Start()
	{
		_warningHelper = GetComponentInChildren<WorldIndicatorHelper>();
		_warningHelper.SetTargetPosition(_transformForIndicatorHelper.position);
		_warningHelper.Hide();
	}

	private void OnDestroy()
	{
		_interactable.selectEntered.RemoveListener(StartDialogue);
		LevelEvents.OnPhoneStartRinging -= StartPhoneRinging;
	}

	private void LateUpdate()
	{
        if (_isRinging && !_audioSource.isPlaying)
            _audioSource.PlayOneShot(_ringingClip);
	}

	public void StartDialogue(SelectEnterEventArgs args)
    {
        if (!_isRinging)
            return;

        StartCoroutine(StartDialogue());
    }

    private IEnumerator StartDialogue()
    {
		_warningHelper.Hide();
		_isRinging = false;
		_audioSource.Stop();
		_audioSource.PlayOneShot(_pickUpClip);

		yield return new WaitForSeconds(_pickUpClip.length);

		if (!LevelManager.Instance.DialogueRunner.IsDialogueRunning)
			LevelManager.Instance.DialogueRunner.StartDialogue(_nodeName);
	}

    private void StartPhoneRinging(string nodeName)
    {
		_warningHelper.Show();
		_isRinging = true;
        _nodeName = nodeName;
    }
}
