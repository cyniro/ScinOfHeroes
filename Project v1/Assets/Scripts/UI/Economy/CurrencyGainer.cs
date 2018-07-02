using System;
using UnityEngine;


/// <summary>
/// A class for currency gain
/// </summary>
[Serializable]
public class CurrencyGainer
{
    /// <summary>
    /// The amount gained with the gain rate
    /// </summary>
    [SerializeField]
    private int m_ConstantCurrencyAddition;

    /// <summary>
    /// The speed of currency gain in units-per-second
    /// </summary>
    [Header("The Gain Rate in additions-per-second")]
    [SerializeField]
    private float m_ConstantCurrencyGainRate;

    public float GainRate
    {
        get { return 1 / m_ConstantCurrencyGainRate; }
    }

    public bool IsActive
    {
        get { return m_ConstantCurrencyGainRate > 0; }
    }

    public int constantCurrencyAddition
    {
        get { return m_ConstantCurrencyAddition; }
    }
}