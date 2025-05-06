using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using System.Reflection;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public bool isInventoryOpen;
    private CanvasGroup cg;

    [System.Serializable]
    public struct ObjectInventoryID
    {
        public int id;
        public int cantidadItems;

        public ObjectInventoryID(int id, int cantidadItems)
        {
            this.id = id;
            this.cantidadItems = cantidadItems;
        }
    }

    public PlayerController playerController;

    [SerializeField]
    ItemsDataBase data;

    [Header("Variables del Drag and Drop")]
    public GraphicRaycaster graphRay;
    private PointerEventData pointerData;
    private List<RaycastResult> raycastResults;
    public static Transform canvas;
    public GameObject selectedObject;
    public Transform exParent;
    public Sprite emptySlotSprite;

    [Header("Prefs y Items")]
    public static GameObject Description;
    public DeletePoster deletePoster;
    public int selectedObjectCantidad;
    public int selectedObjectID;

    public Transform Contenido;
    public Item item;
    //[HideInInspector]
    //public List<ItemSuelto> itemsSueltos = new List<ItemSuelto>;
    //[HideInInspector]
    //public List<ItemSuelto> copiasItemsSueltos = new List<ItemSuelto>;
    //[Space]
    //[Header("Items Soltados")]
    //[Tooltip("Aqui arrastra un GameObject vacío en donde reaparecen los items eliminados del inventario")]
    //public Transform ItemSueltoRespawn;
    //[HideInInspector]
    //public Vector3 originalPos;

    public ShopManager shopManager;

    public List<ObjectInventoryID> inventory = new List<ObjectInventoryID>();

    public static bool InventoryIsOpen { get; private set; }

    private void OnEnable()
    {
        InventoryIsOpen = true;
    }
    private void OnDisable()
    {
        InventoryIsOpen = false;
    }

    private void Start()
    {
        cg = GameManager.Instance.inventoryUIPanel.GetComponent<CanvasGroup>();

        pointerData = new PointerEventData(EventSystem.current);
        raycastResults = new List<RaycastResult>();

        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        Description = GameObject.Find("ItemDescription");
        Description.gameObject.SetActive(false);

        if (deletePoster != null)
        {
            deletePoster.gameObject.SetActive(false);
        }

        canvas = GetComponentInParent<Canvas>()?.transform;

        if (inventory.Count < Contenido.childCount)
        {
            int diff = Contenido.childCount - inventory.Count;
            for (int i = 0; i < diff; i++)
            {
                inventory.Add(new ObjectInventoryID(-1, 0));
            }
        }

        bool tieneDatos = false;
        foreach (var obj in inventory)
        {
            if (obj.id != -1 && obj.cantidadItems > 0)
            {
                tieneDatos = true;
                break;
            }
        }

        if (tieneDatos)
        {
            InventoryUpdate();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (isInventoryOpen)
        {
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            InventoryUpdate();
        }
        else
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            if (!GameManager.Instance.shopScript.shopCanvasGroup ||
                GameManager.Instance.shopScript.shopCanvasGroup.alpha == 0f)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void Update()
    {
        if (isInventoryOpen == true) Arrastrar();
    }
    void Arrastrar()
    {
        if (raycastResults == null)
            raycastResults = new List<RaycastResult>();

        // Detectar cuando el botón izquierdo del ratón es presionado
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            pointerData.position = Input.mousePosition;
            graphRay.Raycast(pointerData, raycastResults);

            if (raycastResults.Count > 0)
            {
                Item itemComponent = raycastResults[0].gameObject.GetComponent<Item>();
                if (itemComponent != null)
                {
                    // Seleccionar el objeto para moverlo
                    selectedObject = raycastResults[0].gameObject;
                    selectedObjectCantidad = itemComponent.itemAmount;
                    selectedObjectID = itemComponent.ID;

                    // Guardar el parent original del objeto
                    exParent = selectedObject.transform.parent;
                    selectedObject.transform.SetParent(canvas);
                    selectedObject.transform.SetAsLastSibling();

                    // Desactivar la interacción con el objeto mientras se arrastra
                    var cg = selectedObject.GetComponent<CanvasGroup>();
                    if (cg == null)
                        cg = selectedObject.AddComponent<CanvasGroup>();

                    cg.blocksRaycasts = false;
                }
            }
        }

        // Mover el objeto con el cursor
        if (selectedObject != null)
        {
            selectedObject.GetComponent<RectTransform>().localPosition = CanvasScreen(Input.mousePosition);
        }

        // Verificar la interacción con los objetos en el área de arrastre
        if (selectedObject != null)
        {
            pointerData.position = Input.mousePosition;
            raycastResults.Clear();
            graphRay.Raycast(pointerData, raycastResults);

            bool hoveringOverEliminar = false;

            // Verificar si estamos sobre el área de eliminación
            foreach (var result in raycastResults)
            {
                if (result.gameObject.CompareTag("Eliminar"))
                {
                    hoveringOverEliminar = true;
                    break;
                }
            }

            // Actualizar la visibilidad de la UI de eliminación
            if (deletePoster != null)
            {
                CanvasGroup dpCg = deletePoster.GetComponent<CanvasGroup>();
                if (dpCg != null)
                {
                    dpCg.alpha = hoveringOverEliminar ? 1 : 0;
                    dpCg.blocksRaycasts = hoveringOverEliminar;
                    dpCg.interactable = hoveringOverEliminar;
                }
                else
                {
                    deletePoster.gameObject.SetActive(hoveringOverEliminar);
                }
            }

            // Si el botón izquierdo es liberado
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                pointerData.position = Input.mousePosition;
                raycastResults.Clear();
                graphRay.Raycast(pointerData, raycastResults);

                // Restaurar el parent original
                selectedObject.transform.SetParent(exParent);
                Transform newParent = exParent;

                if (raycastResults.Count > 0)
                {
                    foreach (var result in raycastResults)
                    {
                        if (result.gameObject == selectedObject)
                            continue;

                        // Verificar si el objeto está siendo soltado sobre un slot vacío
                        if (result.gameObject.CompareTag("Slot"))
                        {
                            if (result.gameObject.GetComponentInChildren<Item>() == null)
                            {
                                newParent = result.gameObject.transform;
                            }
                        }

                        // Verificar si el objeto está siendo soltado sobre otro ítem
                        if (result.gameObject.CompareTag("Item"))
                        {
                            if (result.gameObject == selectedObject)
                                continue;

                            Item resultItem = result.gameObject.GetComponent<Item>();

                            if (resultItem != null)
                            {
                                if (resultItem.ID == selectedObjectID && data.ObjectsDataBase[selectedObjectID].acumulable)
                                {
                                    int stackLimit = data.ObjectsDataBase[selectedObjectID].stackLimit;
                                    int totalAmount = resultItem.itemAmount + selectedObjectCantidad;

                                    if (totalAmount <= stackLimit)
                                    {
                                        // Si no se excede el límite, combinar ítems
                                        resultItem.itemAmount = totalAmount;

                                        int resultIndex = resultItem.transform.parent.GetSiblingIndex();
                                        inventory[resultIndex] = new ObjectInventoryID(selectedObjectID, totalAmount);

                                        // Limpiar el slot original
                                        if (exParent != null)
                                        {
                                            Image exSlotImage = exParent.GetComponent<Image>();
                                            if (exSlotImage != null)
                                                exSlotImage.sprite = emptySlotSprite;
                                        }

                                        int selectedIndex = exParent.GetSiblingIndex();
                                        inventory[selectedIndex] = new ObjectInventoryID(-1, 0);

                                        Destroy(selectedObject);
                                        selectedObject = null;

                                        InventoryUpdate();
                                        return;
                                    }
                                    else
                                    {
                                        int availableSpace = stackLimit - resultItem.itemAmount;

                                        if (availableSpace > 0)
                                        {
                                            // Llenar espacio disponible en el slot
                                            resultItem.itemAmount += availableSpace;

                                            // Actualizar el inventario
                                            int resultIndex = resultItem.transform.parent.GetSiblingIndex();
                                            inventory[resultIndex] = new ObjectInventoryID(selectedObjectID, resultItem.itemAmount);

                                            int selectedIndex = exParent.GetSiblingIndex();
                                            ObjectInventoryID selectedSlot = inventory[selectedIndex];

                                            if (availableSpace >= selectedSlot.cantidadItems)
                                            {
                                                inventory[selectedIndex] = new ObjectInventoryID(-1, 0);
                                                Destroy(selectedObject); // Eliminar el objeto visual
                                            }
                                            else
                                            {
                                                inventory[selectedIndex] = new ObjectInventoryID(selectedSlot.id, selectedSlot.cantidadItems - availableSpace);
                                            }

                                            selectedObject = null;
                                            InventoryUpdate();
                                            return;
                                        }
                                        else
                                        {
                                            // Si no hay espacio, devolver al slot original
                                            selectedObject.transform.SetParent(exParent);
                                            selectedObject.transform.localPosition = Vector3.zero;

                                            var cg = selectedObject.GetComponent<CanvasGroup>();
                                            if (cg != null)
                                                cg.blocksRaycasts = true;

                                            selectedObject = null;
                                            InventoryUpdate();
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    // Si no son iguales, intercambiar los ítems
                                    IntercambiarItems(selectedObject.GetComponent<Item>(), resultItem);

                                    int slot1 = selectedObject.transform.parent.GetSiblingIndex();
                                    int slot2 = resultItem.transform.parent.GetSiblingIndex();

                                    ObjectInventoryID temp = inventory[slot1];
                                    inventory[slot1] = inventory[slot2];
                                    inventory[slot2] = temp;

                                    selectedObject = null;
                                    InventoryUpdate();
                                    return;
                                }
                            }
                        }

                        // Verificar si el objeto está siendo soltado sobre el área de eliminación
                        if (result.gameObject.CompareTag("Eliminar"))
                        {
                            Item selectedItem = selectedObject.GetComponent<Item>();
                            if (selectedItem != null && selectedItem.itemAmount > 0)
                            {
                                deletePoster.selectedItem = selectedObject;
                                deletePoster.originalParent = exParent;
                                deletePoster.EnableDeletePanel();
                                selectedObject = null;
                                return;
                            }
                        }

                        // Verificar si el objeto está siendo soltado sobre la tienda
                        if (result.gameObject.CompareTag("Shop"))
                        {
                            if (selectedObject.GetComponent<Item>().itemAmount >= 2)
                            {
                                shopManager.comprarMas.SetActive(true);
                                shopManager.comprarMas.GetComponent<ComprarMasItems>().id = selectedObject.gameObject.GetComponent<Item>().ID;
                                shopManager.comprarMas.GetComponent<ComprarMasItems>().slider.maxValue = selectedObject.gameObject.GetComponent<Item>().itemAmount;
                                shopManager.comprarMas.GetComponent<ComprarMasItems>().compra = false;
                            }
                            else
                            {
                                shopManager.confCompra.SetActive(true);
                                shopManager.confCompra.GetComponent<ConfirmarCompra>().id = selectedObject.gameObject.GetComponent<Item>().ID;
                                shopManager.confCompra.GetComponent<ConfirmarCompra>().cantidad = selectedObject.gameObject.GetComponent<Item>().itemAmount;
                                shopManager.confCompra.GetComponent<ConfirmarCompra>().compra = false;
                            }
                        }
                    }
                }

                // Si no se realizó ninguna acción, devolver al slot original
                if (selectedObject != null)
                {
                    var cg = selectedObject.GetComponent<CanvasGroup>();
                    if (cg != null)
                        cg.blocksRaycasts = true;

                    selectedObject.transform.SetParent(newParent);
                    selectedObject.transform.localPosition = Vector3.zero;

                    if (exParent != null && exParent.childCount == 0)
                    {
                        Image exSlotImage = exParent.GetComponent<Image>();
                        if (exSlotImage != null)
                            exSlotImage.sprite = emptySlotSprite;
                    }

                    selectedObject = null;
                }
            }
        }

        // Limpiar los resultados del raycast
        raycastResults.Clear();
    }

    public Vector2 CanvasScreen(Vector2 screenPos)
    {
        if (Camera.main == null)
        {
            return Vector2.zero;
        }
        Vector2 viewportPoint = Camera.main.ScreenToViewportPoint(screenPos);
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
        return new Vector2(viewportPoint.x * canvasSize.x, viewportPoint.y * canvasSize.y) - (canvasSize / 2);
    }

    public void AddItem(int id, int cantidad)
    {
        if (id == -1 || cantidad <= 0) return;

        var itemData = data.ObjectsDataBase[id];

        if (itemData.acumulable)
        {
            for (int i = 0; i < inventory.Count && cantidad > 0; i++)
            {
                if (inventory[i].id == id && inventory[i].cantidadItems < itemData.stackLimit)
                {
                    int espacio = itemData.stackLimit - inventory[i].cantidadItems;
                    int aAgregar = Mathf.Min(espacio, cantidad);
                    inventory[i] = new ObjectInventoryID(id, inventory[i].cantidadItems + aAgregar);
                    cantidad -= aAgregar;
                }
            }
        }

        for (int i = 0; i < inventory.Count && cantidad > 0; i++)
        {
            if (inventory[i].id == -1 || inventory[i].cantidadItems == 0)
            {
                int cantidadAAgregar = Mathf.Min(itemData.stackLimit, cantidad);
                inventory[i] = new ObjectInventoryID(id, cantidadAAgregar);
                cantidad -= cantidadAAgregar;
            }
        }

        if (cantidad > 0)
        {
            Debug.LogWarning("Inventario lleno o sin espacio suficiente para más items.");
        }

        InventoryUpdate();
    }

    public void VenderItem(int id, int cantidad)
    {
        //if (index < 0 || index >= inventory.Count)
        //    return;

        //var slot = inventory[index];

        //if (slot.id == -1 || slot.cantidadItems <= 0 || cantidad <= 0)
        //    return;

        //int actualCantidad = slot.cantidadItems;
        //int id = slot.id;

        //if (cantidad < actualCantidad)
        //{
        //    inventory[index] = new ObjectInventoryID(id, actualCantidad - cantidad);
        //}
        //else
        //{
        //    inventory[index] = new ObjectInventoryID(-1, 0);
        //}

        //int puntosAGanar = data.ObjectsDataBase[id].precioVenta * cantidad;
        //GameManager.Instance.points += puntosAGanar;
        //InventoryUpdate();

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == id)
            {
                inventory[i] = new ObjectInventoryID(inventory[i].id, inventory[i].cantidadItems - cantidad);
                if (inventory[i].cantidadItems <= 0)
                {
                    inventory.Remove(inventory[i]);
                    InventoryUpdate();
                    break;
                }
            }
            InventoryUpdate();
        }
    }

    public void DeleteItem(int index, int cantidad)
    {
        if (index < 0 || index >= inventory.Count)
            return;

        var slot = inventory[index];

        if (slot.id == -1 || slot.cantidadItems <= 0 || cantidad <= 0)
            return;

        int actualCantidad = slot.cantidadItems;

        if (cantidad >= actualCantidad)
        {
            inventory[index] = new ObjectInventoryID(-1, 0);
        }
        else
        {
            inventory[index] = new ObjectInventoryID(slot.id, actualCantidad - cantidad);
        }
        InventoryUpdate();
    }

    public void InventoryUpdate()
    {
        for (int i = 0; i < inventory.Count && i < Contenido.childCount; i++)
        {
            Transform slot = Contenido.GetChild(i);

            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }

            if (inventory[i].id != -1 && inventory[i].cantidadItems > 0)
            {
                Item itemUI = Instantiate(item, slot);
                itemUI.ID = inventory[i].id;
                itemUI.itemAmount = inventory[i].cantidadItems;
                itemUI.GetComponent<Image>().sprite = data.ObjectsDataBase[itemUI.ID].icon;
                itemUI.transform.localPosition = Vector3.zero;
                itemUI.transform.localScale = Vector3.one;
                itemUI.gameObject.SetActive(true);
            }
        }
    }

    public void IntercambiarItems(Item item1, Item item2)
    {
        Transform parent1 = item1.transform.parent;
        Transform parent2 = item2.transform.parent;

        Vector3 pos1 = item1.transform.localPosition;
        Vector3 pos2 = item2.transform.localPosition;

        item1.transform.SetParent(parent2);
        item1.transform.localPosition = pos2;

        item2.transform.SetParent(parent1);
        item2.transform.localPosition = pos1;

        InventoryUpdate();
    }

    void PocionVida()
    {
        GameManager.Instance.health += 1;
        DeleteItem(0, 1);
    }

    void PocionFuerza()
    {
        GameManager.Instance.fuerza += 1;
        DeleteItem(1, 1);
    }
}