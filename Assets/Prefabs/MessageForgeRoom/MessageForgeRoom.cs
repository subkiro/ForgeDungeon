using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class MessageForgeRoom : MonoBehaviour
{

    [SerializeField] Button m_ForgeButton;
    [SerializeField] GameObject m_DwarfCharacter;
    Animator m_Animator;

    [SerializeField] RectTransform m_Container;
    [SerializeField] float m_delay=.3f;
    [SerializeField] float m_power=.3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_Animator = m_DwarfCharacter.GetComponentInChildren<Animator>();
        m_ForgeButton.onClick.AddListener(ForgePressed);
    }
    void ForgePressed()
    {
        m_Animator.SetTrigger("2_Attack");
        m_Container.DOKill(true);
        m_Container.DOPunchPosition(Vector2.down*50f*m_power,.3f).SetDelay(m_delay);
    }
    // Update is called once per frame

}
