using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingCidadao : MonoBehaviour
{

    public Transform ombro;

    public PlayerController playerController;

    public bool cidadaoNoOmbro;


    void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (playerController.carregarCidadao == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().freezeRotation = true;
                this.transform.position = ombro.position;
                this.transform.parent = GameObject.Find("Ombro").transform;
                Carregado();
                cidadaoNoOmbro = true;
            }

            if (Input.GetMouseButtonDown(1))
            {
                this.transform.parent = null;
                GetComponent<Rigidbody>().freezeRotation = false;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<CapsuleCollider>().enabled = true;
                Jogado();
                cidadaoNoOmbro = false;
            }

        }

        if (playerController.carregarCidadao == false)
        {
            cidadaoNoOmbro = false;
        }

    }

    /*void OnMouseDown()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().freezeRotation = true;
        this.transform.position = ombro.position;
        this.transform.parent = GameObject.Find("Ombro").transform;
        Carregado();
    }*/

    /*void OnMouseUp()
    {
        this.transform.parent = null;
        GetComponent<Rigidbody>().freezeRotation = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<CapsuleCollider>().enabled = true;
        Jogado();
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

}
