﻿using System;
using UnityEngine;
using UnityEngine.Events;

///////////////Need Work//////////////
// Can add HealthChangeInfo script from TD template
//////////////////////////////////////

[Serializable]
public class Damageable
{
    public float resistance = 0f;
    public float startingHealth = 100f;
    public int lootValue = 50;

    /// <summary>
    /// The alignment of the damager
    /// </summary>
    public SerializableIAlignmentProvider alignment;

    public Action healthChanged;
    public Action died;

    //add a out info damage to check who killed this unit, v2
    //HealthChangeInfo

    /// <summary>
    /// Gets the <see cref="IAlignmentProvider"/> of this instance
    /// </summary>
    public IAlignmentProvider alignmentProvider
    {
        get
        {
            return alignment != null ? alignment.GetInterface() : null;
        }
    }

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

    public virtual void TakeDamage(float damage, IAlignmentProvider damageAlignment, out bool output)
    {

        float effectivDamage = damage * (1 - resistance);

        bool canDamage = damageAlignment == null || alignmentProvider == null ||
                 damageAlignment.CanHarm(alignmentProvider);

        if (isDead || !canDamage)
        {
            output = false;
            return;
        }

        ChangeHealth(-effectivDamage);
        output = true;

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

        if (healthChanged != null)
        {
            healthChanged();
        }
    }
}
