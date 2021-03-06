﻿using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Abstract class for any MonoBehaviours that can take damage
/// </summary>
public class DamageableBehaviour : MonoBehaviour
{
    /// <summary>
    /// The Damageable object
    /// </summary>
    public Damageable configuration;

    /// <summary>
    /// Gets whether this <see cref="DamageableBehaviour" /> is dead.
    /// </summary>
    /// <value>True if dead</value>
    public bool isDead
    {
        get { return configuration.isDead; }
    }

    /// <summary>
    /// The position of the transform
    /// </summary>
    public virtual Vector3 position
    {
        get { return transform.position; }
    }
    /// <summary>
    /// Event that is fired when this instance is removed, such as when pooled or destroyed
    /// </summary>
    public event Action<DamageableBehaviour> removed;

    /// <summary>
    /// Event that is fired when this instance is killed
    /// </summary>
    public event Action<GameObject> died;

    /// <summary>
    /// Occurs when damage is taken
    /// </summary>
    public Action hit;


    protected virtual void Awake()
    {
        configuration.Init();
        configuration.died += OnConfigurationDied;
    }

    public virtual void TakeDamage(float damageValue, Vector3 damagePoint, IAlignmentProvider alignment)
    {
        bool info;
        configuration.TakeDamage(damageValue, alignment, out info);
        if (info)
        {
            if (hit != null)
            {
                hit();
            }
        }
    }

    public virtual void Heal(float amount)
    {
        configuration.ChangeHealth(amount);
    }

    /// <summary>
    /// Kills this damageable
    /// </summary>
    protected virtual void Kill()
    {
        bool info;
        configuration.TakeDamage(configuration.currentHealth, null, out info);
    }

    /// <summary>
    /// Event fired when Damageable takes critical damage
    /// </summary>
    protected virtual void OnConfigurationDied()
    {
        OnDeath();
        Remove();
    }

    /// <summary>
    /// Removes this damageable without killing it and return to pool
    /// </summary>
    public virtual void Remove()
    {
        // Set health to zero so that this behaviour appears to be dead. This will not fire death events
        configuration.SetHealth(0);
        OnRemoved();

        //lootValue is automacly called for now when a unit die, it shoud be not call here.
        //So when this unit reach end path (enemy base) they will not give money to the enemy player
        //can make a scrip name Loot that subscrib to dameable.die event will resolve this problem

        PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
    }

    /// <summary>
    /// Fires kill events
    /// </summary>
    void OnDeath()
    {
        if (died != null)
        {
            died(this.gameObject);
        }
    }

    /// <summary>
    /// Fires the removed event
    /// </summary>
    void OnRemoved()
    {
        if (removed != null)
        {
            removed(this);
        }
    }
}
