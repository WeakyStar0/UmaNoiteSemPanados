using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReticleUI : MonoBehaviour
{
    [SerializeField] private Image reticleImage;
    [SerializeField] private TextMeshProUGUI pickupText;

    void Start()
    {
        HidePickup();
    }

    public void ShowPickup(string itemName)
    {
        Debug.Log("ShowPickup called");
        reticleImage.enabled = true;
        pickupText.enabled = true;
        pickupText.text = $"[E] Pick Up {itemName}";
    }

    public void ShowHint(string text)
    {
        reticleImage.enabled = true;
        pickupText.enabled = true;
        pickupText.text = text;
    }

    public void HidePickup()
    {
        Debug.Log("HidePickup called");
        reticleImage.enabled = false;
        pickupText.enabled = false;
    }
}
