using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class PauseMenu : MonoBehaviour
{
    // Flag to track the game's pause state
    public static bool isPaused = false;

    // Reference to the UI Panel or Canvas containing the pause menu assets
    [SerializeField] private GameObject pauseMenuUI;

     void Update()
    {
        // Added the opening bracket '{' right after the condition
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("Escape Key Pressed");

            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the menu
        Time.timeScale = 1f;          // Reset time to normal speed
        isPaused = false;

    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);  // Show the menu
        Time.timeScale = 0f;          // Freeze game time, physics, and animations
        isPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
