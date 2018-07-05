using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    /// <summary>
    /// Display the cost of the unit
    /// </summary>
    public Text UnitCostText;

    /// <summary>
    /// Display the Icon of the unit
    /// </summary>
    public Image unitIcon;

    /// <summary>
    /// To catch the button for sets his intaractability
    /// </summary>
    public Button buyButton;

    /// <summary>
    /// The image that will mask the button until cd is gone
    /// </summary>
    public Image cdFillImage;

    /// <summary>
    /// Display a countdow set on the unit CD
    /// </summary>
    public Text cdFillText;

    /// <summary>
    /// Use to spawn from the agent selector index, the correct unit
    /// </summary>
    [HideInInspector]
    public int agentSelectorIndex;
    /// <summary>
    /// Use to store the unit script of the unit to get accès to his variables
    /// </summary>
    [HideInInspector]
    public Unite unit;
}
