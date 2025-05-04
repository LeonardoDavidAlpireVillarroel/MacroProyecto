using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI amountText;
    public int itemAmount = 1;
    public int ID;
    public bool acumulable;
    public Sprite crowdedSlotSprite;
    public Button Button;
    public GameObject _description;
    public TextMeshProUGUI Nombre_;
    public TextMeshProUGUI Dato_;
    public Vector3 offset;
    public ItemsDataBase ItemDB;

    private CanvasGroup canvasGroup;

    private bool isPointerOver = false;

    private void Start()
    {
        if (ItemDB == null)
        {
            ItemDB = Resources.Load<ItemsDataBase>("ObjectsDataBase");
            if (ItemDB == null)
            {
                return;
            }
        }

        acumulable = ItemDB.ObjectsDataBase[ID].acumulable;

        Button = GetComponent<Button>();

        if (Inventory.Description != null)
        {
            _description = Inventory.Description;
        }
        else
        {
            return;
        }

        if (_description == null)
        {
            return;
        }

        Nombre_ = _description.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Dato_ = _description.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        if (Nombre_ == null || Dato_ == null)
        {
            return;
        }

        _description.SetActive(false);

        if (!_description.GetComponent<Image>().enabled)
        {
            _description.GetComponent<Image>().enabled = true;
            Nombre_.enabled = true;
            Dato_.enabled = true;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        if (amountText != null)
        {
            amountText.text = itemAmount.ToString();
        }

        Transform parent = transform.parent;
        if (parent != null)
        {
            if (transform.parent.GetComponent<Image>() != null)
            {
                Image parentImage = parent.GetComponent<Image>();
                if (parentImage != null && crowdedSlotSprite != null)
                {
                    parentImage.sprite = crowdedSlotSprite;
                }
            }
        }

        if (_description != null)
        {
            if (transform.parent == Inventory.canvas)
            {
                _description.SetActive(false);
            }
        }
    }

    public void DisableItem()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }
    }

    public void EnableItem()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_description != null && ItemDB != null && ItemDB.ObjectsDataBase.Length > ID && Nombre_ != null && Dato_ != null)
        {
            isPointerOver = true;
            if (Inventory.Description != null)
            {
                _description = Inventory.Description;
                _description.SetActive(true);
                Nombre_.text = ItemDB.ObjectsDataBase[ID].nombre;
                Dato_.text = ItemDB.ObjectsDataBase[ID].description;
                _description.transform.position = transform.position + offset;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPointerOver) return;
        _description.SetActive(false);
        isPointerOver = false;
    }
}