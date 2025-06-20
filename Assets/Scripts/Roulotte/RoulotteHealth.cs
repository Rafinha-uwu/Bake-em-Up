using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoulotteHealth : MonoBehaviour
{
    public Image healthBarFill;
    private float maxHealth = 300f;

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
        healthBarFill.fillAmount = 1f;
    }
    public void GetHit(int damage, GameObject sender, GameObject receiver)
    {
        if (sender.CompareTag("Zombie") && receiver.GetInstanceID() == gameObject.GetInstanceID())
        {
            _audioSource.Play();
            hp -= damage;
            float healthPercentage = hp / maxHealth;
            healthBarFill.fillAmount = healthPercentage;
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
