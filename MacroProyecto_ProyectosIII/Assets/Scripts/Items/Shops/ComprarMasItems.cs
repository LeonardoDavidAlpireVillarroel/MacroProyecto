using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComprarMasItems : MonoBehaviour
{
    [SerializeField]
    ShopManager shop;
    public Slider slider;
    public TextMeshProUGUI Texto;
    public TextMeshProUGUI CantidadText;
    public bool compra = true;
    public int id;
    GameObject cartelConf;

    void Start()
    {
        cartelConf = shop.confCompra;
    }

    void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            CantidadText.text = slider.value.ToString();
        }

        if (compra)
        {
            Texto.text = "¿Cuánto quieres comprar?";
        }
        else
        {
            Texto.text = "¿Cuánto quieres vender?";
        }
    }

    public void Aceptar()
    {
        cartelConf.SetActive(true);
        cartelConf.GetComponent<ConfirmarCompra>().id = id;
        cartelConf.GetComponent<ConfirmarCompra>().cantidad = Mathf.RoundToInt(slider.value);
        slider.value = 1;
        if (compra)
        {
            cartelConf.GetComponent<ConfirmarCompra>().compra = true;
        }
        else
        {
            cartelConf.GetComponent<ConfirmarCompra>().compra = false;
        }
        this.gameObject.SetActive(false);
    }

    public void Cancelar()
    {
        slider.value = 1;
        this.gameObject.SetActive(false);
    }
}
