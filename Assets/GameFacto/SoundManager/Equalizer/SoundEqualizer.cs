using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundEqualizer : MonoBehaviour
{
    public static UnityAction<bool> OnSounEqualizerActive; 

    public FFTWindow window = FFTWindow.Blackman;
    [Range(0, 0.1f)]
    public float filter1 = 0.05f;
    [Range(0, 5f)]
    public float filterMultipliser = 1.2f;

    [Range(5,8)] public int _power = 8;
    public static float GetData(int band, bool useBuffer = true) {
       
        return useBuffer? m_audioBand[band]: m_audioBandBuffer[band];
    }

    public static float GetDataAmplitute( bool useBuffer = true)
    {
        return useBuffer ? Amplitute : AmplitudeBuffer;
    }
    private static float[] samples; 
    private static float[] freqBand;
    private static float[] bandBuffer;
    private float[] m_bufferDecrease;


    private static  float[] m_freqBandHighest;
    private static float[] m_audioBand;
    private static float[] m_audioBandBuffer;

    private static float Amplitute, AmplitudeBuffer;
    private static float m_AmplituteHighest;

    private bool isActive;

    public bool IsActive => isActive;
    private  void Awake()
    {

        var _poweter = (int)Mathf.Pow(2f, _power)*2;
        samples = new float[_poweter];
        freqBand = new float[_power];
        bandBuffer = new float[_power];
        m_bufferDecrease = new float[_power];


        m_freqBandHighest = new float[_power];
        m_audioBand = new float[_power];
        m_audioBandBuffer = new float[_power];

        isActive = false;
    }

    private void Start()
    {
       // OnVolumeChanged(GameManager.Instance.SettingsManager.MusicVolume);

    }
    private void Update()
    {
        if (!isActive) return;
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitute();
    }

    void CreateAudioBands() {
        for (int i = 0; i < _power; i++)
        {
            if (freqBand[i] > m_freqBandHighest[i]) {
                m_freqBandHighest[i] = freqBand[i];
            }

            m_audioBand[i] = (freqBand[i] / m_freqBandHighest[i]);
            m_audioBandBuffer[i] = (bandBuffer[i] / m_freqBandHighest[i]);
            
        }
    }
    void GetSpectrumAudioSource() {
        AudioListener.GetSpectrumData(samples, 0, window);
    }

    void GetAmplitute() {

        float curretnAmplitude = 0;
        float currentAmplitudeBuffer = 0;
        for (int i = 0; i < _power; i++)
        {
            curretnAmplitude += m_audioBand[i];
            currentAmplitudeBuffer += m_audioBandBuffer[i];
        }

        if (curretnAmplitude > m_AmplituteHighest) {
            m_AmplituteHighest = curretnAmplitude;
        }

        Amplitute = curretnAmplitude / m_AmplituteHighest;
        AmplitudeBuffer = currentAmplitudeBuffer / m_AmplituteHighest;
    }
    void BandBuffer() {
        for (int i = 0; i < _power; i++)
        {
            if (freqBand[i] > bandBuffer[i]) { 
                bandBuffer[i] = freqBand[i];
                m_bufferDecrease[i] = filter1;
            }

            if (freqBand[i] < bandBuffer[i])
            {
                bandBuffer[i] -= m_bufferDecrease[i];
                m_bufferDecrease[i] *= filterMultipliser;
            }
        }
    }
    void MakeFrequencyBands() {
        /*22050 / 512 = 43herz per sample
         * 
         * 20-60 herz
         * 250-500
         * 500-2000
         * 2000-4000
         * 4000-6000
         * 6000-20000
         * 
         * 0 - 2 = 46herz
         * 1 - 4 = 172 herz -> 87-258 herz
         * 2 - 8 = 344 herz -> 259-602 herz
         * 3 - 16 = 688 herz -> 603-1290 herz
         * 4 - 32 = 1376 herz -> 1291-2666 herz
         * 5 - 64 = 2752 herz -> 2667-5418 herz
         * 6 - 128 = 5504 herz -> 5419-10922 herz
         * 7 - 256 = 11008 herz -> 10923-21930 herz
         * 
         * total samples 510
         */ 

        int count = 0;
        float average = 0;
        for (int i = 0; i < _power; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if (i == 7) {sampleCount += 2;}


            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count]*(count+1);
                count++;
            }

            average /= count;
            freqBand[i] = average*10;
        }


    }



    private void OnEnable()
    {
        SettingsManager.OnMusicVolumeChanged += OnVolumeChanged;
    }
    private void OnDisable()
    {
        SettingsManager.OnMusicVolumeChanged -= OnVolumeChanged;
    }
    private void OnVolumeChanged(bool MusicIsPlaying)
    {
        isActive = MusicIsPlaying;
        OnSounEqualizerActive?.Invoke(isActive);
    }
}

public interface IEqualizable {
   public int Band { set; get; }
   public float Multiplier { set; get; }
   public bool Active { set; get; }
   public float LerpSpeed { set; get; }

   public void SetDefaults();
   public void OnEqualizerStatusChanged(bool active);
}
