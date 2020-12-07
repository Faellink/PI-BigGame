using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float distTarget;
    public float distChaser;
    public enum AIState
    {
        Idle,
        Fleeing,
        Wander,
        Chasing,
        Dead
    }
    public ZombieAI chaser;
    public Transform currentTarget;
    public AIState currState;
    [Header("Movement")]
    public float runSpeed;
    public float walkSpeed;
    public bool hostile; // is enemy or neutral
    public bool aggressive; // will flee or chase player

    private NavMeshAgent agent;

    [Header("Targeting")]
    public float aggroRange;
    public float loseAggroRange;
    public float attackRange;

    public bool foundPlayer;
    public bool foundTarget;
    public LayerMask playerLayer;
    public LayerMask targetLayer;
    public float ang;

    public Animator anim;

    SkinnedMeshRenderer[] skinnedMeshes;

    Collider[] targetsColliders;
    List<GameObject> targetsObjectList = new List<GameObject>();
    public bool carried;

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
    public float idleTimeRemaining;
    [Header("Attacking")]
    public bool canAttack;
    public float attackCooldownTime;
    public bool attacking;
    private float endAttackCooldownTime;
    public float attackPreventionTime;
    private float stopPrevention;

    [Header("Health")]
    public int health = 1;
    public bool stunned;
    public int enemyHealth = 1;
    public Transform suckEffect;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        
        if(currentTarget != null)
            distTarget = Vector3.Distance(currentTarget.position, transform.position);
        if(currState == AIState.Dead)
        {
            agent.isStopped = true;
            GetComponent<Collider>().enabled = false;
            this.enabled = false;
            return;
        }

        if (attacking && currentTarget != null)
        {
            // rotate towards target
            Vector3 delta = currentTarget.position -  transform.position;
            ang = Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg;

            var euler = transform.localEulerAngles;
            euler.y = ang;
            transform.rotation = Quaternion.Euler(euler);
        }

        DecideState();
        switch(currState)
        {
            case AIState.Idle:
                Idle();
                break;
            case AIState.Wander:
                agent.speed = walkSpeed;
                Wander();
                break;
            case AIState.Fleeing:
                agent.speed = runSpeed;
                Flee();
                break;
            case AIState.Chasing:
                agent.speed = runSpeed;
                Chase();
                break;
            default:
                // do nothing
                break;

        }
        UpdateAnimator();
        if(Time.time > stopPrevention)
        {
            canAttack = true;
        }
    }
    void UpdateAnimator()
    {
        if (hostile)
        {
            anim.SetBool("Zombie", true);
            anim.SetFloat("ZombieBlend", 1);
        }
        else
        {
            anim.SetBool("Zombie", false);
            anim.SetFloat("ZombieBlend", 0);
        }

        if (currState == AIState.Idle || stunned)
        {
            anim.SetBool("Walking", false);
            anim.SetBool("Running", false);
            anim.SetBool("RunAway", false);
        }
        else
            anim.SetBool("Walking", true);

        if (currState == AIState.Chasing)
        {
            anim.SetBool("Running", true);
            anim.SetBool("RunAway", false);
        }
        else if (currState == AIState.Fleeing)
        {
            if (hostile)
            {
                anim.SetBool("Running", false);
                anim.SetBool("RunAway", true);
            }
            else
                anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
            anim.SetBool("RunAway", false);
        }

    }
    public void DecideState()
    {
        if (attacking)
            return;
        // check if player is within aggro range
        float dist = float.PositiveInfinity;
        if(currentTarget != null)
            dist = Vector3.Distance(transform.position, currentTarget.position);
        float playerDist = Vector3.Distance(transform.position, player.transform.position);      
        if (chaser != null)
        {
            dist = Vector3.Distance(transform.position, chaser.transform.position);
            distChaser = dist;
            if(dist > chaser.loseAggroRange || !chaser.hostile)
            {
                currState = AIState.Wander;
                chaser = null;
                CancelSuck();
                StopStun();
                return;
            }
            else
            {
                currState = AIState.Fleeing;
                return;
            }
        }
        

        if (playerDist < aggroRange && hostile)
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
        else if(loseAggroRange < dist && (currState == AIState.Fleeing || currState == AIState.Chasing))
        {
            if (currentTarget != null && currentTarget.GetComponent<ZombieAI>() != null)
            {
                currentTarget.GetComponent<ZombieAI>().chaser = null;
                //currentTarget.GetComponent<ZombieAI>().StopStun();

            }
            currState = AIState.Wander;
        }
        if (hostile)
        {
            dist = float.PositiveInfinity;
            Transform closest = null;
            var targetsColliders = Physics.OverlapSphere(transform.position, wanderRadius, targetLayer);
            foreach (var v in targetsColliders)
            {
                if (v.GetComponent<ZombieAI>() == null 
                    || v.GetComponent<ZombieAI>().hostile 
                    || v.GetComponent<ZombieAI>().currState == AIState.Dead
                    || v.GetComponent<ZombieAI>().carried)
                    continue;
                float d = Vector3.Distance(v.transform.position, transform.position);
                if (d < dist)
                {
                    dist = d;
                    closest = v.transform;
                }
            }
            if (closest != null && dist < aggroRange)
            {
                if(closest != currentTarget && currentTarget !=null )
                {
                    if (currentTarget.GetComponent<ZombieAI>() != null)
                    {
                        currentTarget.GetComponent<ZombieAI>().chaser = null;
                    }
                }
                currState = AIState.Chasing;
                currentTarget = closest;
                if(currentTarget.GetComponent<ZombieAI>() != null)
                {
                    currentTarget.GetComponent<ZombieAI>().chaser = this;
                }
            }
        }
        // check if idle timer is past
        if (currState == AIState.Idle && Time.time > stopIdleTime)
        {
            // stop idling
            Debug.Log("walk");
            currState = AIState.Wander;
            hasWanderPosition = false;
            return;
        }

        
    }

    public void Idle()
    {
        idleTimeRemaining = stopIdleTime - Time.time;
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
                hasWanderPosition = false;
            }
            else
            {
                agent.SetDestination(wanderPosition);
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
        Transform fleeTarget = player.transform;
        if (chaser != null)
            fleeTarget = chaser.transform;
        Vector3 destination = transform.position + ((transform.position - fleeTarget.position).normalized * loseAggroRange );
        float distance = Vector3.Distance(transform.position, fleeTarget.position);
        agent.SetDestination(destination);
        if (hostile && distance < attackRange)
        {
            agent.SetDestination(transform.position);
            endAttackCooldownTime = Time.time + attackCooldownTime;
            Attack();
        }
    }
    public void Chase()
    {
        if(currentTarget.GetComponent<ZombieAI>() != null && currentTarget.GetComponent<ZombieAI>().hostile)
        {
            currState = AIState.Wander;
            currentTarget = null;
            return;
        }
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
        if(!canAttack || stunned)
        {
            return;
        }
        Debug.Log("attc");
        stopPrevention = Time.time + attackPreventionTime;
        canAttack = false;
        anim.SetTrigger("Attack");
        attacking = true;
        if(currentTarget == player.transform)
        {

        }
        else if(currentTarget != null)
        {
            currentTarget.GetComponent<ZombieAI>().Stun();
            currentTarget.GetComponent<ZombieAI>().StartSuck();
        }
    }
    public void FinishAttack()
    {
        if(currentTarget == null || Vector3.Distance(transform.position,currentTarget.position) > attackRange * 1.5f)
        {
            return;
        }
        if (currentTarget.GetComponent<ZombieAI>() != null)
        {
            currentTarget.GetComponent<ZombieAI>().TakeDamage();
            currentTarget.GetComponent<ZombieAI>().CancelSuck();
            currentTarget = null;
            currState = AIState.Wander;
            return;
        }
        else if (currentTarget == player.transform)
            player.TakeDamage();       
    }
    public void EndAttack()
    {
        attacking = false;
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
        //anim.SetTrigger("Hit");
        hostile = true;
        chaser = null;
        StopStun();
    }
    public void TakeEnemyDamage()
    {
        enemyHealth--;
        if (enemyHealth <= 0)
        {
            attacking = false;
            currentTarget = null;
            hostile = false;
            aggressive = false;
            anim.SetTrigger("Revert");
            currState = AIState.Wander;
        }
    }

    public void StartSuck()
    {
        anim.SetTrigger("Suck");
    }

    public void CancelSuck()
    {
        anim.SetTrigger("StopSuck");
    }
    public void ForceAnotherTarget()
    {
        currentTarget = null;
        currState = AIState.Wander;

    }
    public void EndSuck()
    {
        if (enemyHealth <= 0)
            anim.SetTrigger("Revert");
        else
            anim.SetTrigger("StopSuck");
    }
    public void Stun()
    {
        Debug.Log("Stunned");
        stunned = true;
        agent.isStopped = true;
    }
    public void StopStun()
    { 
        Debug.Log("Stop Stunned");

        stunned = false;
        agent.isStopped = false;
    }

    public void GetCarried()
    {
        carried = true;
        agent.isStopped = true;
        if(chaser != null)
        {
            chaser.ForceAnotherTarget();
        }
    }

    public void GetDropped()
    {
        carried = false;
        agent.isStopped = false;
    }

}
