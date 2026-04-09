using System.Collections;
using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public enum HapticType
    {
        HIT,
        DOUBLEHIT,
        POP,
        PEEK,
        NOPE
    }

    private bool m_HapticEnabled = true;
    public bool HapticEnabled { 
        set {
            m_HapticEnabled = value;
        }
        get { return m_HapticEnabled; }
    }

    public void VibradePreset(HapticType type = HapticType.POP)
    {
        if (!HapticEnabled) return;

        switch (type)
        {
            case HapticType.HIT:
                TapHitVibrate(new long[] {  0, 10} );
                break;
            case HapticType.DOUBLEHIT:
                TapHitVibrate(new long[] { 0, 10, 5, 10 });
                break;
            case HapticType.POP:
                TapPopVibrate();
                break;
            case HapticType.PEEK:
                TapPeekVibrate();
                break;
            case HapticType.NOPE:
                TapNopeVibrate();
                break;
            default:
                TapPopVibrate();

                break;
        }
    }
  
    internal void Initialize()
    {
        Vibration.Init();
    }



    public void TapHitVibrate(long[] pattern)
    {
        //Only android
        //Only android
        Vibration.Vibrate(pattern, -1);
      

        //IOS & android
        //  Vibration.VibratePop();
    }
    public void TapPopVibrate()
    {
        //Only android
        long ms = 20;
        Vibration.Vibrate(ms);

        //IOS & android
      //  Vibration.VibratePop();
    }

    public void TapPeekVibrate()
    {
        //IOS and android
        Vibration.VibratePeek();
    }

    public void TapNopeVibrate()
    {
        //Only android
        long[] pattern = { 0, 20, 10, 20,10,30};
        Vibration.Vibrate(pattern,-1);

        //IOS and android
        // Vibration.VibrateNope();
    }




    public void StartVibrateLoop() {
        if (!HapticEnabled) return;

    }
   

  



}
