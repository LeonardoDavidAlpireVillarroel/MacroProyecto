using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class ItemShop : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemsDataBase DB;
    public ShopManager SM;
    public int clase;
    public int ID;
    public int precio;
    public int cantidad;
    public bool acumulable;
    private Image icon;
    public TextMeshProUGUI precioText;
    public TextMeshProUGUI cantidadText;
    public GameObject _descripcion;
    private TextMeshProUGUI Nombre_;
    private TextMeshProUGUI Dato_;
    public Vector3 offset;
    GameObject cartelConf;
    GameObject compraMas;

    void Start()
    {
        Nombre_ = _descripcion.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Dato_ = _descripcion.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _descripcion.SetActive(false);

        acumulable = DB.ObjectsDataBase[ID].acumulable;

        switch  (DB.ObjectsDataBase[ID].clase)
        {
            case ItemsDataBase.Clase.Pocion:
                clase = 1;
                break;
            case ItemsDataBase.Clase.Municion:
                clase = 2;
                break;
            case ItemsDataBase.Clase.Habilidad:
                clase = 3;
                break;
            default:
                clase = 0;
                break;
        }

        cartelConf = SM.confCompra;
        compraMas = SM.comprarMas;
        precio = DB.ObjectsDataBase[ID].precioCompra;
        icon = GetComponent<Image>();
        icon.sprite = DB.ObjectsDataBase[ID].icon;
        precioText.text = precio.ToString();

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (cantidad <= 1)
        {
            cartelConf.SetActive(true);
            cartelConf.GetComponent<ConfirmarCompra>().id = ID;
            cartelConf.GetComponent<ConfirmarCompra>().cantidad = cantidad;
            cartelConf.GetComponent<ConfirmarCompra>().compra = true;
        }
        else
        {
            compraMas.SetActive(true);
            compraMas.GetComponent<ComprarMasItems>().id = ID;
            compraMas.GetComponent<ComprarMasItems>().slider.maxValue = cantidad;
            compraMas.GetComponent<ComprarMasItems>().compra = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ID != 0)
        {
            _descripcion.SetActive(true);
            Nombre_.text = DB.ObjectsDataBase[ID].nombre;
            Dato_.text = DB.ObjectsDataBase[ID].description;
            _descripcion.transform.position = transform.position + offset;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descripcion.SetActive(false);
    }

    void Update()
    {
        if (transform.parent == Inventory.canvas)
        {
            _descripcion.SetActive(false);
        }

        if (ID == 0)
        {
            icon.enabled = false;
            precioText.enabled = false;
            cantidadText.enabled = false;   
        }
        else
        {
            icon.enabled = true;
            precioText.enabled = true;
            cantidadText.enabled = true;
        }

        if (cantidad <= 0)
        {
            cantidad = 0;
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
            cantidadText.text = cantidad.ToString();
        }
    }

    public void ActualizarItem()
    {
        switch (DB.ObjectsDataBase[ID].clase)
        {
            case ItemsDataBase.Clase.Pocion:
                clase = 1;
                break;
            case ItemsDataBase.Clase.Municion:
                clase = 2;
                break;
            case ItemsDataBase.Clase.Habilidad:
                clase = 3;
                break;
            default:
                clase = 0;
                break;
        }

        acumulable = DB.ObjectsDataBase[ID].acumulable;
        precio = DB.ObjectsDataBase[ID].precioCompra;
        icon = GetComponent<Image>();
        icon.sprite = DB.ObjectsDataBase[ID].icon;
        precioText = GetComponentInChildren<TextMeshProUGUI>();
        precioText.text = precio.ToString();
    }
}
