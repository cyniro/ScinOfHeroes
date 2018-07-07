using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// A helper component for self destruction
/// </summary>
public class DisableByTime : MonoBehaviour
{
    [Tooltip("Rather than use timer, will use wait the end of and animation")]
    public bool destroyAfterAnimation;

    /// </summary>
    ///Internal reference to the effect's animator.
    /// </summary>
    private Animator m_MyAnimator;

    /// <summary>
    /// The time before destruction
    /// </summary>
    public float time = 5;

    /// <summary>
    /// The controlling timer
    /// </summary>
    public Timer timer;

    /// <summary>
    /// The exposed death callback
    /// </summary>
    public UnityEvent death;

    protected virtual void Start()
    {
        m_MyAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Potentially initialize the time if necessary
    /// </summary>
    protected virtual void OnEnable()
    {
        if (destroyAfterAnimation)
        {
            if (m_MyAnimator != null)
                return;
        }

        if (timer == null)
        {
            timer = new Timer(time, OnTimeEnd);
        }
        else
        {
            timer.Reset();
        }
    }

    /// <summary>
    /// Update the timer
    /// </summary>
    protected virtual void Update()
    {
        if (destroyAfterAnimation)
        {
            if (m_MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (timer == null)
            {
                return;
            }
            timer.Tick(Time.deltaTime);
        }

    }

    /// <summary>
    /// Fires at the end of timer
    /// </summary>
    protected virtual void OnTimeEnd()
    {
        death.Invoke();
        PoolManager.Instance.poolDictionnary[gameObject.name].UnSpawnObject(gameObject);
        timer.Reset();
    }
}
