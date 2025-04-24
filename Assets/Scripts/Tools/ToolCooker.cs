using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public abstract class ToolCooker : Tool
{
	[SerializeField]
	protected bool _needsCanvas = true;
	[SerializeField]
	protected GameObject _canvasObject;
	[SerializeField]
	private Transform _transformForCanvasToFollow;
	[SerializeField]
	private int _badTimerPercent;
	public float BadTimerMultiplier 
	{ 
		get { return _badTimerPercent / 100f; }
	}

	protected ToolButton _toolButton;
	[SerializeField]
	protected XRSocketToolInteractor _socket;
	protected ToolCanvas _toolCanvas;

	protected virtual void Awake()
	{
		if (_needsCanvas)
		{
            GameObject canvas = Instantiate(_canvasObject, transform.position, transform.rotation);
            _toolCanvas = canvas.GetComponent<ToolCanvas>();
            _toolCanvas.AddTransformToFollow(_transformForCanvasToFollow);
            _toolCanvas.DisableCanvas();
        }
		
		if (_badTimerPercent <= 100f)
		{
			throw new System.NotSupportedException($"Bad Timer Percent is {_badTimerPercent}%, needs to be more than 100%");
		}
	}

	protected virtual void Start()
	{
		_toolButton = GetComponentInChildren<ToolButton>();
	}

	public abstract void SocketSelectedEnter(XRSocketToolInteractor socket);
	public abstract void SocketSelectedExit(XRSocketToolInteractor socket);
	protected abstract void TurnOn();
	protected abstract void TurnOff();
}
