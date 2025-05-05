using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ShopManager : MonoBehaviour
{
    public GameObject GameManager;
    public GameObject Inv;
    [SerializeField]
    private List<ItemShop> ItemCompra;
    private List<ItemShop> itDesactivar = new List<ItemShop>();
    private List<ItemShop> itActivar = new List<ItemShop>();
    private List<ItemShop> itemsVendidos = new List<ItemShop>();
    [SerializeField]
    private ItemsDataBase DB;
    public GameObject ContenedorItems;
    [Header("Carteles")]
    public TextMeshProUGUI cartelPuntos;
    public GameObject insuficientesPuntos;
    public GameObject confCompra;
    public GameObject comprarMas;

    void Start()
    {
        ItemCompra = new List<ItemShop>();
        for (int i = 0; i < ContenedorItems.transform.childCount; i++)
        {
            ItemCompra.Add(ContenedorItems.transform.transform.GetChild(i).GetComponent<ItemShop>());
        }

        confCompra.SetActive(false);
        comprarMas.SetActive(false);
        insuficientesPuntos.SetActive(false);
    }

    void Update()
    {
        cartelPuntos.text = "Puntos: " + GameManager.GetComponent<GameManager>().points.ToString();
    }

   public void ComprarItem(int ItemID, int cantidad)
   {
        if (GameManager.GetComponent<GameManager>().points >= ItemCompra[ItemID].precio * cantidad)
        {
            GameManager.GetComponent<GameManager>().points -= ItemCompra[ItemID].precio * cantidad;
            if (ItemCompra[ItemID].acumulable)
            {
                Inv.GetComponent<Inventory>().AddItem(ItemID, cantidad);
            }
            else
            {
                for (int item = 0; item < cantidad; item++)
                {
                    Inv.GetComponent<Inventory>().AddItem(ItemID, 1);
                }
            }

            ItemCompra[ItemID].cantidad -= cantidad;
        }
        else
        {
            insuficientesPuntos.SetActive(true);
        }
   }

    public void VenderItem(int ItemID, int cantidad)
    {
        for (int i = 0; i < ItemCompra.Count; i++)
        {
            if (ItemCompra[i].ID == ItemID && ItemCompra[i].acumulable)
            {
                GameManager.GetComponent<GameManager>().points += DB.ObjectsDataBase[ItemID].precioVenta * cantidad;
                itemsVendidos.Add(ItemCompra[i]);
                ItemCompra[i].cantidad += cantidad;
                Inv.GetComponent<Inventory>().DeleteItem(ItemID, cantidad);
                return;
            }
            if (!ItemCompra[i].gameObject.activeInHierarchy)
            {
                ItemCompra[i].ID = ItemID;
                ItemCompra[i].cantidad = cantidad;
                ItemCompra[i].gameObject.SetActive(true);
                ItemCompra[i].ActualizarItem();
                itemsVendidos.Add(ItemCompra[i]);
                GameManager.GetComponent<GameManager>().points += DB.ObjectsDataBase[ItemID].precioVenta * cantidad;
                Inv.GetComponent<Inventory>().DeleteItem(ItemID, cantidad);
                break;
            }
        }
    }

    public void EsconderItems(int caso)
    {

    }

    void ActivacionItems(int numero)
    {
        
    }

    void DesactivacionItems(int numero)
    {

    }
}
