using UnityEngine;
using UnityEngine.InputSystem;

public class AirplaneFlightPhysicsSimulation : MonoBehaviour
{
    [Header("Engine")]
    public float thrust = 40;

    [Header("Aerodynamics")]
    public float liftCoefficient = 0.02f;
    public float dragCoefficient = 0.02f;
    public float sideDrag = 2f;

    [Header("BANK TURN")]
    public float turnStrength = 0.5f;

    [Header("STALL")]
    public float stallAngle = 35f;
    public float stallLiftMultiplier = 0.3f;

    [Header("Control")]
    public float pitchPower = 15f;
    public float rollPower = 20f;
    public float yawPower = 15f;

    Rigidbody rb;
    private bool engineOn = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.6f, -0.2f);
    }

    void FixedUpdate()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if(kb.spaceKey.isPressed)
        {
            enabled = true;
            rb.AddRelativeForce(Vector3.forward * thrust, ForceMode.Acceleration);
        }

        float fowardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if(engineOn && fowardSpeed > 5f)
        {
            float lift = fowardSpeed * fowardSpeed * liftCoefficient;

            float pitchAngle = Vector3.Angle(transform.forward,
                                                Vector3.ProjectOnPlane(transform.forward,
                                                Vector3.up));

            if(pitchAngle > stallAngle)
            {
                lift *= stallLiftMultiplier;
            }

            rb.AddForce(transform.up * lift, ForceMode.Acceleration);

            Debug.DrawRay(transform.position, transform.up * 5f, Color.green);
        }

        Vector3 drag = -rb.linearVelocity * dragCoefficient;
        rb.AddForce(drag);

        Vector3 sideVel = Vector3.Dot(rb.linearVelocity, transform.right) * transform.right;
        rb.AddForce(-sideVel * sideDrag);

        float pitch = 0;
        float roll = 0;
        float yaw = 0;

        if (kb.sKey.isPressed) pitch = 1;
        if (kb.wKey.isPressed) pitch = -1;

        if (kb.aKey.isPressed) roll = 1;
        if (kb.dKey.isPressed) roll = -1;

        if (kb.qKey.isPressed) yaw = 1;
        if (kb.eKey.isPressed) yaw = -1;

        rb.AddRelativeTorque(new Vector3(pitch * pitchPower, yaw * yawPower, -roll * rollPower));

        float bankAmount = Vector3.Dot(transform.right, Vector3.up);

        rb.AddForce(transform.right * bankAmount * fowardSpeed * turnStrength);

        Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue);
    }
}
