using UnityEngine;
using System.Collections;
using Photon.Pun;

public class MySynchronizationScript : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;
    Vector3 networkedPosition;
    Quaternion networkedRotation;
    public bool synchronizeVelocity = true; 
    public bool synchronizeAngularVelocity = true;
    public bool isTeleportEnabled = true;
    public float teleportIfDistanceGreaterThan = 1.0f;

    private float distance;
    private float angle;

    private GameObject battleArenaGameobject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        networkedPosition = new Vector3();
        networkedRotation = new Quaternion();

        battleArenaGameobject = GameObject.Find("BattleArena");


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {     
            rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance*(1.0f/PhotonNetwork.SerializationRate));
            rb.rotation = Quaternion.RotateTowards(rb.rotation,networkedRotation, angle*(1.0f/PhotonNetwork.SerializationRate));

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
             //it meams photonView is mine and i am controling this player
             //should send position, veocity etc. data to the other player    
            stream.SendNext(rb.position-battleArenaGameobject.transform.position);
            stream.SendNext(rb.rotation);

            if (synchronizeVelocity)
            {
                stream.SendNext(rb.linearVelocity);
            }
            if (synchronizeAngularVelocity)
            {
                stream.SendNext(rb.angularVelocity);
            }
        
        }
        else
        {
            //called on my player gameobject that exists in remote players game
            networkedPosition = (Vector3)stream.ReceiveNext()+battleArenaGameobject.transform.position;
            networkedRotation = (Quaternion)stream.ReceiveNext();

            if (isTeleportEnabled)
            {
                if(Vector3.Distance(rb.position, networkedPosition)>teleportIfDistanceGreaterThan)
                
                {
                    rb.position = networkedPosition;
                }
            }

            if (synchronizeVelocity  || synchronizeAngularVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                if (synchronizeVelocity)
                {
                    rb.linearVelocity = (Vector3)stream.ReceiveNext();
                    networkedPosition += rb.linearVelocity * lag;

                    distance = Vector3.Distance(rb.position, networkedPosition ); 
                }

                if (synchronizeAngularVelocity)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();
                    networkedRotation = Quaternion.Euler(rb.angularVelocity * lag) * networkedRotation;
                
                    angle = Quaternion.Angle(rb.rotation, networkedRotation);
                
                }
            }


            
        }
    }
}
