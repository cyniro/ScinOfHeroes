using UnityEngine;

public class BuildMenuAnim : MonoBehaviour {

    public Animator anim;

    /// <summary>
    /// Toggle open or close tower menu
    /// </summary>
    /// <param name="_switch">bool for open/close menu</param>
    public void ToggleTowerMenu(bool _switch)
    {
        if (_switch)
        {
            anim.SetBool("IsOpen", _switch);
        }
        else
        {
            anim.SetBool("IsOpen", _switch);
        }
    }
}
