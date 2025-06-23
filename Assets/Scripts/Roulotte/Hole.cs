using UnityEngine;

public class Hole : MonoBehaviour
{
    private RoulotteHealth health;

    private void Start()
    {
        health = GetComponentInParent<RoulotteHealth>();
    }
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Bread")) return;

        if (!other.TryGetComponent<Cream>(out var cream)) return;

        int heal = Mathf.FloorToInt(health.maxHealth * 0.1f);
        health.hp += heal;

        gameObject.SetActive(false);
    }


}
