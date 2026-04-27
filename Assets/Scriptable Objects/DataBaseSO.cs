using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBaseSO", menuName = "Scriptable Objects/DataBaseSO")]
public class DataBaseSO : ScriptableObject
{
    public List<InventoryItemSO> AllInventoryItems
    {
        get
        {
            var items = new List<InventoryItemSO>();
            items.AddRange(MaterialItemsSOs);
            items.AddRange(RawResourceSOs);
            items.AddRange(EndProductSOs);

            return items;
        }
    }

    public List<T> InventoryItems<T>() where T : InventoryItemSO
    {
        return AllInventoryItems.OfType<T>().ToList();
    }
    

     public List<MaterialSO> MaterialItemsSOs=new();
     public List<RawResourceSO> RawResourceSOs=new();
     public List<EndProductSO> EndProductSOs=new();

}
