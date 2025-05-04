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
    public bool isOpen;
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
    public int OSC;
    public int OSID;

    public Transform Contenido;
    public Item item;
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

        item._description = item.transform.Find("ItemDescription")?.gameObject;

        if (deletePoster != null)
        {
            deletePoster.gameObject.SetActive(false);
        }

        canvas = GetComponentInParent<Canvas>()?.transform;
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            GameManager.Instance.shootAction.Disable();
            GameManager.Instance.aimAction.Disable();

            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            InventoryUpdate();
        }
        else
        {
            GameManager.Instance.shootAction.Enable();
            GameManager.Instance.aimAction.Enable();

            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (isOpen) Arrastrar();
    }
    void Arrastrar()
    {
        if (raycastResults == null)
            raycastResults = new List<RaycastResult>();

        // Detectamos cuando se presiona el botón izquierdo del ratón
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
                    OSC = itemComponent.itemAmount;
                    OSID = itemComponent.ID;

                    exParent = selectedObject.transform.parent;
                    selectedObject.transform.SetParent(canvas);
                    selectedObject.transform.SetAsLastSibling();

                    var cg = selectedObject.GetComponent<CanvasGroup>();
                    if (cg == null)
                        cg = selectedObject.AddComponent<CanvasGroup>();

                    cg.blocksRaycasts = false;  // Desactivamos la interacción con el ítem durante el arrastre.
                }
            }
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

            // Solo mostramos el panel de eliminación cuando estamos sobre la zona de eliminar
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

                Transform newParent = exParent;

                if (raycastResults.Count > 0)
                {
                    foreach (var result in raycastResults)
                    {
                        // Si se suelta sobre un slot vacío
                        if (result.gameObject.CompareTag("Slot"))
                        {
                            // Si el slot está vacío, simplemente movemos el ítem allí
                            if (result.gameObject.GetComponentInChildren<Item>() == null)
                            {
                                newParent = result.gameObject.transform;
                            }
                        }

                        // Si se suelta sobre el área de eliminación
                        if (result.gameObject.CompareTag("Eliminar"))
                        {
                            Item selectedItem = selectedObject.GetComponent<Item>();
                            if (selectedItem != null && selectedItem.itemAmount > 0)
                            {
                                // Asignamos el ítem a selectedItem en DeletePoster
                                deletePoster.selectedItem = selectedObject;  // Aquí se hace la asignación

                                deletePoster.EnableDeletePanel();  // Llamamos al panel de eliminación
                                deletePoster.slider.value = 1;

                                // Colocamos el ítem en el panel de eliminar
                                selectedObject.transform.SetParent(deletePoster.transform);
                                selectedObject.transform.localPosition = Vector3.zero;

                                deletePoster.selectedItem = selectedObject;
                                deletePoster.originalParent = exParent;

                                // La cantidad que está a punto de ser eliminada
                                deletePoster.itemAmountToRemove = selectedItem.itemAmount;

                                selectedObject = null;  // Aseguramos que no quede un ítem seleccionado

                                return;
                            }
                        }
                    }
                }

                // Si no se suelta en un slot o en el área de eliminación, el ítem regresa a su lugar
                if (selectedObject != null)
                {
                    selectedObject.transform.SetParent(newParent);
                    selectedObject.transform.localPosition = Vector3.zero;

                    var cg = selectedObject.GetComponent<CanvasGroup>();
                    if (cg != null)
                        cg.blocksRaycasts = true;

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

        raycastResults.Clear();  // Limpiamos la lista de resultados de raycast
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
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == id && data.ObjectsDataBase[id].acumulable)
            {
                inventory[i] = new ObjectInventoryID(inventory[i].id, Mathf.Max(0, inventory[i].cantidadItems + cantidad));
                InventoryUpdate();
                return;
            }
        }

        inventory.Add(new ObjectInventoryID(id, cantidad));
        InventoryUpdate();
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
                    inventory.RemoveAt(i);
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

    List<Item> itemsInventoryList = new List<Item>();

    public void InventoryUpdate()
    {
        for (int i = 0; i < itemsInventoryList.Count; i++)
        {
            if (i < inventory.Count)
            {
                ObjectInventoryID o = inventory[i];
                itemsInventoryList[i].ID = o.id;
                itemsInventoryList[i].GetComponent<Image>().sprite = data.ObjectsDataBase[o.id].icon;
                itemsInventoryList[i].GetComponent<RectTransform>().localPosition = Vector3.zero;
                itemsInventoryList[i].itemAmount = o.cantidadItems;
                itemsInventoryList[i].gameObject.SetActive(true);
            }
            else
            {
                itemsInventoryList[i].gameObject.SetActive(false);
                if (itemsInventoryList[i]._description != null)
                {
                    itemsInventoryList[i]._description.SetActive(false);
                }
                itemsInventoryList[i].gameObject.transform.parent.GetComponent<Image>().sprite = emptySlotSprite;
            }
        }

        if (inventory.Count > itemsInventoryList.Count)
        {
            for (int i = itemsInventoryList.Count; i < inventory.Count; i++)
            {
                if (Contenido.GetChild(i).childCount == 0)
                {
                    Item it = Instantiate(item, Contenido.GetChild(i));
                    itemsInventoryList.Add(it);

                    if (Contenido.GetChild(0).childCount >= 2)
                    {
                        for (int s = 0; s < Contenido.childCount; s++)
                        {
                            if (Contenido.GetChild(s).childCount == 0)
                            {
                                it.transform.SetParent(Contenido.GetChild(s));
                                break;
                            }
                        }
                    }

                    it.transform.position = Vector3.zero;
                    it.transform.localScale = Vector3.one;

                    ObjectInventoryID o = inventory[i];
                    itemsInventoryList[i].ID = o.id;
                    itemsInventoryList[i].GetComponent<RectTransform>().localPosition = Vector3.zero;
                    itemsInventoryList[i].GetComponent<Image>().sprite = data.ObjectsDataBase[o.id].icon;
                    itemsInventoryList[i].itemAmount = o.cantidadItems;
                    itemsInventoryList[i].Button.onClick.RemoveAllListeners();
                    itemsInventoryList[i].Button.onClick.AddListener(() => gameObject.SendMessage(data.ObjectsDataBase[o.id].Void, SendMessageOptions.DontRequireReceiver));
                    itemsInventoryList[i].gameObject.SetActive(true);
                }
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