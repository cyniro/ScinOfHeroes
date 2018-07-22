using UnityEngine;
using Towers;

/// <summary>
/// UI component that displays towers that can be built on this level.
/// </summary>
public class BuildSidebar : MonoBehaviour
{
    /// <summary>
    /// The prefab spawned for each button
    /// </summary>
    public TowerSpawnButton towerSpawnButton;

    /// <summary>
    /// Initialize the tower spawn buttons
    /// </summary>
    protected virtual void Start()
    {
        if (GameManager2.Instance == null)
        {
            Debug.LogError("[UI] No GameManager for tower list");
        }
        foreach (Tower tower in GameManager2.Instance.towerLibrary)
        {
            TowerSpawnButton button = Instantiate(towerSpawnButton, transform);
            button.InitializeButton(tower);
            button.buttonTapped += OnButtonTapped;
            button.draggedOff += OnButtonDraggedOff;
        }
    }

    /// <summary>
    /// Sets the GameUI to build mode with the <see cref="towerData"/>
    /// </summary>
    /// <param name="towerData"></param>
    void OnButtonTapped(Tower towerData)
    {
        var gameUI = GameUI.Instance;
        if (gameUI.isBuilding)
        {
            gameUI.CancelGhostPlacement();
        }
        gameUI.SetToBuildMode(towerData);
    }

    /// <summary>
    /// Sets the GameUI to build mode with the <see cref="towerData"/> 
    /// </summary>
    /// <param name="towerData"></param>
    void OnButtonDraggedOff(Tower towerData)
    {
        if (!GameUI.Instance.isBuilding)
        {
            GameUI.Instance.SetToDragMode(towerData);
        }
    }

    /// <summary>
    /// Unsubscribes from all the tower spawn buttons
    /// </summary>
    void OnDestroy()
    {
        TowerSpawnButton[] childButtons = GetComponentsInChildren<TowerSpawnButton>();

        foreach (TowerSpawnButton towerButton in childButtons)
        {
            towerButton.buttonTapped -= OnButtonTapped;
            towerButton.draggedOff -= OnButtonDraggedOff;
        }
    }
}