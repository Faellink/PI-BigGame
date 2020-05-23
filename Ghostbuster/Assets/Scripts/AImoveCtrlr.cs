using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AImoveCtrlr : MonoBehaviour
{
    public GameObject targetObject;
    NavMeshAgent agentAI;
    public float maxSphereRadius,rayLength, maxSightAngle;
    private Vector3 wanderToPos;
    private bool seeChar,clean;

    // Start is called before the first frame update
    void Start()
    {
        agentAI = GetComponent<NavMeshAgent>();
        wanderToPos = SeeRandomPointInSpace();
        seeChar = false;

    }

    private void OnDrawGizmos() 
    {
        Gizmos.color= new Color (1,1,0,0.2f);
        Gizmos.DrawSphere(transform.position,maxSphereRadius);
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Cupcake") == true)
        {
            targetObject = GameObject.FindGameObjectWithTag("Char");
            SeeTargetCharacter();
        }
    }

    public Vector3 SeeRandomPointInSpace()
    {
        Vector3 randomPoint = (Random.insideUnitSphere*maxSphereRadius)+transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPoint,out navHit,maxSphereRadius,-1);
        return new Vector3(navHit.position.x,transform.position.y,navHit.position.z);
    }

    void RoamToPoint()
    {
        if(Vector3.Distance(transform.position,wanderToPos)<2f)
        {
            wanderToPos = SeeRandomPointInSpace();
        }else
        {
            agentAI.SetDestination(wanderToPos);
        }
    }

    void SeeTargetCharacter()
    {
        seeChar = true;
        this.gameObject.tag = "Inimigo";
    }
    // Update is called once per frame
    void Update()
    {
        RoamToPoint(); 

        RaycastHit rayHit;
        Physics.Raycast(this.transform.position, this.transform.right, out rayHit, rayLength);

        if(rayHit.collider.CompareTag("Char")||rayHit.collider.CompareTag("Player"))
        {
            Destroy(rayHit.collider.gameObject);
        }
        
        if(this.gameObject.CompareTag("Inimigo"))
        {
            agentAI.SetDestination(targetObject.transform.position);
        
        }else 
        {
        RoamToPoint();
        this.gameObject.tag = "Cidadao";
        }
    }
}
