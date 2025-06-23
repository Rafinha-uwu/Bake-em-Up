using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Center : MonoBehaviour
{
    public XROrigin xrOrigin;
    public Transform desiredHeadsetPosition; // Where you want the headset to appear (position only)

    void Start()
    {
        StartCoroutine(AlignHeadset());
    }

    System.Collections.IEnumerator AlignHeadset()
    {
        yield return new WaitForSeconds(0.1f); // Wait for tracking to initialize

        Transform cameraTransform = xrOrigin.Camera.transform;
        Vector3 headsetWorldPos = cameraTransform.position;

        // Calculate position offset (ignore Y to keep real-world head height)
        Vector3 offset = desiredHeadsetPosition.position - headsetWorldPos;
        offset.y = 0; // Preserve headset's vertical position

        // Move the whole XR Rig
        xrOrigin.transform.position += offset;
    }
}
