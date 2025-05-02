using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ZombieRepath : MonoBehaviour
{
    public static void RepathNearbyZombies(Vector3 position, float radius, GameObject sender)
    {
        Collider[] nearbyZombies = Physics.OverlapSphere(position, radius);

        foreach (var col in nearbyZombies)
        {
            if (col.CompareTag("Zombie") && sender.GetInstanceID() != col.gameObject.GetInstanceID())
            {
                NavMeshAgent agent = col.GetComponent<NavMeshAgent>();
                EnemyNavigation enemyNav = col.GetComponent<EnemyNavigation>();

                if (agent != null && agent.enabled && enemyNav != null)
                {
                    // Re-set the same destination to trigger re-pathing
                    agent.SetDestination(enemyNav.GetDestination());
                }
            }
        }
    }
}