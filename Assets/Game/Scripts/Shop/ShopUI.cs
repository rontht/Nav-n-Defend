using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <summary>
/// Handles the shop's currency display UI:
/// - Updates the coin counter
/// - Responds to currency changes
/// - Manages XR raycaster for VR/AR interaction
/// 
/// This component should be attached to the shop's main UI canvas.
/// </summary>
public class ShopUI : MonoBehaviour
{   
    [Header("UI References")]
    public TextMeshProUGUI currencyText;
    
    [Header("XR Settings")]
    public TrackedDeviceGraphicRaycaster xrRaycaster;      private void Start()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.onCoinsChanged += UpdateCurrencyText;
            UpdateCurrencyText();
        }
        else
        {
            Debug.LogError("PlayerStats.Instance is null. Ensure there is a PlayerStats component in the scene.");
        }
    }
    
    private void UpdateCurrencyText()
    {
        if (currencyText == null)
        {
            Debug.LogError("Currency Text reference is missing on ShopUI");
            return;
        }

        if (PlayerStats.Instance == null)
        {
            currencyText.text = "Coins: ERROR";
            Debug.LogError("PlayerStats.Instance is null");
            return;
        }

        currencyText.text = $"Coins: {PlayerStats.Instance.coins}";
    }
    
    private void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.onCoinsChanged -= UpdateCurrencyText;
        }
    }
}
