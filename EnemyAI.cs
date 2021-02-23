using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Transform Target;
    NavMeshAgent navMeshAgent;

    [SerializeField] float Range = 10f;
    float distanceToTarget = Mathf.Infinity;

    [SerializeField] int Damage = 10;
    [SerializeField] float turningSpeed = 5f;

    public bool isProvoked = false;
    bool dead;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        
    }

    void Update()
    {
        dead = GetComponent<EnemyHealth>().IsDead();
        if (dead) { return; }
        distanceToTarget = Vector3.Distance(Target.position, transform.position);
        if (isProvoked)
        {
            EngageTarget();
        }
        else if (distanceToTarget <= Range)
        {
            isProvoked = true;
        }

    }

    private void EngageTarget()
    {
        lookTarget();

        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.SetDestination(Target.position);
            GetComponent<Animator>().SetBool("Attack", false);
            GetComponent<Animator>().SetTrigger("Move");
            
        }
        if (distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            GetComponent<Animator>().SetBool("Attack", true);
        }
    }

    private void lookTarget()
    {
        Vector3 direction = (Target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turningSpeed);
    }


    public void DecreasePlayerHealth()
    {
        Target.GetComponent<PlayerHealth>().decrease(Damage);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
