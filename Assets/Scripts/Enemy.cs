using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement & Aggro")]
    public float moveSpeed = 3.5f;
    public float sightRange = 7f;
    public float stopDistance = 0.4f; // Prevents jittering when directly on top of the player

    [Header("Line of Sight & Ledges")]
    public Transform player;          // Auto-finds object tagged "Player" if left empty
    public LayerMask groundLayer;     // Blocks line of sight through walls/floors
    public Transform ledgeCheck;      // Optional: keeps enemy from charging off cliffs
    public bool stopAtLedges = false;

    [Header("Combat")]
    public int contactDamage = 1;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isChasing;
    private float moveDirection;

    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Automatically find the Player by Tag if not dragged into the Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            StopMoving();
            return;
        }

        // Check if Player is within range and not behind a wall
        if (CanSeePlayer())
        {
            ChasePlayer();
        }
        else
        {
            StopMoving();
        }
    }

    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. Out of range
        if (distanceToPlayer > sightRange) return false;

        // 2. Cast an invisible line from Enemy to Player; if it hits the Ground layer, a wall is blocking view!
        RaycastHit2D hitWall = Physics2D.Linecast(transform.position, player.position, groundLayer);
        if (hitWall.collider != null) return false;

        return true;
    }

    private void ChasePlayer()
    {
        float xDifference = player.position.x - transform.position.x;

        // If already horizontally aligned with the player, stop moving so it doesn't rapidly flip back and forth
        if (Mathf.Abs(xDifference) <= stopDistance)
        {
            StopMoving();
            return;
        }

        moveDirection = Mathf.Sign(xDifference); // -1 for Left, 1 for Right

        // Optional: Stop at the edge of a platform instead of falling off
        if (stopAtLedges && ledgeCheck != null)
        {
            Vector2 checkPos = new Vector2(transform.position.x + (moveDirection * 0.5f), ledgeCheck.position.y);
            bool hasGroundAhead = Physics2D.Raycast(checkPos, Vector2.down, 0.5f, groundLayer);
            if (!hasGroundAhead)
            {
                StopMoving();
                return;
            }
        }

        // Move toward player
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        // Flip sprite toward player (just like PlayerController)
        if (spriteRenderer != null)
        {
            if (moveDirection > 0.01f) spriteRenderer.flipX = false;
            else if (moveDirection < -0.01f) spriteRenderer.flipX = true;
        }

        // Trigger Walk Animation
        if (anim != null) anim.SetBool(IsWalkingHash, true);
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        if (anim != null) anim.SetBool(IsWalkingHash, false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<Health>(out Health playerHealth))
            {
                playerHealth.TakeDamage(contactDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draws the detection radius in the Scene view
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Draws the Line of Sight ray to the player (Green = Clear, Red = Blocked by wall)
        if (player != null)
        {
            bool canSee = CanSeePlayer();
            Gizmos.color = canSee ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}