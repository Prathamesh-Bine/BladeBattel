using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    public Transform playerSwitcherTransform;
    public GameObject[] SpinnerTopModels;
    public int PlayerSelectionNumber = 0;

    

    [Header("UI ")]   
    public Button next_Button;
    public Button previous_Button;
    public TextMeshProUGUI playerModelType_Text;
    public GameObject uI_Selection;
    public GameObject uI_AfterSelection;


    #region Unity Methods

    void Start()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);
    }

    void Update()
    {
    }

    #endregion

    #region UI Callback Methods

    public void OnSelectButtonClicked()
    {
        uI_Selection.SetActive(false);
        uI_AfterSelection.SetActive(true);
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable{{MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, PlayerSelectionNumber}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
        
    }

    public void OnReselectButtonClicked()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);
    }

    public void OnBattelButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Gameplay");

    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    
    }

    public void NextPlayer()
    {
        PlayerSelectionNumber+=1;
        if(PlayerSelectionNumber > SpinnerTopModels.Length-1)
        {
            PlayerSelectionNumber = 0;
        }
        next_Button.interactable = false;
        previous_Button.interactable = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));
        if(PlayerSelectionNumber==0 || PlayerSelectionNumber == 1) //First 2 models are Attack type 
        {
            playerModelType_Text.text = "ATTACK";
        }
        else
        {
            playerModelType_Text.text = "DEFEND";
        }

    }

    public void PreviousPlayer()
    {
        PlayerSelectionNumber-=1;
        if(PlayerSelectionNumber < 0)
        {
            PlayerSelectionNumber = SpinnerTopModels.Length-1;
        }
         next_Button.interactable = false;
        previous_Button.interactable = false;
        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));
        if(PlayerSelectionNumber==0 || PlayerSelectionNumber == 1) //First 2 models are Attack type 
        {
            playerModelType_Text.text = "ATTACK";
        }
        else
        {
            playerModelType_Text.text = "DEFEND";
        }
    }

    #endregion

    #region Private Methods

    IEnumerator Rotate(Vector3 axis,Transform transformToRotate, float angle, float duration = 1.0f)
    {
        Quaternion originalRotation = transformToRotate.rotation;

        Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler(axis * angle);

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transformToRotate.rotation = finalRotation;
        next_Button.interactable = true;
        previous_Button.interactable = true;
    }

    #endregion
}