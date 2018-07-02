using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple implementation of UI for player base health
/// </summary>
public class PlayerBaseHealth : MonoBehaviour
{
    /// <summary>
    /// The text element to display information on
    /// </summary>
    public Text display;

    /// <summary>
    /// Subscrib to the player homebase damaged event and update display
    /// </summary>
    protected virtual void Start()
    {
        PlayerStats playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogWarning("something wrong happened" + this);
            return;
        }
        playerStats.damaged += UpdateDisplay;

        UpdateDisplay();
    }


    /// <summary>
    /// Get the current health of the home base and display it on display
    /// </summary>
    protected void UpdateDisplay()
    {
        PlayerStats playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogWarning("something wrong happened" + this);
            return;
        }
        display.text = playerStats.currentHealth.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Unsubscrib to playerbasedamage event
    /// </summary>
    protected void OnDestroy()
    {
        PlayerStats playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogWarning("something wrong happened" + this);
            return;
        }
        playerStats.damaged -= UpdateDisplay;
    }
}
