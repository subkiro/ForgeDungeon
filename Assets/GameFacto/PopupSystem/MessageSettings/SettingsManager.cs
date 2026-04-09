
using UnityEngine;
using UnityEngine.Events;

public class SettingsManager : MonoBehaviour
{
    public static UnityAction<bool> OnMusicVolumeChanged;

    private const string m_SoundVolueID = "Settings_SoundVolue";
    private const string m_MusicVolueID = "Settings_MusicVolue";
    private const string m_VibrationsID = "Settings_Vibrations";

    private bool m_SoundVolue = true;
    private bool m_MusicVolue = true;
    private bool m_Vibrations = true;


    public bool SoundVolume
    {
        set
        {
            m_SoundVolue = value;
            PlayerPrefs.SetFloat(m_SoundVolueID, value ? 1 : 0);
            GameManager.Instance.SoundManager.SetSoundVolume(value?1:0);

        }
        get
        {
            float value = PlayerPrefs.GetFloat(m_SoundVolueID, 1);
            return value == 1 ? true : false;
        }
    }

    public bool MusicVolume
    {
        set
        {
            m_MusicVolue = value;
            PlayerPrefs.SetFloat(m_MusicVolueID, value ? 1 : 0);
            GameManager.Instance.SoundManager.SetMusicVolume(value ? 1 : 0);
            OnMusicVolumeChanged?.Invoke(value);
        }
        get
        {
            return PlayerPrefs.GetFloat(m_MusicVolueID, 1) == 1 ? true : false;
        }
    }

    public bool Vibrations
    {
        set
        {

            m_Vibrations = value;
            PlayerPrefs.SetFloat(m_VibrationsID, m_Vibrations ? 1 : 0);
            GameManager.Instance.HapticManager.HapticEnabled = value;
        }
        get
        {
            return PlayerPrefs.GetFloat(m_VibrationsID, 1) == 1 ? true : false;
        }
    }



    public void Initialize()
    {
        SoundVolume = SoundVolume;
        MusicVolume = MusicVolume;
        Vibrations = Vibrations;



    }





}