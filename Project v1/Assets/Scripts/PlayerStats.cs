using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    #region Singleton
    public static PlayerStats Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);

        Instance = this;
    }
    #endregion

    public static int Rounds;

    public event Action damaged;
    public event Action goldChanged;

    public int startMoney = 400;
    public int startingHealth = 20;
    
    /// <summary>
    /// The controller for gaining currency
    /// </summary>
    public CurrencyGainer currencyGainer;

    private int m_CurrentMoney;
    private int m_CurrentHealth;

    /// <summary>
    /// Return how much money the player have
    /// </summary>
    public int currentMoney
    {
        get
        { return m_CurrentMoney; }
        private set { }
    }

    /// <summary>
    /// How much health PlayerBase have
    /// </summary>
    public int currentHealth
    {
        get { return m_CurrentHealth; }
        private set { }
    }

    void Start()
    {
        m_CurrentMoney = startMoney;
        m_CurrentHealth = startingHealth;
        Rounds = 0;
        if (currencyGainer.IsActive)
        {
            InvokeRepeating("MoneyOnTime", 0, currencyGainer.GainRate);
        }
    }

    public void DecreaseLife(int damage = 1)
    {
        if (currentHealth - damage <= 0)
            currentHealth = 0;
        else
            currentHealth -= damage;

        if (damaged != null)
        {
            damaged();
        }

        if (m_CurrentHealth <= 0)
        {
            GameManager2.Instance.GameOver();
        }
    }

    public void ChangeMoney(int amount)
    {
        currentMoney += amount;
        goldChanged();
    }

    private void MoneyOnTime()
    {
        ChangeMoney(currencyGainer.constantCurrencyAddition);
    }

    /// <summary>
    /// Clear the singleton
    /// </summary>
    protected void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
