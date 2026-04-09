using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundBeatListenerColor : MonoBehaviour, IEqualizable
{
    [field: SerializeField] public int Band { set; get; }
    [field: SerializeField] public float Multiplier { set; get; }
    [field: SerializeField] public float LerpSpeed { set; get; }

    public bool Active { set; get; }

    [SerializeField] Transform objTarget;
     Image Target;
     SpriteRenderer TargetSR;
    [SerializeField] float startValue;
    [SerializeField] bool UseBuffer = true;
    [SerializeField] bool UseAmplitude = false;


    //Defautls
    private Color initColor;
    private void Awake()
    {

        if (objTarget == null) objTarget = this.transform;


        if (objTarget.TryGetComponent<Image>(out Target)) { }

        if (objTarget.TryGetComponent<SpriteRenderer>(out TargetSR)) { }

        if (Target != null)
        {
            initColor = Target.color;
        }
        if (TargetSR != null)
        {
            initColor = TargetSR.color;
        }


    }

    private void Start()
    {
       // OnEqualizerStatusChanged(GameManager.Instance.SoundManager.SoundEqualizer.IsActive);


    }

    private void Update()
    {
        if (!Active) return;
        var scaleValue = UseAmplitude ? SoundEqualizer.GetDataAmplitute(UseBuffer) : SoundEqualizer.GetData(band: Band, UseBuffer);

        if (Target != null)
        {
            Target.color = new Color(initColor.r, initColor.g, initColor.b, Mathf.Clamp01(startValue + scaleValue * Multiplier));

        }
        if (TargetSR != null)
        {
            TargetSR.color = new Color(initColor.r, initColor.g, initColor.b, Mathf.Clamp01(startValue + scaleValue * Multiplier));

        }
    }
    public void SetDefaults()
    {
        if (Target != null)
        {
            Target.color = initColor;
        }
        if (TargetSR != null)
        {
            TargetSR.color = initColor;

        }
    }
   public void OnEqualizerStatusChanged(bool active)
    {
        this.Active = active;
        if (!active) {
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
