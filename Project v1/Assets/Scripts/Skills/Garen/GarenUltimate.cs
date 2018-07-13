using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarenUltimate : Skills {

    public int ultimateDamage = 1000;

    protected override void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.R) && skill == skill.dispo)
        {
            Debug.Log("R been pressed !");

            List<Targetable> targets = hero.targetter.GetAllTargets();

            foreach (Targetable target in targets)
            {
                target.TakeDamage(ultimateDamage, target.position, hero.configuration.alignmentProvider);
            }
            skill = skill.inCD;
        }
    }

    
}
