﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UnitesMenu : NetworkBehaviour
{
    private List<int> priceList = new List<int>(5);
    private List<float> CDList = new List<float>(5);
    private List<bool> onCDList = new List<bool>(5);

    //private PoolManager poolManager;
    //private AgentSelector agentSelector;
    private PlayerStats playerStats;

    public Vector3 spawnPoint;

    public List<Image> agentButtonsImageList;
    public List<Button> agentButtonsList;
    public List<Text> agentButtonsTextsList;


    private void Awake()
    {
        //poolManager = PoolManager.Instance;
        //agentSelector = AgentSelector.Instance;
        playerStats = PlayerStats.Instance;

        if (playerStats != null)
        {
            playerStats.goldChanged += UpdateButtons;
        }

        for (int i = 0; i < AgentSelector.Instance.selectedAgents.Count; i++)
        {
            agentButtonsImageList[i].sprite = AgentSelector.Instance.selectedAgents[i].GetComponent<Unite>().icon;
            CDList.Add(AgentSelector.Instance.selectedAgents[i].GetComponent<Unite>().CDBetweenSpawns);
            priceList.Add(AgentSelector.Instance.selectedAgents[i].GetComponent<Unite>().cost);
            onCDList.Add(false);
        }
    }


    private void Update()
    {
        int i = 0;

        foreach (Button button in agentButtonsList)
        {
            if (onCDList[i])
            {
                CDList[i] -= Time.deltaTime;
                agentButtonsTextsList[i].text = string.Format("{0:00.00}", CDList[i]);

                if (CDList[i] <= 0)
                {
                    onCDList[i] = false;
                    CDList[i] = AgentSelector.Instance.selectedAgents[i].GetComponent<Unite>().CDBetweenSpawns;
                    agentButtonsTextsList[i].text = null;
                    if (PlayerStats.Instance.currentMoney > priceList[i])
                    {
                        button.interactable = true;
                    }
                }
            }
            i++;
        }
    }

    public void SpawnUnite(int indexUniteToSpawn)
    {
        foreach (NetworkPlayer player in NetworkManager.Instance.connectedPlayers)
        {
            if (player.hasAuthority)
            {
                player.CmdSpawnFromPool(indexUniteToSpawn, player.spawnPoint.position);
            }
        }

        onCDList[indexUniteToSpawn] = true;
        agentButtonsList[indexUniteToSpawn].interactable = false;

        PlayerStats.Instance.ChangeMoney(-priceList[indexUniteToSpawn]);
    }



    public void UpdateButtons()
    {
        int i = 0;

        foreach (Button button in agentButtonsList)
        {
            if (PlayerStats.Instance.currentMoney < priceList[i])
            {
                if (button.interactable == true)
                    button.interactable = false;
            }
            else if (onCDList[i] == false)
            {
                if (button.interactable == false)
                    button.interactable = true;
            }

            i++;
        }
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.goldChanged -= UpdateButtons;
        }
        else
            Debug.LogWarning("Can't Unsub goldChanged event" + this);
    }
}
