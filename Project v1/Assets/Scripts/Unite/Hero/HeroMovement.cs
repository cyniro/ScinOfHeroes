using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.AI;


/// <summary>
/// This script will handle mouvement by using agent.desitation. While mouving towards his target, the gameobject will set a bool on or off to play animation.
/// </summary>

public class HeroMovement : NetworkBehaviour
{
    #region Singleton


    /// <summary>
    /// Gets the UIPFCController instance if it exists
    /// </summary>
    public static HeroMovement Instance
    {
        get;
        protected set;
    }

    public static bool InstanceExists
    {
        get { return Instance != null; }
    }

    #endregion

    /// </summary>
    /// Whether initialization has taken place.
    /// </summary>
    private bool m_Initialized;

    //private Transform targetedEnemy; ///hero ou tour ennemie 
    private bool walking; ///The bool to be set while walking, this way the animator will know if he is playing the right animation
    //private bool enemyClicked;
    protected Animator anim;

    /// </summary>
    /// Reference if needed
    /// </summary>
    private Rigidbody m_Rigidbody;
    public Rigidbody Rigidbody
    {
        get
        {
            return m_Rigidbody;
        }
    }

    /// <summary>
    /// Reference to the AttackingAgent for initializing
    /// </summary>
    protected AttackingUnit m_AttackingAgent;

    /// <summary>
    /// Reference to the navagent that will be affected
    /// </summary>
    protected NavMeshAgent m_NavMeshAgent;

    private void Awake()
    {
        m_AttackingAgent = GetComponent<AttackingUnit>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Init by HeroManager
    /// </summary>
    public void Init()
    {
        if (m_AttackingAgent != null)
            m_AttackingAgent.Initialize();
        else
            Debug.LogWarning("m_AttackingAgent is null");

        anim = GetComponent<Animator>();

        enabled = false;
        m_NavMeshAgent.enabled = false;
        m_Initialized = true;
    }

    /// <summary>
    /// Used for reset by default
    /// </summary>
    public void SetDefaults()
    {
        enabled = true;
        m_NavMeshAgent.enabled = true;
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
    }

    void Update()
    {
        if (!m_Initialized)
        {
            return;
        }

        if (hasAuthority)
        {

            if (m_NavMeshAgent.pathPending)
                return;

            if (Input.GetButtonDown("Fire2"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); ///cast a ray from the camera to mouse cursor
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100)) ///if we hit a collider
                {
                    CmdSetDestination(hit.point);
                }
            }
        }
        walking = (!m_NavMeshAgent.hasPath || Mathf.Abs(m_NavMeshAgent.velocity.sqrMagnitude) < float.Epsilon) ? false : true;
        anim.SetBool("IsWalking", walking);
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

    public void DisableMovement()
    {
        float newSpeed = 0f;
        m_NavMeshAgent.speed = newSpeed;
        m_NavMeshAgent.isStopped = true;
    }

    public void EnableMovement()
    {
        float newSpeed = m_AttackingAgent.originalMovementSpeed;
        m_NavMeshAgent.speed = newSpeed;
        m_NavMeshAgent.isStopped = false;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //private void MoveAndAttack()
    //{
    //    if (targetedEnemy == null)
    //        return;

    //    navMeshAgent.destination = targetedEnemy.position;

    //    if (navMeshAgent.remainingDistance >= shootDistance) ///tant que la cible est plus loin que la range d attaque, continuer de se déplacer + walking anim
    //    {
    //        navMeshAgent.isStopped = false;
    //        walking = true;
    //    }
    //    if (navMeshAgent.remainingDistance <= shootDistance)
    //    {
    //        navMeshAgent.isStopped = true;
    //        walking = false;
    //        ///may be use to launch an attack on a tower or an hero, while deactivated the Targetter’s SearchRate (configures how often a new closest target is selected).
    //        //Attack Script
    //    }
    //}

    ///////////////////////////////////// if you want to have sync the real position ///////////////////////////

    // Source : https://forum.unity.com/threads/unet-multiplayer-interpolation-and-latency-compensation.398102/#post-2605088
    // carfull, this is from 2016, since that unity made a big update on his network

    //using UnityEngine;
    //using UnityEngine.EventSystems;
    //using UnityEngine.Networking;

    //public class HeroMovement : NetworkBehaviour
    //{
    //    NavMeshAgent agent;

    //    [SyncVar]
    //    Vector3 realPosition = Vector3.zero;
    //    [SyncVar]
    //    Quaternion realRotation;
    //    private float updateInterval;

    //    void Start()
    //    {
    //        agent = GetComponent<NavMeshAgent>();
    //    }

    //    void Update()
    //    {
    //        if (isLocalPlayer)
    //        {
    //            // code that moves the player (in my case using a navmesh)
    //            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
    //            {
    //                RaycastHit hit;

    //                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
    //                {
    //                    agent.destination = hit.point;
    //                }
    //            }

    //            // update the server with position/rotation
    //            updateInterval += Time.deltaTime;
    //            if (updateInterval > 0.11f) // 9 times per second
    //            {
    //                updateInterval = 0;
    //                CmdSync(transform.position, transform.rotation);
    //            }
    //        }
    //        else
    //        {
    //            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
    //            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
    //        }
    //    }

    //    [Command]
    //    void CmdSync(Vector3 position, Quaternion rotation)
    //    {
    //        realPosition = position;
    //        realRotation = rotation;
    //    }
    //}

}