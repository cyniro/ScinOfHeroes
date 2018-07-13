using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{

    [Header("Use Laser")]


    public int damageOverTime = 30;
    public float slowAmount = 0.5f;

    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;
    public Transform firePoint;

    private Targetter targetter;

    private Targetable target
    {
        get { return targetter.GetTarget(); }
    }

    protected override void Awake()
    {
        base.Awake();
        targetter = GetComponentInChildren<Targetter>();
    }

    private void Update()
    {
        if (targetter.GetTarget() == null)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                impactEffect.Stop();
                impactLight.enabled = false;
            }
            return;

        }
        Laser();
    }

    void Laser()
    {
        float damage = damageOverTime * Time.deltaTime;
        targetter.GetTarget().TakeDamage(damage, target.position, null);
        targetter.GetTarget().GetComponent<IDamageable>().AddBuff("LaserSlow", slowAmount, BuffType.MSBoost);

        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;

        impactEffect.transform.position = target.position + dir.normalized;

        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    }
}
