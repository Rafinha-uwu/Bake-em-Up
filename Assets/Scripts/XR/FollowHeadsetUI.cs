using UnityEngine;

public class FollowHeadsetUI : MonoBehaviour
{
    public Transform headset; // Assign your XR camera here
    public float distance = 2f;
    public float heightOffset = 0f;
    public float followSpeed = 5f;
    public float angleThreshold = 15f; // Safe zone in degrees

    private Vector3 lastTargetPosition;

    void Start()
    {
        if (headset != null)
        {
            lastTargetPosition = headset.position + headset.forward * distance;
            lastTargetPosition.y = headset.position.y + heightOffset;
            transform.position = lastTargetPosition;
        }
    }

    void Update()
    {
        if (headset == null) return;

        Vector3 directionToCanvas = (transform.position - headset.position).normalized;
        Vector3 headsetForward = headset.forward;
        directionToCanvas.y = 0;
        headsetForward.y = 0;

        float angle = Vector3.Angle(headsetForward, directionToCanvas);

        if (angle > angleThreshold)
        {
            // Update target position when outside safe zone
            lastTargetPosition = headset.position + headset.forward * distance;
            lastTargetPosition.y = headset.position.y + heightOffset;
        }

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, lastTargetPosition, Time.deltaTime * followSpeed);

        // Smooth rotation to face the player
        Vector3 lookDirection = headset.position - transform.position;
        lookDirection.y = 0; // Lock rotation to horizontal plane
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * followSpeed);
    }
}
