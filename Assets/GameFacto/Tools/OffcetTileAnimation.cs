using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffcetTileAnimation : MonoBehaviour
{
    [SerializeField] RawImage m_BackgroundRaw;
    public Vector2 m_backgroundSpeed;

    // Update is called once per frame
    private void Update()
    {
        var offcet = m_BackgroundRaw.uvRect;
        offcet.y -= Time.deltaTime * m_backgroundSpeed.y;
        offcet.x -= Time.deltaTime * m_backgroundSpeed.x;
        m_BackgroundRaw.uvRect = offcet;
    }
}
