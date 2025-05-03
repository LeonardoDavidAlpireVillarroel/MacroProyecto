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
        public Type type;
        public bool acumulable;
        public string description;
        public string Void;
    }

    public enum Type
    {
        consumable,
        equipable
    }

    public InventoryObject[] ObjectsDataBase;
}
