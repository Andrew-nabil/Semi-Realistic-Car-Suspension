using UnityEngine;
public class CarController : MonoBehaviour
{
    
    [Header("References")] 
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;

    [Header("Suspension Settings")] 
    //The maximum force the spring can do when its compressed
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness; //D = zeta * 2 * sqrt(springStiffness*carMass
    //The Standard Length of the theoretical spring when it's not compressed or stretched
    [SerializeField] private float restLength;
    //The maximum distance that the spring can compress or stretch from the rest position 
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;
    private int[] _wheelsGrounded = new int[4];
    private bool _isGrounded = false;

    [Header("Input")] 
    private float _moveInput = 0;
    private float _steerInput = 0;

    
    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1;
    
    private Vector3 _currenLocalVelocity = Vector3.zero;
    private float _velocityRatio = 0;

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
        float dragMagnitude = -currentSideSpeed * dragCoefficient;
        Vector3 dragForce = dragMagnitude * transform.right;
        carRb.AddForceAtPosition(dragForce,carRb.worldCenterOfMass,ForceMode.Acceleration);
    }

    private void Turn()
    {
        carRb.AddRelativeTorque(steerStrength * _steerInput * turningCurve.Evaluate(Mathf.Abs(_velocityRatio))*Mathf.Sign(_velocityRatio)*carRb.transform.up,ForceMode.Acceleration);
    }

    private void Accelerate()
    {
        carRb.AddForceAtPosition(acceleration * _moveInput * transform.forward, accelerationPoint.position,ForceMode.Acceleration);
        print(_moveInput);
    }

    private void Decelerate()
    {
        carRb.AddForceAtPosition(deceleration * _moveInput * -transform.forward, accelerationPoint.position,ForceMode.Acceleration);
        print(_moveInput);
    }
    

    #endregion
   
    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateVelocity();
        Move();
        

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
