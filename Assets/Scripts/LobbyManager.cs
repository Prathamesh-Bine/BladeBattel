using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header ("LOGIN UI")]
    public InputField playerNameInputField;
    public GameObject uI_LoginGameObject;

    [Header ("LOBBY UI")]
    public GameObject uI_LobbyGameObject;
    public GameObject uI_3DGameObject;

    [Header("Connection Status UI")]
    public GameObject uI_ConnectionStatusGameObject;
    public Text ConnectionStatusText;

    public bool showConnectionStatus = false;


    #region Unity Methods
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            uI_LobbyGameObject.SetActive(true);
        uI_3DGameObject.SetActive(true);
        uI_ConnectionStatusGameObject.SetActive(false);
        uI_LoginGameObject.SetActive(false);
        }
        else
        {
            uI_LobbyGameObject.SetActive(false);
            uI_3DGameObject.SetActive(false);
            uI_ConnectionStatusGameObject.SetActive(false);
            uI_LoginGameObject.SetActive(true);
        }
        
    }
        
    

    // Update is called once per frame
    void Update()
    {
    if (showConnectionStatus)
        {
            ConnectionStatusText.text = "Connecting Status:" + PhotonNetwork.NetworkClientState;
        }   
    }
    #endregion

    #region UI Callback Methods
    public void OnEnterGameButtonClicked()

    {   
        
        string playerName = playerNameInputField.text;
    
    if(!string.IsNullOrEmpty(playerName)){
        uI_LobbyGameObject.SetActive(false);
        uI_3DGameObject.SetActive(false);
        uI_LoginGameObject.SetActive(false);

        
        uI_ConnectionStatusGameObject.SetActive(true);
        showConnectionStatus = true;
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
                
            }

        }
        else{
          Debug.Log("Player name is empty. Please enter a valid name.");  
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }



    #endregion

    #region Photon Callbacks Methods

    public override void OnConnected()
    {
        Debug.Log("Connected to the Internet.");
    }
    public override void OnConnectedToMaster()
    {
        
        uI_LoginGameObject.SetActive(false);
        uI_ConnectionStatusGameObject.SetActive(false);
        uI_LobbyGameObject.SetActive(true);
        uI_3DGameObject.SetActive(true);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has connected to the Photon server.");
    }

    #endregion
}
