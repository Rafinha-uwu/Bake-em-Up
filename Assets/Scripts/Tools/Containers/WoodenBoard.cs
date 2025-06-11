using System.Collections;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WoodenBoard : MonoBehaviour
{
	[SerializeField]
	private XRSocketInteractor _doughSocket;
	[SerializeField]
	private ShapedDoughsSocketsManager _shapedDoughsSocketsManager;

    [SerializeField]
    private ParticleSystem particles;

    private Dough _doughOnBoard;
	private bool _hasShapedDough;

	public delegate void WoodenBoardHandler();
	public event WoodenBoardHandler OnDoughOnBoard;
	public event WoodenBoardHandler OnDoughRemovedFromBoard;
	public event WoodenBoardHandler OnDoughKneaded;

	[SerializeField]
	private Material _doughHelperMaterial;
	private Mesh _doughMesh;
	private Matrix4x4 _doughMatrix;
	private bool _showDoughOnBoard = false;

	private void Start()
	{
		_shapedDoughsSocketsManager = GetComponentInChildren<ShapedDoughsSocketsManager>();
		
		Mixer mixer = LevelManager.Instance.GetMixer();
		mixer.OnMixingComplete += ShowDoughMeshOnBoard;
		mixer.OnMixingFailed += HideDoughMeshOnBoard;
	}

	private void OnEnable()
	{
		_doughSocket.selectExited.AddListener(DoughRemoved);
	}

	private void OnDisable()
	{
		_doughSocket.selectExited.RemoveListener(DoughRemoved);
	}

	private void OnDestroy()
	{
		OnDoughOnBoard = null;
		OnDoughRemovedFromBoard = null;
		OnDoughKneaded = null;
}

	private void Update()
	{
		if (_showDoughOnBoard)
		{
			DrawHelperMesh();
		}
	}

	public void ShapedDoughsGridIsEmpty()
	{
		_hasShapedDough = false;
	}

	public void ReleaseAllDough()
	{
        _shapedDoughsSocketsManager.ReleaseAllDough();
    }

	private void DoughRemoved(SelectExitEventArgs args)
	{
		_doughSocket.socketActive = false;
		_doughOnBoard = null;

		if(!args.interactableObject.IsUnityNull())
			StartCoroutine(DetectIfLeftSocketByPlayerHand(args.interactableObject));
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Dough Roller"))
		{
			if (!_doughOnBoard)
				return;
            particles.Play();

            if (_doughOnBoard.KneadDough()){
				OnDoughKneaded?.Invoke();	
			}

            return;
		}

		if (_hasShapedDough)
		{
			_shapedDoughsSocketsManager.OnContainerTriggerEnter(other.gameObject);
			return;
		}

		if (_doughSocket.hasSelection)
			return;

		XRBaseInteractable interactable = other.gameObject.GetComponentInParent<XRBaseInteractable>();

		if(interactable.TryGetComponent<Bowl>(out var auxBowl))
		{
			if (auxBowl.HasCompletedDough && auxBowl.GetDough().CompareTag("Dough"))
			{
				GameObject dough = Instantiate(auxBowl.GetDough(), _doughSocket.transform.position, Quaternion.identity);
				auxBowl.DoughRemoved();
				return;
			}
		}

		if (interactable.isSelected)
			return;

		if (other.gameObject.CompareTag("Dough"))
		{
			HideDoughMeshOnBoard();
			OnDoughOnBoard?.Invoke();
			
			_doughSocket.socketActive = true;
			_doughSocket.interactionManager.SelectEnter(_doughSocket as IXRSelectInteractor, interactable as IXRSelectInteractable);

			_doughOnBoard = other.gameObject.GetComponentInParent<Dough>();
			_doughOnBoard.transform.SetParent(null, true);

			return;
		}
		else if (other.gameObject.CompareTag("Shaped Dough"))
		{
			_hasShapedDough = true;
			RecipeData recipe = other.gameObject.GetComponentInParent<ShapedDough>().GetRecipe();
			_shapedDoughsSocketsManager.ReceivedItem(recipe, other.gameObject);

			return;
		}
	}

	private IEnumerator DetectIfLeftSocketByPlayerHand(IXRSelectInteractable interactable)
	{
		yield return new WaitForEndOfFrame();

		if (!interactable.isSelected)
			OnDoughRemovedFromBoard?.Invoke();
		else
			ShowDoughMeshOnBoard();
	}


	private void ShowDoughMeshOnBoard()
	{
		if (_doughMesh.IsUnityNull())
		{
			GameObject dough = LevelManager.Instance.GetBowl().GetDough();
			MeshFilter meshFilter = dough.GetComponentInChildren<MeshFilter>();
			_doughMesh = meshFilter.sharedMesh;

			_doughMatrix = UtilsClass.GetHoverMeshMatrix(dough.GetComponent<XRBreadInteractable>(), meshFilter, 1f, _doughSocket);
		}

		_showDoughOnBoard = true;
	}

	private void HideDoughMeshOnBoard()
	{
		_showDoughOnBoard = false;
	}

	private void DrawHelperMesh()
	{
		Graphics.DrawMesh(
				_doughMesh,
				_doughMatrix,
				_doughHelperMaterial,
				gameObject.layer
		);
	}
}