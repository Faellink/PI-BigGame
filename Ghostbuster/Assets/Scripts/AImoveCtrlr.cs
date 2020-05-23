using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AImoveCtrlr : MonoBehaviour
{
    public GameObject targetObject;
    NavMeshAgent agentAI;

    public int aiNumber;


    // Start is called before the first frame update
    void Start()
    {
        agentAI = GetComponent<NavMeshAgent>();
        aiNumber = 0;
    }

    void SetAiActions()
    {
        if(aiNumber == 1)
        {
            targetObject = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        agentAI.SetDestination(targetObject.transform.position);
    }
}
