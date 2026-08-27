using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ARPlacementAndPlaneController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    ARPlaneManager m_ARPlaneManager;
    ARPlacementManager m_ARPlacementManager;

    public GameObject placeButton;
    public GameObject adjustButton;
    public GameObject searchForGameButton;
    public GameObject scaleSlider;


    public TextMeshProUGUI informUIPanel_Text;

    private void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        m_ARPlacementManager = GetComponent<ARPlacementManager>();

    }
    
    
    void Start()
    {
        placeButton.SetActive(true);
        scaleSlider.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);
        informUIPanel_Text.text = "Move phone to detect planes and place Battle Arena.";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableARPlacementAndPlaneDetection()
    {
        m_ARPlacementManager.enabled = false;
        m_ARPlaneManager.enabled = false;
        SetAllPlanesActiveOrDeactive(false);

        scaleSlider.SetActive(false);

        placeButton.SetActive(false);
        adjustButton.SetActive(true);
        searchForGameButton.SetActive(true);
        informUIPanel_Text.text = "Great! You placed the Arena.. Now Search for Game to Battle!";



    }
    public void EnableARPlacementAndPlaneDetection(){
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;
        SetAllPlanesActiveOrDeactive(true);

        scaleSlider.SetActive(true);

        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);
        informUIPanel_Text.text = "Move phone to detect planes and place Battle Arena.";
    }

    private void SetAllPlanesActiveOrDeactive(bool value)
    {
        foreach(var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }




}
