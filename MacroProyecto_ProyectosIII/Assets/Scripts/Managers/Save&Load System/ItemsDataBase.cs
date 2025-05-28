using UnityEngine;

[CreateAssetMenu(fileName = "BaseDatos", menuName = "Inventario/Lista", order = 1)]
public class ItemsDataBase : ScriptableObject
{
    [System.Serializable]
    public struct InventoryObject
    {
        public string nombre;
        public int ID;
        public Sprite icon;
        public int precioCompra;
        public int precioVenta;
        public Clase clase;
        public Type type;
        public bool acumulable;
        public string description;
        public string Void;
        public int cantidadInicialTienda;
        public int stackLimit;
        public int puntosAlRecoger;
    }

    public enum Clase
    {
        Pocion,
        Municion,
        Habilidad
    }

    public enum Type
    {
        consumable,
        equipable
    }

    public InventoryObject[] ObjectsDataBase;

    public InventoryObject? GetItemByID(int id)
    {
        foreach (var item in ObjectsDataBase)
        {
            if (item.ID == id)
                return item;
        }
        return null;
    }
}
