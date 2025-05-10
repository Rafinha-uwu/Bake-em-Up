using UnityEngine;

[CreateAssetMenu(menuName = "Waves/Events/Spawn Item")]
public class SpawnItemEvent : WaveEventBase
{
    public GameObject itemPrefab;
    public Vector3 spawnPosition;

    public override void Execute()
    {
        if (itemPrefab != null)
        {
            GameObject.Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
