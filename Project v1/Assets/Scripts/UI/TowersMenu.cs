using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowersMenu : MonoBehaviour
{
    private BuildManager buildManager;
    private PlayerStats playerStats;

    public List<TurretBlueprint> bluePrintList;
    public List<Button> buttonList;

    public GameObject arrow;
    public GameObject inverseArrow;
    public Animator anim;

    void Start()
    {
        buildManager = BuildManager.Instance;
        playerStats = PlayerStats.Instance;

        if (playerStats != null)
        {
            playerStats.goldChanged += UpdateButtons;
        }
    }

    public void SelectTurretToBuild(int index)
    {
        Debug.Log("Standard Turret Selected");
        buildManager.SelectTurretToBuild(bluePrintList[index]);
    }


    public void OpenMenu()
    {
        anim.SetTrigger("OpenTowersMenu");
        arrow.SetActive(false);
        inverseArrow.SetActive(true);
    }


    public void CloseMenu()
    {
        anim.SetTrigger("CloseTowersMenu");
        arrow.SetActive(true);
        inverseArrow.SetActive(false);
    }

    public void UpdateButtons()
    {
        int i = 0;

        foreach (Button button in buttonList)
        {
            if (PlayerStats.Instance.currentMoney < bluePrintList[i].cost)
            {
                if (button.interactable == true)
                    button.interactable = false;
            }
            else 
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
