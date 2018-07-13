using System.Collections;
using UnityEngine;

/// <summary>
/// The Mage First Spell, strike and electric ray in front of him
/// </summary>
public class ElectricRaySkill : Skills {

    public GameObject effect;
    public int enemyNumberToActivate = 0;
    public SplashDamager splashDamager;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (splashDamager != null)
        {
            splashDamager.enabled = false;
        }
        else
        {
            Debug.LogWarning("Mage1stSkill SplashDamager null");
        }
    }
    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();
       
        if (skill == skill.dispo && (m_AllTargets.Count >= enemyNumberToActivate))
        {
            skill = skill.actif;
            ActiveSpell();
        }
	}

    private void ActiveSpell()
    {
        splashDamager.enabled = true;
        effect.SetActive(true);
        StartCoroutine(TurnOffEffect());

        skill = skill.inCD;
    }

    IEnumerator TurnOffEffect()
    {
        splashDamager.enabled = false;

        yield return new WaitForSeconds(0.5f);

        effect.SetActive(false);
    }
}