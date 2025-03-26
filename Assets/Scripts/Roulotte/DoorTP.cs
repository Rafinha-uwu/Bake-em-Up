using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTP : MonoBehaviour
{
    public void TP0()
    {
        SceneManager.LoadScene(sceneBuildIndex:0);
    }

    public void TP1()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

}
