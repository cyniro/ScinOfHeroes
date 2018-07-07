using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Towers : DamageableBehaviour, IDamageable
{

    [Header("Setup")]
    public Targetter targetter;

    protected virtual void Start()
    {
        //This is for test purpose, normally this shoud be done via the script that build this tower
        configuration.SetHealth(configuration.startingHealth);
    }

    /// <summary>
    /// Return the Effect's Fx List that runs on this units
    /// </summary>
    public List<GameObject> GetEffectFxList()
    {
        Debug.LogWarning("Towers dont have EffectFxList, you dumbass !!!");
        return null;
    }

    /// <summary>
    /// Returns a targetter that implement ITowerRadiusVisualizer
    /// </summary>
    /// <returns>ITowerRadiusVisualizers of tower</returns>
    public ITowerRadiusProvider GetRadiusVisualizer()
    {
        ITowerRadiusProvider visualizer = targetter as ITowerRadiusProvider;
        if (visualizer != null)
        {
            return visualizer;
        }
        return null;
    }

    protected virtual void OnDisable()
    {

    }

    public Alignement GetAlignement()
    {
        return targetter.alignement;
    }

    #region IDamageable Methode Useless
    public void AddBuff(string buffName, float value, BuffType buffType)
{
    Debug.LogWarning("Towers dont get Buffs, you dumbass !!!");
}

public void RemoveBuff(string buffName, float value, BuffType buffType)
{
    Debug.LogWarning("Towers dont get Buffs, you dumbass !!!");
}

public Dictionary<string, List<float>> GetASBoostsDictionary()
{
    Debug.LogWarning("Methode not supposed to be called on a tower");
    return null;
}

public Dictionary<string, List<float>> GetMSBoostsDictionary()
{
    Debug.LogWarning("Methode not supposed to be called on a tower");
    return null;
}

public Dictionary<string, List<float>> GetResiBoostsDictionary()
{
    Debug.LogWarning("Methode not supposed to be called on a tower");
    return null;
}
public void SetFx(GameObject fxPrefab)
{
    Debug.LogWarning("Methode not supposed to be called on a tower");

}
public void RemoveFx(GameObject fxPrefab)
{
    Debug.LogWarning("Methode not supposed to be called on a tower");
}
    #endregion

}
