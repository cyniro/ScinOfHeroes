using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Networking;

public class HeroMovement : NetworkBehaviour
{
    private Animator anim;

    /// <summary>
    /// Reference to the navagent that will be affected
    /// </summary>
    private NavMeshAgent m_NavMeshAgent;
    private Ray shootRay;
    private RaycastHit shootHit;
    private Transform targetedEnemy; //hero ou tour ennemie 

    /// <summary>
    /// This must be delete whenn unitAgent will be creat and replace by m_Agent.originalMovementSpeed;
    /// </summary>
    private float originalMovementSpeed;

    private bool walking;

    private bool enemyClicked;
    private float nextFire;

    public float shootDistance = 10f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        originalMovementSpeed = m_NavMeshAgent.speed;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {

            if (m_NavMeshAgent.pathPending)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //cast a ray from the camera to mouse cursor
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) //if we hit a collider
            {
                if (NetworkManager.InstanceExists)
                {
                    if (hasAuthority)
                    {
                        CmdSetDestination(hit.point);

                    }
                }
                else
                {

                    if (hit.collider.CompareTag("Hero") || hit.collider.CompareTag("Tower")) // ajouter alignement
                    {
                        targetedEnemy = hit.transform;
                        enemyClicked = true;
                    }
                    else
                    {
                        enemyClicked = false;
                        m_NavMeshAgent.destination = hit.point;
                        m_NavMeshAgent.isStopped = false;
                    }

                    if (enemyClicked)
                    {
                        MoveAndAttack();
                    }
                }

            }
            if (anim != null)
            {
                walking = (!m_NavMeshAgent.hasPath || Mathf.Abs(m_NavMeshAgent.velocity.sqrMagnitude) < float.Epsilon) ? false : true;
                anim.SetBool("IsWalking", walking);
            }
            else
                Debug.LogWarning("anim is null on Hero" + this);
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

    [Command]
    protected void CmdSetDestination(Vector3 destination)
    {
        RpcSetDestination(destination);
    }

    [ClientRpc]
    protected void RpcSetDestination(Vector3 destination)
    {
        m_NavMeshAgent.destination = destination;
        m_NavMeshAgent.isStopped = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisableMovement()
    {
        float newSpeed = 0f;
        m_NavMeshAgent.speed = newSpeed;
        m_NavMeshAgent.isStopped = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableMovement()
    {
        float newSpeed = originalMovementSpeed; // will be replace futher on by: m_Agent.originalMovementSpeed
        m_NavMeshAgent.speed = newSpeed;
        m_NavMeshAgent.isStopped = false;
    }
}