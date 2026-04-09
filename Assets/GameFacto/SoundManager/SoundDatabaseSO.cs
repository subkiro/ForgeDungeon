using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
[CreateAssetMenu(menuName = "Scriptable Objects/Create SoundDatabase SO")]

public class SoundDatabaseSO : ScriptableObject
{
    [ListDrawerSettings(ShowPaging =false)]
    public List<SoundData> AllSounds;

    public SoundData GetSoundID(string id)
    {
        var sound = AllSounds.Find(x => x.ID == id);
        return sound;
    }

   
    [Serializable]
    public struct SoundData
    {



        [HorizontalGroup, GUIColor(@"GetColor"), HideLabel]
        public string ID;
        [HorizontalGroup, SerializeField] bool UseList;
        [HorizontalGroup, SerializeField,LabelWidth(20)] public  float Vol ;
        [HorizontalGroup, SerializeField, ShowIf("UseList"), HideLabel]
        public List<AudioClip> m_ClipList;
        [HorizontalGroup, HideIf("UseList"), SerializeField,     InlineEditor(InlineEditorModes.SmallPreview), HideLabel]

        AudioClip m_Clip;
        public AudioClip Clip
        {
            get
            {
                if (UseList && m_ClipList != null && m_ClipList.Count > 0)
                {
                    int index = UnityEngine.Random.Range(0, m_ClipList.Count);
                    return m_ClipList[index];
                }
                else
                    return m_Clip;
            }

        }
        [SerializeField, HorizontalGroup, HideLabel, LabelWidth(30)]
        private ColorTag Tag;
        public enum ColorTag { 
         W, Y, G, C,R
        }
        Color GetColor() {

            switch (Tag)
            {
                case ColorTag.W:
                    return Color.white;
                case ColorTag.Y:
                    return Color.yellow;
                case ColorTag.G:
                    return Color.green;
                case ColorTag.C:
                    return Color.cyan;
                case ColorTag.R:
                    return Color.red;
                default:
                    return Color.white;
            }
        }
        
    }



}
