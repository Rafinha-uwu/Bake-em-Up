using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public bool Military = false;
    public bool Scientist = false;

    [SerializeField] private GameObject blackout;

    public void Update()
    {
        if (Military)
        {
            StartCoroutine(M());
        }
        else if (Scientist)
        {
            StartCoroutine(S());
        }
    }

    public IEnumerator S()
    {
        //Narrative
        yield return new WaitForSeconds(6f);
        blackout.GetComponent<Animator>().Play("Dark");

    }

    public IEnumerator M()
    {
        //Narrative
        yield return new WaitForSeconds(6f);
        blackout.GetComponent<Animator>().Play("Dark");
    }
}
