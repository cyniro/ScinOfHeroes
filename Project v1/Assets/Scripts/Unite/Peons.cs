using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Peons : Unite
{
    private NavMeshAgent navMeshAgent;
    private Vector3 buildPosition;
    private bool building = false;
    private bool onTheWay = false;
    private float constructionSpeed = .25f;
    private int peonsAlive;
    private float peonActualLife;
    private float constructedPoints;
    private TurretBlueprint blueprint;
    private Node nodeDestination;
    private bool backing;
    private int peonToDisable = 0;
    private GameObject _buildingTower;// ajouter au blueprint

    public Transform home;
    public List<GameObject> peons;
    public GameObject buildingTower;//  ajouter au blueprint
    public Alignement alignement;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        navMeshAgent = GetComponent<NavMeshAgent>();
        building = false;
        onTheWay = false;
        constructionSpeed = 0.25f;
        peonsAlive = 4;
        constructedPoints = 0f;
        peonActualLife = configuration.startingHealth;
        _buildingTower = null;
        peonToDisable = 0;
        backing = false;

        navMeshAgent.enabled = false;
        navMeshAgent.enabled = true;

        for (int i = 0; i < peons.Count; i++)
        {
            if (!peons[i].activeSelf)
                peons[i].SetActive(true);
        }
    }

    protected override void Update()
    {
        if (onTheWay)
        {
            if (navMeshAgent.remainingDistance <= Mathf.Epsilon)
            {
                transform.LookAt(buildPosition);
                navMeshAgent.isStopped = true;
                BuildATower();
                onTheWay = false;
            }
        }

        if (building)
        {
            if (constructedPoints >= blueprint.constructionPoints)
            {
                PoolManager.Instance.poolDictionnary[_buildingTower.name].UnSpawnObject(_buildingTower);
                GameObject turretBuild = PoolManager.Instance.poolDictionnary[blueprint.prefab.name].GetFromPool(buildPosition);
                PoolManager.Instance.poolDictionnary[BuildManager.Instance.buildEffect.name].GetFromPool(buildPosition);
                Debug.Log("Tower built");
                building = false;
                nodeDestination.turret = turretBuild;
                nodeDestination.EnableConstruction();

                PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
            }
            else
            {
                constructedPoints += constructionSpeed * peonsAlive * Time.deltaTime;
            }

        }

        if (backing)
        {
            if (navMeshAgent.remainingDistance <= Mathf.Epsilon)
            {
                PlayerStats.Instance.ChangeGold(blueprint.cost);
                PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
            }
        }
    }

    public void GoBuildATower(TurretBlueprint _blueprint, Vector3 _buildPosition, Node _nodeDestination)
    {
        buildPosition = _buildPosition;
        blueprint = _blueprint;
        nodeDestination = _nodeDestination;
        onTheWay = true;

        navMeshAgent.destination = buildPosition;
    }


    private void BuildATower()
    {
        _buildingTower = PoolManager.Instance.poolDictionnary[buildingTower.name].GetFromPool(buildPosition);    
        building = true;
    }

    public override void TakeDamage(float amount)
    {
        peonActualLife -= amount;

        if (peonActualLife <= 0)
        {
            peonsAlive -= 1;
            peonActualLife = configuration.startingHealth;

            if (peonsAlive <= 0)
            {
                Remove();
                return;
            }
            peons[peonToDisable].SetActive(false);
            peonToDisable++;
        }
    }

    public override void Remove()
    {
        base.Remove();
        if (_buildingTower != null)
            PoolManager.Instance.poolDictionnary[_buildingTower.name].UnSpawnObject(_buildingTower);

        nodeDestination.EnableConstruction();
    }

    public override void Heal(float amount)
    {
        peonActualLife = Mathf.Min(peonActualLife + amount, configuration.startingHealth);
    }

    public void CancelBuilding()
    {
        if (building)
            return;

        navMeshAgent.isStopped = false;
        navMeshAgent.destination = home.position;
        backing = true;
        onTheWay = false;
    }

    public override Alignement GetAlignement()
    {
        return alignement;
    }
}
