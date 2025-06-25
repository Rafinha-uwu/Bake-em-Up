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
    public int hp = 300;

    [SerializeField]
    private TakeDamageFX _damageFX;

    private AudioSource _audioSource;

    public List<GameObject> Holes = new();

    public int currentHoles = 0;
    private int maxHoles = 4;

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
            ShowHealth();
            _damageFX.PlayFX();

            if (hp <= (maxHealth * 0.9f) && hp > (maxHealth * 0.5f))
            {
                // 20% chance to enable a hole
                if (Random.value < 0.2f)
                {
                    maxHoles = 3;
                    TryEnableHole();
                }
            }

            if (hp <= (maxHealth * 0.5f) && hp >= 1)
            {
                maxHoles = 7;
                // 100% chance to enable a hole
                TryEnableHole();
            }

            if (hp < 1)
            {
                EndGame();
            }
        }
    }

    public void ShowHealth()
    {
        float healthPercentage = hp / maxHealth;
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = healthPercentage;
        }
    }

    private void TryEnableHole()
    {
        if (currentHoles >= maxHoles)
            return;

        foreach (GameObject hole in Holes)
        {
            if (!hole.activeSelf)
            {
                hole.SetActive(true);
                currentHoles++;
                break;
            }
        }
    }

    private void EndGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
