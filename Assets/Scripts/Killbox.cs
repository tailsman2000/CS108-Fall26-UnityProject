using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the trigger is the Player
        if (collision.CompareTag("Player"))
        {
            // Try to access the player's Health component and force lethal damage
            if (collision.TryGetComponent<Health>(out Health playerHealth))
            {
                // Instantly drain remaining health to trigger death/respawn
                playerHealth.TakeDamage(999); 
            }
            else
            {
                // Fallback: If no health component, just snap them back to the checkpoint immediately
                if (collision.TryGetComponent<PlayerRespawn>(out PlayerRespawn respawnScript))
                {
                    respawnScript.TriggerRespawn();
                }
            }

            Debug.Log("Player fell into the abyss!");
        }
    }
}