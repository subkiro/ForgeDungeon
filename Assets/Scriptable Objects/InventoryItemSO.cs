using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Scriptable Objects/InventoryItemSO")]
public abstract class InventoryItemSO : ScriptableObject
{
    public string name;

    public Sprite Icon;
    public Sprite Background;

    public List<Stats> BasicStats;
}

[CreateAssetMenu(fileName = "Material Scriptable", menuName = "Scriptable Objects/Material Scriptable")]
public class RawResourceSO : InventoryItemSO
{

}



[CreateAssetMenu(fileName = "Material Scriptable", menuName = "Scriptable Objects/Material Scriptable")]
public class EndProductSO : InventoryItemSO
{

}
