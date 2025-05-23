using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public static class UtilsClass
{
	public static float GetAngleFromVectorFloat(Vector3 dir)
	{
		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if (n < 0) n += 360;

		return n;
	}

	public static Matrix4x4 GetHoverMeshMatrix(IXRInteractable interactable, MeshFilter meshFilter, float hoverScale, IXRInteractor interactor)
	{
		var interactableAttachTransform = interactable.GetAttachTransform(interactor);
		if(interactable is XRBreadInteractable)
		{
			XRBreadInteractable breadInteractable = interactable as XRBreadInteractable;
			interactableAttachTransform = breadInteractable.GetSocketTransform();
		}

		var grabInteractable = interactable as XRGrabInteractable;

		// Get the "static" pose of the interactable's attach transform in world space.
		// While the XR Grab Interactable is selected, the Attach Transform pose may have been modified
		// by user code, and we assume it will be restored back to the initial captured pose.
		// When Use Dynamic Attach is enabled, we can instead rely on using the dedicated GameObject for this interactor.
		Pose interactableAttachPose;
		if (grabInteractable != null && !grabInteractable.useDynamicAttach &&
			grabInteractable.isSelected &&
			interactableAttachTransform != interactable.transform &&
			interactableAttachTransform.IsChildOf(interactable.transform))
		{
			// The interactable's attach transform must not change parent Transform while selected
			// for the pose to be calculated correctly. This transforms the captured pose in local space
			// into the current pose in world space. If the pose of the attach transform was not modified
			// after being selected, this will be the same value as calculated in the else statement.
			var localAttachPose = grabInteractable.GetLocalAttachPoseOnSelect(grabInteractable.firstInteractorSelecting);
			var attachTransformParent = interactableAttachTransform.parent;
			interactableAttachPose =
				new Pose(attachTransformParent.TransformPoint(localAttachPose.position),
					attachTransformParent.rotation * localAttachPose.rotation);
		}
		else
		{
			interactableAttachPose = new Pose(interactableAttachTransform.position, interactableAttachTransform.rotation);
		}

		var attachOffset = meshFilter.transform.position - interactableAttachPose.position;
		var interactableLocalPosition = InverseTransformDirection(interactableAttachPose, attachOffset) * hoverScale;
		var interactableLocalRotation = Quaternion.Inverse(Quaternion.Inverse(meshFilter.transform.rotation) * interactableAttachPose.rotation);

		Vector3 position;
		Quaternion rotation;

		var interactorAttachTransform = interactor.GetAttachTransform(interactable);
		var interactorAttachPose = new Pose(interactorAttachTransform.position, interactorAttachTransform.rotation);
		if (grabInteractable == null || grabInteractable.trackRotation)
		{
			position = interactorAttachPose.rotation * interactableLocalPosition + interactorAttachPose.position;
			rotation = interactorAttachPose.rotation * interactableLocalRotation;
		}
		else
		{
			position = interactableAttachPose.rotation * interactableLocalPosition + interactorAttachPose.position;
			rotation = meshFilter.transform.rotation;
		}

		// Rare case that Track Position is disabled
		if (grabInteractable != null && !grabInteractable.trackPosition)
			position = meshFilter.transform.position;

		var scale = meshFilter.transform.lossyScale * hoverScale;

		return Matrix4x4.TRS(position, rotation, scale);
	}

	private static Vector3 InverseTransformDirection(Pose pose, Vector3 direction)
	{
		return Quaternion.Inverse(pose.rotation) * direction;
	}
}
