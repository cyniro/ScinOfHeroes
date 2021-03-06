﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Extensions;

/// <summary>
/// A Wave is a TimedBehaviour, that uses the RepeatingTimer to spawn enemies
/// </summary>
public class Wave : TimedBehaviour
{
    /// <summary>
    /// A list of instructions on how to spawn enemies
    /// </summary>
    public List<SpawnInstruction> spawnInstructions;

    /// <summary>
    /// The index of the current enemy to spawn
    /// </summary>
    protected int m_CurrentIndex;

    /// <summary>
    /// The RepeatingTimer used to spawn enemies
    /// </summary>
    protected RepeatingTimer m_SpawnTimer;

    /// <summary>
    /// The event that is fired when a Wave is completed
    /// </summary>
    public event Action waveCompleted;

    public virtual float progress
    {
        get { return (float)(m_CurrentIndex) / spawnInstructions.Count; }
    }

    /// <summary>
    /// Initializes the Wave
    /// </summary>
    public virtual void Init()
    {
        // If the wave is empty then warn the level designer and fire complete event
        if (spawnInstructions.Count == 0)
        {
            Debug.LogWarning("[LEVEL] Empty Wave");
            SafelyBroadcastWaveCompletedEvent();
            return;
        }

        m_SpawnTimer = new RepeatingTimer(spawnInstructions[0].delayToSpawn, SpawnCurrent);
        StartTimer(m_SpawnTimer);
    }

    /// <summary>
    /// Handles spawning the current agent and sets up the next agent for spawning
    /// </summary>
    protected virtual void SpawnCurrent()
    {
        Spawn();
        if (!TrySetupNextSpawn())
        {
            SafelyBroadcastWaveCompletedEvent();
            // this is required so wave progress is still accurate
            m_CurrentIndex = spawnInstructions.Count;
            StopTimer(m_SpawnTimer);
        }
    }

    /// <summary>
    /// Spawns the current agent
    /// </summary>
    protected void Spawn()
    {
        SpawnInstruction spawnInstruction = spawnInstructions[m_CurrentIndex];
        SpawnAgent(spawnInstruction.agentUnit, spawnInstruction.startingNode);
    }

    /// <summary>
    /// Tries to setup the next spawn
    /// </summary>
    /// <returns>true if there is another spawn instruction, false if not</returns>
    protected bool TrySetupNextSpawn()
    {
        bool hasNext = spawnInstructions.Next(ref m_CurrentIndex);
        if (hasNext)
        {
            SpawnInstruction nextSpawnInstruction = spawnInstructions[m_CurrentIndex];
            if (nextSpawnInstruction.delayToSpawn <= 0f)
            {
                SpawnCurrent();
            }
            else
            {
                m_SpawnTimer.SetTime(nextSpawnInstruction.delayToSpawn);
            }
        }

        return hasNext;
    }

    /// <summary>
    /// Spawns the agent
    /// </summary>
    /// <param name="agentConfig">The agent to spawn</param>
    /// <param name="node">The starting node that the agent uses</param>
    protected virtual void SpawnAgent(Unit enemy, Node node)
    {
        Vector3 spawnPosition = node.GetRandomPointInNodeArea();

        GameObject _enemy = PoolManager.Instance.poolDictionnary[enemy.name].GetFromPool(spawnPosition);
        Unit enemyUnit = _enemy.GetComponent<Unit>();
        enemyUnit.transform.position = spawnPosition;
        enemyUnit.Initialize();
        enemyUnit.SetNode(node);
        enemyUnit.transform.rotation = node.transform.rotation;

    }

    /// <summary>
    /// Launch the waveCompleted event
    /// </summary>
    protected void SafelyBroadcastWaveCompletedEvent()
    {
        if (waveCompleted != null)
        {
            waveCompleted();
        }
    }
}

