using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
using Unity.VisualScripting.Dependencies.NCalc;

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


        //if (GameManager.GetComponent<GameManager>().points >= ItemCompra[ItemID].precio * cantidad)
        //{
        //    GameManager.GetComponent<GameManager>().points -= ItemCompra[ItemID].precio * cantidad;
        //    if (ItemCompra[ItemID].acumulable)
        //    {
        //        Inv.GetComponent<Inventory>().AddItem(ItemID, cantidad);
        //    }
        //    else
        //    {
        //        for (int item = 0; item < cantidad; item++)
        //        {
        //            Inv.GetComponent<Inventory>().AddItem(ItemID, 1);
        //        }
        //    }
        //    ItemCompra[ItemID].cantidad -= cantidad;
        //}
        //else
        //{
        //    insuficientesPuntos.SetActive(true);
        //}
    }

    public void VenderItem(int index, int cantidad)
    {
        for (int i = 0; i < ItemCompra.Count; i++)
        {
            if (ItemCompra[i].ID >= index && ItemCompra[i].acumulable)
            {
                GameManager.GetComponent<GameManager>().points += DB.ObjectsDataBase[index].precioVenta * cantidad;
                Inv.GetComponent<Inventory>().VenderItem(index, cantidad);
                return;
            }
            if (!ItemCompra[i].gameObject.activeInHierarchy)
            {
                ItemCompra[i].ID = index;
                ItemCompra[i].cantidad = cantidad;
                ItemCompra[i].gameObject.SetActive(true);
                ItemCompra[i].ActualizarItem();
                GameManager.GetComponent<GameManager>().points += DB.ObjectsDataBase[index].precioVenta * cantidad;
                Inv.GetComponent<Inventory>().VenderItem(index, cantidad);
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
