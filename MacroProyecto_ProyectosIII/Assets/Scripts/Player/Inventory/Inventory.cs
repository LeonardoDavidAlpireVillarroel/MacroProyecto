using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;

[System.Serializable]
public class ObjectInventoryID
{
    public int id;
    public int cantidadItems;

    public ObjectInventoryID(int id, int cantidadItems)
    {
        this.id = id;
        this.cantidadItems = cantidadItems;
    }
}

[System.Serializable]
public class InventoryWrapper
{
    public List<ObjectInventoryID> items;
}

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public bool isInventoryOpen;
    private CanvasGroup cg;

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

    [Header("PopUp Inventario lleno")]
    public CanvasGroup messagePanel;
    public float showDuration = 2f;
    public float fadeDuration = 0.5f;

    private Coroutine currentRoutine;

    [Header("Contenido Inventario")]
    public Transform Contenido;
    public Item item;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetInventoryAsString()
    {
        InventoryWrapper wrapper = new InventoryWrapper { items = inventory };
        return JsonUtility.ToJson(wrapper);
    }

    public void SetInventoryFromString(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        InventoryWrapper wrapper = JsonUtility.FromJson<InventoryWrapper>(json);
        if (wrapper != null && wrapper.items != null)
        {
            inventory = wrapper.items;
        }
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
            GameManager.Instance.playerController.playerInput.actions.FindActionMap("UI").Enable();
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            InventoryUpdate();
        }
        else
        {
            GameManager.Instance.playerController.playerInput.actions.FindActionMap("UI").Disable();
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (isInventoryOpen)
        {
            Arrastrar();
        }
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
                                    int stackLimit = data.ObjectsDataBase[selectedObjectID].stackLimit;
                                    int totalAmount = resultItem.itemAmount + selectedObjectCantidad;

                                    if (totalAmount <= stackLimit)
                                    {
                                        resultItem.itemAmount = totalAmount;

                                        int resultIndex = resultItem.transform.parent.GetSiblingIndex();
                                        inventory[resultIndex] = new ObjectInventoryID(selectedObjectID, totalAmount);

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
                                            resultItem.itemAmount += availableSpace;

                                            int resultIndex = resultItem.transform.parent.GetSiblingIndex();
                                            inventory[resultIndex] = new ObjectInventoryID(selectedObjectID, resultItem.itemAmount);

                                            int selectedIndex = exParent.GetSiblingIndex();
                                            ObjectInventoryID selectedSlot = inventory[selectedIndex];

                                            if (availableSpace >= selectedSlot.cantidadItems)
                                            {
                                                inventory[selectedIndex] = new ObjectInventoryID(-1, 0);
                                                Destroy(selectedObject);
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

                        if (result.gameObject.CompareTag("Shop"))
                        {
                            if (selectedObject.GetComponent<Item>().itemAmount >= 2)
                            {
                                shopManager.confCompra.GetComponent<ConfirmarCompra>().slotIndex = selectedObject.transform.parent.GetSiblingIndex();
                                shopManager.comprarMas.SetActive(true);
                                shopManager.comprarMas.GetComponent<ComprarMasItems>().id = selectedObject.gameObject.GetComponent<Item>().ID;
                                shopManager.comprarMas.GetComponent<ComprarMasItems>().slider.maxValue = selectedObject.gameObject.GetComponent<Item>().itemAmount;
                                shopManager.comprarMas.GetComponent<ComprarMasItems>().compra = false;
                            }
                            else
                            {
                                shopManager.confCompra.GetComponent<ConfirmarCompra>().slotIndex = selectedObject.transform.parent.GetSiblingIndex();
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
            FindFirstObjectByType<InventoryFullMessage>().ShowMessage();
        }

        InventoryUpdate();
    }

    public void ShowMessage()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowAndFade());
    }

    private IEnumerator ShowAndFade()
    {
        yield return StartCoroutine(FadeCanvasGroup(messagePanel, 0, 1, fadeDuration));

        yield return new WaitForSeconds(showDuration);

        yield return StartCoroutine(FadeCanvasGroup(messagePanel, 1, 0, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }

    public void VenderItem(int id, int cantidad)
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
                InventoryUpdate();
            }
        }
    }

    public void VenderItemEnSlot(int slotIndex, int cantidad)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Count)
            return;

        var slot = inventory[slotIndex];

        if (slot.id == -1 || slot.cantidadItems < cantidad)
        {
            Debug.LogWarning("Cantidad inválida para vender.");
            return;
        }

        inventory[slotIndex] = new ObjectInventoryID(slot.id, slot.cantidadItems - cantidad);

        if (inventory[slotIndex].cantidadItems <= 0)
        {
            inventory[slotIndex] = new ObjectInventoryID(-1, 0);
        }

        InventoryUpdate();
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
            Image slotImage = slot.GetComponent<Image>();

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
            else
            {
                if (slotImage != null)
                {
                    slotImage.sprite = emptySlotSprite;
                }
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

    public bool TieneEspacioEnInventario(int id, int cantidad)
    {
        var itemData = data.ObjectsDataBase[id];
        int cantidadDisponible = cantidad;

        if (itemData.acumulable)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].id == id && inventory[i].cantidadItems < itemData.stackLimit)
                {
                    int espacio = itemData.stackLimit - inventory[i].cantidadItems;
                    cantidadDisponible -= espacio;

                    if (cantidadDisponible <= 0)
                        return true;
                }
            }
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == -1 || inventory[i].cantidadItems == 0)
            {
                int espacioSlot = itemData.stackLimit;
                cantidadDisponible -= espacioSlot;

                if (cantidadDisponible <= 0)
                    return true;
            }
        }

        return false;
    }
}