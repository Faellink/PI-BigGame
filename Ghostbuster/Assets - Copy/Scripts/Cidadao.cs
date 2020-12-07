using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cidadao : MonoBehaviour
{

    Animator anim;
    Rigidbody rigid;
    CapsuleCollider capsuleCollider;

    public List<Collider> RagDollParts = new List<Collider>();

    public Transform ombro;

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

    void OnMouseDown()
    {
        Carregado();
        capsuleCollider.enabled = false;
        rigid.useGravity = false;
        rigid.freezeRotation = true;
        rigid.velocity = Vector3.zero;
        this.transform.position = ombro.position;
        this.transform.parent = GameObject.Find("Ombro").transform;
        
    }

    /*void OnMouseUp()
    {
        this.transform.parent = null;
        rigid.freezeRotation = false;
        rigid.useGravity = true;
        capsuleCollider.enabled = false;
        Jogado();
        //SetRagdollOn();
    }*/

    void Carregado()
    {
        this.transform.forward = ombro.forward * -1;
        this.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
    }

    void Jogado()
    {
        //this.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
        this.transform.up = ombro.up;
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
}
