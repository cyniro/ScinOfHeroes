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


    /// <summary>
    /// Toggle open or close tower menu
    /// </summary>
    /// <param name="_switch">bool for open/close menu</param>
    public void ToggleTowerMenu(bool _switch)
    {
        if(_switch)
        {
            anim.SetBool("IsOpen", _switch);
        }
        else
        {
            anim.SetBool("IsOpen", _switch);
        }
    }

    public void UpdateButtons()
    {
        int i = 0;

        foreach (Button button in buttonList)
        {
            if (PlayerStats.Instance.currentGold < bluePrintList[i].cost)
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
    }
}
