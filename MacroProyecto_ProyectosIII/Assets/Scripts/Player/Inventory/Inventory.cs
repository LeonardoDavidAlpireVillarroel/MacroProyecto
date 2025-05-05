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

        InventoryUpdate();
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

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            pointerData.position = Input.mousePosition;
            graphRay.Raycast(pointerData, raycastResults);

            if (raycastResults.Count > 0)
            {
                Item itemComponent = raycastResults[0].gameObject.GetComponent<Item>();
                if (itemComponent != null)
                {
                    selectedObject = raycastResults[0].gameObject;
                    selectedObjectCantidad = itemComponent.itemAmount;
                    selectedObjectID = itemComponent.ID;

                    exParent = selectedObject.transform.parent;
                    selectedObject.transform.SetParent(canvas);
                    selectedObject.transform.SetAsLastSibling();

                    var cg = selectedObject.GetComponent<CanvasGroup>();
                    if (cg == null)
                        cg = selectedObject.AddComponent<CanvasGroup>();

                    cg.blocksRaycasts = false;
                }
            }
        }

        if (selectedObject != null)
        {
            selectedObject.GetComponent<RectTransform>().localPosition = CanvasScreen(Input.mousePosition);
        }

        if (selectedObject != null)
        {
            RectTransform rt = selectedObject.GetComponent<RectTransform>();
            if (rt != null)
                rt.position = Input.mousePosition;

            pointerData.position = Input.mousePosition;
            raycastResults.Clear();
            graphRay.Raycast(pointerData, raycastResults);

            bool hoveringOverEliminar = false;

            foreach (var result in raycastResults)
            {
                if (result.gameObject.CompareTag("Eliminar"))
                {
                    hoveringOverEliminar = true;
                    break;
                }
            }

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

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                pointerData.position = Input.mousePosition;
                raycastResults.Clear();
                graphRay.Raycast(pointerData, raycastResults);
                selectedObject.transform.SetParent(exParent);
                Transform newParent = exParent;

                if (raycastResults.Count > 0)
                {
                    foreach (var result in raycastResults)
                    {
                        if (result.gameObject == selectedObject)
                            continue;

                        if (result.gameObject.CompareTag("Slot"))
                        {
                            if (result.gameObject.GetComponentInChildren<Item>() == null)
                            {
                                newParent = result.gameObject.transform;
                            }
                        }

                        if (result.gameObject.CompareTag("Item"))
                        {
                            if (result.gameObject == selectedObject)
                                continue;

                            Item resultItem = result.gameObject.GetComponent<Item>();

                            if (resultItem != null)
                            {
                                if (resultItem.ID == selectedObjectID && data.ObjectsDataBase[selectedObjectID].acumulable)
                                {
                                    resultItem.itemAmount += selectedObjectCantidad;

                                    if (exParent != null)
                                    {
                                        Image exSlotImage = exParent.GetComponent<Image>();
                                        if (exSlotImage != null)
                                            exSlotImage.sprite = emptySlotSprite;
                                    }

                                    Destroy(selectedObject);
                                    selectedObject = null;

                                    for (int i = 0; i < inventory.Count; i++)
                                    {
                                        if (Contenido.GetChild(i) == exParent)
                                        {
                                            inventory[i] = new ObjectInventoryID(-1, 0);
                                            break;
                                        }
                                    }

                                    SincronizarInventarioDesdeUI();
                                    return;
                                }
                                else
                                {
                                    Transform parentA = selectedObject.transform.parent;
                                    Transform parentB = resultItem.transform.parent;

                                    selectedObject.transform.SetParent(parentB);
                                    selectedObject.transform.localPosition = Vector3.zero;

                                    resultItem.transform.SetParent(parentA);
                                    resultItem.transform.localPosition = Vector3.zero;

                                    CanvasGroup cgSelected = selectedObject.GetComponent<CanvasGroup>();
                                    if (cgSelected != null) cgSelected.blocksRaycasts = true;

                                    CanvasGroup cgResult = resultItem.GetComponent<CanvasGroup>();
                                    if (cgResult != null) cgResult.blocksRaycasts = true;

                                    SincronizarInventarioDesdeUI();

                                    selectedObject = null;
                                    return;
                                }
                            }
                        }

                        if (result.gameObject.CompareTag("Eliminar"))
                        {
                            Item selectedItem = selectedObject.GetComponent<Item>();
                            if (selectedItem != null && selectedItem.itemAmount > 0)
                            {
                                deletePoster.selectedItem = selectedObject;

                                deletePoster.EnableDeletePanel();
                                deletePoster.slider.value = 1;

                                selectedObject.transform.SetParent(deletePoster.transform);
                                selectedObject.transform.localPosition = Vector3.zero;

                                deletePoster.selectedItem = selectedObject;
                                deletePoster.originalParent = exParent;

                                deletePoster.itemAmountToRemove = selectedItem.itemAmount;

                                selectedObject = null;

                                return;
                            }
                        }

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

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == id)
            {
                inventory[i] = new ObjectInventoryID(id, inventory[i].cantidadItems + cantidad);
                InventoryUpdate();
                return;
            }
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == -1 || inventory[i].cantidadItems <= 0)
            {
                inventory[i] = new ObjectInventoryID(id, cantidad);
                InventoryUpdate();
                return;
            }
        }
    }

    public void DeleteItem(int id, int cantidad)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == id)
            {
                int nuevaCantidad = inventory[i].cantidadItems - cantidad;

                if (nuevaCantidad <= 0)
                {
                    inventory[i] = new ObjectInventoryID(-1, 0);
                }
                else
                {
                    inventory[i] = new ObjectInventoryID(inventory[i].id, nuevaCantidad);
                }

                InventoryUpdate();
                break;
            }
        }
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

            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = emptySlotSprite;
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

        SincronizarInventarioDesdeUI();
        InventoryUpdate();
    }

    public void SincronizarInventarioDesdeUI()
    {
        for (int i = 0; i < Contenido.childCount; i++)
        {
            Transform slot = Contenido.GetChild(i);
            Item itemUI = slot.GetComponentInChildren<Item>();

            if (itemUI != null && itemUI.itemAmount > 0 && itemUI.ID >= 0)
            {
                inventory[i] = new ObjectInventoryID(itemUI.ID, itemUI.itemAmount);
            }
            else
            {
                inventory[i] = new ObjectInventoryID(-1, 0);
            }
        }
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