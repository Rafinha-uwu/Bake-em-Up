using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField] private int hp = 1;

    private NavMeshObstacle obstacle;

    private void OnEnable()
    {
        HitEvent.OnHit += GetHit;
    }

    private void OnDisable()
    {
        HitEvent.OnHit -= GetHit;
    }

    private void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
    }

    public void GetHit(int damage, GameObject sender, GameObject receiver)
    {
        if (sender.CompareTag("Bread") && receiver.GetInstanceID() == gameObject.GetInstanceID())
        {
            Debug.Log("LEVASTE COM UM PAO");
            hp -= damage;
            if (hp < 1)
            {
                OnDeath();
            }
        }
    }


    void OnDeath()
    {
        if (obstacle != null) obstacle.enabled = false;

        // Trigger repath for others
        ZombieRepath.RepathNearbyZombies(transform.position, 5f);

        // Destroy the zombie
        Destroy(gameObject);
    }
}
