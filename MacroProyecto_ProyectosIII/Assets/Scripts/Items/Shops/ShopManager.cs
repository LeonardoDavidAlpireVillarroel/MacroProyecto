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

    [Header("Actualize Shop")]
    public TextMeshProUGUI tiendaTimerText;
    private float tiempoParaActualizar = 25f;
    private float temporizador = 0f;

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

        tiendaTimerText.text = FormatearTiempo(tiempoParaActualizar);
    }

    void Update()
    {
        cartelPuntos.text = "Puntos: " + GameManager.GetComponent<GameManager>().points.ToString();

        temporizador += Time.deltaTime;

        if (temporizador >= tiempoParaActualizar)
        {
            ActualizarTienda();
            temporizador = 0f;
        }
        else
        {
            tiendaTimerText.text = FormatearTiempo(tiempoParaActualizar - temporizador);
        }
    }

   public void ComprarItem(int ItemID, int cantidad)
   {
        ItemShop itemASerComprado = ItemCompra.Find(item => item.ID == ItemID);

        if (itemASerComprado == null)
        {
            Debug.LogWarning("Item no encontrado en la tienda.");
            return;
        }

        if (GameManager.GetComponent<GameManager>().points >= itemASerComprado.precio * cantidad)
        {
            GameManager.GetComponent<GameManager>().points -= itemASerComprado.precio * cantidad;

            if (itemASerComprado.acumulable)
            {
                Inv.GetComponent<Inventory>().AddItem(ItemID, cantidad);
            }
            else
            {
                for (int i = 0; i < cantidad; i++)
                {
                    Inv.GetComponent<Inventory>().AddItem(ItemID, 1);
                }
            }

            itemASerComprado.cantidad -= cantidad;
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
            if (ItemCompra[i].ID == ItemID)
            {
                GameManager.GetComponent<GameManager>().points += DB.ObjectsDataBase[ItemID].precioVenta * cantidad;

                Inv.GetComponent<Inventory>().DeleteItem(ItemID, cantidad);

                ItemCompra[i].cantidad -= cantidad;

                if (ItemCompra[i].cantidad <= 0)
                {
                    ItemCompra[i].gameObject.SetActive(false);
                    ItemCompra.RemoveAt(i);
                }

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

    private void ActualizarTienda()
    {
        foreach (var item in ItemCompra)
        {
            if (item.ID != 0)
            {
                item.cantidad = DB.ObjectsDataBase[item.ID].cantidadInicialTienda;
                item.ActualizarItem();
                item.gameObject.SetActive(true);
            }
        }
    }
    string FormatearTiempo(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }
}
