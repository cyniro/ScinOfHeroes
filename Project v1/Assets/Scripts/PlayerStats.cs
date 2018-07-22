using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    public static int Rounds;

    /// <summary>
    /// Event fired when the Player home base is damaged
    /// </summary>
    public event Action damaged;
    /// <summary>
    /// Event fired when the amount of Gold change
    /// </summary>
    public event Action goldChanged;

    public int startMoney = 400;
    public int startingHealth = 20;
    
    /// <summary>
    /// The controller for gaining currency
    /// </summary>
    public FloatIncreaser goldByTime;

    private int m_CurrentGold;
    private int m_CurrentHealth;

    /// <summary>
    /// Return how much money the player have
    /// </summary>
    public int currentGold
    {
        get { return m_CurrentGold; }
    }

    /// <summary>
    /// How much health PlayerBase have
    /// </summary>
    public int currentHealth
    {
        get { return m_CurrentHealth; }
    }

    protected override void Awake()
    {
        base.Awake();
        m_CurrentGold = startMoney;
        m_CurrentHealth = startingHealth;
    }


    void Start()
    {
        Rounds = 0;
        if (goldByTime.IsActive)
        {
            InvokeRepeating("MoneyOnTime", 0, goldByTime.GainRate);
        }
    }

    public void DecreaseLife(int damage = 1)
    {
        if (m_CurrentHealth - damage <= 0)
            m_CurrentHealth = 0;
        else
            m_CurrentHealth -= damage;

        if (damaged != null)
        {
            damaged();
        }

        if (m_CurrentHealth <= 0)
        {
            GameManager2.Instance.GameOver();
        }
    }

    /// <summary>
    /// Determines if the specified cost is affordable.
    /// </summary>
    /// <returns><c>true</c> if this cost is affordable; otherwise, <c>false</c>.</returns>
    public bool CanAfford(int cost)
    {
        return m_CurrentGold >= cost;
    }

    /// <summary>
    /// Method for trying to purchase, returns false for insufficient funds
    /// </summary>
    /// <returns><c>true</c>, if purchase was successful i.e. enough currency <c>false</c> otherwise.</returns>
    public bool TryPurchase(int cost)
    {
        // Cannot afford this item
        if (!CanAfford(cost))
        {
            return false;
        }
        ChangeGold(-cost);
        return true;
    }

    public void ChangeGold(int amount)
    {
        m_CurrentGold += amount;

        if (goldChanged != null)
        {
            goldChanged();
        }
    }

    private void MoneyOnTime()
    {
        ChangeGold(goldByTime.constantCurrencyAddition);
    }
}
