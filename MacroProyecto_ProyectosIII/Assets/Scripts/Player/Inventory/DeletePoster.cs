using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeletePoster : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI quantityText;
    public Button confirmButton;
    public Button cancelButton;
    public GameObject deletePanel;
    public GameObject selectedItem;
    public Transform originalParent;
    public int itemAmountToRemove;

    public Sprite emptySlotSprite;

    private Item selectedItemComponent;

    private void Awake()
    {
        deletePanel.SetActive(false);

        confirmButton.onClick.AddListener(ConfirmarEliminacion);
        cancelButton.onClick.AddListener(CancelarEliminacion);
    }

    public void EnableDeletePanel()
    {
        if (selectedItem == null)
        {
            return;
        }

        selectedItemComponent = selectedItem.GetComponent<Item>();
        itemAmountToRemove = selectedItemComponent.itemAmount;

        deletePanel.SetActive(true);
        selectedItem.GetComponent<Item>().DisableItem();

        slider.maxValue = itemAmountToRemove;
        slider.value = 1;

        quantityText.text = $"{slider.value}/{itemAmountToRemove}";

        slider.onValueChanged.AddListener((value) => UpdateQuantityText());

        if (selectedItem != null)
        {
            selectedItemComponent = selectedItem.GetComponent<Item>();
        }
    }

    private void ConfirmarEliminacion()
    {
        if (selectedItemComponent != null)
        {
            int cantidadEliminada = Mathf.RoundToInt(slider.value);

            Inventory inventory = GameManager.Instance.inventoryUIPanel.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.DeleteItem(selectedItemComponent.ID, cantidadEliminada);
            }

            selectedItemComponent.itemAmount -= cantidadEliminada;

            if (selectedItemComponent.itemAmount <= 0)
            {
                if (originalParent != null)
                {
                    Image slotImage = originalParent.GetComponent<Image>();
                    if (slotImage != null)
                    {
                        slotImage.sprite = emptySlotSprite;
                    }
                }

                Destroy(selectedItemComponent.gameObject);
            }
            else
            {
                selectedItemComponent.EnableItem();
                selectedItem.transform.SetParent(originalParent);
                selectedItem.transform.localPosition = Vector3.zero;
            }

            CloseDeletePanel();
        }
    }

    private void CancelarEliminacion()
    {
        CloseDeletePanel();
    }

    private void CloseDeletePanel()
    {
        deletePanel.SetActive(false);

        if (selectedItem != null && originalParent != null)
        {
            selectedItem.transform.SetParent(originalParent);
            selectedItem.transform.localPosition = Vector3.zero;
            selectedItem.GetComponent<Item>().EnableItem();
        }
    }

    public void UpdateQuantityText()
    {
        if (slider != null && quantityText != null)
        {
            quantityText.text = $"{Mathf.RoundToInt(slider.value)}/{itemAmountToRemove}";
        }
    }
}
