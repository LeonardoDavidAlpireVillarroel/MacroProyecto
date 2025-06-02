using UnityEngine;

public class EnemiTutorial : MonoBehaviour
{
    public int vida = 1;

    private TipoAtaque tipoDeMuerte = TipoAtaque.Ninguno;

    public void TakeDamage(int daño, TipoAtaque tipoAtaque)
    {
        vida -= daño;

        if (vida <= 0)
        {
            tipoDeMuerte = tipoAtaque;
            Die();
        }
    }

    void Die()
    {
        switch (tipoDeMuerte)
        {
            case TipoAtaque.Melee:
                TutorialManager.Instance.OnEnemyKilled(TipoAtaque.Melee);
                break;
            case TipoAtaque.Disparo:
                TutorialManager.Instance.OnEnemyKilled(TipoAtaque.Disparo);
                break;
        }

        Destroy(gameObject);
    }
}
