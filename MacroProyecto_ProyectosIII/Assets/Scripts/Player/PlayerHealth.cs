using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public void TakeDamage(int damage)
    {
        GameManager gm = GameManager.Instance;

        if (gm == null) return;

        if (!gm.isInvulnerable)
        {
            gm.health -= damage;

            if (gm.health <= 0)
            {
                gm.health = 0;
                gm.playerHUD.UpdateAllLifes(gm.health);
                gm.TriggerGameOver();
            }
            else
            {
                gm.playerHUD.UpdateAllLifes(gm.health);
            }
        }
    }
}
