using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tourbilol : Skills
{
    [SerializeField]
    private float damageOverTime;
    [SerializeField]
    private float attackRate;

    public SplashDamager splashDamager;

    protected override void OnEnable()
    {
        base.OnEnable();
        cdEnd += StopTourbilol;
        if (splashDamager != null)
        {
            splashDamager.enabled = false;
        }

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        cdEnd -= StopTourbilol;
    }

    protected override void Update()
    {
        base.Update();

        if (skill == skill.dispo && m_CurrentTarget != null)
        {
            skill = skill.actif;
            ActiveTourbilol();
        }
    }

    private void ActiveTourbilol()
    {
        if (hero.anim != null)
        {
            hero.anim.Play("Tourbilol");
        }
        if (splashDamager != null)
        {
            splashDamager.enabled = true;
        }
    }

    private void TourbilolDealDamage()
    {
        if (splashDamager != null)
        {
            splashDamager.enabled = true;
        }
    }


    private void StopTourbilol()
    {
        if (hero.anim != null)
        {
            hero.anim.Play("Default");
        }
        if (splashDamager != null)
        {
            splashDamager.enabled = false;
        }
    }
}
