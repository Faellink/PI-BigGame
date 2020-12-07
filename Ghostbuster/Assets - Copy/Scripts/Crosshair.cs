using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public PlayerController playerController;

    public GameObject crosshairOn;
    public GameObject crosshairOff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.hookshotEnabled == true)
        {
            crosshairOn.SetActive(true);
            crosshairOff.SetActive(false);
        }
        else
        {
            crosshairOn.SetActive(false);
            crosshairOff.SetActive(true);
        }
    }


}
