
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public struct Stats
{
    [GUIColor("GetButtonColor")]
    public StatType Stat_Type;
    public float Stat_Value;
    public bool isConstant;

    public void Print()
    {
        string constantText = isConstant ? " (Constant)" : "";
        Debug.Log($"<color={GetButtonColor()}>{Stat_Type}</color>: {Stat_Value}{constantText}");
    }
    public Color GetButtonColor() {
        switch (Stat_Type)
        {
            case StatType.BattaryCapacity:
                return Color.red;
            case StatType.DigSpeed:
                return Color.cyan;
            
            default:
                return Color.white;
        }
    }

}
public enum StatType { BattaryCapacity, DamagePower, DigSpeed, AttackSpeed, XP,Coin,Empty }
