using System;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Toggle))]
public class InventoryItemCell_Toggle : MonoBehaviour
{
    public Toggle Toggle {get;set;}
    public ItemCellView ItemCellView {set;get;}

    void Awake()
    {
        Toggle = GetComponent<Toggle>();
        ItemCellView = GetComponentInChildren<ItemCellView>();
    }
    public void Initialize(InventoryItemSO itemData)
    {
        ItemCellView.Initialize(itemData);
    }

}
