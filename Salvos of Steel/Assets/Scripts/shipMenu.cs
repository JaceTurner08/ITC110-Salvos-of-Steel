using UnityEngine;
using UnityEngine.SceneManagement;

public class shipMenu : MonoBehaviour
{
    public void shipSelectBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void ship1Select()
    {
        Debug.Log("P1 selects light ship.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ship2Select()
    {
        Debug.Log("P1 selects medium ship.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ship3Select()
    {
        Debug.Log("P1 selects heavy ship.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
