using UnityEngine;

public class DonutRoller : MonoBehaviour
{
    [SerializeField] private float rollSpeed = 5f;
    [SerializeField] private float growthRate = 1.5f; //50%
    [SerializeField] private float growInterval = 1f;
    [SerializeField] private float maxScale = 5f;

    private AudioSource _audioSource;
    public AudioClip[] donutgrowth_sounds;


    private bool isRolling = false;
    private Rigidbody rb;
    [SerializeField]
    private BoxCollider _triggerCollider;
    private float nextGrowTime = 0f;
    private Vector3 rollDirection;
    private Vector3 lastPosition;
    private int sound_number = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        _audioSource = GetComponent<AudioSource>();
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
                PlayDonutSound();
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
		if (!isRolling && collision.gameObject.CompareTag("Ground"))
		{
            _triggerCollider.enabled = true;
			gameObject.GetComponent<Animator>().enabled = false;
			isRolling = true;

			rollDirection.y = 0f;
			rollDirection.Normalize();


			Vector3 flatForward = new Vector3(rollDirection.x, 0f, rollDirection.z);
			float targetY = Quaternion.LookRotation(flatForward).eulerAngles.y;

			if (targetY > 180f) targetY -= 360f;

			if (Mathf.Abs(targetY) >= 5f)
			{
				targetY *= 0.8f;
			}

			transform.rotation = Quaternion.Euler(0f, targetY, 90f);

			rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		}
	}

    private void Die()
    {
        Destroy(gameObject);
    }


    private void PlayDonutSound()
    {
        if (sound_number >= 0 && sound_number < donutgrowth_sounds.Length)
        {
            _audioSource.clip = donutgrowth_sounds[sound_number];
            _audioSource.Play();
            sound_number++;
        }
    }
}
