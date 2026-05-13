

using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public string PlayerID;
    public string PlayerName;

    private ReactiveStackableDictionary<InventoryItemSO> m_PlayerInventory = new ReactiveStackableDictionary<InventoryItemSO>();
    public ReactiveStackableDictionary<InventoryItemSO> PlayerInvetory=>m_PlayerInventory;





    public ReactiveVariable<int> Coins;

    private bool m_Initialise;

    public void InitialisePlayer()
    {
        PlayerID =  "Player";
        PlayerName =  "Player";
        int savedCoins = PlayerPrefs.HasKey("Coins") ? PlayerPrefs.GetInt("Coins") : GameManager.Instance.GameConstants.PlayerInitSettings.Coins;
        Coins = new ReactiveVariable<int>(savedCoins);
        Coins.Changed+=SaveCoins;
        
        PlayerPrefs.Save();

        m_Initialise = true;
    }

    
   

    





    #region Inventory
    public bool CanBuy(int cost)
    {
        return cost > Coins.Value ? false : true;
    }

   
    
   
   private void SaveCoins(int prevValue,int newValue)
    {
        Debug.LogWarning("Updating COINS");    
        PlayerPrefs.SetInt("Coins", newValue);
        PlayerPrefs.Save();
    }
   




    #endregion


    #region Debug 
    [ContextMenu("Reset DailySpinTickets")]
    public void Debug_DailySpinTickets() {
        PlayerPrefs.DeleteKey("DailySpinTickets");
    }
    #endregion

}
