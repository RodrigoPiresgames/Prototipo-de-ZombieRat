using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float      horizontalSpeed = 200.0f;
    [SerializeField] private float      jumpSpeed = 200.0f;
    [SerializeField] private Transform  groundProbe;
    [SerializeField] private float      groundProbeRadius = 5.0f;
    [SerializeField] private LayerMask  groundMask;
    [SerializeField] private float      maxJumpTime = 0.1f;
    [SerializeField] private float      fallGravityScale = 5.0f;
    [SerializeField] private int        maxHealth = 3;
    [SerializeField] private float      invulnerabilityTime = 2.0f;
    [SerializeField] private float      blinkTime = 0.1f;
    [SerializeField] private float      knockbackIntensity = 100.0f;
    [SerializeField] private float      knockbackDuration = 0.5f;
    [SerializeField] private float      deadTime = 3.0f;
    [SerializeField] private float      levelHeight = 10.0f;
    [SerializeField] private GameObject energyballPrefab;
    [SerializeField] private float      energyballBallSpawnXOffset = 10.0f;
    [SerializeField] private float      energyballBallSpawnYOffset = 10.0f;
    [SerializeField] private float      firecooldownTime = 2.0f;

    private Transform       tf;
    private Vector3         currentPosition;
    private Collider2D      playerCol;
    private Rigidbody2D     rb;
    private Animator        anim;
    private SpriteRenderer  spriteRenderer;
    private float           jumpTime;
    private int             health;
    private float           invulnerabilityTimer = 0;
    private float           blinkTimer = 0;
    private float           inputLockTimer = 0;
    private float           deadTimer = 0;
    private bool            canMove = true;
    private bool            isHidden = false;
    private bool            canHide = false;
    private bool            canGoUp = false;
    private bool            canFire = true;
    private Vector3         energyballSpawn;
    private float           firecooldownTimer = 0;


    private bool isInvulnerable
    {
        get { return invulnerabilityTimer > 0; }
        set { if (value) invulnerabilityTimer = invulnerabilityTime; else invulnerabilityTimer = 0; }
    }

    private bool isInputLocked => !canMove || (inputLockTimer > 0) || (deadTimer > 0);

    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        tf = GetComponent<Transform>();
        playerCol = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = maxHealth;
    }

    private void Update()
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 currentPosition = tf.position;
        bool    onGround = IsOnGround();

        if(onGround)
        {
            anim.SetBool("Jumping", false);
        }

        if (deadTimer > 0)
        {
            deadTimer -= Time.deltaTime;

            if (deadTimer <= 0)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (firecooldownTimer > 0)
        {
            firecooldownTimer -= Time.deltaTime;

            if (firecooldownTimer <= 0)
            {
                canFire = true;
            }
        }


        if (Input.GetButtonDown("Fire"))
        {
            if (canFire == true)
            {
                anim.SetTrigger("Fire");
                canFire = false;
                firecooldownTimer = firecooldownTime;
            }
        }

        if (Input.GetButtonDown("Hide"))
        {
            if (canHide == true)
            {
                if (isHidden == false)
                {
                    rb.isKinematic = true;
                    playerCol.enabled = false;
                    rb.velocity = Vector3.zero;
                    canMove = false;
                    isHidden = true;
                    spriteRenderer.enabled = false;
                }
                else
                {
                    rb.isKinematic = false;
                    playerCol.enabled = true;
                    canMove = true;
                    isHidden = false;
                    spriteRenderer.enabled = true;
                }
            }
        }

        if (Input.GetButtonDown("GoUp"))
        {
            if (canGoUp == true)
            {
                transform.position = new Vector3(currentPosition.x, currentPosition.y + levelHeight, currentPosition.z);
                canGoUp = false;
            }
        }

        if (isInputLocked)
        {
            inputLockTimer -= Time.deltaTime;
        }
        else
        {
            float hAxis = Input.GetAxis("Horizontal");
            currentVelocity.x = hAxis * horizontalSpeed;

            if (Input.GetButtonDown("Jump"))
            {
                if (onGround)
                {
                    anim.SetBool("Jumping", true);
                    rb.gravityScale = 1.0f;
                    currentVelocity.y = jumpSpeed;
                    jumpTime = Time.time;
                }
            }
            else if (Input.GetButton("Jump"))
            {
                float elapsedTime = Time.time - jumpTime;
                if (elapsedTime > maxJumpTime)
                {
                    rb.gravityScale = fallGravityScale;
                }
            }
            else
            {
                rb.gravityScale = fallGravityScale;
            }

            rb.velocity = currentVelocity;

            if ((currentVelocity.x > 0) && (transform.right.x < 0))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if ((currentVelocity.x < 0) && (transform.right.x > 0))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;

            if (invulnerabilityTimer > 0)
            {
                blinkTimer -= Time.deltaTime;

                if (blinkTimer < 0)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                    blinkTimer = blinkTime;
                }
            }
            else
            {
                spriteRenderer.enabled = true;
            }
        }
    }

    private bool IsOnGround()
    {
        var collider = Physics2D.OverlapCircle(groundProbe.position, groundProbeRadius, groundMask);

        return (collider != null);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundProbe != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundProbe.position, groundProbeRadius);
        }
    }

    public void DealDamage(int damage, Transform sourceDamageTransform)
    {
        if (isInvulnerable || health <= 0) return;

        health = health - damage;

        Debug.Log($"Ouch, health={health}");

        if (health <= 0)
        {
            anim.SetTrigger("Die");

            rb.velocity = new Vector2(0, jumpSpeed );

        }
        else
        {
            isInvulnerable = true;

            if (sourceDamageTransform != null)
            {

                Vector2 direction = new Vector2(0.0f, 1.0f);

                direction.x = Mathf.Sign(transform.position.x - sourceDamageTransform.position.x);

                Knockback(direction);
            }
        }
    }

    public float GetJumpSpeed()
    {
        return jumpSpeed;
    }

     public void AnimationFire ()
    {
        energyballSpawn = new Vector3(transform.position.x - energyballBallSpawnXOffset, transform.position.y - energyballBallSpawnYOffset);
        Instantiate(energyballPrefab, energyballSpawn, transform.rotation);
    }

    private void Knockback(Vector2 direction)
    {
        rb.velocity = direction * knockbackIntensity;

        inputLockTimer = knockbackDuration;
    }

    public void Die()
    {
        Destroy(gameObject);
        health = 0;
        SceneManager.LoadScene("GameScene");
    }

    public int GetHealth()
    {
        return health;
    }

    public void UpdateHide(bool canUpdate) => canHide = canUpdate;

    public void UpdateGoUp(bool canUpdate) => canGoUp = canUpdate;

}
