using DG.Tweening;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

public enum MusicType
{
    MainMenu, InGame,Tutorial
}
public class SoundManager : MonoBehaviour
{
    public AudioClip DefaultSound;
    [InlineEditor]
    public SoundDatabaseSO SoundDatabase;

    public AudioMixerGroup AudioMaster;
    public AudioMixerGroup AudioSfx;
    public AudioMixerGroup AudioMusic;
    public float _multiplier = 30; //How fast the volume increase and decrease with slider

    [Header("MUSIC")]
    public AudioSource MusicAudioSource;
    [SerializeField] private MusicTypeData[] MusicData;
    //public AudioClip MainMenuMusicClip;
    //public AudioClip BattleMusicClip;
    //public AudioClip TutorialMusicClip;

    public enum AudioMixerGroups { Master, Sfx, Music }

  

    public void Initialize()
    {
        Prepare(maxPoolingObjects);
    }

    #region POOLING
    public int maxPoolingObjects = 20;
    Queue<AudioSource> soundQueue = new Queue<AudioSource>();
   

    public void Prepare(int maxAmount)
    {


        for (int i = 0; i < maxAmount; i++)
        {
            GameObject g = new GameObject("soundPool");
            g.transform.SetParent(this.transform);
            g.SetActive(false);
            AudioSource source = g.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.rolloffMode = AudioRolloffMode.Logarithmic;  
            soundQueue.Enqueue(source);
        }

    }
    #endregion

    public void PlayGivenSound(AudioClip clip, AudioMixerGroups masterIndex = AudioMixerGroups.Sfx, float volume = 0.5f, float pitch = 1)
    {
        if (clip == null)
        {
            Tools.Log($" <color=red>Sound Clip ref</color> is Null");
            return;
        }

        _=CreateAudio(clip, (int)masterIndex,  volume, pitch: pitch);
    }

    public void PlayGivenSound(string soundID="", AudioMixerGroups masterIndex = AudioMixerGroups.Sfx,  float volume = 0.5f, float pitch = 1)
    {
        SoundDatabaseSO.SoundData sound = SoundDatabase.GetSoundID(soundID);
        if (sound.Clip==null)
        {         
            Tools.Log($"<color=yellow> {soundID}</color> <color=red>Sound Clip ref</color> is Null");
            return;
        }
        AudioClip clip = sound.Clip;

        _=CreateAudio(clip,(int)masterIndex, sound.Vol* volume, pitch: pitch);
    }
   

    public async Awaitable CreateAudio(AudioClip audioClip, int masterIndex, float volume, float pitch = 1)
    {
        if (audioClip == null) { Tools.Log("Audio clip is null, please check the sound", Color.red); return; }

        if (soundQueue.Count > 0)
        {

            AudioSource audioSource = soundQueue.Dequeue();

            audioSource.gameObject.SetActive(true);

            if (masterIndex == 0)
                audioSource.outputAudioMixerGroup = AudioMaster;
            else if (masterIndex == 1)
                audioSource.outputAudioMixerGroup = AudioSfx;
            else if (masterIndex == 2)
                audioSource.outputAudioMixerGroup = AudioMusic;


            audioSource.clip = audioClip;
            audioSource.loop = false;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();


            await Awaitable.WaitForSecondsAsync(audioClip.length);
            audioSource.gameObject.SetActive(false);
            soundQueue.Enqueue(audioSource);
          


        }
    }

    public void Mute()
    {
        SetMusicVolume(0.0001f);
        SetSoundVolume(0.0001f);
    }

    public void UnMute()
    {
        SetMusicVolume(0.8f);
        SetSoundVolume(0.8f);
    }

    public void SetMasterVolume(float vol)
    {
        if (vol == 0) vol = 0.0001f;
        AudioMusic.audioMixer.SetFloat("Exposed_MasterParam", Mathf.Log10(vol) * _multiplier);

    }

    public void SetMusicVolume(float vol)
    {
        if (vol == 0) vol = 0.0001f;
        AudioMusic.audioMixer.SetFloat("Exposed_MusicParam", Mathf.Log10(vol) * _multiplier);

    }

    public void SetSoundVolume(float vol)
    {
        if (vol == 0) vol = 0.0001f;
        AudioSfx.audioMixer.SetFloat("Exposed_SoundParam", Mathf.Log10(vol) * _multiplier);
    }

    public async Awaitable PlayMusic(MusicType musicType, float toVolume = .1f)
    {

        await StopMusic(0.3f).AsyncWaitForCompletion();
        AudioClip clip = MusicData.First(x => x.MusicType == musicType).MusicClip;

        if (clip == null) return;

        MusicAudioSource.clip = clip;
        MusicAudioSource.volume = 0;
        MusicAudioSource.Play();


        await MusicAudioSource.DOFade(toVolume, 1f).SetId("Music").ToAwaitable();
    }

    public Tween StopMusic(float speed = 1)
    {

      if(DOTween.IsTweening("Music"))  DOTween.Kill("Music");
      
      return MusicAudioSource.DOFade(0, speed).OnComplete(() => { MusicAudioSource.Stop(); });
    }



    [Serializable]
    public struct MusicTypeData {
        public MusicType MusicType;
        public AudioClip MusicClip;
    }

   
}
