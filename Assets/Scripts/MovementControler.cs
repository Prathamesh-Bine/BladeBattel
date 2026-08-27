using UnityEngine;

public class MovementControler : MonoBehaviour
{
    [SerializeField] public float speed=4f;
    public Joystick joystick;
    private Rigidbody rb;
    private Vector3 velocityVector=Vector3.zero;
    [SerializeField] public float maxVelocityChange= 4f;

    [SerializeField] public float tiltAmount = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb=GetComponent<Rigidbody>(); 
        
    }

    // Update is called once per frame
    void Update()
    {

        float _xMovementInput= joystick.Horizontal;
        float _zMovementInput= joystick.Vertical;
        
        //Calculate Movement Vector
        Vector3 _movementHorizontal = transform.right * _xMovementInput;
        Vector3 _movementVertical = transform.forward * _zMovementInput;

        Vector3 _movementVelocityVector = (_movementHorizontal + _movementVertical)*speed;
    
        Move(_movementVelocityVector);

        transform.rotation = Quaternion.Euler(joystick.Vertical*speed*tiltAmount,0,-1*joystick.Horizontal*speed*tiltAmount);
    }

    void Move(Vector3 movementVelocityVector)
    {
        velocityVector=movementVelocityVector;
    }

    private void FixedUpdate()
    {
        if(velocityVector!=Vector3.zero)
        {
            //GEt rigidbody's current velocity
            Vector3 velocity = rb.linearVelocity;
            Vector3 velocityChange = (velocityVector - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x,-maxVelocityChange,maxVelocityChange); 
            velocityChange.z = Mathf.Clamp(velocityChange.z,-maxVelocityChange,maxVelocityChange); 
            velocityChange.y = 0f;
            rb.AddForce(velocityChange, ForceMode.Acceleration);
        }

        
    }
}
