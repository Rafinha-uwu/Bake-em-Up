using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

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
        //LevelManager.Instance.DialogueRunner.StartDialogue("Scientist_END");
        yield return new WaitForSeconds(6f);
        blackout.GetComponent<Animator>().Play("Dark");

    }

    public IEnumerator M()
    {
        //LevelManager.Instance.DialogueRunner.StartDialogue("Military_END");
        yield return new WaitForSeconds(6f);
        blackout.GetComponent<Animator>().Play("Dark");
    }
}
