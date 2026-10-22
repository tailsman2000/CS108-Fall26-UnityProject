using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    public Health playerHealth;       
    public Image healthBarImage;      

    [Header("Sprite Variations (Index 0 = Empty, Index 4 = Full)")]
    public Sprite[] healthSprites;    

    private int lastKnownHealth = -1;

    void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerHealth = player.GetComponent<Health>();
        }

        if (healthBarImage == null)
        {
            healthBarImage = GetComponent<Image>();
        }

        // Force an immediate update on frame 1
        UpdateHealthUI();
    }

    void LateUpdate()
    {
        // Only update when health actually changes to save performance
        if (playerHealth != null && healthBarImage != null && healthSprites != null && healthSprites.Length > 0)
        {
            int currentHP = playerHealth.GetCurrentHealth();
            if (currentHP != lastKnownHealth)
            {
                lastKnownHealth = currentHP;
                UpdateHealthUI();
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null) return;

        // If health has dropped to 0 or below, force index 0 (Empty sprite) immediately
        int currentHP = playerHealth.GetCurrentHealth();
        if (currentHP <= 0)
        {
            if (healthSprites != null && healthSprites.Length > 0)
            {
                healthBarImage.sprite = healthSprites[0];
            }
            return;
        }

        int clampedHP = Mathf.Clamp(currentHP, 0, healthSprites.Length - 1);
        healthBarImage.sprite = healthSprites[clampedHP];
    }
}