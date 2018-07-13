using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// This class manage the player units's menu
/// </summary>
public class UnitMenu : MonoBehaviour
{
    /// <summary>
    /// a sctuct use to keep track of unit button's unit cd.
    /// </summary>
    private class UnitInCd
    {
        public UnitButton unitButton;
        public float unitCd;
    }

    /// <summary>
    /// List of the Button that will be use to spawn Units
    /// </summary>
    public List<UnitButton> unitButtonList = new List<UnitButton>();

    /// <summary>
    /// If a SpawnPoint is no avaible, the spawnable units will spawn there
    /// </summary>
    public Transform defaultSpawnPoint;

    /// <summary>
    /// Keet track of units in cd
    /// </summary>
    private List<UnitInCd> unitsInCD = new List<UnitInCd>();

    /// <summary>
    /// Use to catch the instance of PlayerStats on startu
    /// </summary>
    private PlayerStats playerStats;


    /// <summary>
    /// Sub to goldchange event to update button's states
    /// </summary>
    private void Start()
    {
        playerStats = PlayerStats.Instance;

        if (playerStats != null)
        {
            playerStats.goldChanged += UpdateButtons;
        }

        InitMenu();
    }

    /// <summary>
    /// Init all button with AgentSelected Helps and update them
    /// </summary>
    private void InitMenu()
    {
        //catch Agent selected in AngentSelector and set button
        AgentSelector agentSelector = AgentSelector.Instance;
        if (agentSelector != null)
        {
            for (int i = 0; i < agentSelector.selectedAgents.Count; i++)
            {
                Unite m_unit = AgentSelector.Instance.selectedAgents[i].GetComponent<Unite>();
                UnitButton unitButton = unitButtonList[i];

                unitButton.unitIcon.sprite = m_unit.icon;
                unitButton.UnitCostText.text = m_unit.cost.ToString();
                unitButton.cdFillText.enabled = false;
                unitButton.agentSelectorIndex = i;
                unitButton.unit = m_unit;

                m_unit.isInCD = false;
            }

            if (agentSelector.selectedAgents.Count < unitButtonList.Count)
            {
                RemoveEmptyFromList();
            }
            UpdateButtons();
        }
    }

    /// <summary>
    /// Remove empty button from unitButtonList
    /// </summary>
    private void RemoveEmptyFromList()
    {
        AgentSelector agentSelector = AgentSelector.Instance;

        List<int> empty = new List<int>();

        for (int i = agentSelector.selectedAgents.Count; i < unitButtonList.Count; i++)
        {
            empty.Add(i);
        }
        for (int i = empty.Count - 1; i >= 0; i--)
        {
            UnitButton unitButton = unitButtonList[empty[i]];

            unitButtonList.RemoveAt(empty[i]);
            Destroy(unitButton.gameObject);
        }
    }

    /// <summary>
    /// Spawn the button's unit and place it in cd
    /// </summary>
    /// <param name="unitButton">Contain all reference needed: button, unit, cd etc</param>
    public void OnClick(UnitButton unitButton)
    {
        Cooldown(unitButton);

        Spawn(unitButton);
    }

    /// <summary>
    /// Ask for buy the Unit, then call to spawn
    /// <param name="unitButton">has all infomation needed by htis methode</param>
    /// </summary>
    protected void Spawn(UnitButton unitButton)
    {
        int cost = unitButton.unit.cost;
        bool succesPurchase = PlayerStats.Instance.TryPurchase(cost);

        if (succesPurchase)
        {
            if (NetworkManager.InstanceExists)
            {
                foreach (NetworkPlayer player in NetworkManager.Instance.connectedPlayers)
                {
                    if (player.hasAuthority)
                    {
                        int indexUniteToSpawn = unitButton.agentSelectorIndex;
                        Vector3 positionToSpawn;
                        if (player.spawnPoint != null)
                        {
                            positionToSpawn = player.spawnPoint.position;
                        }
                        else
                        {
                            Debug.LogWarning("player.spawnPoint is not set");
                            positionToSpawn = defaultSpawnPoint.position;
                        }

                        player.CmdSpawnFromPool(indexUniteToSpawn, positionToSpawn);
                    }
                }
            }
            else
            {
                int indexUniteToSpawn = unitButton.agentSelectorIndex;
                GameObject go = PoolManager.Instance.poolDictionnary
                    [AgentSelector.Instance.selectedAgentsString[indexUniteToSpawn]].GetFromPool(defaultSpawnPoint.position);
                go.transform.rotation = defaultSpawnPoint.rotation;

                Unite unit = go.GetComponent<Unite>();
                unit.configuration.SetHealth(unit.configuration.startingHealth);
            }
        }
    }

    /// <summary>
    /// Unsub to goldchange event
    /// </summary>
    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.goldChanged -= UpdateButtons;
        }
    }

    /// <summary>
    /// Display a countdown regarding the if the unit spwaning is on CD
    /// </summary>
    private void Update()
    {
        if (unitsInCD.Count == 0)
        {
            return;
        }
        for (int i = 0; i < unitsInCD.Count; i++)
        {
            UnitButton unitButton = unitsInCD[i].unitButton;
            Unite unit = unitButton.unit;

            if (!unit.isInCD)
            {
                continue;
            }
            unitButton.cdFillText.enabled = true;
            unitButton.cdFillImage.enabled = true;

            float cooldownNormalize = Time.deltaTime / unit.cDBetweenSpawns;
            float cooldown = (unitsInCD[i].unitCd -= Time.deltaTime);
            if (cooldown > 60)
            {
                string minutes = ((int)cooldown / 60).ToString("00");
                string seconds = ((int)cooldown % 60).ToString("00");
                if (seconds == "60")
                    seconds = "00";
                unitButton.cdFillText.text = minutes + ":" + seconds;
            }
            else
                unitButton.cdFillText.text = cooldown.ToString("0");

            unitButton.cdFillImage.fillAmount -= cooldownNormalize;
            if (cooldown <= 0)
            {
                Reset(unitsInCD[i]);
            }
        }
    }

    /// <summary>
    /// Atualize the button's interactable state
    /// </summary>
    private void UpdateButtons()
    {
        for (int i = 0; i < unitButtonList.Count; i++)
        {
            if (playerStats.CanAfford(unitButtonList[i].unit.cost) && !unitButtonList[i].buyButton.interactable)
            {
                if (!unitButtonList[i].unit.isInCD)
                {
                    unitButtonList[i].buyButton.interactable = true;
                }
            }
            else if (!playerStats.CanAfford(unitButtonList[i].unit.cost) || unitButtonList[i].unit.isInCD && unitButtonList[i].buyButton.interactable)
            {
                unitButtonList[i].buyButton.interactable = false;
            }
        }
    }

    /// <summary>
    /// Call when button as been pressed, and set a cooldown on the button
    /// </summary>
    public void Cooldown(UnitButton unitButton)
    {
        // fill the image to start animation cd
        unitButton.cdFillImage.fillAmount = 1;

        unitButton.unit.isInCD = true;

        UnitInCd unitInCd = new UnitInCd();
        unitInCd.unitButton = unitButton;
        unitInCd.unitCd = unitButton.unit.cDBetweenSpawns;

        unitsInCD.Add(unitInCd);
    }

    /// <summary>
    /// Used to enable the Agent's purchasing after his cooldown is gone
    /// </summary>
    private void Reset(UnitInCd unitInCd)
    {
        UnitButton unitButton = unitInCd.unitButton;

        unitButton.unit.isInCD = false;
        unitButton.cdFillText.enabled = false;
        unitButton.cdFillImage.enabled = false;

        unitsInCD.Remove(unitInCd);

        UpdateButtons();
    }
}