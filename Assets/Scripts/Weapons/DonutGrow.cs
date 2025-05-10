using UnityEngine;

public class DonutRoller : MonoBehaviour
{
    [SerializeField] private float rollSpeed = 5f;
    [SerializeField] private float growthRate = 1.5f; //50%
    [SerializeField] private float growInterval = 1f;
    [SerializeField] private float maxScale = 5f;

    private bool isRolling = false;
    private Rigidbody rb;
    private float nextGrowTime = 0f;
    private Vector3 rollDirection;
    private Vector3 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (isRolling && transform.localScale.x < maxScale)
        {
            rb.MovePosition(transform.position + rollDirection * rollSpeed * Time.deltaTime);

            if (Time.time >= nextGrowTime)
            {
                nextGrowTime = Time.time + growInterval;
                transform.localScale *= growthRate;

                if (transform.localScale.x >= maxScale)
                {
                    Invoke("Die", 3);
                }
            }
        }
        else
        {
            Vector3 frameVelocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            if (frameVelocity.sqrMagnitude > 0.01f)
            {
                rollDirection = frameVelocity;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isRolling && collision.collider.CompareTag("Ground"))
        {

            gameObject.GetComponent<Animator>().enabled = false;
            isRolling = true;

            rollDirection.y = 0f;
            rollDirection.Normalize();


            Vector3 flatForward = new Vector3(rollDirection.x, 0f, rollDirection.z);
            float targetY = Quaternion.LookRotation(flatForward).eulerAngles.y;

            if (targetY > 180f) targetY -= 360f;

            if (Mathf.Abs(targetY) >= 5f)
            {
                targetY *= 0.5f;
            }

            transform.rotation = Quaternion.Euler(0f, targetY, 90f);



            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
