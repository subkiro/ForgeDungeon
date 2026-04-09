using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RewardUiItem : MonoBehaviour
{
    public TMP_Text AmountText;
    public Image RewardIcon;
    public RewardType RewardType;
    public RewardTypeData RewardData;

    public void SetData(RewardTypeData rewardData)
    {
        RewardData = rewardData;
        AmountText.text = rewardData.Amount.ToString();
        RewardIcon.sprite = rewardData.Icon;
        RewardType = rewardData.RewardType;
    }

}
