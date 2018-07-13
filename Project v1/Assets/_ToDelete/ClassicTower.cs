using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicTower : Tower, IDamageable
{
    [Header("Use Bullets (default)")]
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float fireCountdown = 0f;

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

    void Update()
    {
        if (target == null)
        {
            return;
        }


        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject bulletGO = PoolManager.Instance.poolDictionnary[bulletPrefab.name].GetFromPool(firePoint.position);
        bulletGO.transform.rotation = firePoint.rotation;

        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
            bullet.Seek(target);
    }
}
