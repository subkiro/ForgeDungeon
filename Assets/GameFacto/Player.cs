

using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static UnityAction<int> OnCoinsUpdated; 
    public string PlayerID;
    public string PlayerName;




    public int m_Coins;
    public int Coins => m_Coins;

    private bool m_Initialise;

    public void InitialisePlayer()
    {
        PlayerID =  "Player";
        PlayerName =  "Player";
        UpdateCoins(PlayerPrefs.HasKey("Coins") ? PlayerPrefs.GetInt("Coins") : GameManager.Instance.GameConstants.PlayerInitSettings.Coins);
        
        
        PlayerPrefs.Save();

        m_Initialise = true;
    }

    
   
    public void UpdateInventory(RewardType type, int value)
    {
        switch (type)
        {
            case RewardType.COIN:
                UpdateCoins(value);
                break;

        }
    }

    





    #region Inventory
    public bool CanBuy(int cost)
    {
        return cost > m_Coins ? false : true;
    }

   
   
   
   private void UpdateCoins(int coins)
    {
        Debug.LogWarning("Updating COINS");
        m_Coins += coins;
        if (m_Coins < 0) m_Coins = 0;
        PlayerPrefs.SetInt("Coins", m_Coins);
        PlayerPrefs.Save();
        OnCoinsUpdated?.Invoke(m_Coins);
    }
   




    #endregion


    #region Debug 
    [ContextMenu("Reset DailySpinTickets")]
    public void Debug_DailySpinTickets() {
        PlayerPrefs.DeleteKey("DailySpinTickets");
    }
    #endregion

}
