using UnityEngine;
using System.Collections;
using Photon.Pun;
using TMPro;
public class PlayerSetup : MonoBehaviourPun
{
    public TextMeshProUGUI PlayerNameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (photonView.IsMine)
        {
            transform.GetComponent<MovementControler>().enabled = true;
            transform.GetComponent<MovementControler>().joystick.gameObject.SetActive(true);
        }
        else
        {
            transform.GetComponent<MovementControler>().enabled = false;
            transform.GetComponent<MovementControler>().joystick.gameObject.SetActive(false);
            
        }
        SetPlayerName();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetPlayerName()
    {
        if (PlayerNameText != null)
        {


            if (photonView.IsMine)
            {
                PlayerNameText.text= "YOU";
                PlayerNameText.color=Color.red;
            }
            else
            {
                PlayerNameText.text = photonView.Owner.NickName;
            }
        }
    }
}
