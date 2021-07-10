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
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (EnemyHealth < 0 || player == null) { return; }
        DisBtw = Vector2.Distance(player.gameObject.transform.position, gameObject.transform.position);
        Gun.LookAt(player.transform);
        if (DisBtw > MinDis && DisBtw < MaxDis)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.gameObject.transform.position, EnemySpeed * Time.deltaTime);
        }
        LookAtPlayer();
        if(IsShoot && DisBtw <= MinDis)
        {
            StartCoroutine(Fire());
        }
    }

    private void LookAtPlayer()
    {
        Vector3 diff = player.transform.position - transform.position;
        float rotationValue = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        Gun.localRotation = Quaternion.Euler(0, 0, rotationValue);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player") 
        {
            Destroy(collision.gameObject);
            TakeDamage(); 
        }
    }

    void TakeDamage()
    {
        EnemyHealth--;
        if (EnemyHealth <= 0) { Destroy(gameObject, DestroyTime); }
    }

    IEnumerator Fire()
    {   
        IsShoot = false;
        GameObject Bullet = Instantiate(Bullets,Gun.position,Gun.rotation,Gun.transform) as GameObject;
        Destroy(Bullet, BulletLifeTime);
        yield return new WaitForSeconds(TimeBtwShots);
        IsShoot = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Gun.position, player.transform.position);
    }
}
