using UnityEngine;
using UnityEngine.AI;
using System.Collections;



public class HeroMovement : MonoBehaviour
{
    private Animator anim;

    /// <summary>
    /// Reference to the navagent that will be affected
    /// </summary>
    private NavMeshAgent m_NavMeshAgent;
    private Ray shootRay;
    private RaycastHit shootHit;
    private Transform targetedEnemy; //hero ou tour ennemie 

    private bool walking;

    private bool enemyClicked;
    private float nextFire;
    //private AttackAffector attackAffector;

    public float shootDistance = 10f;
    //public float shootRate = .5f;
    //public HeroAttack attackingScript;

    void Awake()
    {
        //anim = GetComponent<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        //attackAffector = GetComponent<AttackAffector>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //cast a ray from the camera to mouse cursor
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) //if we hit a collider
            {
                if (hit.collider.CompareTag("Hero") || hit.collider.CompareTag("Tower")) // ajouter alignement
                {
                    targetedEnemy = hit.transform;
                    enemyClicked = true;
                }
                else
                {
                    walking = true;
                    enemyClicked = false;
                    m_NavMeshAgent.destination = hit.point;
                    m_NavMeshAgent.isStopped = false;
                }
            }
        }

        if (enemyClicked)
        {
            MoveAndAttack();
        }

        if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
        {
            if (!m_NavMeshAgent.hasPath || Mathf.Abs(m_NavMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
                walking = false;
        }
        else
        {
            walking = true;
        }

        if (anim != null)
        {
            walking = (!m_NavMeshAgent.hasPath || Mathf.Abs(m_NavMeshAgent.velocity.sqrMagnitude) < float.Epsilon) ? false : true;
            anim.SetBool("IsWalking", walking);
        }
    }

    private void MoveAndAttack()
    {
        if (targetedEnemy == null)
            return;
        m_NavMeshAgent.destination = targetedEnemy.position;
        if (m_NavMeshAgent.remainingDistance >= shootDistance) //tant que la cible est plus loin que la range d attaque, continuer de se déplacer + walking anim
        {
            m_NavMeshAgent.isStopped = false;
            walking = true;
        }

        if (m_NavMeshAgent.remainingDistance <= shootDistance)
        {
            transform.LookAt(targetedEnemy);
            m_NavMeshAgent.isStopped = true;
        }
    }
}