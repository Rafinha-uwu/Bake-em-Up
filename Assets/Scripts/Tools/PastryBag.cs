using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable)), RequireComponent(typeof(Resettable))]
public class PastryBag : ToolContainer
{
	[SerializeField]
	private Transform _canvasTransformLeftHand; 
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

	protected override void Awake()
	{
		base.Awake();
		_toolCanvas = _pastryBagCanvas;
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
		if(_grabbed)
			DrawProjection();
	}

	public void Shot()
	{
		if (_remainingCream == 0)
			return;

		_remainingCream -= 1;
		_dispara.OnDispara();

        if (_remainingCream == 0)
		{
			_recipeData = null;
			_maxCream = 0;
		}
		_pastryBagCanvas.UpdateCounter(_remainingCream);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Cream"))
		{
			XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();
			if (interactable == null || interactable.isSelected)
				return;

			PastryCream cream = other.gameObject.GetComponentInParent<PastryCream>();
			_recipeData = cream.GetRecipe();
			AddCream();

			Destroy(cream.gameObject);
			return;
		}
	}

	private void SelectEntered(SelectEnterEventArgs args)
	{
		if (args.interactorObject.transform.CompareTag("Player"))
		{
			_grabbed = true;
			Transform canvasFollow = args.interactableObject.IsSelectedByLeft() ? _canvasTransformLeftHand : _transformForCanvasToFollow;
			_pastryBagCanvas.AddTransformToFollow(canvasFollow);
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
		Vector3 startVelocity = 5f * _dispara.shootPoint.forward;
		int i = 0;
		_lineRenderer.SetPosition(i, startPosition);
		for (float time = 0; time < _linePoints; time += _timeBetweenPoints)
		{
			i++;
			Vector3 point = startPosition + time * startVelocity;
			point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y * 0.15f / 2f * time * time);

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
