using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class ShopUI : MonoBehaviour
{   
    [Header("UI References")]
    public TextMeshProUGUI currencyText;
    
    [Header("XR Settings")]
    public TrackedDeviceGraphicRaycaster xrRaycaster;
    
    private void Start()
    {
        UpdateCurrencyText();
        ShopManager.Instance.onCurrencyChanged.AddListener(UpdateCurrencyText);
    }
    
    private void UpdateCurrencyText()
    {
        if (currencyText != null)
            currencyText.text = $"Coins: {ShopManager.Instance.currentCurrency}";
    }
    
    private void OnDestroy()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.onCurrencyChanged.RemoveListener(UpdateCurrencyText);
        }
    }
}
