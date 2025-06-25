using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public bool Military = false;
    public bool Scientist = false;
    public bool On = false;

    [SerializeField] private GameObject blackout;

    public void Update()
    {
        if (On)
        {
            if (Military)
            {
                StartCoroutine(M());
                On = false;
            }
            else if (Scientist)
            {
                StartCoroutine(S());
                On = false;
            }
        }
    }

    public IEnumerator S()
    {
        LevelManager.Instance.DialogueRunner.StartDialogue("ScientistFinal");
        yield return new WaitForSeconds(6f);
        blackout.GetComponent<Animator>().Play("Dark");
        SceneManager.LoadScene("Main Menu");

    }

    public IEnumerator M()
    {
        LevelManager.Instance.DialogueRunner.StartDialogue("MilitarFinal");
        yield return new WaitForSeconds(6f);
        blackout.GetComponent<Animator>().Play("Main Menu");
    }
}
