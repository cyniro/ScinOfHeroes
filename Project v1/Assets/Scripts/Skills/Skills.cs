using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum skill
{
    inCD,
    actif,
    dispo,
}

public class Skills : MonoBehaviour
{
    /// <summary>
    /// Set how many seconds until the skill end
    /// </summary>
    private float duration;

    private float CD;

    /// <summary>
    /// Set how many seconds the skill last
    /// </summary>
    [SerializeField]
    protected float baseDuration;
    [SerializeField]
    protected float baseCD;

    protected skill skill;

    protected UnityAction cdEnd;


    public Hero hero;
    public Sprite UISprite;

    public skill Skill { get { return skill; } }

    public float returnCD { get { return CD; } }

    /// <summary>
    /// Returns the current targetter target
    /// </summary>
    protected Targetable m_CurrentTarget
    {
        get
        {
            return hero.targetter.GetTarget();
        }
    }

    /// <summary>
    /// Returns all the targets within the collider of the targetter
    /// </summary>
    protected List<Targetable> m_AllTargets
    {
        get
        {
            return hero.targetter.GetAllTargets();
        }
    }

    protected virtual void OnEnable()
    {
        duration = baseDuration;
        CD = baseCD;
        skill = skill.dispo;
    }

    private void OnAcquiredTarget(Targetable target)
    {
    }

    protected virtual void OnDisable()
    {
    }

    private void OnDestroy()
    {
        hero.targetter.acquiredTarget -= OnAcquiredTarget;
    }


    protected virtual void Update()
    {
        if (skill == skill.actif)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                if (cdEnd != null)
                    cdEnd();

                skill = skill.inCD;
                duration = baseDuration;
            }
        }
        else if (skill == skill.inCD)
        {
            CD -= Time.deltaTime;
            if (CD <= 0)
            {
                skill = skill.dispo;
                CD = baseCD;
            }
        }

    }
}
