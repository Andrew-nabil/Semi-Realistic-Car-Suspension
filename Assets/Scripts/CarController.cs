
using UnityEngine;

public class CarController : MonoBehaviour
{
    
    [Header("References")] 
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;
    [SerializeField] private Transform flWheel;
    [SerializeField] private Transform frWheel;
    [SerializeField] private Transform brWheel;
    [SerializeField] private Transform blWheel;
    [SerializeField] private Transform frs;
    [SerializeField] private Transform fls;

    [Header("Suspension Settings")] 
    //The maximum force the spring can do when its compressed
    [SerializeField] private float springStiffness =30000;
    [SerializeField] private float damperStiffness= 4000; //D = zeta * 2 * sqrt(springStiffness*carMass
    //The Standard Length of the theoretical spring when it's not compressed or stretched
    [SerializeField] private float restLength = 0.5f;
    //The maximum distance that the spring can compress or stretch from the rest position 
    [SerializeField] private float springTravel = 0.25f;
    [SerializeField] private float wheelRadius = 0.12f;
    private int[] _wheelsGrounded = new int[4];
    private bool _isGrounded;

    [Header("Input")] 
    private float _moveInput;
    private float _steerInput;

    
    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1;
    [SerializeField] private float brakingDeceleration = 100;
    [SerializeField] private float brakingDragCoefficient = 0.5f;
    [SerializeField] private float wheelRotSpeed = 20;
    
    private Vector3 _currenLocalVelocity = Vector3.zero;
    private float _velocityRatio;

    #region Car Status Check

    private void GroundCheck()
    {
        int tempGroundedWheels = 0;
        foreach (var wheel in _wheelsGrounded)
        {
            tempGroundedWheels += wheel;
        }

        _isGrounded = tempGroundedWheels > 1;
    }

    private void CalculateVelocity()
    {
        _currenLocalVelocity = transform.InverseTransformDirection(carRb.velocity);
        _velocityRatio = _currenLocalVelocity.z / maxSpeed;
    }

    #endregion

    #region Input Handling

    private void GetPlayerInput()
    {
        _moveInput = Input.GetAxis("Vertical");
        //_moveInput = Input.GetAxis("Acc");
        _steerInput = Input.GetAxis("Horizontal");
        
    }

    #endregion

    #region Suspension Function
    private void Suspension()
    {
        for (int i =0; i< rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;
            if (Physics.Raycast(rayPoints[i].position, -Vector3.up, out hit, maxLength + wheelRadius, drivable))
            {
                _wheelsGrounded[i] = 1;
                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;
                float springVelocity = Vector3.Dot(carRb.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = springVelocity * damperStiffness;
                float springForce = springCompression * springStiffness;
                float totalForce = springForce - dampForce;
                carRb.AddForceAtPosition(totalForce*rayPoints[i].up,rayPoints[i].position);
                Debug.DrawLine(rayPoints[i].position,hit.point,Color.red);
            }
            else
            {
                _wheelsGrounded[i] = 0;
                Debug.DrawLine(rayPoints[i].position,rayPoints[i].position+(wheelRadius+maxLength)* -rayPoints[i].up,Color.green);
            }
        }
        
    }


    

    #endregion

    #region Movement

    private void RotateWheels()
    {
        float wheelRot =  wheelRotSpeed * _velocityRatio * Time.deltaTime;
        frWheel.Rotate(Vector3.up,wheelRot,Space.Self);
        flWheel.Rotate(Vector3.up,wheelRot,Space.Self);
        brWheel.Rotate(Vector3.up,wheelRot,Space.Self);
        blWheel.Rotate(Vector3.up,wheelRot,Space.Self);
        
    }

    private void Move()
    {
        if (_isGrounded)
        {
            Accelerate();
            Decelerate();
            Turn();
            SideDrag();
        }
    }

    private void SideDrag()
    {
        float currentSideSpeed = _currenLocalVelocity.x;
        float dragMagnitude = -currentSideSpeed * (Input.GetKey(KeyCode.Space) ? brakingDragCoefficient : dragCoefficient);
        Vector3 dragForce = dragMagnitude * transform.right;
        carRb.AddForceAtPosition(dragForce,carRb.worldCenterOfMass,ForceMode.Acceleration);
    }

    private void Turn()
    {
        carRb.AddRelativeTorque(steerStrength * _steerInput * turningCurve.Evaluate(Mathf.Abs(_velocityRatio))*Mathf.Sign(_velocityRatio)*carRb.transform.up,ForceMode.Acceleration);

        if (_steerInput !=0)
        {
            frs.localRotation = Quaternion.Euler(0,_steerInput*35,0);
            fls.localRotation = Quaternion.Euler(0,_steerInput*35,0);
        }

        
        

    }

    private void Accelerate()
    {
        if (_currenLocalVelocity.z <= maxSpeed)
        {
            carRb.AddForceAtPosition(acceleration * _moveInput * carRb.transform.forward, accelerationPoint.position,ForceMode.Acceleration);
        }
        
        
        
        
        
    }

    private void Decelerate()
    {
        if (_currenLocalVelocity.z <= maxSpeed)
        {
            
            carRb.AddForce((Input.GetKey(KeyCode.Space) ? brakingDeceleration :deceleration) * _velocityRatio * -carRb.transform.forward,ForceMode.Acceleration);
            
        }
        
        
    }
    

    #endregion
   
    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateVelocity();
        Move();
        RotateWheels();
        
        
            /*print(Input.GetKey(KeyCode.JoystickButton0) + " 0");
            print(Input.GetKey(KeyCode.JoystickButton1)+" 1");
            print(Input.GetKey(KeyCode.JoystickButton2)+" 2");
            print(Input.GetKey(KeyCode.JoystickButton3)+" 3");
            print(Input.GetKey(KeyCode.JoystickButton4)+" 4");
            print(Input.GetKey(KeyCode.JoystickButton5)+" 5");
            print(Input.GetKey(KeyCode.JoystickButton6)+" 6");
            print(Input.GetKey(KeyCode.JoystickButton7)+" 7");
            print(Input.GetKey(KeyCode.JoystickButton8)+" 8");
            print(Input.GetKey(KeyCode.JoystickButton9)+" 9");
            print(Input.GetKey(KeyCode.JoystickButton10)+" 10");
            print(Input.GetKey(KeyCode.JoystickButton11)+" 11");
            print(Input.GetKey(KeyCode.JoystickButton12)+" 12");
            print(Input.GetKey(KeyCode.JoystickButton13)+" 13");
            print(Input.GetKey(KeyCode.JoystickButton14)+" 14");
            print(Input.GetKey(KeyCode.JoystickButton15)+" 15");
            print(Input.GetKey(KeyCode.JoystickButton16)+" 16");*/
        
    }

    private void Update()
    {
        GetPlayerInput();
        
    }


    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
        
    }
}
