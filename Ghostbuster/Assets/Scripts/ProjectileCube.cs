using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCube : MonoBehaviour
{
    public LayerMask stopLayerMask;
    private void OnCollisionEnter(Collision collision)
    {
        if(stopLayerMask == (stopLayerMask | (1 << collision.gameObject.layer)))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            gameObject.layer = LayerMask.NameToLayer("Ground");
        }
    }
}

