using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    Renderer button;

    public Material materialOn, materialOff;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        button.material = materialOn;
    }

    private void OnMouseExit()
    {
        button.material = materialOff;
    }
}
