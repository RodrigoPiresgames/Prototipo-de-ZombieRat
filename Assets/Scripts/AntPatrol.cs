using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntPatrol : MonoBehaviour
{
    [SerializeField] private float      speed = 100;
    [SerializeField] private Transform  wallProbe;
    [SerializeField] private Transform  groundProbe;
    [SerializeField] private float      probeRadius = 5;
    [SerializeField] private LayerMask  probeMask;
    [SerializeField] private int        damage = 1;
    [SerializeField] private int        maxHealth = 2;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private IntValue   scoreValue;
    [SerializeField] private AudioClip  hurtSound;
    [SerializeField] private float      stunTime = 5;
    [SerializeField] private Vector3    currentVelocity;
    [SerializeField] private Animator        animEffect;
    [SerializeField] private float           dirX = 1;
    
    private Transform       tf;
    private Rigidbody2D     rb;
    private int             health;
    private bool            isStunned = false;
    private float           stunTimer;

    void Start()
    {
        animEffect.gameObject.SetActive(false);
        tf = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    void Update()
    {
        currentVelocity = rb.velocity;

        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            currentVelocity.x = 0;
            rb.velocity = currentVelocity;
            return;

        }
        if (stunTimer <= 0) 
        {
            isStunned = false;

            animEffect.gameObject.SetActive(false);

            Collider2D collider = Physics2D.OverlapCircle(wallProbe.position, probeRadius, probeMask);
            if (collider != null)
            {
                currentVelocity = SwitchDirection(currentVelocity);
            }
            else
            {
                collider = Physics2D.OverlapCircle(groundProbe.position, probeRadius, probeMask);

                if (collider == null)
                {
                    currentVelocity = SwitchDirection(currentVelocity);
                }
            }

            currentVelocity.x = speed * dirX;

            rb.velocity = currentVelocity;

            if ((currentVelocity.x > 0) && (transform.right.x > 0))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if ((currentVelocity.x < 0) && (transform.right.x < 0))
            {
                transform.rotation = Quaternion.identity;
            }
        }
    }

    private Vector3 SwitchDirection(Vector3 currentVelocity)
    {
        dirX = -dirX;

        if (currentVelocity.y > 0)
        {
            currentVelocity.y = 0;
        }

        return currentVelocity;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        var player = collider.GetComponent<Player>();
        if (player != null)
        {
            
            Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
            

            player.DealDamage(damage, transform);
        }
    }

    public void DealDamage(int damage)
    {

        if (isStunned == false)
        {
            health = health - damage;

            Debug.Log($"Ouch Enemy, health={health}");

            isStunned = true;

            stunTimer = stunTime;

            animEffect.gameObject.SetActive(true);
            
        }


        if (health <= 0)
        {
            SoundManager.Get().Play(hurtSound, transform.position);
            transform.position = new Vector3(transform.position.x, transform.position.y + 15, transform.position.z);
            health = maxHealth;


            /*if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, transform.rotation);
            }*/
        }
    }

    void OnDrawGizmos()
    {
        if (wallProbe)
        {
            Gizmos.color = new Color(1.0f, 0.0f, 1.0f, 0.5f);
            Gizmos.DrawSphere(wallProbe.position, probeRadius);
        }

        if (groundProbe)
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            Gizmos.DrawSphere(groundProbe.position, probeRadius);
        }
    }
}