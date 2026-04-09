using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialSequenceHelper : MonoBehaviour
{
    public GameObject AimPowerAnim, AimAngleAnim;
    GameObject m_FocusPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (m_FocusPrefab == null)
        {
            //m_FocusPrefab = PrefabManager.Instance.PrefabList.MessageTeacheingFocus;
        }
    }




    private void HideAimAnims()
    {
        AimPowerAnim.SetActive(false);
        AimAngleAnim.SetActive(false);
    }



    public void ShowTutorialDragPowerFocus()
    {
        MessageTeachingFocus message = PopUpManager.Instance.ShowSimple<MessageTeachingFocus>(m_FocusPrefab);

        string loca = "";//
       
        message.AddSpeechData(loca, TextAnchor.UpperLeft, spriteIdex: 2)
               .AddOnFinish((state) =>
               {
                   switch (state)
                   {
                       case MessageTeachingFocus.CompleteState.NormalCompleted:
                           ShowTutorialPowerAngleDetailed();
                           break;
                       case MessageTeachingFocus.CompleteState.Skipped:
                           ShowTutorialPowerAngleSimple();
                           break;
                   }
               })
               .AddTransparency(0f)
               .ShowPopUp();



    }

    public void ShowTutorialPowerAngleSimple()
    {
        MessageTeachingFocus message = PopUpManager.Instance.ShowSimple<MessageTeachingFocus>(m_FocusPrefab);
        message.AddTransparency(0f, false, false)
               .AddOnStart(() =>
               {
                   //GameManager.LogWarning("START ANGLE ANIM");
                   //AimAngleAnim.SetActive(true);
                   //MainTutorialManager.Instance.ReadyToShoot(0);
                   //MainTutorialManager.Instance.StartCheckForAngleTask();
                   //EventManager.StartListening("FailedTutorialAngleTask", () => AimAngleAnim.SetActive(true));

               })
              .AddEventListener("SucceedTutorialAngleTask")
              .AddOnFinish((compleState) =>
              {
                  //GameManager.LogWarning("FINISHED ANGLE");
                  //MainTutorialManager.Instance.JumpTaskTo(11);
                  //AimAngleAnim.SetActive(false);
                  //EventManager.StopListening("FailedTutorialAngleTask", () => AimAngleAnim.SetActive(true));
                  //EventManager.StopListening("HideAimAnims", HideAimAnims);
              })
              .ShowPopUp();


    }

    public void ShowTutorialPowerAngleDetailed()
    {
        MessageTeachingFocus message = PopUpManager.Instance.ShowSimple<MessageTeachingFocus>(m_FocusPrefab);
        string loca = "";//

        message.AddSpeechData(loca)
              .AddTransparency(0f, false, false)
              .AddOnStart(() =>
              {
                  Tools.LogWarning("START POWER ANIM");
                  AimPowerAnim.SetActive(true);
                  //MainTutorialManager.Instance.ReadyToShoot(0);
                  //MainTutorialManager.Instance.StartCheckForPowerTask();
                  //EventManager.StartListening("FailedTutorialPowerTask", () =>
                  //{
                  //    ToolsGamesNGames.LogWarning("FAILED POWER");
                  //    AimPowerAnim.SetActive(true);
                  //});
              })
              .AddEventListener("SucceedTutorialPowerTask")
              .AddOnFinish((state) =>
              {
                  AimPowerAnim.SetActive(false);
                  //GameManager.LogWarning("FINISH POWER ANIM");
                  //EventManager.StopListening("FailedTutorialPowerTask", () => AimPowerAnim.SetActive(true));
              })
              .ShowPopUp(true);


        string loca2 = "";//
       message.AddSpeechData(loca2)
              .AddTransparency(0f, false, false)
              .AddOnStart(() =>
              {
                  //GameManager.LogWarning("START ANGLE ANIM");

                  //MainTutorialManager.Instance.ReadyToShoot(0);
                  //MainTutorialManager.Instance.StartCheckForAngleTask();
                  //EventManager.StartListening("FailedTutorialAngleTask", () => AimAngleAnim.SetActive(true));

              })
              .AddEventListener("SucceedTutorialAngleTask")
              .AddOnFinish((state) =>
              {
                  //GameManager.LogWarning("FINISHED ANGLE");
                  //MainTutorialManager.Instance.JumpTaskTo(11);
                  //AimAngleAnim.SetActive(false);
                  //EventManager.StopListening("FailedTutorialAngleTask", () => AimAngleAnim.SetActive(true));
                  //EventManager.StopListening("HideAimAnims", HideAimAnims);
              })
              .ShowPopUp(true);

        message.ShowPopUpQueue();
    }

}
