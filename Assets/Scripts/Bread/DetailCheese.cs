using UnityEngine;
using System.Collections;

public class DetailCheese : MonoBehaviour
{
    [SerializeField] private GameObject smallCheese;

    private Vector3 GetLowestPoint()
    {
        float lowestY = Mathf.Infinity;
        Vector3 lowestPoint = transform.position;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            Vector3 rendBottom = rend.bounds.min;
            if (rendBottom.y < lowestY)
            {
                lowestY = rendBottom.y;
                lowestPoint = new Vector3(transform.position.x, rendBottom.y, transform.position.z);
            }
        }

        return lowestPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Roulotte") && smallCheese != null)
        {
            StartCoroutine(SpawnCheeses());
        }
    }

    private IEnumerator SpawnCheeses()
    {
        Vector3 baseSpawnPos = GetLowestPoint();
        int spawnCount = Random.Range(1, 3);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * 0.03f;
            Vector3 offset = new Vector3(offset2D.x, 0f, offset2D.y);
            Vector3 finalPos = baseSpawnPos + offset;

            Quaternion randomYRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            Instantiate(smallCheese, finalPos, randomYRotation);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
