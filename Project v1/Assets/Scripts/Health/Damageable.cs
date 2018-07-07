using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Damageable
{
    public float resistance = 0f;
    public float startingHealth = 100f;
    public int lootValue = 50;

    public Action healthChanged;
    public Action died;

    /// <summary>
    /// Gets the current health.
    /// </summary>
    public float currentHealth { protected set; get; }

    public float baseResistance { protected set; get; }

    /// <summary>
    /// Gets the normalised health.
    /// </summary>
    public float normalisedHealth
    {
        get
        {
            return currentHealth / startingHealth;
        }
    }

    /// <summary>
    /// Gets whether this instance is dead.
    /// </summary>
    public bool isDead
    {
        get { return currentHealth <= 0f; }
    }

    public virtual void Init()
    {
        baseResistance = resistance;
        currentHealth = startingHealth;
    }

    public virtual void TakeDamage(float amount)
    {

        float effectivDamage = amount * (1 - resistance);

        if (isDead)
        {
            return;
        }

        ChangeHealth(-effectivDamage);

        if (isDead)
        {
            died();
        }
    }

    /// <summary>
    /// Changes the health.
    /// </summary>
    /// <param name="healthModifier">Health amount gain or lost.</param>
    /// <param name="info">HealthChangeInfo for this change</param>
    public void ChangeHealth(float healthModifier)
    {
        currentHealth += healthModifier;
        currentHealth = Mathf.Clamp(currentHealth, 0f, startingHealth);

        if (healthChanged != null)
        {
            healthChanged();
        }
    }

    /// <summary>
    /// Sets this instance's health directly.
    /// </summary>
    /// <param name="health">
    /// The value to set <see cref="currentHealth"/> to
    /// </param>
    public void SetHealth(float health)
    {
        currentHealth = health;
        Debug.Log("SetHealth"+ this);
        if (healthChanged != null)
        {
            Debug.Log("healthChanged SetHealth" + this);
            healthChanged();
        }
    }
}
