using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndProduct", menuName = "InventoryItems/EndProductOS")]
public class EndProductSO : InventoryItemSO
{
    [Header("End Product Variables")]
    [SerializeField]
    public List<BaseParameter> BaseParametersList = new List<BaseParameter>();

}

[System.Serializable]
public struct BaseParameter
{
    public string Key;
    public double Value;
}