using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public enum AIState
    {
        Idle,
        Fleeing,
        Wander,
        Chasing,
        Dead
    }
    public Transform currentTarget;
    public AIState currState;
    [Header("Movement")]
    public float runSpeed;

    public bool aggressive;

    private NavMeshAgent agent;

    [Header("Targeting")]
    public float aggroRange;
    public float attackRange;

    public bool foundPlayer;
    public bool foundTarget;
    public LayerMask playerLayer;
    public LayerMask targetLayer;

    public Animator anim;

    SkinnedMeshRenderer[] skinnedMeshes;

    Collider[] targetsColliders;
    List<GameObject> targetsObjectList = new List<GameObject>();

    public PlayerController player;
    [Header("Wandering")]
    public bool hasWanderPosition;
    public Vector3 wanderPosition;
    public float wanderRadius;
    public float maxWanderCooldown;
    public float minWanderCooldown;
    public float wanderThreshold;
    float WanderCooldown
    {
        get
        {
            float r = Random.Range(0f, 1f);
            return Mathf.Lerp(minWanderCooldown, maxWanderCooldown, r);
        }

    }
    private float stopIdleTime;
    [Header("Attacking")]
    public float attackCooldownTime;
    public bool attacking;
    private float endAttackCooldownTime;

    [Header("Health")]
    public int health = 1;
    public bool stunned;

    public Transform suckEffect;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        DecideState();
        switch(currState)
        {
            case AIState.Idle:
                Idle();
                break;
            case AIState.Wander:
                Wander();
                break;
            case AIState.Fleeing:
                Flee();
                break;
            case AIState.Chasing:
                Chase();
                break;
            default:
                // do nothing
                break;

        }
        UpdateAnimator();
    }
    void UpdateAnimator()
    {
        float velocity = agent.velocity.magnitude / agent.speed;
        anim.SetFloat("WalkRun", velocity);
        if(currState != AIState.Idle && currState != AIState.Dead)
        {
            anim.SetBool("isRuning", true);
        }
        else
        {
            anim.SetBool("isRuning", false);
        }
    }
    public void DecideState()
    {       
        // check if player is within aggro range
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist < aggroRange)
        {
            if (aggressive)
            {
                currState = AIState.Chasing;
                currentTarget = player.transform;
                return;
            }
            else
            {
                currState = AIState.Fleeing;
                currentTarget = player.transform;
                return;
            }
        }
        dist = float.PositiveInfinity;
        Transform closest = null;
        var targetsColliders = Physics.OverlapSphere(transform.position, wanderRadius, targetLayer);
        foreach(var v in targetsColliders)
        {
            float d = Vector3.Distance(v.transform.position, transform.position);
            if (d < dist)
            {
                dist = d;
                closest = v.transform;
            }
        }
        if(closest != null && dist < aggroRange)
        {
            currState = AIState.Chasing;
            currentTarget = closest;
        }
        // check if idle timer is past
        if (currState == AIState.Idle && Time.time > stopIdleTime)
        {
            // stop idling
            currState = AIState.Wander;
            return;
        }

    }

    public void Idle()
    {

    }
    public void Wander()
    {
        if(hasWanderPosition)
        {
            float dist = Vector3.Distance(transform.position, wanderPosition);
            if (dist < wanderThreshold)
            {
                stopIdleTime = Time.time + WanderCooldown;
                currState = AIState.Idle;
            }
        }
        else
        {
            wanderPosition = RandomNavSphere(transform.position, wanderRadius, -1,wanderRadius/10f);
            hasWanderPosition = true;
            agent.SetDestination(wanderPosition);
        }
    }
    public void Flee()
    {
        if (attacking)
        {
            if (Time.time > endAttackCooldownTime)
            {
                attacking = false;
            }
            else
                return;
        }
        Vector3 destination = transform.position + ((transform.position - currentTarget.position));
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        agent.SetDestination(destination);
        if (distance < attackRange)
        {
            agent.SetDestination(transform.position);
            endAttackCooldownTime = Time.time + attackCooldownTime;
            Attack();
        }
    }
    public void Chase()
    {
        if(attacking)
        {
            if (Time.time > endAttackCooldownTime)
            {
                attacking = false;
            }
            else
                return;
        }
        Vector3 destination = currentTarget.position + (currentTarget.position - transform.position).normalized * attackRange * 0.75f;
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        agent.SetDestination(destination);
        if (distance < attackRange)
        {
            agent.SetDestination(transform.position);
            endAttackCooldownTime = Time.time + attackCooldownTime;
            Attack();
        }
    }

    public void Attack()
    {
        attacking = true;
        if(currentTarget.GetComponent<CidadaoDamage>() != null)
        {
            currentTarget.GetComponent<CidadaoDamage>().DamageTimer();
        }
        else if(currentTarget == player.transform)
        {
            player.TakeDamage();
        }
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask, float minRadius = 0)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        if(randDirection.magnitude < minRadius)
        {
            randDirection = randDirection.normalized * minRadius;
        }

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    public void TakeDamage()
    {
        health--;
        if(health<=0)
        {
            Destroy(gameObject);
        }
    }

    public void Stun()
    {
        stunned = true;
        agent.isStopped = true;
    }
    public void StopStun()
    {
        stunned = false;
        agent.isStopped = false;
    }
}
