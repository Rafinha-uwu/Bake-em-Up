using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoulotteHealth : MonoBehaviour
{
    [SerializeField] private int hp = 300;
    [SerializeField]
    private TakeDamageFX _damageFX;

    private AudioSource _audioSource;

    [SerializeField] private TextMeshProUGUI healthDisplay;

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
        _audioSource = GetComponent<AudioSource>();
    }
    public void GetHit(int damage, GameObject sender, GameObject receiver)
    {
        if (sender.CompareTag("Zombie") && receiver.GetInstanceID() == gameObject.GetInstanceID())
        {
            _audioSource.Play();
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
