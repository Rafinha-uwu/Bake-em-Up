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

	[Header("Dialogue")]
	[SerializeField]
	private string _phoneNodeName;

	[SerializeField]
	private Transform _spawnRecipePoint;

	private bool _isRinging;

    private XRBaseInteractable _interactable;
	private WorldIndicatorHelper _warningHelper;

	private void Awake()
	{
		_interactable = GetComponent<XRBaseInteractable>();
        _interactable.selectEntered.AddListener(StartDialogue);

        LevelEvents.OnPhonesStartRinging += StartPhoneRinging;
	}

	protected virtual void Start()
	{
		_warningHelper = GetComponentInChildren<WorldIndicatorHelper>();
		_warningHelper.SetTargetPosition(_transformForIndicatorHelper.position);
		_warningHelper.Hide();
	}

	private void OnDestroy()
	{
		_interactable.selectEntered.RemoveListener(StartDialogue);
		LevelEvents.OnPhonesStartRinging -= StartPhoneRinging;
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

	public void SpawnNewRecipe(GameObject newRecipe)
	{
		Instantiate(newRecipe, _spawnRecipePoint.position, _spawnRecipePoint.rotation);
	}

	private IEnumerator StartDialogue()
    {
		_warningHelper.Hide();
		_isRinging = false;
		_audioSource.Stop();
		_audioSource.PlayOneShot(_pickUpClip);

		yield return new WaitForSeconds(_pickUpClip.length);

		if (!LevelManager.Instance.DialogueRunner.IsDialogueRunning)
			LevelManager.Instance.DialogueRunner.StartDialogue(_phoneNodeName);
	}

    private void StartPhoneRinging()
    {
		_warningHelper.Show();
		_isRinging = true;
    }
}
