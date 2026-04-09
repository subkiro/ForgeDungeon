using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBeatListener : MonoBehaviour, IEqualizable
{
    [field: SerializeField] public int Band { set; get; }
    [field: SerializeField] public float Multiplier { set; get; }
    [field: SerializeField] public float LerpSpeed { set; get; }
    public bool Active { set; get; }


    [SerializeField] Transform Target;
    [SerializeField] bool UseBuffer = true;
    [SerializeField] bool UseAmplitude = false;


    private Vector3 initScale;
  

    private void Awake()
    {
        if (Target == null) Target = this.transform;

        initScale =Target.localScale;


    }
    private void LateUpdate()
    {
        var scaleValue = UseAmplitude? SoundEqualizer.GetDataAmplitute(UseBuffer): SoundEqualizer.GetData(band: Band, UseBuffer);
        var targetScale = new Vector3(scaleValue * Multiplier + initScale.x, scaleValue * Multiplier + initScale.y, initScale.z);
        Target.localScale = Vector3.Lerp(Target.localScale, targetScale, LerpSpeed*Time.deltaTime) ;
    }

    public void SetDefaults()
    {
        Target.localScale = initScale;
    }
    public void OnEqualizerStatusChanged(bool active)
    {
        this.Active = active;
        if (!Active)
        {
            SetDefaults();
        }
    }

    private void OnEnable()
    {
        SoundEqualizer.OnSounEqualizerActive += OnEqualizerStatusChanged;

    }
    private void OnDisable()
    {
        SoundEqualizer.OnSounEqualizerActive -= OnEqualizerStatusChanged;

    }
}
