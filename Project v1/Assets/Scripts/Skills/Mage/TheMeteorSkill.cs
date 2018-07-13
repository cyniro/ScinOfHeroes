using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheMeteorSkill : Skills
{
    public GameObject meteor;
    public float offset;

	protected override void Update ()
    {
        base.Update();

        if (skill == skill.dispo && m_CurrentTarget != null)
        {
            Vector3 targetPosition = m_CurrentTarget.position;
            ActiveSkill(targetPosition);
        }
	}

    private void ActiveSkill(Vector3 targetPosition)
    {
        Vector3 meteorSpawnPosition = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
        GameObject _meteor = PoolManager.Instance.poolDictionnary[meteor.name].GetFromPool(meteorSpawnPosition);

        _meteor.GetComponent<Meteor>().SetDestination(targetPosition);
        skill = skill.inCD;
    }




}
