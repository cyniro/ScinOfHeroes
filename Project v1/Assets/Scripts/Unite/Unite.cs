using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

/*//////////////////////////////////////////////////////////
 
     
     HEY IF YOU SEE THAT THERE IS STILL WORK TO DO, CHECK BELLOW!

    -LootValue is no perfect and should be in an other scrip
    -add navmesh agent
    -make a script attack
     
     ps: please
     
 /////////////////////////////////////////////////////////*/

/// <summary>
/// Type of the Buff can be
/// </summary>
public enum BuffType
{
    ResistanceBoost,
    ASBoost,
    MSBoost
}

public class Unite : DamageableBehaviour, IDamageable
{
    #region Variables

    private float fireCountdown = 0f;

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
    protected List<GameObject> effectFxList;


    /// <summary>
    /// use to check if this unit is on CD
    /// </summary>
    protected bool m_IsInCD;

    public UnityAction attackAction;

    [Header("Stats")]
    [SerializeField]
    protected float m_FireRate = 1f;
    [SerializeField]
    private float m_MovementSpeed = 10f;
    [SerializeField]
    protected float m_CdBetweenSpawn = 60;

    public bool range = false;
    public bool melee = false;

    [SerializeField]
    protected int meleeDamage; //auto attack damage, must be ignored if range , faire un CustomEditor (https://docs.unity3d.com/ScriptReference/Editor.OnInspectorGUI.html) pour hide/show ref suivant le bool

    [Header("Setup")]
    public Targetter targetter;
    public Sprite icon;
    public UnitData unitData;
    public GameObject bulletPrefab; // must be ignored if melee ,  faire un CustomEditor (https://docs.unity3d.com/ScriptReference/Editor.OnInspectorGUI.html) pour hide/show ref suivant le bool
    public int cost;
    /// <summary>
    /// Scale adjustment for an applied affect
    /// </summary>
    public float appliedEffectScale = 1;
    /// <summary>
    /// Position offset for an applied affect
    /// </summary>
    public Vector3 appliedEffectOffset = Vector3.zero;
    #endregion

    #region Proprieties

    /// <summary>
    /// Gets this unit's original movement speed
    /// </summary>
    public float baseMovementSpeed { get; private set; }

    /// <summary>
    /// Gets this unit's spawn state by CD
    /// </summary>
    public bool isInCD { get; set; }

    /// <summary>
    /// Return the cd of this unit
    /// </summary>
    public float cDBetweenSpawns
    {
        get { return m_CdBetweenSpawn; }
    }


    /// <summary>
    /// Gets this unit's movement speed
    /// </summary>
    public float movementSpeed
    {
        get { return m_MovementSpeed; }
    }

    public Animator anim
    {
        get { return anim; }
    }


    public float fireRate
    {
        get { return m_FireRate; }
    }

    /// <summary>
    /// Gets this unit's original fire rate
    /// </summary>
    public float basefireRate { get; private set; }

    /// <summary>
    /// Return position offset for an applied affect
    /// </summary>
    public Vector3 GetEffectOffset()
    {
        return appliedEffectOffset;
    }

    /// <summary>
    /// Return scale adjustment for an applied affect
    /// </summary>
    public float GetEffectScale()
    {
        return appliedEffectScale;
    }

    /// <summary>
    /// Return the Effect's Fx List that runs on this units
    /// </summary>
    public List<GameObject> GetEffectFxList()
    {
        return effectFxList;
    }

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
    #endregion

    protected override void Awake()
    {
        base.Awake();
        LazyLoad();
    }

    /// <summary>
    /// This is a lazy way of caching several components utilised by the unit
    /// </summary>
    protected virtual void LazyLoad()
    {
        configuration.died += ResetUnit;
        baseMovementSpeed = m_MovementSpeed;
        basefireRate = m_FireRate;
        aSBoostsDictionary = new Dictionary<string, List<float>>();
        mSBoostsDictionary = new Dictionary<string, List<float>>();
        resiBoostsDictionary = new Dictionary<string, List<float>>();
        effectFxList = new List<GameObject>();
    }

    protected virtual void OnEnable()
    {
        LazyLoad();
    }

    protected virtual void Update()
    {
        if (targetter.target == null)
        {
            return;
        }

        targetter.LockOnTarget();

        if (fireCountdown <= 0f)
        {
            Attack();
            fireCountdown = 1f / m_FireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    protected virtual void Attack()
    {
        if (attackAction != null)
        {
            Debug.Log("fire attackAction");
            attackAction();
        }

        if (range)
        {
            GameObject bulletGO = PoolManager.Instance.poolDictionnary[bulletPrefab.name].GetFromPool(targetter.firePoint.position);
            bulletGO.transform.rotation = targetter.firePoint.rotation;

            Bullet bullet = bulletGO.GetComponent<Bullet>();

            if (bullet != null)
                bullet.Seek(targetter.target);
        }

        if (melee)
        {
            targetter.targetEnemy.TakeDamage(meleeDamage);
        }
    }

    #region Buff & Fx
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
                        m_FireRate = basefireRate;
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
                        m_FireRate = basefireRate * m_ASBuffs;

                    }
                }
                break;
            case BuffType.MSBoost:
                {
                    if (!SearchAndDeleteEmptyEntry(mSBoostsDictionary))
                    {
                        m_MovementSpeed = baseMovementSpeed;
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
                        m_MovementSpeed = baseMovementSpeed * m_MSBuffs;

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
            m_fxPrefab.transform.localPosition = appliedEffectOffset;
            m_fxPrefab.transform.localScale *= appliedEffectScale;
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

    #endregion

    public virtual Alignement GetAlignement()
    {
        return targetter.alignement;
    }

    /// <summary>
    /// Reset this unit
    /// </summary>
    public override void Remove()
    {
        base.Remove();
        ResetUnit();
    }

    /// <summary>
    /// This will null some stuff
    /// </summary>
    protected void ResetUnit()
    {
        configuration.died -= ResetUnit;

        aSBoostsDictionary = null;
        resiBoostsDictionary = null;
        mSBoostsDictionary = null;
        effectFxList = null;
    }
}
