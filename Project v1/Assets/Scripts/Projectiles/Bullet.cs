using UnityEngine;

public class Bullet : MonoBehaviour {

    private Targetable target;

    public float speed = 70f;
    public int damage = 50;

    public float explosionRadius = 0f;

    public GameObject impactEffect;

    /// <summary>
    /// The alignment of the damager
    /// </summary>
    public SerializableIAlignmentProvider alignment;

    /// <summary>
    /// Gets the alignment of the damager
    /// </summary>
    public IAlignmentProvider alignmentProvider
    {
        get { return alignment != null ? alignment.GetInterface() : null; }
    }

    public void Seek (Targetable _target)
    {
        target = _target;
    }
	void Update () {
		if (target == null)
        {
            PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target.position);
	}

    void HitTarget()
    {
        if (impactEffect != null)
        {
            GameObject impactEffectInst = PoolManager.Instance.poolDictionnary[impactEffect.name].GetFromPool(transform.position);
            impactEffectInst.transform.rotation = transform.rotation;

        }

        if (explosionRadius > 0f)
        {
            Explode();
        }else
        {
            Damage(target);
        }

        PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Targetable m_Targetable = collider.GetComponent<Targetable>();
                if (m_Targetable != null)
                {
                    Damage(m_Targetable);
                }
            }
        }

    }

    void Damage (Targetable enemy)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(damage, enemy.position, alignmentProvider);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
