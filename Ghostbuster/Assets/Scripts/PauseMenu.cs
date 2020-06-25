using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public bool isPaused;
    public GameObject pauseMenuCanvas;
    public GameObject gameHud;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused == true)
            {
                isPaused = false;
                Time.timeScale = 1;
                Cursor.visible = false;
                pauseMenuCanvas.SetActive(false);
                gameHud.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                isPaused = true;
                Cursor.visible = true;
                Time.timeScale = 0;
                gameHud.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                pauseMenuCanvas.SetActive(true);
            }
        }
        
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        gameHud.SetActive(true);
        pauseMenuCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
