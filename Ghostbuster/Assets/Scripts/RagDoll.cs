﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour
{
    public Collider mainCollider;
    public Collider[] allColliders; 

    // Start is called before the first frame update
    void Start()
    {
        mainCollider = GetComponent<CapsuleCollider>();
        allColliders = GetComponentsInChildren<Collider>(true);
        SetRagdoll(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetRagdoll(true);
        }
    }

    public void SetRagdoll(bool isRagDoll)
    {
        foreach (var col in allColliders)
        {
            col.enabled = isRagDoll;
        }
        mainCollider.enabled = !isRagDoll;
        GetComponent<Rigidbody>().useGravity = !isRagDoll;
        GetComponent<Animator>().enabled = !isRagDoll;
    }
}
