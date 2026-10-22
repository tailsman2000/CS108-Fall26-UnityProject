using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 currentCheckpointPosition;
    private Health playerHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private Collider2D playerCollider;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 1.5f;

    void Awake()
    {
        playerHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        playerCollider = GetComponent<Collider2D>();

        // Set initial spawn position to where the player starts in the scene
        currentCheckpointPosition = transform.position;
    }

    // Call this from your Checkpoint.cs trigger script
    public void UpdateCheckpoint(Vector3 newPosition)
    {
        currentCheckpointPosition = newPosition;
        Debug.Log("Checkpoint Updated: " + newPosition);
    }

    // Call this when the player dies (you can hook this into Health.cs or trigger it on death)
    public void TriggerRespawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // Wait for death animation/delay
        yield return new WaitForSeconds(respawnDelay);

        // 1. Move player to checkpoint
        transform.position = currentCheckpointPosition;

        // 2. Reset health back to maximum
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        // 3. Re-enable physics, collision, sprite, and movement controls
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }
        if (playerCollider != null) playerCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (playerController != null) playerController.enabled = true;

        Debug.Log("Player Respawned at: " + currentCheckpointPosition);
    }
}