using UnityEngine;
using TMPro;

public class ConfirmarCompra : MonoBehaviour
{
    [SerializeField]
    ItemsDataBase db;
    [SerializeField]
    ShopManager SM;
    [SerializeField]
    TextMeshProUGUI texto;
    public int id;
    public int cantidad;
    public bool compra = true;

    public int slotIndex = -1;

    void Update()
    {
        if (compra)
        {
            texto.text = "¿Comprar " + cantidad + " " + db.ObjectsDataBase[id].nombre + " por un valor de " + db.ObjectsDataBase[id].precioCompra * cantidad + "?";
        }
        else
        {
            texto.text = "¿Vender " + cantidad + " " + db.ObjectsDataBase[id].nombre + " por un valor de " + db.ObjectsDataBase[id].precioVenta * cantidad + "?";
        }
    }

    public void Aceptar()
    {
        if (compra)
        {
            Inventory inventory = GameManager.Instance.inventory;
            ItemsDataBase.InventoryObject itemData = db.ObjectsDataBase[id];

            int cantidadDisponible = cantidad;

            if (itemData.acumulable)
            {
                for (int i = 0; i < inventory.inventory.Count && cantidadDisponible > 0; i++)
                {
                    var slot = inventory.inventory[i];
                    if (slot.id == id && slot.cantidadItems < itemData.stackLimit)
                    {
                        int espacio = itemData.stackLimit - slot.cantidadItems;
                        cantidadDisponible -= Mathf.Min(espacio, cantidadDisponible);
                    }
                }
            }

            for (int i = 0; i < inventory.inventory.Count && cantidadDisponible > 0; i++)
            {
                var slot = inventory.inventory[i];
                if (slot.id == -1 || slot.cantidadItems == 0)
                {
                    int espacio = itemData.stackLimit;
                    cantidadDisponible -= Mathf.Min(espacio, cantidadDisponible);
                }
            }

            if (cantidadDisponible > 0)
            {
                inventory.ShowMessage();
                gameObject.SetActive(false);
                return;
            }

            SM.ComprarItem(id, cantidad);
        }
        else
        {
            if (slotIndex != -1)
            {
                SM.VenderItemDesdeSlot(slotIndex, cantidad);
            }
        }

        gameObject.SetActive(false);
    }

    public void Cancelar()
    {
        SM.GetComponentInChildren<ConfirmarCompra>().gameObject.SetActive(false);
    }
}
