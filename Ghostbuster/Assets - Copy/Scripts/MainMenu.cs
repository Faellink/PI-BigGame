using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public float rotTime;

    public GameObject upButton, downButton, leftButton, rightButton, currentButton;

    Camera camera;

    public Material matOn, matOff;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();

        //camera.transform.rotation = Quaternion.Slerp(transform.rotation, currentButton.transform.rotation, rotTime);

        // camera.transform.LookAt(currentButton.transform, Vector3.up);

        Vector3 lTargetDir = currentButton.transform.position - transform.position;

        //lTargetDir.y = 0.0f;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * (rotTime * 0.1f ));

    }

    void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentButton = upButton;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentButton = downButton;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentButton = leftButton;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentButton = rightButton;
        }
    }


}
