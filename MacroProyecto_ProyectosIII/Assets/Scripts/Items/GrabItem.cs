using UnityEngine;

public class GrabItem : MonoBehaviour
{
    public int cantidad;
    public int ID;
    public Inventory inv;

    private bool pickedUp = false;

    void Start()
    {
        if (inv == null)
        {
            inv = FindFirstObjectByType<Inventory>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player"))
        {
            if (inv != null)
            {
                if (!inv.TieneEspacioEnInventario(ID, cantidad))
                {
                    inv.ShowMessage();
                    return;
                }

                pickedUp = true;
                inv.AddItem(ID, cantidad);
                Destroy(gameObject);
            }
        }
    }
}