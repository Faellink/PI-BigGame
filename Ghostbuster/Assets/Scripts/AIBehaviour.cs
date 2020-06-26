using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AIBehaviour : MonoBehaviour
{

    //NavMeshAgent cidadaoAi;

    public float wanderRadius;
    public float wanderTimer;

    public float runSpeed;

    //private Transform target;
    private NavMeshAgent agent;
    private float timer;

    public bool foundPlayer;
    public bool foundTarget;
    public LayerMask playerLayer;
    public LayerMask targetLayer;

   public  Animator cidadaoAnim;

    SkinnedMeshRenderer[] skinnedMeshes;


    Collider[] targetsColliders;
    List<GameObject> targetsObjectList = new List<GameObject>();

    PlayerController playerController;
    private State state;


    private enum State
    {
        Saved,
        Wander,
        Escaping,
        Agressive,
        Waiting,
    }


    private void Start()
    {
        cidadaoAnim = GetComponent<Animator>();
        skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        state = State.Wander;
        timer = wanderTimer;
        //
        //state = State.Waiting;
    }


    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case State.Wander:
                Wander();
                break;
            case State.Escaping:
                Escaping();
                break;
            case State.Agressive:
                //FollowTarget();
                break;
            case State.Waiting:
                IsWaiting();
                break;
            case State.Saved:
                break;
        }

        foundPlayer = Physics.CheckSphere(transform.position, wanderRadius, playerLayer);
        foundTarget = Physics.CheckSphere(transform.position, wanderRadius, targetLayer);
        SearchTarget();

        Debug.Log(agent.velocity.magnitude);

    }

    void Wander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        if (foundPlayer)
        {
            state = State.Escaping;
        }

    }

    void Escaping()
    {
        Vector3 runTo = transform.position + ((transform.position - playerController.transform.position));
        agent.speed = runSpeed;
        float distance = Vector3.Distance(transform.position, playerController.transform.position);
        if (distance < wanderRadius)
        {
            agent.SetDestination(runTo);
        }

        if (!foundPlayer)
        {
            state = State.Wander;
        }

    }

    void SearchTarget()
    {
        targetsColliders = Physics.OverlapSphere(transform.position, wanderRadius, targetLayer);
        foreach (Collider targetCollider in targetsColliders)
        {
            if (!targetsObjectList.Contains(targetCollider.gameObject))
            {
                targetsObjectList.Add(targetCollider.gameObject);
            }
        }

        if (targetsColliders.Length == 0)
        {
            targetsObjectList.Clear();
        }
        else if (targetsColliders.Length > 0)
        {
            agent.SetDestination(targetsObjectList[0].transform.position);
        }
    }

    void IsWaiting()
    {
        agent.Stop();
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            /*foreach(SkinnedMeshRenderer skin in skinnedMeshes)
            {
                skin.enabled = false;
            }*/

        }
    }


}
