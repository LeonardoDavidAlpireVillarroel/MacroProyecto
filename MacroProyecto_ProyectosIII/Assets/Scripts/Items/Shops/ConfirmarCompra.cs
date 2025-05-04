using UnityEngine;
using UnityEngine.UI;
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

    void Start()
    {
        
    }

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
            SM.ComprarItem(id, cantidad);
        }
        else
        {
            SM.VenderItem(id, cantidad);
        }
        SM.GetComponentInChildren<ConfirmarCompra>().gameObject.SetActive(false);
    }

    public void Cancelar()
    {
        SM.GetComponentInChildren<ConfirmarCompra>().gameObject.SetActive(false);
    }
}
