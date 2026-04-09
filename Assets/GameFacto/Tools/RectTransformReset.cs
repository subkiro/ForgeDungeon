using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectTransformReset : MonoBehaviour
{
    public WhenType InitOn = WhenType.Awake;
    public Vector3 InitPosition = Vector3.zero;
    private RectTransform rect;
    private void Awake()
    {
        this.rect = this.GetComponent<RectTransform>();

        if (InitOn == WhenType.Awake) {
            rect.anchoredPosition = InitPosition;
        }
            
    }
    private void Start()
    {
        if (InitOn == WhenType.Start)
        {
            rect.anchoredPosition = InitPosition;
        }
    }

    private void OnEnable()
    {
        if (InitOn == WhenType.OnEnable)
        {
            rect.anchoredPosition = InitPosition;
        }
    }
    private void OnDisable()
    {
        if (InitOn == WhenType.OnDisable)
        {
            rect.anchoredPosition = InitPosition;
        }
    }
    public enum WhenType {
        Awake,
        Start,
        OnEnable,
        OnDisable
    }
        

}
