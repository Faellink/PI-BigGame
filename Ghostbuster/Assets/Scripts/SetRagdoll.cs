using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRagdoll : MonoBehaviour
{

    Animator anim;
    Rigidbody rigid;
    CapsuleCollider capsuleCollider;

    public List<Collider> RagDollParts = new List<Collider>();

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = this.gameObject.GetComponent<CapsuleCollider>();
        SetRagDoll();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetRagDoll()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            if (c.gameObject != this.gameObject)
            {
                c.isTrigger = true;
                RagDollParts.Add(c);
            }
        }
    }

    void SetRagdollOn()
    {
        rigid.useGravity = false;
        rigid.velocity = Vector3.zero;

        capsuleCollider.enabled = false;
        anim.enabled = false;

        foreach (Collider c in RagDollParts)
        {
            c.isTrigger = false;
            c.attachedRigidbody.velocity = Vector3.zero;
        }
    }

    void SetRagdollOff()
    {

    }

    void OnMouseDown()
    {
        SetRagdollOn(); 
    }
}
