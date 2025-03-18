using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private int hp = 1;
    [SerializeField] private int hitDamage = 1;

    private void OnEnable()
    {
        HitEvent.OnHit += GetHit;
    }

    private void OnDisable()
    {
        HitEvent.OnHit -= GetHit;
    }

    public void GetHit(int damage, GameObject sender, GameObject receiver)
    {
        if (sender.CompareTag("Bread") && receiver.GetInstanceID() == gameObject.GetInstanceID())
        {
            Debug.Log("LEVASTE COM UM PAO");
            hp -= damage;
            if (hp < 1)
            {
                Death();
            }
        }
    }

    private void Death()
    {
        Debug.Log("VOU MORRER");
        Destroy(gameObject);
    }
}
