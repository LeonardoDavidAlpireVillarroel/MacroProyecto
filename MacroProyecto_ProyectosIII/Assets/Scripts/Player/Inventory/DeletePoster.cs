using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeletePoster : MonoBehaviour
{
    [SerializeField]
    Inventory Inv;
    public Slider slider;
    public TextMeshProUGUI ammountText;

    private void Start()
    {
       Inv = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    private void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            slider.maxValue = Inv.OSC;
            ammountText.text = slider.value.ToString();
        }
    }

    public void Aceptar()
    {
        Inv.DeleteItem(Inv.OSID, Mathf.RoundToInt(slider.value));
        slider.value = 1;
        this.gameObject.SetActive(false);
    }

    public void Cancelar()
    {
        slider.value = 1;
        this.gameObject.SetActive(false);
    }
}
