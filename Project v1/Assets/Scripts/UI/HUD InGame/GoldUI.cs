using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class for controlling the displaying the currency
/// </summary>
public class GoldUI : MonoBehaviour
{
    /// <summary>
    /// The text element to display information on
    /// </summary>
    public Text display;

    protected PlayerStats m_PlayerStats;

    /// <summary>
    /// Assign the correct currency value
    /// </summary>
    protected virtual void Start()
    {
        if (PlayerStats.Instance != null)
        {
            m_PlayerStats = PlayerStats.Instance;

            UpdateDisplay();
            m_PlayerStats.goldChanged += UpdateDisplay;
        }
        else
        {
            Debug.LogError("[UI] No PlayerStats to get Gold from");
        }
    }

    /// <summary>
    /// Unsubscribe from events
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (m_PlayerStats != null)
        {
            m_PlayerStats.goldChanged -= UpdateDisplay;
        }
    }

    /// <summary>
    /// A method for updating the display based on the current currency
    /// </summary>
    protected void UpdateDisplay()
    {
        display.text = m_PlayerStats.currentGold.ToString();
    }
}