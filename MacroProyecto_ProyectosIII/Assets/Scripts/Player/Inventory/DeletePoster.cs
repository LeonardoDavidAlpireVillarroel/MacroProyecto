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
        itemAmountToRemove = selectedItemComponent.itemAmount;  // Asignamos el valor del inventario al itemAmountToRemove

        deletePanel.SetActive(true);
        selectedItem.GetComponent<Item>().DisableItem();

        // Establecemos el maxValue del slider según la cantidad de ítems en el inventario
        slider.maxValue = itemAmountToRemove;
        slider.value = 1;  // Inicializa el valor del slider a 1 (puede ajustarse según el comportamiento deseado)

        // Actualiza el texto con la cantidad inicial
        quantityText.text = $"{slider.value}/{itemAmountToRemove}";

        // Escuchar cambios en el slider para actualizar el texto
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

            selectedItemComponent.itemAmount -= cantidadEliminada;

            if (selectedItemComponent.itemAmount <= 0)
            {
                // Si el ítem se destruye, actualizamos el sprite del slot vacío
                if (originalParent != null)
                {
                    Image slotImage = originalParent.GetComponent<Image>();
                    if (slotImage != null)
                    {
                        // Usamos el emptySlotSprite del Inventory
                        slotImage.sprite = emptySlotSprite;  // Usamos el sprite vacío del Inventory
                    }
                }

                // Destruimos el objeto del ítem
                Destroy(selectedItemComponent.gameObject);
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
