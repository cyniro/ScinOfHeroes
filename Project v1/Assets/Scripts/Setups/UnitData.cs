using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "PoolData/Unit", order = 1)]
public class UnitData : ScriptableObject
{
    public List<GameObject> objectToPool = new List<GameObject>();
}
