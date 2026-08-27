using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

public class ScaleController : MonoBehaviour
{
    XROrigin m_ARSessionOrigin;

    public Slider scaleSlider;

    private void Awake()
    {
        m_ARSessionOrigin = GetComponent<XROrigin>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scaleSlider.onValueChanged.AddListener(OnSliderValeChanged);
        
    }
    public void OnSliderValeChanged(float value)
    {
        if(scaleSlider != null)
        {
            m_ARSessionOrigin.transform.localScale = Vector3.one /value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
