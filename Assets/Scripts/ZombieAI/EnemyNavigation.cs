using UnityEngine;
using UnityEngine.AI;


public class EnemyNavigation : MonoBehaviour
{
    private LevelManager levelManager;
    private NavMeshAgent agent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        levelManager = GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(LevelManager.Instance.roulote.position);
    }
}
