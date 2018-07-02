using System.Collections.Generic;
using UnityEngine;

public class RadiusVisualizerController : MonoBehaviour
{
    /// <summary>
    /// Prefab used to visualize effect radius of tower
    /// </summary>
    public GameObject radiusVisualizerPrefab;

    public float radiusVisualizerHeight = 0.02f;

    /// <summary>
    /// The local euler angles
    /// </summary>
    public Vector3 localEuler;

    private GameObject m_RadiusVisualizers;

    /// <summary>
    /// Sets up the radius visualizer for a tower or ghost tower
    /// </summary>
    /// <param name="tower">
    /// The tower to get the data from
    /// </param>
    /// <param name="ghost">Transform of ghost to parent the visualiser to.</param>
    public void SetupRadiusVisualizers(Towers tower, Transform ghost = null)
    {
        // Create necessary affector radius visualizations
        ITowerRadiusProvider provider = tower.GetRadiusVisualizer();

        m_RadiusVisualizers = Instantiate(radiusVisualizerPrefab);

        GameObject radiusVisualizer = m_RadiusVisualizers;
        radiusVisualizer.SetActive(true);
        radiusVisualizer.transform.SetParent(ghost == null ? tower.transform : ghost);
        radiusVisualizer.transform.localPosition = new Vector3(0, radiusVisualizerHeight, 0);
        radiusVisualizer.transform.localScale = Vector3.one * provider.effectRadius * 2.0f;
        radiusVisualizer.transform.localRotation = new Quaternion { eulerAngles = localEuler };

        var visualizerRenderer = radiusVisualizer.GetComponent<Renderer>();
        if (visualizerRenderer != null)
        {
            visualizerRenderer.material.color = provider.effectColor;
        }
    }


    /// <summary>
    /// Hides the radius visualizer
    /// </summary>
    public void HideRadiusVisualizers()
    {

        m_RadiusVisualizers.transform.parent = transform;
        m_RadiusVisualizers.SetActive(false);
    }
}
