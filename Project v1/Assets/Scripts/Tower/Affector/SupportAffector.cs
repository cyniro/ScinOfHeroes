using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*//////////////////////////////////////////////////////////
 
     
     HEY IF YOU SEE THAT THERE IS STILL WORK TO DO, CHECK BELLOW!

    -Clean the mess and add summaries on you methodes and variables
    -Check if all team's members are okay with the Heal Coroutine( instant heal of wait healRate)
     
     ps: please
     
 /////////////////////////////////////////////////////////*/
/// <summary>
/// Uses a trigger to Buffs/Debuff the unit and attacj/remove an FX
/// </summary>
public class SupportAffector : PassiveAffector
{
    /// <summary>
    /// Enum use to specify if the action to do, buff the unit or remove a buff
    /// </summary>
    public enum BuffStat
    {
        Buff,
        DeBuff
    }

    [Header("Healer")]
    public bool healer;
    public float healAmount;
    public float healRate;
    public GameObject healFxPrefab;

    [Header("BoostAS")]
    public bool isAttackSpeedBooster;
    [Range(1, 10)]
    public float aSModifier = 1f;
    public GameObject attackSpeedFxPrefab;

    [Header("BoostMS")]
    public bool isMouvmentSpeedBooster;
    [Range(1, 10)]
    public float mSModifier = 1f;
    public GameObject movementSpeedFxPrefab;

    [Header("BoostRes")]
    public bool isResistanceBooster;
    [Range(0, 1)]
    public float resModifier;
    public GameObject resistanceFxPrefab;

    [Header("")]
    /// <summary>
    /// The particle system that plays when an entity enters the sphere
    /// </summary>
    public ParticleSystem enterParticleSystem;

    /// <summary>
    /// The audio source that plays when an entity enters the sphere
    /// </summary>
    public AudioSource enterAudioSource;

    /// <summary>
    /// List used to keep track off allies in Range
    /// </summary>
    private List<Unit> alliesInRange = new List<Unit>();

    /// <summary>
    /// Sub to events
    /// </summary>
    protected virtual void OnEnable()
    {
        targetter.allyEntersRange += OnFriendEnterRange;
        targetter.allyExitsRange += OnFriendExitRange;
    }

    /// <summary>
    /// unSub events
    /// </summary>
    protected void OnDisable()
    {
        StopAllCoroutines();
        targetter.allyEntersRange -= OnFriendEnterRange;
        targetter.allyExitsRange -= OnFriendExitRange;

        foreach (Unit ally in alliesInRange)
        {
            RemoveAllBuffs(ally);
        }

        alliesInRange.Clear();
    }

    /// <summary>
    /// Fonction called when targetter spot an allie and trigger the allyEntersRange event
    /// We check if the gameobject is nothing but and Unit/Hero/Peon then it's time to support it
    /// </summary>
    /// <param name="ally">Unit Ally that enter the targetter's range</param>
    private void OnFriendEnterRange(Unit ally)
    {
        if (ally == null || ally.isDead)
        {
            return;
        }

        alliesInRange.Add(ally);

        ally.removed += RemoveAllBuffs;

        //play sound/particle effect on enter range
        if (enterParticleSystem != null)
        {
            enterParticleSystem.Play();
        }
        if (enterAudioSource != null)
        {
            enterAudioSource.Play();
        }

        //Check what this tower is capable of
        if (healer && alliesInRange.Count <= 1)
        {
            StartCoroutine(Heal());
            ally.SetFx(healFxPrefab);
        }
        if (isMouvmentSpeedBooster)
        {
            SetMsBuff(ally, BuffStat.Buff);
            ally.SetFx(movementSpeedFxPrefab);
        }
        if (isResistanceBooster)
        {
            SetResiBuff(ally, BuffStat.Buff);
            ally.SetFx(resistanceFxPrefab);
        }
        if (isAttackSpeedBooster)
        {
            SetASBuff(ally, BuffStat.Buff);
            ally.SetFx(attackSpeedFxPrefab);
        }
    }

    /// <summary>
    /// Called when the targetter event "allyExitsRange" is trigger:
    /// Remove buff from it and remove it from the alliesInTangeList
    /// </summary>
    /// <param name="ally">the allie who has exit the range</param>
    private void OnFriendExitRange(Unit ally)
    {
        alliesInRange.Remove(ally);
        RemoveAllBuffs(ally);
    }

    /// <summary>
    /// Removes all buffs on the ally,
    /// </summary>
    /// <param name="ally">the allie who is target by this methode</param>
    private void RemoveAllBuffs(DamageableBehaviour ally)
    {
        Unit m_Unit = ally.GetComponent<Unit>();
        if (m_Unit == null)
        {
            return;
        }

        m_Unit.removed -= RemoveAllBuffs;

        if (healer)
        {
            if (alliesInRange.Count == 0)
            {
                StopAllCoroutines();
            }
            m_Unit.RemoveFx(healFxPrefab);
        }
        if (isMouvmentSpeedBooster)
        {
            SetMsBuff(m_Unit, BuffStat.DeBuff);
            if (!m_Unit.GetMSBoostsDictionary().ContainsKey("SupportTowerBuffMS"))
            {
                m_Unit.RemoveFx(movementSpeedFxPrefab);
            }
        }
        if (isResistanceBooster)
        {
            SetResiBuff(m_Unit, BuffStat.DeBuff);
            if (!m_Unit.GetResiBoostsDictionary().ContainsKey("SupportTowerBuffResi"))
            {
                m_Unit.RemoveFx(resistanceFxPrefab);
            }
        }
        if (isAttackSpeedBooster)
        {
            SetASBuff(m_Unit, BuffStat.DeBuff);
            if (!m_Unit.GetASBoostsDictionary().ContainsKey("SupportTowerBuffAS"))
            {
                m_Unit.RemoveFx(attackSpeedFxPrefab);
            }
        }
    }

    /// <summary>
    /// Simple Coroutine that keep heal allies in tower's range
    /// </summary>
    IEnumerator Heal()
    {
        while (this.enabled)
        {
            yield return new WaitForSeconds(healRate);

            foreach (Targetable ally in alliesInRange)
            {
                ally.Heal(healAmount);
            }
        }
    }

    /// <summary>
    /// Fonction called to boost/unboost Resistance of an ally
    /// </summary>
    /// <param name="ally">the Unit that is buffed</param>
    /// <param name="state">specify if it's a buff or a debuff</param>
    private void SetResiBuff(Unit ally, BuffStat state)
    {
        if (ally == null || ally.isDead)
        {
            Debug.Log("Something wrong happened");
            return;
        }
        switch (state)
        {
            case BuffStat.Buff:
                {
                    ally.AddBuff("SupportTowerBuffResi", resModifier, BuffType.ResistanceBoost);
                }
                break;
            case BuffStat.DeBuff:
                {
                    ally.RemoveBuff("SupportTowerBuffResi", resModifier, BuffType.ResistanceBoost);
                }
                break;
        }
    }

    /// <summary>
    /// Fonction called to boost/unboost MovementSpeed of an ally
    /// </summary>
    /// <param name="ally">the Unit that is buffed/unbuffed</param>
    /// <param name="state">specify if it's a buff or a debuff</param>
    private void SetMsBuff(Unit ally, BuffStat state)
    {
        if (ally == null || ally.isDead)
        {
            Debug.Log("Something wrong happened");
            return;
        }
        switch (state)
        {
            case BuffStat.Buff:
                {
                    ally.AddBuff("SupportTowerBuffMS", mSModifier, BuffType.MSBoost);
                }
                break;
            case BuffStat.DeBuff:
                {
                    ally.RemoveBuff("SupportTowerBuffMS", mSModifier, BuffType.MSBoost);
                }
                break;
        }
    }

    /// <summary>
    /// Fonction called to boost/unboost AttackSpeed of an ally
    /// </summary>
    /// <param name="ally">the Unit that is buffed/unbuffed</param>
    /// <param name="state">specify if it's a buff or a debuff</param>
    private void SetASBuff(Unit ally, BuffStat state)
    {
        if (ally == null || ally.isDead)
        {
            Debug.Log("Something wrong happened");
            return;
        }
        switch (state)
        {
            case BuffStat.Buff:
                {
                    ally.AddBuff("SupportTowerBuffAS", aSModifier, BuffType.ASBoost);

                }
                break;
            case BuffStat.DeBuff:
                {
                    ally.RemoveBuff("SupportTowerBuffAS", aSModifier, BuffType.ASBoost);
                }
                break;
        }
    }
}
