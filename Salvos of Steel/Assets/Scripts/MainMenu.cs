using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void playGame  ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void LevelSelectBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void quitGame ()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
    public void level1Select ()
    {
        Debug.Log("Tropical Madness Selected.");
    }
    public void level2Select()
    {
        Debug.Log("Iceberg Quarters Selected.");
    }
    public void level3Select()
    {
        Debug.Log("Wooden Lagoon Selected.");
    }
}
