using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float spinSpeed = 3600f;
    public bool dospin= false;
    private Rigidbody rb;

    public GameObject PlayerGraphics;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dospin)
        {
            PlayerGraphics.transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
        }
    }
}
