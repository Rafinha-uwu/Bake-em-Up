using UnityEngine;

public class RoulotteHealth : MonoBehaviour
{
    [SerializeField] private int hp = 300;
    [SerializeField]
    private TakeDamageFX _damageFX;    

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
        if (sender.CompareTag("Zombie") && receiver.GetInstanceID() == gameObject.GetInstanceID())
        {
            Debug.Log("LEVASTE COM UM ZOMBIE RAUUURRR");
            hp -= damage;
            _damageFX.PlayFX();
            if (hp < 1)
            {
                EndGame();
            }
        }
    }

    private void EndGame()
    {
        Application.Quit();
    }
}
