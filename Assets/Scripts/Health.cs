using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invincibilityDuration = 0.5f;

    [Header("Hit Flash Settings")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.12f;

    [Header("Events")]
    public UnityEvent<int> OnDamaged;
    public UnityEvent OnDeath;

    private int currentHealth;
    private bool isInvincible;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damageAmount;
        OnDamaged?.Invoke(currentHealth);

        // Trigger the red hit flash
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (invincibilityDuration > 0f)
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void Die()
    {
        currentHealth = 0;
        
        // Disable physics and collision immediately for all objects
        if (TryGetComponent<Collider2D>(out Collider2D col)) col.enabled = false;
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) rb.simulated = false;
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr)) sr.enabled = false;

        // Check if this is the Player or an Enemy
        if (CompareTag("Player"))
        {
            if (TryGetComponent<PlayerController>(out PlayerController controller)) controller.enabled = false;
            if (TryGetComponent<PlayerRespawn>(out PlayerRespawn respawnScript)) respawnScript.TriggerRespawn();
        }
        else
        {
            // If it's an enemy, disable its AI script and trigger enemy respawn
            if (TryGetComponent<Enemy>(out Enemy enemyScript)) enemyScript.enabled = false;
            if (TryGetComponent<EnemyRespawn>(out EnemyRespawn enemyRespawn)) enemyRespawn.TriggerRespawn();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isInvincible = false;
    }
}