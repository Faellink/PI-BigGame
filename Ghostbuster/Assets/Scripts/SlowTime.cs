using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTime : MonoBehaviour
{

    public float bulletTimeMultiplier = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TestSlowTimeInput())
        {
            Time.timeScale = bulletTimeMultiplier;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    bool TestSlowTimeInput()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
}
