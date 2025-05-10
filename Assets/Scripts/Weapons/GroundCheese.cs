using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class GroundCheese : MonoBehaviour
{
    private float countdown = 10;
    private bool countdownStarted = false;

    private Dictionary<NavMeshAgent, float> originalSpeeds = new();

    public void Start()
    {
        countdownStarted = true;
    }

    public void Update()
    {
        if (countdownStarted)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                gameObject.GetComponent<Animator>().Play("Die");
                Invoke("Die", 2);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (root.CompareTag("Zombie"))
        {
            NavMeshAgent agent = root.GetComponent<NavMeshAgent>();
            if (agent != null && !originalSpeeds.ContainsKey(agent))
            {
                originalSpeeds[agent] = agent.speed;
                agent.speed *= 0.2f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (root.CompareTag("Zombie"))
        {
            NavMeshAgent agent = root.GetComponent<NavMeshAgent>();
            if (agent != null && originalSpeeds.ContainsKey(agent))
            {
                agent.speed = originalSpeeds[agent];
                originalSpeeds.Remove(agent);
            }
        }
    }


    private void Die()
    {
        Destroy(gameObject);
    }
}
