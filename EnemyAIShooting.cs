using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIShooting : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] Player player;
    [SerializeField] GameObject Bullets;

    [Header("Distance Between Player")]
    [SerializeField] float MaxDis = 5f;
    [SerializeField] float MinDis = 1f;
    float DisBtw;

    [Header("Enemy Constraints")]
    [SerializeField] int EnemyHealth = 10;
    [SerializeField] float EnemySpeed = 10f;
    [SerializeField] float DestroyTime = 3f;
    [SerializeField] Transform Gun;
 
    [Header("Bullet Constraints")]
    [SerializeField] float BulletLifeTime = 5f;
    [SerializeField] float TimeBtwShots = 3f;
    bool IsShoot = true;

    void Start()
    {
        //caching a reference to the player.
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        //if enemies health is zero or the player is not in the scene, return immedietly 
        if (EnemyHealth < 0 || player == null) { return; }
        
        //if the player is present, then check for the distance between the player and this game object.
        DisBtw = Vector2.Distance(player.gameObject.transform.position, gameObject.transform.position);
        
        //Point the gun towards the player.
        Gun.LookAt(player.transform);
        
        // if the player is visible to enemy or the player is range of enemy
        if (DisBtw > MinDis && DisBtw < MaxDis)
        {
            //move more closely towards enemy.
            transform.position = Vector2.MoveTowards(transform.position, player.gameObject.transform.position, EnemySpeed * Time.deltaTime);
        }
        
        //call the function for pointing gun towards the player.
        LookAtPlayer();
        
        //if all the condition for shooting is true and the enemy is in shooting range.
        if(IsShoot && DisBtw <= MinDis)
        {
            //then start firing the gun.
            StartCoroutine(Fire());
        }
    }
    
    //the function for pointing gun towards the player.
    private void LookAtPlayer()
    {
        //finding the distance difference between the player and this enemy.
        //creating a new value for rotation in the direction of player
        //and assigning it to the gun.
        Vector3 diff = player.transform.position - transform.position;
        float rotationValue = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        Gun.localRotation = Quaternion.Euler(0, 0, rotationValue);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if the player get collided with player, accidently or intentionally
        //kill the player instantly and take some damge.
        if(collision.gameObject.tag == "Player") 
        {
            Destroy(collision.gameObject);
            TakeDamage(); 
        }
    }
    
    //function for taking damage
    void TakeDamage()
    {
        //decrease the health
        //check whether health became zero, if yes destroy the gameobject within certain time.
        //the certain time is to play the animation of death
        EnemyHealth--;
        if (EnemyHealth <= 0) { Destroy(gameObject, DestroyTime); }
    }

    //the function call for firing the bullet.
    IEnumerator Fire()
    {   
        //instantiate the bullet as gameobject 
        //wait until the time between shots to complete and fire again.
        IsShoot = false;
        GameObject Bullet = Instantiate(Bullets,Gun.position,Gun.rotation,Gun.transform) as GameObject;
        Destroy(Bullet, BulletLifeTime);
        yield return new WaitForSeconds(TimeBtwShots);
        IsShoot = true;
    }

    //for debugging process, to check whether the gun is pointed towards player or not.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Gun.position, player.transform.position);
    }
}
