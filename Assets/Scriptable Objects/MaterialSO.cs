using UnityEngine;

[CreateAssetMenu(fileName = "Material", menuName = "Scriptable Objects/MaterialSO")]
public class MaterialSO : InventoryItemSO
{
    public bool IsStackable;

    public double quality;

}
