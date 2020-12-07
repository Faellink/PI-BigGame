using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMenu : MonoBehaviour
{
    //0,6
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    //45
    public float yawLimitX = 0f;
    //30
    public float pitchLimitY = 0f;
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        yaw = Mathf.Clamp(yaw, (yawLimitX * -1), yawLimitX);
        pitch = Mathf.Clamp(pitch, (pitchLimitY * -1), pitchLimitY);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);*/
    }

    private void LateUpdate()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        yaw = Mathf.Clamp(yaw, (yawLimitX * -1), yawLimitX);
        pitch = Mathf.Clamp(pitch, (pitchLimitY * -1), pitchLimitY);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }


}
