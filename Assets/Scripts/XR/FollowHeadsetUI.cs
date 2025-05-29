using UnityEngine;

public class FollowHeadsetUI : MonoBehaviour
{
    public Transform headset; // Assign your XR camera here
    public float distance = 2f;
    public float heightOffset = 0f;
    public float followSpeed = 5f;

    void Update()
    {
        if (headset == null) return;

        // Calculate target position in front of the headset
        Vector3 targetPosition = headset.position + headset.forward * distance;
        
        targetPosition.y = headset.position.y + heightOffset;

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        

        // Smooth rotation to face the player
        Vector3 lookDirection = headset.position - transform.position;
        lookDirection.y = 0; // Lock rotation to horizontal plane
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * followSpeed);
    }
}