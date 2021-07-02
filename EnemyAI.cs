using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //Reference of target to follow to activate the enemy
    //Attach Player having NavMeshAgent to this component through unity inspector
    [SerializeField] Transform Target;
    
    //caching of NavMeshAgent for easier use
    NavMeshAgent navMeshAgent;

    //the range between target and enemy to activate the enemy
    [SerializeField] float Range = 10f;
    float distanceToTarget = Mathf.Infinity;

    //Health and Speed characteristics of enemy
    [SerializeField] int Damage = 10;
    [SerializeField] float turningSpeed = 5f;

    //Bool to know whether the enemy is activated or not
    public bool isProvoked = false;
    
    //bool to know whether the enemy is alive or not
    bool dead;

    void Start()
    {
        //caching navMeshagent
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //checking whether the enemy is still alive
        //if not then return immediately
        dead = GetComponent<EnemyHealth>().IsDead();
        if (dead) { return; }
        
        //assigning the distance to enemy from target
        distanceToTarget = Vector3.Distance(Target.position, transform.position);
        if (isProvoked)
        {
            //calling the movement function if activated
            EngageTarget();
        }
        //if not yet activated, compare their distance,
        //whether the target have reached the range.
        else if (distanceToTarget <= Range)
        {
            //if the target entered the range then activate
            isProvoked = true;
        }

    }
    
    //Movement Function
    private void EngageTarget()
    {
        //Look at direction of target function
        lookTarget();
        
        //compare distace to target and stopping distance that assigned in navmesh agent in target.
        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {   
            //if enemy haven't reached the stopping condition, move towards target
            navMeshAgent.SetDestination(Target.position);
            //you can implement the animation codes for movement in here
        }
    }
    
    //Look in direcetion of target
    private void lookTarget()
    { 
        //calculate new direction vector from target's position to enemies position
        Vector3 direction = (Target.position - transform.position).normalized;
        
        //make new qauternion with the new direction vector we calculated to assign that to the enemie's rotation
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        
        //assign the created quaternion to the enemy
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turningSpeed);
    }

    //gizmos to give a visual representation of range for debugging process
    //for altering the range for a better gameplay.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
