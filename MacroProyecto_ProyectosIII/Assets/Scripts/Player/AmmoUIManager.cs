using TMPro;
using UnityEngine;

public class AmmoUIManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText;

    void Start()
    {
        UpdateAmmoUI();
    }

    public void UpdateAmmoUI()
    {
        int cantidadMunicion = 0;

        foreach (var item in Inventory.Instance.inventory)
        {
            if (item.id == -1 || item.cantidadItems <= 0) continue;

            var itemData = Inventory.Instance.data.ObjectsDataBase[item.id];

            if (item.id == 3 || itemData.clase == ItemsDataBase.Clase.Municion)
            {
                cantidadMunicion += item.cantidadItems;
            }
        }

        if (ammoText != null)
        {
            ammoText.text = "" + cantidadMunicion;
        }
    }
}
