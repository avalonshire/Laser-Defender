
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Shooting")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject laserEnemyPrefab;
    [SerializeField] float enemyProjectileSpeed = 5f;
    [SerializeField] GameObject explosionParticles;

    [Header("AudioFX")]
    [SerializeField] [Range(0,1)]float explodeSFXVolume = 0.7f;
    [SerializeField] [Range(0, 1)] float enemyFireSFXVolume = 0.7f;
    [SerializeField] AudioClip explodeSFX;
    [SerializeField] AudioClip enemyFireSFX;

    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 20;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }
    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laserEnemy = Instantiate(laserEnemyPrefab, transform.position, Quaternion.identity) as GameObject;
        laserEnemy.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -enemyProjectileSpeed);
        AudioSource.PlayClipAtPoint(enemyFireSFX, Camera.main.transform.position, enemyFireSFXVolume);
    }


    private void OnTriggerEnter2D(Collider2D other) //the other thing that bumped into the collision or us which is the enemy
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer) //passing damageDealer parameter of DamageDealer type for this function to use
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();// Destroy laser upon impact
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionParticles, transform.position, transform.rotation);
        Destroy(explosion, 0.8f);
        AudioSource.PlayClipAtPoint(explodeSFX, Camera.main.transform.position, explodeSFXVolume);
    }
}
