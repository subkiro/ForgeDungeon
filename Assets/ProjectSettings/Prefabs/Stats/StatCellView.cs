using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class StatCellView : MonoBehaviour
{
    [SerializeField] Image m_StatIcon;
    [SerializeField] TMP_Text m_StatText;
    [SerializeField] RewardType m_RewardType;

    public Image StatIcon=>m_StatIcon;
    public RewardType RewardType => m_RewardType;


    void Awake()
    {
        GameManager.OnInitializationComplete+=Initialize;
    }
    private void Initialize()
    {
        m_StatIcon.sprite = GameManager.Instance.AssetScriptableData.GetSprite(m_RewardType);

        switch (m_RewardType)
        {
            case RewardType.COIN:
                OnStatUpdated(GameManager.Instance.Player.Coins.Value,GameManager.Instance.Player.Coins.Value);
                break;
          
        }

        Subscrivbe();
    }
    private void OnStatUpdated(int valueOld,int valueNew) { 
        m_StatText.text = Tools.ShortNumeric(valueNew);
    }



    void Subscrivbe() {
        switch (m_RewardType)
        {
            case RewardType.COIN:
                GameManager.Instance.Player.Coins.Changed += OnStatUpdated;
                break;

        }
    }

    void UnSubscribe() {
        switch (m_RewardType)
        {
            case RewardType.COIN:
                GameManager.Instance.Player.Coins.Changed-= OnStatUpdated;
                break;
        }
    }


    private void OnDestroy()
    {
        UnSubscribe();
    }
}
