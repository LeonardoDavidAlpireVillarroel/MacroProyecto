using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

        if (itemASerComprado == null) return;

        int precioTotal = itemASerComprado.precio * cantidad;

        if (GameManager.GetComponent<GameManager>().points >= precioTotal)
        {
            GameManager.GetComponent<GameManager>().points -= precioTotal;

            Inv.GetComponent<Inventory>().AddItem(ItemID, cantidad);

            itemASerComprado.cantidad -= cantidad;

            itemASerComprado.ActualizarItem();
        }
        else
        {
            insuficientesPuntos.SetActive(true);
        }
    }

    public void VenderItem(int index, int cantidad)
    {
        GameManager.GetComponent<GameManager>().points += DB.ObjectsDataBase[index].precioVenta * cantidad;

        Inv.GetComponent<Inventory>().VenderItem(index, cantidad);
    }

    public void VenderItemDesdeSlot(int slotIndex, int cantidad)
    {
        Inventory inv = Inv.GetComponent<Inventory>();
        var item = inv.inventory[slotIndex];

        if (item.id == -1 || item.cantidadItems < cantidad)
        {
            Debug.LogWarning("Slot vacío o cantidad inválida");
            return;
        }

        int ganancia = DB.ObjectsDataBase[item.id].precioVenta * cantidad;
        GameManager.GetComponent<GameManager>().points += ganancia;

        inv.VenderItemEnSlot(slotIndex, cantidad);
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
