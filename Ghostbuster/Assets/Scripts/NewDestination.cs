using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDestination : MonoBehaviour
{
    public int posX;
    public int posZ;

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Cidadao"))
        {
            posX = Random.Range(100,400);
            posZ = Random.Range(100,400);
            this.gameObject.transform.position = new Vector3 (posX,transform.position.y,posZ);
        }
    }
}
