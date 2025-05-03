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
        _description = Inventory.Description;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_description != null && ItemDB != null && ItemDB.ObjectsDataBase.Length > ID && Nombre_ != null && Dato_ != null)
        {
            _description.SetActive(true);
            Nombre_.text = ItemDB.ObjectsDataBase[ID].nombre;
            Dato_.text = ItemDB.ObjectsDataBase[ID].description;
            _description.transform.position = transform.position + offset;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_description != null)
        {
            _description.SetActive(false);
        }
    }
}
