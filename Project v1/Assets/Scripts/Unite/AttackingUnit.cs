using UnityEngine;
using Towers;

/// <summary>
/// An implementation of Agent that will attack 
/// any Towers that block its path
/// </summary>
public class AttackingUnit : Unit
    {
        /// <summary>
        /// Bools to set the behaviour of an agent/units/hero.
        /// </summary>
        [Tooltip("Won't attack while moving")]
        public bool stopForAttacking;
        [Tooltip("will have a layer enemy mask for targetting")]
        public bool gotLayerEnemyMask;
        [Tooltip("[For Heroes] need a Movement Script to work. Won't be driven by a automatic navmesh.setdestination")]
        public bool moveByPointAndClick;
        [Tooltip("Won't need to have the path Blocked for attacking")]
        public bool dontNeedPathBlockedToAttack;

        /// <summary>
        /// The physics mask the agent searches on
        /// </summary>
        public LayerMask enemyLayerMask;

        /// <summary>
        /// Tower to target
        /// </summary>
        protected Tower m_TargetTower;

        /// <summary>
        /// The attached attack affector
        /// </summary>
        protected AttackAffector m_AttackAffector;
        public AttackAffector AttackAffector
        {
            get
            {
                return m_AttackAffector;
            }
        }

        /// <summary>
        /// Is this agent currently engaging a tower?
        /// </summary>
        protected bool m_IsAttacking;

        public override void Initialize()
        {
            base.Initialize();
            // Attack affector
            if (gotLayerEnemyMask)
                m_AttackAffector.Initialize(configuration.alignmentProvider, enemyLayerMask);
            else
                m_AttackAffector.Initialize(configuration.alignmentProvider);

            if (stopForAttacking)
            {
                if (m_AttackAffector != null)
                    m_AttackAffector.m_Targetter.acquiredTarget += OnAcquiredTarget;
                m_AttackAffector.m_Targetter.lostTarget += OnLostTarget;
            }
        }


        /// <summary>
        /// Use to not make/make the Agent following default behaviour of the navmesh agent.
        /// </summary>
        protected override void Update()
        {
            if (!moveByPointAndClick)
            { base.Update(); }
        }

        /// <summary>
        /// Call when the agent has a target.
        /// </summary>
        public virtual void OnAcquiredTarget(Targetable acquiredTarget)
        {
            m_NavMeshAgent.isStopped = true;
        }

        /// <summary>
        /// Call when the agent has no target.
        /// </summary>
        public virtual void OnLostTarget()
        {
            m_NavMeshAgent.isStopped = false;
        }

        /// <summary>
        /// Unsubscribes from tracked towers removed event
        /// and disables the attached attack affector
        /// </summary>
        public override void Remove()
        {
            base.Remove();
            if (m_TargetTower != null)
            {
                m_TargetTower.removed -= OnTargetTowerDestroyed;
            }
            m_AttackAffector.enabled = false;
            m_TargetTower = null;
            m_AttackAffector.trackingEnemy = null;
            if (m_AttackAffector != null)
            {
                m_AttackAffector.m_Targetter.acquiredTarget -= OnAcquiredTarget;
                m_AttackAffector.m_Targetter.lostTarget -= OnLostTarget;
            }
        }

        /// <summary>
        /// Gets the closest tower to the agent
        /// </summary>
        /// <returns>The closest tower</returns>
        protected Tower GetClosestTower()
        {
            var towerController = m_AttackAffector.m_Targetter.GetTarget() as Tower;
            return towerController;
        }

        /// <summary>
        /// Caches the Attack Affector if necessary
        /// </summary>
        protected override void LazyLoad()
        {
            base.LazyLoad();
            if (m_AttackAffector == null)
            {
                m_AttackAffector = GetComponent<AttackAffector>();
            }
        }

        /// <summary>
        /// If the tower is destroyed while other agents attack it, ensure it becomes null
        /// </summary>
        /// <param name="tower">The tower that has been destroyed</param>
        protected virtual void OnTargetTowerDestroyed(DamageableBehaviour tower)
        {
            if (m_TargetTower == tower)
            {
                m_TargetTower.removed -= OnTargetTowerDestroyed;
                m_TargetTower = null;
            }
        }

        /// <summary>
        /// Peforms the relevant path update for the states <see cref="Agent.State.OnCompletePath"/>, 
        /// <see cref="Agent.State.OnPartialPath"/> and <see cref="Agent.State.Attacking"/>
        /// </summary>
        protected override void PathUpdate()
        {
            switch (state)
            {
                case State.OnCompletePath:
                    OnCompletePathUpdate();
                    break;
                case State.OnPartialPath:
                    OnPartialPathUpdate();
                    break;
                case State.Attacking:
                    AttackingUpdate();
                    break;
            }
        }

        /// <summary>
        /// Change to <see cref="Agent.State.OnCompletePath" /> when path is no longer blocked or to
        /// <see cref="Agent.State.Attacking" /> when the agent reaches <see cref="AttackingUnit.m_TargetTower" />
        /// </summary>
        protected override void OnPartialPathUpdate()
        {
            if (!isPathBlocked)
            {
                state = State.OnCompletePath;
                return;
            }

            // Check for closest tower at the end of the partial path
            //m_AttackAffector.towerTargetter.transform.position = m_NavMeshAgent.pathEndPosition;
            Tower tower = GetClosestTower();
            if (tower != m_TargetTower)
            {
                // if the current target is to be replaced, unsubscribe from removed event
                if (m_TargetTower != null)
                {
                    m_TargetTower.removed -= OnTargetTowerDestroyed;
                }

                // assign target, can be null
                m_TargetTower = tower;

                // if new target found subscribe to removed event
                if (m_TargetTower != null)
                {
                    m_TargetTower.removed += OnTargetTowerDestroyed;
                }
            }
            if (m_TargetTower == null)
            {
                return;
            }
            float distanceToTower = Vector3.Distance(transform.position, m_TargetTower.transform.position);
            if (!(distanceToTower < m_AttackAffector.m_Targetter.effectRadius))
            {
                return;
            }
            if (!m_AttackAffector.enabled)
            {
                m_AttackAffector.m_Targetter.transform.position = transform.position;
                m_AttackAffector.enabled = true;
            }
            state = State.Attacking;
            m_NavMeshAgent.isStopped = true;
        }

        /// <summary>
        /// The agent attacks until the path is available again or it has killed the target tower
        /// </summary>
        protected void AttackingUpdate()
        {
            if (m_TargetTower != null)
            {
                return;
            }
            MoveToNode();

            // Resume path once blocking has been cleared
            if (!dontNeedPathBlockedToAttack)
            {
                m_AttackAffector.enabled = false;
            }
            m_IsAttacking = false;
            m_NavMeshAgent.isStopped = false;
            state = isPathBlocked ? State.OnPartialPath : State.OnCompletePath;

            // Move the Targetter back to the agent's position
            // m_AttackAffector.towerTargetter.transform.position = transform.position;
        }
    }