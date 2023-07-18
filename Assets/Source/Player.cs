using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character,IDamagable
{
    [Header("Input")]
    public KeyCode meleeAttackKey = KeyCode.Mouse0;//Mouse0 left click.
    public KeyCode rangedAttackKey = KeyCode.Mouse1;
    public KeyCode JumpKey = KeyCode.Space;
    public string xMoveAxis = "Horizontal";

    [Header("Combat")]
    public Transform meleeattackOrigin = null;
    public Transform rangedAttackOrigin = null;
    public GameObject projectile = null;
    public float meleeAttackRadius = 0.6f;
    public float meleedamage = 2f;
    public float meleeAttackDelay = 1.1f;
    public float rangedAttackDelay = 0.3f;
    public float freezeDelay = 0.4f;
    public LayerMask enemyLayer = 6;

    //Private member to Apply physics to CHaracter and Take input from the Entity.and to attract value from input.
    private float moveIntensionX = 0;//will carry value from xMoveAxis.
    private bool attemptJump = false;//true when space bar is hit
    private bool attemptMeleeAttack = false;//true whem left click.
    private bool attemptRangedAttack = false;
    private float timeUntilMeleeReady = 0;
    private float timeUntilRangedReady = 0;
    private bool isMeleeAttacking = false;
    private bool isFrozen = false;

    void Update()
    {
        GetInput();//to register player input , everyframe
        HandleJump();
        HandleMeleeAttack();
        HandleRangedAttack();
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        HandleRun();
    }

    void OnDrawGizmosSelected ()
    {
        Debug.DrawRay(transform.position, -Vector2.up * groundedLeeway, Color.red);
        if (meleeattackOrigin!=null)
        {
            Gizmos.DrawWireSphere(meleeattackOrigin.position, meleeAttackRadius);

        }
    }

    private void GetInput()
    {
        moveIntensionX = Input.GetAxis(xMoveAxis);
        attemptMeleeAttack = Input.GetKeyDown(meleeAttackKey);
        attemptRangedAttack = Input.GetKeyDown(rangedAttackKey);
        attemptJump = Input.GetKeyDown(JumpKey);
    }
    
    private void HandleRun()
    {
        if (moveIntensionX>0 && transform.rotation.y == 0 && !isMeleeAttacking) //if want to move left and current rotaion is to left then
        {
            transform.rotation = Quaternion.Euler(0, 180f, 0);//flip character on Y axis -> its now facing right.
        }
        else if (moveIntensionX < 0 && transform.rotation.y != 0 && !isMeleeAttacking)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (!isFrozen || !isGrounded())
        {
            Rb2D.velocity = new Vector2(moveIntensionX * speed, Rb2D.velocity.y);
        }
        else if (isGrounded())
        {
            Rb2D.velocity = new Vector2(0, Rb2D.velocity.y);
        }
        
    }

    private void HandleJump()
    {
        if (attemptJump && isGrounded())
        {
            Rb2D.velocity = new Vector2(Rb2D.velocity.x, jumpForce);
        }
    }

    private void HandleMeleeAttack()
    {
        if (attemptMeleeAttack && timeUntilMeleeReady<=0)
        {
            Debug.Log("Player: AttemptMeleeAttack");
            Collider2D[] overlappedColliders = Physics2D.OverlapCircleAll(meleeattackOrigin.position, meleeAttackRadius, enemyLayer);
            for (int i = 0; i < overlappedColliders.Length; i++)
            {
                IDamagable enemyAttributes = overlappedColliders[i].GetComponent<IDamagable>();
                if (enemyAttributes != null)
                {
                    enemyAttributes.ApplyDamage(meleedamage);
                }
            }
            timeUntilMeleeReady = meleeAttackDelay;
        }
        else
        {
            timeUntilMeleeReady -= Time.deltaTime;
        }
    }

    private void HandleRangedAttack()
    {
        if (attemptRangedAttack && timeUntilRangedReady <= 0)
        {
            Debug.Log("Player: AttemptRangedAttack");
            Instantiate(projectile, rangedAttackOrigin.position, rangedAttackOrigin.rotation);
            timeUntilRangedReady = rangedAttackDelay;
        }
        else
        {
            timeUntilRangedReady -= Time.deltaTime;
        }
    }
    private void HandleAnimations()
    {
        Animator.SetBool("Grounded",isGrounded());

        if (attemptMeleeAttack)
        {
            if (!isMeleeAttacking)
            {
                StartCoroutine(MeleeAttackAnimDelay());
                StartCoroutine(ActionFrozenDelay());
            }
        }

        if (attemptRangedAttack)
        {
            Animator.SetTrigger("Shoot");
        }

        if (attemptJump && isGrounded() || Rb2D.velocity.y>1f)
        {
            if (!isMeleeAttacking)
            {
                Animator.SetTrigger("Jump");
            }
        }
        if (Mathf.Abs(moveIntensionX)>0.1f && isGrounded())
        {
            Animator.SetInteger("AnimState", 2);
        }
        else
        {
            Animator.SetInteger("AnimState", 0);
        }
    }
    private IEnumerator MeleeAttackAnimDelay()
    {
        Animator.SetTrigger("Attack");//animation for the action
        isMeleeAttacking = true;//set that flag true
        yield return new WaitForSeconds(meleeAttackDelay);//wait for its delay/ cooldown time for animation
        isMeleeAttacking = false;//set flag false so we can use animation again.
    }
    private IEnumerator ActionFrozenDelay()
    {
        isFrozen = true;
        yield return new WaitForSeconds(freezeDelay);
        isFrozen = false;
    }
    public virtual void ApplyDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
}
