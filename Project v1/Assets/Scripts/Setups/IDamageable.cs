using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Event that is fired when this instance is removed, such as when pooled or destroyed
    /// </summary>
    event Action<DamageableBehaviour> removed;

    Dictionary<string, List<float>> GetASBoostsDictionary();
    Dictionary<string, List<float>> GetMSBoostsDictionary();
    Dictionary<string, List<float>> GetResiBoostsDictionary();

    void TakeDamage(float damageValue, Vector3 damagePoint, IAlignmentProvider alignmen);
    void Heal(float amount);
    void AddBuff(string buffName, float value, BuffType buffType);
    void RemoveBuff(string buffName, float value, BuffType buffType);
    void SetFx(GameObject Fx);
    void RemoveFx(GameObject Fx);
}
