using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;

public class BattleScript : MonoBehaviourPun
{
    public Spinner spinnerScript;
    private Rigidbody rb;

    public GameObject UI_3D_GameObject;
    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameobject;

    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;

    public bool isAttacker;
    public bool isDefender;

    private bool isDead = false;

    [Header("Combat Stats")]
    public float attackStat = 10f;
    public float defenseStat = 50f;

    [Header("Global Damage Tuning")]
    [Tooltip("Multiplier for all collision damage. Set to 3 for 3x damage, 4 for 4x damage, etc.")]
    public float damageScale = 4.0f; // Change this to 3.0f or 4.0f as desired

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
        }
        else if (gameObject.name.Contains("Defender"))
        {
            isAttacker = false;
            isDefender = true;
        }

        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed.ToString("F0");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.05f, 0);
                // instantiate collision Effect Particlesystem
                GameObject collisionEffectGameobject = GetPooledObject();
                if (collisionEffectGameobject != null)
                {
                    collisionEffectGameobject.transform.position = effectPosition;
                    collisionEffectGameobject.SetActive(true);
                    collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                    // Deactivate collision effect particle system after some seconds
                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));
                }
            }

            float mySpeed = gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude;
            
            Debug.Log("My speed: " + mySpeed + "-----otherPlayerSpeed: " + otherPlayerSpeed);
            
            if (mySpeed > otherPlayerSpeed)
            {
                Debug.Log("You Damaged the other player");
                
                // 1. Calculate Raw Kinetic Energy Damage scaled by damageScale (3x, 4x, etc.)
                float myMass = rb.mass;
                float rawDamage = 0.5f * myMass * (mySpeed * mySpeed) * attackStat * damageScale;

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // Send the scaled raw damage across the network
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, rawDamage);
                }
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float incomingRawDamage)
    {
        if (!isDead)
        {
            // 2. Apply Defense Mitigation
            float damageMultiplier = 100f / (100f + defenseStat);
            float finalDamage = incomingRawDamage * damageMultiplier;

            spinnerScript.spinSpeed -= finalDamage;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed.ToString("F0");

            if (currentSpinSpeed < 100)
            {
                Die();
            }
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        GetComponent<MovementControler>().enabled = false;
        rb.freezeRotation = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript.spinSpeed = 0f;

        UI_3D_GameObject.SetActive(false);

        if (photonView.IsMine)
        {
            StartCoroutine(ReSpawn()); 
        }
    }

    IEnumerator ReSpawn()
    {
        GameObject canvasGameObject = GameObject.Find("Canvas");
        if (deathPanelUIGameobject == null)
        {
            deathPanelUIGameobject = Instantiate(deathPanelUIPrefab, canvasGameObject.transform);
        }
        else
        {
            deathPanelUIGameobject.SetActive(true);
        }
        
        Text respawnTimeText = deathPanelUIGameobject.transform.Find("RespawnTimeText").GetComponent<Text>();
        float respawnTime = 8.0f;
        respawnTimeText.text = respawnTime.ToString(".00");

        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");
            GetComponent<MovementControler>().enabled = false;
        }
        
        deathPanelUIGameobject.SetActive(false);
        GetComponent<MovementControler>().enabled = true;
        photonView.RPC("ReBorn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ReBorn()
    {
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed.ToString("F0");

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        UI_3D_GameObject.SetActive(true);

        isDead = false; 
    }
    
    public List<GameObject> pooledObjects;
    public int amountToPool = 8;
    public GameObject CollisionEffectPrefab;

    void Start()
    {
        CheckPlayerType();

        if (photonView.IsMine)
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);
    }
}