using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    public int levelNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void EncimaButton()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("achoe");
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(levelNumber);
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
