using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable)), RequireComponent(typeof(Resettable))]
public class PastryBag : ToolContainer
{
	[SerializeField]
	private Transform _canvasTransform;
	public int _remainingCream = 0;
	private int _maxCream = 0;
	private bool _grabbed = false;

	private XRGrabInteractable _interactable;
	[SerializeField]
	private CounterCanvas _pastryBagCanvas;

	private Shoot _dispara;

	[Header("Projectile Trajectory")]
	[SerializeField]
	private LineRenderer _lineRenderer;
	[SerializeField]
	[Range(10, 100)]
	private int _linePoints = 25;
	[SerializeField]
	[Range(0.01f, 0.25f)]
	private float _timeBetweenPoints = 0.1f;
	[SerializeField]
	private LayerMask _projectileCollisionMask;
	private int aimCount = 5;

	[SerializeField]
	private float bulletSpeed = 4.5f;
	[SerializeField]
	private float bulletMass = 0.15f;


	protected void Awake()
	{
		_pastryBagCanvas.AddTransformToFollow(_canvasTransform);
		_pastryBagCanvas.UpdateCounter(_remainingCream);

		_interactable = GetComponent<XRGrabInteractable>();
		_interactable.selectEntered.AddListener(SelectEntered);
		_interactable.selectExited.AddListener(SelectExited);

		_dispara = GetComponent<Shoot>();
	}

	private void OnDestroy()
	{
		_interactable.selectEntered.RemoveListener(SelectEntered);
		_interactable.selectExited.RemoveListener(SelectExited);
	}

	private void Update()
	{
		if(_grabbed && aimCount > 0)
			DrawProjection();
	}

	public void Shot()
	{
		if (_remainingCream == 0)
			return;

		if (!_dispara.OnDispara())
			return;

		_remainingCream -= 1;

		aimCount -= 1;
		if (aimCount <= 0)
			_lineRenderer.enabled = false;

		if (_remainingCream == 0)
		{
			_recipeData = null;
			_maxCream = 0;
		}
		_pastryBagCanvas.UpdateCounter(_remainingCream);
	}

	public void EnableCanvas()
	{
		if (!_pastryBagCanvas.IsUnityNull())
			_pastryBagCanvas.EnableCanvas();
	}

	public void DisableCanvas()
	{
		if (!_pastryBagCanvas.IsUnityNull())
			_pastryBagCanvas.DisableCanvas();
	}

	private void OnTriggerEnter(Collider other)
	{
		XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
		if (interactable.IsUnityNull())
			return;

		if (interactable.TryGetComponent<Bowl>(out var auxBowl))
		{
			if (auxBowl.HasCompletedDough && auxBowl.GetDough().CompareTag("Cream"))
			{
				auxBowl.GetRecipe(out _recipeData);
				Debug.Log(_recipeData.name);
				LevelEvents.BakedNewRecipe(_recipeData);
				AddCream();
				auxBowl.DoughRemoved();
				return;
			}
		}
	}

	private void SelectEntered(SelectEnterEventArgs args)
	{
		if (args.interactorObject.transform.CompareTag("Player"))
		{
			_grabbed = true;
			EnableCanvas();
		}
	}

	private void SelectExited(SelectExitEventArgs args)
	{
		if (args.interactorObject.transform.CompareTag("Player"))
		{
			_grabbed = false;
			_lineRenderer.enabled = false;
			DisableCanvas();
		}
	}

	private void AddCream()
	{
		_remainingCream += _recipeData.shapedDoughCount;
		
		_maxCream = _recipeData.shapedDoughCount * 2;
		
		if(_remainingCream > _maxCream)
		{
			_remainingCream = _maxCream;
		}

		_pastryBagCanvas.UpdateCounter(_remainingCream);
	}

	private void DrawProjection()
	{
		_lineRenderer.enabled = true;
		_lineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenPoints) + 1;
		Vector3 startPosition = _dispara.shootPoint.position;
		Vector3 startVelocity = bulletSpeed * _dispara.shootPoint.forward;
		int i = 0;
		_lineRenderer.SetPosition(i, startPosition);
		for (float time = 0; time < _linePoints; time += _timeBetweenPoints)
		{
			i++;
			Vector3 point = startPosition + time * startVelocity;
			point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y * bulletMass / 2f * time * time);

			_lineRenderer.SetPosition(i, point);

			Vector3 lastPosition = _lineRenderer.GetPosition(i - 1);

			if (Physics.Raycast(lastPosition,
				(point - lastPosition).normalized,
				out RaycastHit hit,
				(point - lastPosition).magnitude,
				_projectileCollisionMask))
			{
				_lineRenderer.SetPosition(i, hit.point);
				_lineRenderer.positionCount = i + 1;
				return;
			}
		}
	}
}
