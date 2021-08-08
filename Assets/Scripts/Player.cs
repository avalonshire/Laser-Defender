using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.1f;
    [SerializeField] GameObject explosionParticles;
    [SerializeField] int health = 200;
   
    Coroutine firingCoroutine;

    [Header("Audio")]
    [SerializeField] AudioClip explodeSFX;
    [SerializeField] AudioClip playerFireSFX;
    [SerializeField] [Range(0, 1)] float playerDieSFXVolume = 0.7f;
    [SerializeField] [Range(0, 1)] float playerFireSFXVolume = 0.7f;

    float xMin;
    float xMax;
    float yMin;
    float yMax;
    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        
    }

 
    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
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
            DiePlayer();
        }
    }

    private void DiePlayer()
    {
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionParticles, transform.position, transform.rotation);
        Destroy(explosion, 2f);
        AudioSource.PlayClipAtPoint(explodeSFX, Camera.main.transform.position, playerDieSFXVolume);
        FindObjectOfType<Level>().LoadGameOver();
    }

    public int GetHealth()
    {
        return health;
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinously()
    {
        while (true)
        {
            GameObject laser = Instantiate(
                   laserPrefab,
                   transform.position,
                   Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            yield return new WaitForSeconds(projectileFiringPeriod);
            AudioSource.PlayClipAtPoint(playerFireSFX, Camera.main.transform.position, playerFireSFXVolume);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }
    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

}
