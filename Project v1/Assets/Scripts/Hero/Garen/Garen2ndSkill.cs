﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Garen2ndSkill : Skills
{
    [SerializeField]
    protected float damageReduction;

    public GameObject shieldEffect;

    protected override void OnEnable()
    {
        base.OnEnable();
        hero.takeDamageAction += ActiveSkill;
        cdEnd += ResetSkill;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        hero.takeDamageAction -= ActiveSkill;
        cdEnd -= ResetSkill;
    }


    private void ResetSkill()
    {
        hero.RemoveBuff("GarenShield", damageReduction, BuffType.ResistanceBoost);
        hero.RemoveFx(shieldEffect);
        //shieldEffect.SetActive(false);
    }


    private void ActiveSkill()
    {
        if (skill == skill.dispo)
        {
            skill = skill.actif;
            hero.AddBuff("GarenShield", damageReduction, BuffType.ResistanceBoost);
            hero.SetFx(shieldEffect);
            //shieldEffect.SetActive(true);
        }
    }

}
