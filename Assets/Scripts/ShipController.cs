using System;
using UnityEngine;


public class ShipController : MonoBehaviour
{
    // TODO: Clean up code.
    public event Action velocityChanged;
    public event Action<float> thrustChanged;
    public event Action<float, string> angularVelocityChanged;
    public event Action<bool> stabilityAssistChanged;
    public event Action<bool> lightsSwitched;

    public float altitude;
    public bool isShipOnGround = false;

    [SerializeField] float maxRotationalVelocity = 1000f;
    [SerializeField] float rotationSensitivity = 1f;
    [SerializeField] float maxThrust = 0.01f;
    [SerializeField] float thrustSensitivity = 0.0001f;
    [SerializeField] float thrustLevel = 0f;
    [SerializeField] Vector3 localAngularSpeed;
    [SerializeField] GameObject groundPoint;
    [Header("Lights")]
    [SerializeField] GameObject light1;
    [SerializeField] GameObject light2;

    private bool lightsOn = false;
    private bool toStopThrust = false;
    private float currentThrust = 0f;
    private bool isVelocityChanged = false;
    private bool stabilityAssistOn = false; // Is stability assist  enabled. (Stopping the rotation)
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxRotationalVelocity;
    }
    private void FixedUpdate()
    {
        CalculateAltitude();
        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);
        localAngularSpeed = localAngularVelocity;
        ProcessThrust();
        ProcessRotation();
        if (stabilityAssistOn)
            ProcessStoppingRotation();         
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Switching stability assist on and off.
        {
            stabilityAssistOn = !stabilityAssistOn;
            if (stabilityAssistChanged != null)
                stabilityAssistChanged(stabilityAssistOn);
        }     
        if(Input.GetKeyDown(KeyCode.L))
        {
            lightsOn = !lightsOn;
            ToggleLights(lightsOn);
        }
        toStopThrust = ProcessEngineShutDownInput();
    }
    private void LateUpdate()
    {
        if(isVelocityChanged)
        {
            if (velocityChanged != null)
                velocityChanged();
            isVelocityChanged = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        isShipOnGround = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (velocityChanged != null)
            velocityChanged();
        isShipOnGround = false;
    }
    
    private void ToggleLights(bool lightsOn)
    {
        light1.SetActive(lightsOn);
        light2.SetActive(lightsOn);
        if (lightsSwitched != null)
            lightsSwitched(lightsOn);
    }
    private void CalculateAltitude()
    {
        Physics.Raycast(groundPoint.transform.position, -groundPoint.transform.position, out RaycastHit hit);
        Debug.DrawLine(groundPoint.transform.position, hit.point);
        altitude = (hit.point - groundPoint.transform.position).magnitude;
    }
    private void ProcessRotationOnAxis(string axisName, Vector3 directionOfTorque)
    {
        var axisInput = Input.GetAxis(axisName);
        if (axisInput == 0)
        {
            if (angularVelocityChanged != null)
                angularVelocityChanged(axisInput, axisName);
            return;
        }
        var torque = axisInput * rotationSensitivity;
        rb.AddRelativeTorque(directionOfTorque * torque);        
        if (angularVelocityChanged != null)
            angularVelocityChanged(axisInput, axisName);
    }
    private void ProcessRotation()
    {
        ProcessRotationOnAxis("Horizontal", Vector3.right);
        ProcessRotationOnAxis("Vertical", Vector3.forward);
        ProcessRotationOnAxis("Roll", -Vector3.up);
    }
    private void ProcessStoppingRotation()
    {
        if(rb.angularVelocity.magnitude < 0.005f)
        {
            rb.angularVelocity = Vector3.zero;
            if (angularVelocityChanged != null)
            {
                angularVelocityChanged(0, "Vertical");
                angularVelocityChanged(0, "Horizontal");
                angularVelocityChanged(0, "Roll");
            }               
            return;
        }
        else
        {
            rb.angularVelocity *= 0.98f;
            if (angularVelocityChanged != null)
            {
                angularVelocityChanged(-localAngularSpeed.z, "Vertical");
                angularVelocityChanged(-localAngularSpeed.x, "Horizontal");
                angularVelocityChanged(localAngularSpeed.y, "Roll");
            }
        }
    }
    private bool ProcessEngineShutDownInput()
    {
        var input = Input.GetKey(KeyCode.X);
        return input;
    }
    private void ProcessThrust()
    {
        if (toStopThrust)
        {
            // Shut down the engine.
            currentThrust = 0f; 
            thrustLevel = 0f;
            if (thrustChanged != null)
                thrustChanged(thrustLevel);
            toStopThrust = false;
            return;
        }
        float deltaThrust = maxThrust / 10;
        var thrustOffset = Input.GetAxis("Thrust");
        currentThrust += thrustOffset * deltaThrust * thrustSensitivity;
        currentThrust = Mathf.Clamp(currentThrust, 0f, maxThrust);
        thrustLevel = currentThrust / maxThrust;
        thrustLevel = Mathf.Clamp(thrustLevel, 0, 1);
        if (currentThrust != 0 || thrustOffset != 0)
        {
            if(!isShipOnGround)
                isVelocityChanged = true;
            rb.AddRelativeForce(Vector3.up * currentThrust);
            if (thrustChanged != null)
                thrustChanged(thrustLevel);
        }
    }
}
