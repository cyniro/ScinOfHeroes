using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Towers;

/// <summary>
/// A button controller for spawning towers
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class TowerSpawnButton : MonoBehaviour, IDragHandler
{
    /// <summary>
    /// The text attached to the button
    /// </summary>
    public Text buttonText;

    public Image towerIcon;

    public Button buyButton;

    public Color priceDefaultColor;

    public Color priceInvalidColor;

    /// <summary>
    /// Fires when the button is tapped
    /// </summary>
    public event Action<Tower> buttonTapped;

    /// <summary>
    /// Fires when the pointer is outside of the button bounds
    /// and still down
    /// </summary>
    public event Action<Tower> draggedOff;

    /// <summary>
    /// The tower controller that defines the button
    /// </summary>
    Tower m_Tower;

    /// <summary>
    /// Cached reference to level currency
    /// </summary>
    PlayerStats playerStats;

    /// <summary>
    /// The attached rect transform
    /// </summary>
    RectTransform m_RectTransform;

    /// <summary>
    /// Checks if the pointer is out of bounds
    /// and then fires the draggedOff event
    /// </summary>
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(m_RectTransform, eventData.position))
        {
            if (draggedOff != null)
            {
                draggedOff(m_Tower);
            }
        }
    }

    /// <summary>
    /// Define the button information for the tower
    /// </summary>
    /// <param name="towerData">
    /// The tower to initialize the button with
    /// </param>
    public void InitializeButton(Tower towerData)
    {
        m_Tower = towerData;
        if (towerData.levels.Length > 0)
        {
            TowerLevel firstTower = towerData.levels[0];
            buttonText.text = firstTower.cost.ToString();
            towerIcon.sprite = firstTower.levelData.icon;
        }
        else
        {
            Debug.LogWarning("[Tower Spawn Button] No level data for tower");
        }

        if (PlayerStats.instanceExists)
        {
            playerStats = PlayerStats.Instance;
            playerStats.goldChanged += UpdateButton;
        }
        else
        {
            Debug.LogWarning("[Tower Spawn Button] No game manager to get gold change event ");
        }
        UpdateButton();
    }

    /// <summary>
    /// Cache the rect transform
    /// </summary>
    protected virtual void Awake()
    {
        m_RectTransform = (RectTransform)transform;
    }

    /// <summary>
    /// Unsubscribe from events
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.goldChanged -= UpdateButton;
        }
    }

    /// <summary>
    /// The click for when the button is tapped
    /// </summary>
    public void OnClick()
    {
        if (buttonTapped != null)
        {
            buttonTapped(m_Tower);
        }
    }

    /// <summary>
    /// Update the button's button state based on cost
    /// </summary>
    void UpdateButton()
    {
        if (playerStats == null)
        {
            return;
        }

        // Enable button
        if (playerStats.CanAfford(m_Tower.purchaseCost) && !buyButton.interactable)
        {
            buyButton.interactable = true;
            buttonText.color = priceDefaultColor;
        }
        else if (!playerStats.CanAfford(m_Tower.purchaseCost) && buyButton.interactable)
        {
            buyButton.interactable = false;
            buttonText.color = priceInvalidColor;
        }
    }
}