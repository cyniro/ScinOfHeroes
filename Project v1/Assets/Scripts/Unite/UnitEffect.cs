using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of the Buff can be
/// </summary>
public enum BuffType
{
    ResistanceBoost,
    ASBoost,
    MSBoost
}

public class UnitEffect : Targetable
{
    /// <summary>
    /// Position offset for an applied affect
    /// </summary>
    public Vector3 appliedEffectOffset = Vector3.zero;

    /// <summary>
    /// Scale adjustment for an applied affect
    /// </summary>
    public float appliedEffectScale = 1;

    /// <summary>
    /// Reference to the unit that will be affected
    /// </summary>
    public Unit m_Unit;

    /// <summary>
    /// Reference to the Affector to catch fire rate
    /// </summary>
    public AttackAffector m_Affector;

    /// <summary>
    /// Gets this agent's original movement speed
    /// </summary>
    protected float originalFireRate;

    /// <summary>
    /// Contains this unit's AsBoost buffs and there values 
    /// </summary>
    private Dictionary<string, List<float>> aSBoostsDictionary;
    /// <summary>
    /// Contains this unit's MSBoost buffs and there values 
    /// </summary>
    private Dictionary<string, List<float>> mSBoostsDictionary;
    /// <summary>
    /// Contains this unit's ResistanceBoost buffs and there values 
    /// </summary>
    private Dictionary<string, List<float>> resiBoostsDictionary;

    /// <summary>
    /// List use to keep track of all Fx's Effect on this unit
    /// </summary>
    private List<GameObject> effectFxList;

    /// <summary>
    /// Gets this agent's original movement speed
    /// </summary>
    public float originalMovementSpeed { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LazyLoad();
        m_Unit.removed += ResetUnit;

        aSBoostsDictionary = new Dictionary<string, List<float>>();
        mSBoostsDictionary = new Dictionary<string, List<float>>();
        resiBoostsDictionary = new Dictionary<string, List<float>>();
        effectFxList = new List<GameObject>();

        originalMovementSpeed = m_Unit.navMeshAgent.speed;
        originalFireRate = m_Affector.fireRate;
    }

    /// <summary>
    /// A lazy way to ensure that <see cref="m_Agent"/> will not be null
    /// </summary>
    protected virtual void LazyLoad()
    {
        if (m_Unit == null)
        {
            m_Unit = GetComponent<Unit>();
        }
        if (m_Affector == null)
        {
            m_Affector = GetComponent<AttackAffector>();
        }
    }

    #region Buff & Fx
    /// <summary>
    /// Return the Dictionary of Attack Speed's Boost
    /// </summary>
    public Dictionary<string, List<float>> GetASBoostsDictionary()
    {
        return aSBoostsDictionary;
    }
    /// <summary>
    /// Return the Dictionary of Movement Speed's Boost
    /// </summary>
    public Dictionary<string, List<float>> GetMSBoostsDictionary()
    {
        return mSBoostsDictionary;
    }
    /// <summary>
    /// Return the Dictionary of Resistance's Boost
    /// </summary>
    public Dictionary<string, List<float>> GetResiBoostsDictionary()
    {
        return resiBoostsDictionary;
    }

    /// <summary>
    /// Called when a buff must be add. Tower, spells and summoners can use this methode
    /// </summary>
    /// <param name="buffName">The name of the Buff we will Add</param>
    /// <param name="value">The value used to change stats</param>
    public virtual void AddBuff(string buffName, float value, BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.ASBoost:
                {
                    if (aSBoostsDictionary.ContainsKey(buffName))
                    {
                        aSBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                    else
                    {
                        List<float> buffNameList = new List<float>();
                        aSBoostsDictionary.Add(buffName, buffNameList);
                        aSBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                }
                break;
            case BuffType.MSBoost:
                {
                    if (mSBoostsDictionary.ContainsKey(buffName))
                    {
                        mSBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                    else
                    {
                        List<float> buffNameList = new List<float>();
                        mSBoostsDictionary.Add(buffName, buffNameList);
                        mSBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                }
                break;
            case BuffType.ResistanceBoost:
                {
                    if (resiBoostsDictionary.ContainsKey(buffName))
                    {
                        resiBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                    else
                    {
                        List<float> buffNameList = new List<float>();
                        resiBoostsDictionary.Add(buffName, buffNameList);
                        resiBoostsDictionary[buffName].Add(value);
                        ActualiseBuffs(buffType);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Called when a buff msut be removed. Tower, spells and summoners can use this methode
    /// </summary>
    /// <param name="buffName">The name of the Buff we will Remove</param>
    /// <param name="value">The value used to change stats</param>
    public virtual void RemoveBuff(string buffName, float value, BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.ASBoost:
                {
                    if (aSBoostsDictionary.ContainsKey(buffName))
                    {
                        aSBoostsDictionary[buffName].Remove(value);
                        if (aSBoostsDictionary[buffName].Count == 0)
                        {
                            aSBoostsDictionary.Remove(buffName);
                        }
                        ActualiseBuffs(buffType);
                    }
                }
                break;
            case BuffType.MSBoost:
                {
                    if (mSBoostsDictionary.ContainsKey(buffName))
                    {
                        mSBoostsDictionary[buffName].Remove(value);
                        if (mSBoostsDictionary[buffName].Count == 0)
                        {
                            mSBoostsDictionary.Remove(buffName);
                        }
                        ActualiseBuffs(buffType);
                    }
                }
                break;
            case BuffType.ResistanceBoost:
                {
                    if (resiBoostsDictionary.ContainsKey(buffName))
                    {
                        resiBoostsDictionary[buffName].Remove(value);
                        if (resiBoostsDictionary[buffName].Count == 0)
                        {
                            resiBoostsDictionary.Remove(buffName);
                        }
                        ActualiseBuffs(buffType);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Will actualise the stats of the unit regarding his buffs
    /// </summary>
    protected void ActualiseBuffs(BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.ASBoost:
                {
                    if (!SearchAndDeleteEmptyEntry(aSBoostsDictionary))
                    {
                        m_Affector.fireRate = originalFireRate;
                    }
                    else
                    {
                        float m_ASBuffs = 0;
                        foreach (KeyValuePair<string, List<float>> aSbuff in aSBoostsDictionary)
                        {
                            float max = 0;
                            foreach (float item in aSbuff.Value)
                            {
                                max = Mathf.Max(max, item);
                            }
                            m_ASBuffs += max;
                        }
                        m_Affector.fireRate = originalFireRate * m_ASBuffs;

                    }
                }
                break;
            case BuffType.MSBoost:
                {
                    if (!SearchAndDeleteEmptyEntry(mSBoostsDictionary))
                    {
                        m_Unit.navMeshAgent.speed = originalMovementSpeed;
                    }
                    else
                    {
                        float m_MSBuffs = 0;
                        foreach (KeyValuePair<string, List<float>> mSbuff in mSBoostsDictionary)
                        {
                            float max = 0;
                            foreach (float item in mSbuff.Value)
                            {
                                max = Mathf.Max(max, item);
                            }
                            m_MSBuffs += max;
                        }
                        m_Unit.navMeshAgent.speed = originalMovementSpeed * m_MSBuffs;

                    }
                }
                break;
            case BuffType.ResistanceBoost:
                {
                    if (!SearchAndDeleteEmptyEntry(resiBoostsDictionary))
                    {
                        configuration.resistance = configuration.baseResistance;
                    }
                    else
                    {
                        float m_ResBuffs = configuration.baseResistance;
                        foreach (KeyValuePair<string, List<float>> resBuff in resiBoostsDictionary)
                        {
                            float max = 0;
                            foreach (float item in resBuff.Value)
                            {
                                max = Mathf.Max(max, item);
                            }
                            m_ResBuffs += (1 - m_ResBuffs) / (1 / max);
                        }
                        configuration.resistance = m_ResBuffs;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Check a dictionary's key if there value contains an empty list
    /// </summary>
    /// <param name="dictionary">Dictionary to check</param>
    protected bool SearchAndDeleteEmptyEntry(Dictionary<string, List<float>> dictionary)
    {
        if (dictionary.Count == 0)
        {
            return false;
        }
        else
        {
            List<string> itemsToRemove = new List<string>();

            foreach (var pair in dictionary)
            {
                if (pair.Value.Count.Equals(0))
                    itemsToRemove.Add(pair.Key);
            }

            foreach (string item in itemsToRemove)
            {
                dictionary.Remove(item);
            }
            return true;
        }
    }

    /// <summary>
    /// Add a visual for the buff/debuff that is affecting the allie
    /// </summary>
    /// <param name="allie">allie affect by the visual</param>
    /// <param name="fxPrefab">visual to set</param>
    /// <param name="position">position on the allie for the visual</param>
    /// <param name="scale">scale of the visual</param>
    public void SetFx(GameObject fxPrefab = null)
    {
        if ((!effectFxList.Contains(fxPrefab)) && fxPrefab != null)
        {
            GameObject m_fxPrefab = PoolManager.Instance.poolDictionnary[fxPrefab.name].GetFromPool(transform.position);
            m_fxPrefab.name = fxPrefab.name;
            effectFxList.Add(m_fxPrefab);
            m_fxPrefab.transform.parent = transform;
            m_fxPrefab.transform.localPosition = m_Unit.appliedEffectOffset;
            m_fxPrefab.transform.localScale *= m_Unit.appliedEffectScale;
        }
    }

    /// <summary>
    /// Remove a visual for the buff/debuff that is affecting the allie
    /// </summary>
    /// <param name="allie">allie affect by the visual</param>
    /// <param name="fxPrefab">visual to set</param>
    /// <param name="position">position on the allie for the visual</param>
    /// <param name="scale">scale of the visual</param>
    public void RemoveFx(GameObject fxPrefab = null)
    {
        if (fxPrefab != null)
        {
            for (int i = 0; i < effectFxList.Count; i++)
            {
                if (fxPrefab.name.GetHashCode() == effectFxList[i].name.GetHashCode())
                {
                    GameObject m_fxPrefab = effectFxList[i];

                    effectFxList.Remove(effectFxList[i]);

                    m_fxPrefab.transform.localScale = Vector3.one;
                    PoolManager.Instance.poolDictionnary[m_fxPrefab.name].UnSpawnObject(m_fxPrefab);
                }
            }
        }
    }

    /// <summary>
    /// This will null some stuff
    /// </summary>
    protected void ResetUnit(DamageableBehaviour removed)
    {
        m_Unit.removed -= ResetUnit;

        aSBoostsDictionary = null;
        resiBoostsDictionary = null;
        mSBoostsDictionary = null;
        effectFxList = null;
    }
    #endregion
}

