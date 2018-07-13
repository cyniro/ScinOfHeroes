using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The 3rd Speel of the Mage, this will add force to enemy's RB to blow them away
/// </summary>
public class EnemyBlowerSkill : Skills
{
    public float m_ExplosionForce;
    public float m_ExplosionRadius;
    public GameObject skillEffect;
	
	protected override void Update ()
    {
        base.Update();

        if (skill == skill.dispo && hero.configuration.currentHealth <= hero.configuration.startingHealth/3)
        {
            ActiveSkill();
        }
	}


    private void ActiveSkill()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius);

        foreach (Collider enemy in colliders)
        {
            var targetable = enemy.GetComponent<Targetable>();
            if (!IsTargetableValid(targetable))
            {
                continue;
            }

            Rigidbody targetRigidbody = enemy.GetComponent< Rigidbody>();
            if (!targetRigidbody)
            {
                continue;
            }

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
        }
            skill = skill.inCD;
    }

    /// <summary>
    /// Checks if the targetable is a valid target
    /// </summary>
    /// <param name="targetable"></param>
    /// <returns>true if targetable is vaild, false if not</returns>
    protected virtual bool IsTargetableValid(Targetable targetable)
    {
        if (targetable == null)
        {
            return false;
        }

        IAlignmentProvider targetAlignment = targetable.configuration.alignmentProvider;
        bool canDamage = hero.configuration.alignmentProvider == null || targetAlignment == null ||
                         hero.configuration.alignmentProvider.CanHarm(targetAlignment);

        return canDamage;
    }
}
