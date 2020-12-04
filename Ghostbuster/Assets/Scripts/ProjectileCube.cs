using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCube : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
