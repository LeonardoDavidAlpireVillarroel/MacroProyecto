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
            inv = Inventory.Instance;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player"))
        {
            if (inv != null)
            {
                pickedUp = true;
                inv.AddItem(ID, cantidad);
                Destroy(gameObject);
            }
        }
    }
}
