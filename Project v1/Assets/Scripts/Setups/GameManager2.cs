using UnityEngine;
using UnityEngine.Networking;

public class GameManager2 : NetworkBehaviour
{
    #region Singleton
    public static GameManager2 Instance;

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (waveManager == null)
        {
            Debug.LogWarning("waveManager is not fill" + this);
        }
        else
            waveManager.spawningCompleted += OnSpawningCompleted;
    }

    private void OnDisable()
    {
        Instance = null;
        waveManager.spawningCompleted -= OnSpawningCompleted;
    }
    #endregion

    public WaveManager waveManager;
    public GameObject gameOverUI;
    public GameObject completeLevelUI;

    /// <summary>
    /// The tower library for this level
    /// </summary>
    public TowerLibrary towerLibrary;


    public static bool GameIsOver;
    public int playerInGameScene;

    private void Start()
    {
        GameIsOver = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown("e"))
            GameOver();
    }

    public void GameOver()
    {
        if (GameIsOver)
            return;

        //gameOverUI.SetActive(true);
        GameIsOver = true;
    }

    public void WinLevel()
    {
        GameIsOver = true;
        completeLevelUI.SetActive(true);
    }

    public void InitPool()
    {
        Debug.Log("InitPool dans GM2");

        if (playerInGameScene >= 2)
        {
            foreach (NetworkPlayer player in NetworkManager.Instance.connectedPlayers)
            {
                if (player.hasAuthority)
                {
                    player.RegisterAgentSelectedList();
                }
            }
        }
    }

    private void OnSpawningCompleted()
    {
        //if we need to do a thing when all enemy waves spawned are completed
    }
}
