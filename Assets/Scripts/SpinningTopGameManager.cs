using UnityEngine;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using TMPro;

public class SpinningTopGameManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public GameObject uI_InformPanelGameobject;
    public TextMeshProUGUI uI_InfoText;
    public GameObject SearchForGameButtonGameobject;
    public GameObject adjust_Button;
    public GameObject raycastCenter_Image;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uI_InformPanelGameobject.SetActive(true);
           
    }

    // Update is called once per frame
    void Update(){    
    }


    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        uI_InfoText.text = "Searching for available rooms...";
        PhotonNetwork.JoinRandomRoom();

        SearchForGameButtonGameobject.SetActive(false);
    }

    public void OnQuitMatchButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }
    }


    
    #endregion



    #region PHOTON Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message); 
        uI_InfoText.text = message;

        CreateAndJoinRoom();
    }


    public override void OnJoinedRoom()
    {
        adjust_Button.SetActive(false);
        raycastCenter_Image.SetActive(false);
        if(PhotonNetwork.CurrentRoom.PlayerCount==1)
        {
            uI_InfoText.text = "Joied to "+PhotonNetwork.CurrentRoom.Name+"Waiting for other Player....";
        }
        else
        {
            uI_InfoText.text="Joined to"+PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameobject, 2.0f));

        }

        Debug.Log("joined to"+ PhotonNetwork.CurrentRoom.Name);
    }


    


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "joined to" + PhotonNetwork.CurrentRoom.Name+"Player Count: "+PhotonNetwork.CurrentRoom.PlayerCount);
        uI_InfoText.text =  newPlayer.NickName + "joined to" + PhotonNetwork.CurrentRoom.Name+"Player Count: "+PhotonNetwork.CurrentRoom.PlayerCount;    
        StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameobject, 2.0f));
    }


    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }
    
    
    #endregion



    #region Private Methods

    void CreateAndJoinRoom()
    {
        string randomRoomName= "Room" + Random.Range(0,1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);

    }
    IEnumerator DeactivateAfterSeconds(GameObject _GameObject, float _Seconds)
    {
        yield return new WaitForSeconds(_Seconds);
        _GameObject.SetActive(false);
    }

    #endregion
}
