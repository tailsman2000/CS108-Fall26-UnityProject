using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    private float horizontalInput;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Combat")]
    public Transform attackPoint;      // Place this right at your character's hand (e.g., X: 0.7, Y: 0.2)
    public float attackRangeX = 3.0f;  // Total length of the whip extending forward from the hand
    public float attackRangeY = 0.6f;  // Height of the whip hitbox
    public int attackDamage = 1;
    public float attackCooldown = 0.4f;
    public LayerMask enemyLayers;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private float lastAttackTime;
    private bool isAttacking;

    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int AttackTriggerHash = Animator.StringToHash("attack");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. Lock movement input while swinging the whip
        if (isAttacking)
        {
            horizontalInput = 0f;
        }
        else
        {
            horizontalInput = 0f;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontalInput = -1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontalInput = 1f;
            }
        }

        // 2. Trigger walking animation
        if (anim != null)
        {
            bool isMoving = Mathf.Abs(horizontalInput) > 0.01f;
            anim.SetBool(IsWalkingHash, isMoving);
        }

        // 3. Flip sprite & move AttackPoint to the active hand side
        if (spriteRenderer != null && !isAttacking)
        {
            if (horizontalInput > 0.01f)
            {
                spriteRenderer.flipX = false;
                if (attackPoint != null)
                    attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0f);
            }
            else if (horizontalInput < -0.01f)
            {
                spriteRenderer.flipX = true;
                if (attackPoint != null)
                    attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0f);
            }
        }

        // 4. Attack Input (J key or Left Mouse Button)
        if (Keyboard.current != null && (Keyboard.current.jKey.wasPressedThisFrame || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }

        if (horizontalInput > 0.01f)
        {
        spriteRenderer.flipX = false;
        if (attackPoint != null)
            {
                attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0f);
                attackPoint.localScale = new Vector3(1f, 1f, 1f); // Whip faces Right
            }
        }
        else if (horizontalInput < -0.01f)
        {
        spriteRenderer.flipX = true;
        if (attackPoint != null)
            {
                attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0f);
                attackPoint.localScale = new Vector3(-1f, 1f, 1f); // Flips child WhipVisual to face Left!
            }
        }

        // 5. Jump detection
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded && !isAttacking)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // Calculates the center of the box so it starts at AttackPoint (the hand) and extends forward
    private Vector2 GetHitboxCenter()
    {
        float facingDirection = (spriteRenderer != null && spriteRenderer.flipX) ? -1f : 1f;
        return (Vector2)attackPoint.position + new Vector2(facingDirection * (attackRangeX * 0.5f), 0f);
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;

        if (anim != null) anim.SetTrigger(AttackTriggerHash);

        // Stop horizontal sliding when swinging on the ground
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    // Can also be called via an Animation Event on the exact whip-crack frame
    public void DealWhipDamage()
    {
        if (attackPoint == null) return;

        Vector2 boxCenter = GetHitboxCenter();
        Vector2 boxSize = new Vector2(attackRangeX, attackRangeY);

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, enemyLayers);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent<Health>(out Health enemyHealth))
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }

        if (attackPoint != null)
        {
            // Small yellow dot showing where the hand (AttackPoint) is anchored
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, 0.08f);

            // Cyan rectangle extending forward from the hand
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(GetHitboxCenter(), new Vector3(attackRangeX, attackRangeY, 0f));
        }
    }
    
    
}