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
    public GameObject inventoryPanel;

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
        if (isOpen)
        {
            InventoryUpdate();

            pointerData = new PointerEventData(null);
            raycastResults = new List<RaycastResult>();

            Description = GameObject.Find("ItemDescription");
            if (Description == null)
            {
                return;
            }

            if (deletePoster != null)
            {
                deletePoster.gameObject.SetActive(false);
            }

            canvas = gameObject.transform;
            if (canvas == null)
            {
                return;
            }
        }        
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
    }

    private void Update()
    {
        Arrastrar();
    }
    void Arrastrar()
    {
        if (raycastResults == null)
        {
            raycastResults = new List<RaycastResult>();
        }

        if (Input.GetMouseButtonDown(1))
        {
            pointerData.position = Input.mousePosition;

            if (graphRay == null)
            {
                return;
            }

            if (raycastResults == null)
            {
                return;
            }

            graphRay.Raycast(pointerData, raycastResults);

            if (raycastResults.Count > 0)
            {
                Item itemComponent = raycastResults[0].gameObject.GetComponent<Item>();
                if (itemComponent != null)
                {
                    selectedObject = raycastResults[0].gameObject;
                    if (selectedObject == null)
                    {
                        return;
                    }

                    OSC = itemComponent.itemAmount;
                    OSID = itemComponent.ID;

                    exParent = selectedObject.transform.parent;

                    if (selectedObject != null && selectedObject.GetComponent<RectTransform>() != null)
                    {
                        selectedObject.transform.SetParent(canvas);
                    }
                    else
                    {
                        selectedObject = null;
                    }
                }
            }
        }

        if (selectedObject != null)
        {
            RectTransform selectedObjectRectTransform = selectedObject.GetComponent<RectTransform>();
            if (selectedObjectRectTransform != null)
            {
                selectedObjectRectTransform.localPosition = CanvasScreen(Input.mousePosition);
            }
        }

        if (selectedObject != null && Input.GetMouseButtonUp(1))
        {
            pointerData.position = Input.mousePosition;
            raycastResults.Clear();

            if (graphRay == null)
            {
                return;
            }

            graphRay.Raycast(pointerData, raycastResults);

            if (selectedObject != null)
            {
                selectedObject.transform.SetParent(exParent);

                if (raycastResults.Count > 0)
                {
                    foreach (var resultado in raycastResults)
                    {
                        if (resultado.gameObject == selectedObject) continue;

                        if (resultado.gameObject.CompareTag("Slot"))
                        {
                            if (resultado.gameObject.GetComponentInChildren<Item>() == null)
                            {
                                selectedObject.transform.SetParent(resultado.gameObject.transform);
                            }
                        }

                        if (resultado.gameObject.CompareTag("Item"))
                        {
                            Item resultItem = resultado.gameObject.GetComponentInChildren<Item>();
                            if (resultItem != null && resultItem.ID == selectedObject.GetComponent<Item>().ID && selectedObject.gameObject.GetComponent<Item>().itemAmount < 1)
                            {
                                resultItem.itemAmount += selectedObject.GetComponent<Item>().itemAmount;
                            }
                            else
                            {
                                selectedObject.transform.SetParent(resultado.gameObject.transform.parent);
                                resultado.gameObject.transform.SetParent(exParent);
                                resultado.gameObject.transform.localPosition = Vector3.zero;
                            }
                        }

                        if (resultado.gameObject.CompareTag("Eliminar"))
                        {
                            Item selectedItem = selectedObject.GetComponent<Item>();
                            if (selectedItem != null && selectedItem.itemAmount >= 1)
                            {
                                deletePoster.gameObject.SetActive(true);
                            }
                            else
                            {
                                deletePoster.gameObject.SetActive(false);
                                DeleteItem(selectedItem.ID, selectedItem.itemAmount);
                            }
                        }
                    }
                }

                if (selectedObject != null)
                {
                    selectedObject.transform.localPosition = Vector3.zero;
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
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == id && data.ObjectsDataBase[id].acumulable)
            {
                inventory[i] = new ObjectInventoryID(inventory[i].id, inventory[i].cantidadItems + cantidad);
                InventoryUpdate();
                return;
            }
        }

        if (!data.ObjectsDataBase[id].acumulable)
        {
            inventory.Add(new ObjectInventoryID(id, cantidad));
        }
        else
        {
            inventory.Add(new ObjectInventoryID(id, cantidad));
        }

        InventoryUpdate();
    }

    public void DeleteItem(int id, int cantidad)
    {
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
                itemsInventoryList[i].Button.onClick.RemoveAllListeners();
                itemsInventoryList[i].Button.onClick.AddListener(() => gameObject.SendMessage(data.ObjectsDataBase[o.id].Void, SendMessageOptions.DontRequireReceiver));
                itemsInventoryList[i].gameObject.SetActive(true);
            }
            else
            {
                itemsInventoryList[i].gameObject.SetActive(false);
                itemsInventoryList[i]._description.SetActive(false);
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
