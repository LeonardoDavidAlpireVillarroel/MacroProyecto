using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public TextMeshProUGUI shopingPoints;
    public GameObject[] vidas;

    void Update()
    {
        shopingPoints.text = GameManager.Instance.TotalPoints.ToString();
    }

    public void ActualizePoints(int totalPoints)
    {
        shopingPoints.text = totalPoints.ToString();
    }

    public void DesactivateLifes(int index)
    {
        vidas[index].SetActive(false);
    }

    public void ActivateLife(int index)
    {
        vidas[index].SetActive(true);
    }
}
