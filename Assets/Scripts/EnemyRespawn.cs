using System.Collections;
using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    private Vector3 initialPosition;
    private Health enemyHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Enemy enemyController;
    private Collider2D enemyCollider;

    [Header("Enemy Respawn Settings")]
    [SerializeField] private float respawnDelay = 5.0f; // Time before enemy comes back to life

    void Awake()
    {
        initialPosition = transform.position;
        enemyHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyController = GetComponent<Enemy>();
        enemyCollider = GetComponent<Collider2D>();
    }

    public void TriggerRespawn()
    {
        StartCoroutine(EnemyRespawnRoutine());
    }

    private IEnumerator EnemyRespawnRoutine()
    {
        // Wait for the respawn timer
        yield return new WaitForSeconds(respawnDelay);

        // 1. Move enemy back to their original spawn point
        transform.position = initialPosition;

        // 2. Refill health
        if (enemyHealth != null)
        {
            enemyHealth.ResetHealth();
        }

        // 3. Re-enable physics, sprite, collision, and AI script
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }
        if (enemyCollider != null) enemyCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (enemyController != null) enemyController.enabled = true;

        Debug.Log("Enemy respawned at: " + initialPosition);
    }
}