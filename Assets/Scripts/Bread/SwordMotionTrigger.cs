using UnityEngine;

public class SwordMotionTrigger : MonoBehaviour
{
    public float swingThreshold = 1.5f; // tune this value
    private Vector3 lastPosition;
    private SwordSwingSound swingSound;

    void Start()
    {
        lastPosition = transform.position;
        swingSound = GetComponent<SwordSwingSound>();
    }

    void Update()
    {
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
        if (velocity.magnitude > swingThreshold)
        {
            swingSound.PlaySwingSound();
        }

        lastPosition = transform.position;
    }
}
