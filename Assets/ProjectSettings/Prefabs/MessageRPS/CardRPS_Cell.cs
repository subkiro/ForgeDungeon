using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CardRPS_Cell : MonoBehaviour
{

   public RPS_ID ID;
   public RectTransform Container;
   public CanvasGroup Group;
   public Image Icon;

   public RectTransform BackFace;

    public bool Interactable{set=>Group.interactable=value; get=>Group.interactable;}
   public void ResetCard()
    {
        Group.alpha = 1;
        this.transform.localPosition=Vector3.zero;
        this.transform.localScale = Vector3.one;
        this.transform.localEulerAngles = Vector3.zero;
    }
   private bool m_ishidden;
    
   [ShowInInspector, OnValueChanged("EditorUpdate")]
   public bool IsHidden
    {
        set  {
            m_ishidden = value;
            BackFace?.gameObject.SetActive(value);
            }
        get =>m_ishidden;
    }

    void EditorUpdate()
    {
                    BackFace?.gameObject.SetActive(m_ishidden);

    }

}

public enum RPS_ID
{
    Rock,
    Paper,
    Scissor
}
