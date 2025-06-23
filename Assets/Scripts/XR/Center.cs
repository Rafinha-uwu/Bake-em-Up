using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Center : MonoBehaviour
{
    public XROrigin xrOrigin;

    void Start()
    {
        Invoke("RecenterRig", 0.5f); // Small delay to ensure headset is tracked
    }

    void RecenterRig()
    {
        // Get current headset position relative to origin
        Transform cameraTransform = xrOrigin.Camera.transform;
        Vector3 headsetPosition = cameraTransform.position;

        // Zero out Y axis if you only want to recenter on XZ plane
        Vector3 offset = new Vector3(headsetPosition.x, 0, headsetPosition.z);

        // Move the rig so that headset appears at world (0, 0, 0)
        xrOrigin.MoveCameraToWorldLocation(xrOrigin.transform.position - offset);
    }
}
