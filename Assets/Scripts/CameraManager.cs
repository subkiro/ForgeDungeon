using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{




    [SerializeField] Camera m_MainCamera;
    public Camera MainCamera { set; get; }
    public CinemachineBrain MainCameraBrain { set; get; }

    [SerializeField] CinemachineCamera m_BaseCamera;


    public CinemachineCamera BaseCamera => m_BaseCamera;



    [SerializeField] AnimationCurve m_CameraEase;




    public void Initialize()
    {
         MainCamera = m_MainCamera ? m_MainCamera : Camera.main;
        MainCameraBrain = MainCamera.GetComponent<CinemachineBrain>();
        Application.targetFrameRate = 120;


    }


    [SerializeField] float duration, strengh;
    public void ShakeCamera(float duration = .3f, float strength = 1f, UnityAction OnComplete = null)
    {

        DOTween.Kill(this, true);
        Sequence s = DOTween.Sequence();
        s.SetId(this);

        s.Append(MainCamera.transform.DOShakePosition(duration, strength, vibrato: 4, randomness: 90).SetEase(Ease.InOutSine));
        s.Append(MainCamera.transform.DOShakeRotation(duration, Vector3.forward * strength / 2, vibrato: 4, randomness: 90).SetEase(Ease.InOutSine));
        s.Join(MainCamera.DOOrthoSize(-Random.Range(0.2f, 1f), duration / 2).SetLoops(2, LoopType.Yoyo).SetRelative().SetEase(Ease.InOutSine));
        s.OnComplete(() =>
        {

            OnComplete?.Invoke();
        });


    }
    public void PunchCamera(float duration = .3f, float strength = 1f, UnityAction OnComplete = null)
    {

        DOTween.Kill(this, true);
        MainCamera.DOOrthoSize(strength, duration)
            .SetLoops(2, LoopType.Yoyo)
            .SetRelative()
            .OnComplete(() =>
            {
                OnComplete?.Invoke();
            });



    }
    [Button]
    public void ShakeCameraImpulse(float strength = 1)
    {
        CinemachineImpulseSource impulse = this.GetComponent<CinemachineImpulseSource>();
        impulse.GenerateImpulse(strength);
    }





}

