using UnityEngine;
using UnityEngine.SceneManagement;

public class shipMenu2 : MonoBehaviour
{
    public void shipSelectBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void ship1Select()
    {
        Debug.Log("P2 selects light ship.");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ship2Select()
    {
        Debug.Log("P2 selects medium ship.");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ship3Select()
    {
        Debug.Log("P2 selects heavy ship.");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void shipSelectConfirm()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
