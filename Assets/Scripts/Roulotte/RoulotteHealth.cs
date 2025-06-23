using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoulotteHealth : MonoBehaviour
{
    public Image healthBarFill;
    public float maxHealth = 300f;

    [SerializeField] public int hp = 300;
    [SerializeField]
    private TakeDamageFX _damageFX;

    private AudioSource _audioSource;

    [SerializeField] private TextMeshProUGUI healthDisplay;

    public List<GameObject> Holes = new();

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
            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = healthPercentage;
            }
            //healthDisplay.text = $"{hp} - HP";
            _damageFX.PlayFX();

            if (hp <= (maxHealth * 0.9) && hp > (maxHealth * 0.5))
            {
                //50 50
            }
            if (hp <= (maxHealth * 0.5) && hp >= 1)
            {
                //100
            }
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
