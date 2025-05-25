using UnityEngine;

public class GrabItem : MonoBehaviour
{
    public int cantidad;
    public int ID;
    public Inventory inv;
    public ItemsDataBase itemDB;

    private bool pickedUp = false;

    void Start()
    {
        if (inv == null)
        {
            inv = Inventory.Instance;
        }

        if (itemDB == null)
            itemDB = Resources.Load<ItemsDataBase>("ObjectsDataBase");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player"))
        {
            if (inv == null || itemDB == null) return;

            if (!inv.TieneEspacioEnInventario(ID, cantidad))
            {
                inv.ShowMessage();
                return;
            }

            pickedUp = true;

            inv.AddItem(ID, cantidad);

            LevelController nivel = FindFirstObjectByType<LevelController>();
            if (nivel != null)
            {
                nivel.IncrementarItemsRecolectados(ID);
                nivel.AddCollectedItem(gameObject);
            }

            gameObject.SetActive(false);
        }
    }
}