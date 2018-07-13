using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float speed;
    public float damage;
    public float explosionRadius;
    
    private Vector3 m_TargetPosition;
    private float step;
    private SplashDamager splashDamager;

    private void OnEnable()
    {
        splashDamager = GetComponent<SplashDamager>();
        if (splashDamager != null)
        {
            splashDamager.enabled = false;
        }
    }

    private void Update()
    {
        step = speed * Time.deltaTime;
        transform.LookAt(m_TargetPosition);
        transform.position = Vector3.MoveTowards(transform.position, m_TargetPosition, step);

        if (transform.position == m_TargetPosition)
        {
            if (splashDamager != null)
            {
                splashDamager.enabled = true;
            }
            PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
        }
    }

    public void SetDestination(Vector3 targetPosition)
    {
        m_TargetPosition = targetPosition;
    }

    private void OnDisable()
    {
        if (splashDamager != null)
        {
            splashDamager.enabled = false;
        }
    }
}
