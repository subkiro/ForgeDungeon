using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaritySettings", menuName = "Balancingparameters/RaritySettingsSO")]
public class RaritySettingsSO : ScriptableObject
{
    public List<RarityData> RarityParameters = new List<RarityData>();
}
