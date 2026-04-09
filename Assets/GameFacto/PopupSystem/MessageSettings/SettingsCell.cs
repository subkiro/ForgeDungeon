using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SettingsCell : MonoBehaviour
{

    public SettingsType type;
    public Image icon;
    public TMP_Text title;
    public Toggle toggle;
    public Image BG_Icon;
    public Color BG_On;
    public Color BG_Off;
    public Image Handle;


    public enum SettingsType
    {
        SOUND, MUSIC, VIBRATION
    }

}

