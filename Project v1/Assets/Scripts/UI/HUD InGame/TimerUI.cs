using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class for controlling the displaying of current time since start of the game
/// </summary>
public class TimerUI : MonoBehaviour
{

    /// <summary>
    /// The text element to display information on
    /// </summary>
    public Text display;

    float startTime;

    /// <summary>
    /// Assign the correct currency value
    /// </summary>
    protected virtual void Start()
    {
        startTime = Time.time;
    }

    /// <summary>
    /// A method for updating the display based on the current currency
    /// </summary>
    protected void Update()
    {
        float time = Time.time - startTime;

        string minutes = ((int)time / 60).ToString("00");
        string seconds = ((int)time % 60).ToString("00");
        if (seconds == "60")
            seconds = "00";
        display.text = minutes + ":" + seconds;
    }
}