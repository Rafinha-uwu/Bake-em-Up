using UnityEngine;
using UnityEngine.UI;

public class TVController : MonoBehaviour
{
    [SerializeField] private Image military;
    [SerializeField] private Image scientist;
    [SerializeField] private GameObject both;

    [SerializeField] private Canvas tv_narrative;
    [SerializeField] private Canvas tv_gameplay;

    private void OnEnable()
    {
        ChangeNarrativeEvent.OnChange += ChangeNarrator;
    }

    private void OnDisable()
    {
        ChangeNarrativeEvent.OnChange -= ChangeNarrator;
    }



    private void ChangeNarrator(string narrator)
    {
        tv_gameplay.gameObject.SetActive(false);
        tv_narrative.gameObject.SetActive(true);
        switch (narrator)
        {
            case "military":
                military.gameObject.SetActive(true);
                scientist.gameObject.SetActive(false);
                both.gameObject.SetActive(false);
                break;
            case "scientist":
                military.gameObject.SetActive(false);
                scientist.gameObject.SetActive(true);
                both.gameObject.SetActive(false);
                break;
            case "both":
                military.gameObject.SetActive(false);
                scientist.gameObject.SetActive(false);
                both.SetActive(true);
                break;
            case "Gameplay":
                tv_gameplay.gameObject.SetActive(true);
                tv_narrative.gameObject.SetActive(false);
                break;
        }
    }
}