using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Level1";

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting application...");
        Application.Quit();
    }
}
