using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoulotteHealth : MonoBehaviour
{
    [SerializeField] private int hp = 300;
    [SerializeField]
    private TakeDamageFX _damageFX;

    [SerializeField] private TextMeshProUGUI healthDisplay;

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
            hp -= damage;
            healthDisplay.text = $"{hp} - HP";
            _damageFX.PlayFX();
            if (hp < 1)
            {   
                EndGame();
            }
        }
    }

    private void EndGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
