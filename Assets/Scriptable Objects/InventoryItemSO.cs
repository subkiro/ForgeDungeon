using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Scriptable Objects/InventoryItemSO")]
public abstract class InventoryItemSO : ScriptableObject
{
    public string ID;
    public string DisplayName;

    public Sprite Icon;
    public Sprite Background;

    public List<Stats> BasicStats;
}
