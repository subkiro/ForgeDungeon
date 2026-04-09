using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof (Image))]
[RequireComponent(typeof(ParallelGaussianBlur))]
public class UIBlur : MonoBehaviour
{
    private int m_ResWidth;
    private int m_ResHeight;
    private Camera _camera;
    private Texture2D _screenShot;
    private Image m_ImageBlur;
    public void Initialize(float Delayed = 0.3f)
    {
       
        m_ImageBlur = GetComponent<Image>();
        m_ResWidth = Screen.width/4;
        m_ResHeight =  Screen.height/4;
        TakeScreenShot(Delayed);


    }

   
    private void TakeScreenShot(float Delayed)
    {

        RenderTexture rt = new RenderTexture(m_ResWidth, m_ResHeight, 24);

        Camera.main.targetTexture = rt;
        _screenShot = new Texture2D(m_ResWidth, m_ResHeight, TextureFormat.RGBA32, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        _screenShot.ReadPixels(new Rect(0, 0, m_ResWidth, m_ResHeight), 0, 0);
        _screenShot.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);


        Sprite tempSprite = Sprite.Create(_screenShot, new Rect(0, 0, m_ResWidth, m_ResHeight), new Vector2(0, 0));
        m_ImageBlur.sprite = tempSprite;

        GetComponent<ParallelGaussianBlur>().GaussianBlur(m_ImageBlur, m_ResWidth,m_ResHeight, ()=>ShowBluredImage(true,Delayed));

      

    }


    public void ShowBluredImage(bool show = true, float Delayed =0) {

        if (show)
        {
            m_ImageBlur.DOFade(0, 0f).SetUpdate(true);
            m_ImageBlur.enabled = true;
            m_ImageBlur.DOFade(1, 0.3f).SetDelay(Delayed).SetUpdate(true);
        }
        else {
           
            m_ImageBlur.DOFade(0, .1f).SetUpdate(true).onComplete +=()=>{
                Destroy(this.gameObject);
            };

        }


    }
   
    
}
